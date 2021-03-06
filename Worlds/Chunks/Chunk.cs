﻿using InfinityCore.API.Chunks;
using InfinityCore.API.Loader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace InfinityCore.Worlds.Chunk
{
    public sealed class Chunk
    {
        public Vector2 Position => new Vector2(x, y);

        internal int x;
        internal int y;

        internal string chunkInternalName;

        internal static int sizeX = 200;
        internal static int sizeY = 150;

        internal List<ModChunk> modChunk = new List<ModChunk>();
        internal List<Player> activePlayer = new List<Player>();

        internal bool IsActive = false;

        public Chunk(int x, int y)
        {
            this.x = x;
            this.y = y;
            chunkInternalName = $"region-{x / sizeX}-{y / sizeY}";
        }

        public T GetModChunk<T>() where T : ModChunk => (T)modChunk.SingleOrDefault(i => i is T);


        public TagCompound Save()
        {
            return new TagCompound()
            {
                ["internalName"] = chunkInternalName,
                ["positionX"] = x,
                ["positionY"] = y,
                ["moddedData"] = SaveModChunk()
            };
        }

        internal TagCompound SaveModChunk()
        {
            TagCompound tag = new TagCompound();
            foreach (var chunk in modChunk)
            {
                tag.Add(chunk.GetType().FullName, chunk.Save());
            }
            return tag;
        }

        internal void Load(TagCompound data)
        {
            foreach (KeyValuePair<string, object> keyValuePair in data)
            {
                if (ChunkLoader.chunkDictionnary.ContainsKey(keyValuePair.Key))
                {
                    ModChunk chunk = (ModChunk)Activator.CreateInstance(keyValuePair.Value.GetType());
                    chunk.Load(data.GetCompound(keyValuePair.Key));
                    modChunk.Add(chunk);
                }
            }
            AddMissingModChunk();
        }

        internal void AddMissingModChunk()
        {
            Dictionary<string, ModChunk> missingChunks = new Dictionary<string, ModChunk>(ChunkLoader.chunkDictionnary);
            //Filter existing chunk from missing one
            foreach (ModChunk existingModChunk in modChunk)
            {
                if (missingChunks.ContainsKey(existingModChunk.GetType().FullName))
                {
                    missingChunks.Remove(existingModChunk.GetType().FullName);
                }
            }

            foreach (KeyValuePair<string, ModChunk> missingChunk in missingChunks)
            {
                ModChunk newChunk = (ModChunk)Activator.CreateInstance(missingChunk.Value.GetType());
                if (!newChunk.CanExist)
                {
                    continue;
                }

                if (WorldGen.gen && newChunk.ImpactWorldGen)
                {
                    InfinityCoreWorld.generationChunks.Add(newChunk);
                }
                modChunk.Add(newChunk);
            }
        }

        internal void Update()
        {
            foreach (ModChunk chunk in modChunk)
            {
                if (chunk.IsGlobal)
                {
                    continue;
                }
                chunk.Update();
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (ModChunk modChunk in modChunk)
            {
                modChunk.Draw(spriteBatch, this);
            }
        }

        internal void NetSendChunk(ModPacket packet)
        {
            int modChunkCount = modChunk.Count;
            packet.Write(modChunkCount);
            for (int i = 0; i < modChunkCount; i++)
            {
                packet.Write(modChunk[i].GetType().Name);
                modChunk[i].NetSend(packet);
            }
        }

        internal void NetReceiveChunk(BinaryReader reader, bool syncMode = false)
        {
            int modChunkCount = reader.ReadInt32();

            if (syncMode)
            {
                AddMissingModChunk();
            }
            for (int i = 0; i < modChunkCount; i++)
            {
                ModChunk chunkType = ChunkLoader.chunkDictionnary[reader.ReadString()];

                foreach (ModChunk chunk in modChunk)
                {
                    if (chunk.GetType().FullName == chunkType.GetType().FullName)
                    {
                        chunk.NetReceive(reader);
                    }
                }
            }
        }

        internal bool CheckActivity(Player player)
        {
            Rectangle rec = new Rectangle(x * 16, y * 16, sizeX * 16, sizeY * 16);
            IsActive = rec.Contains(player.position.ToPoint());
            if (IsActive)
            {
                activePlayer.Add(player);
            }
            return activePlayer.Count != 0;
        }

        internal void CheckActivity()
        {

            foreach (Player player in new List<Player>(activePlayer))
            {
                Rectangle rec = new Rectangle(x * 16, y * 16, sizeX * 16, sizeY * 16);
                IsActive = rec.Contains(player.position.ToPoint());
                if (!IsActive)
                {
                    activePlayer.Remove(player);
                }
            }
        }
    }
}
