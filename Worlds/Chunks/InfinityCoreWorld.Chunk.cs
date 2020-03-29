using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfinityCore.API.Chunks;
using InfinityCore.API.Loader;
using InfinityCore.Players;
using InfinityCore.Worlds.Chunk;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace InfinityCore.Worlds
{
    partial class InfinityCoreWorld : ModWorld
    {

        internal static Dictionary<string, Chunk.Chunk> chunkList = new Dictionary<string, Chunk.Chunk>();

        internal static List<Chunk.Chunk> activeChunksList = new List<Chunk.Chunk>();

        internal static List<ModChunk> generationChunks = new List<ModChunk>();

        private TagCompound SaveChunk()
        {
            TagCompound tag = new TagCompound();
            mod.Logger.Info($"{chunkList.Count}");
            foreach (string chunkPosition in chunkList.Keys)
            {
                tag.Add(chunkPosition, new TagCompound()
                {
                    ["x"] = chunkList[chunkPosition].x,
                    ["y"] = chunkList[chunkPosition].y,
                    ["chunkData"] = chunkList[chunkPosition].Save()
                });
            }

            return tag;
        }

        private void LoadChunk(TagCompound tag)
        {
            foreach (KeyValuePair<string, object> chunk in tag)
            {
                TagCompound chunkData = tag.GetCompound(chunk.Key);
                Chunk.Chunk newChunk = new Chunk.Chunk(chunkData.GetInt("x"), chunkData.GetInt("y"));
                newChunk.Load(chunkData);
                /*foreach (var chunkType in ChunkLoader.chunkDictionnary.Values)
                {
                    if (chunkData.ContainsKey(chunkType.GetType().Name))
                    {
                        ModChunk modChunk = (ModChunk) Activator.CreateInstance(chunkType.GetType());
                        modChunk.Load(tag.GetCompound(chunkType.GetType().Name));
                    }
                }*/
                chunkList.Add(chunk.Key, newChunk);
            }

            ThreadPool.QueueUserWorkItem(UpdateChunkLoop);
        }


        private void UpdateChunkLoop(object context)
        {
            List<ModChunk> universalUpdateChunks = new List<ModChunk>();
            while (!Main.gameMenu)
            {
                if (!isSaving)
                {
                    foreach (ModChunk universalUpdateChunk in universalUpdateChunks)
                    {
                        universalUpdateChunk.Update();
                    }

                    activeChunksList = GetActiveChunks();
                    foreach (Chunk.Chunk chunk in activeChunksList)
                    {
                        chunk.Update();
                        chunk.CheckActivity();
                    }
                    
                }
            }

            List<Chunk.Chunk> GetActiveChunks()
            {

                if (Main.netMode == 0)
                {
                    return new List<Chunk.Chunk>()
                    {
                        Main.LocalPlayer.GetCurrentChunk()
                    };
                }
                List<Chunk.Chunk> activeChunkList = new List<Chunk.Chunk>();
                foreach (var player in Main.player)
                {
                    
                    if (player == null)
                    {
                        continue;
                    }

                    player.GetCurrentChunk().CheckActivity(player);
                    activeChunkList.Add(player.GetCurrentChunk());
                }
                return activeChunkList;
            }
        }

        private void CreateChunk(GenerationProgress progress = null)
        {
            progress.Message = "Dividing the world in section";
            CreateChunk();
        }

        private void GenerateChunk(GenerationProgress progress = null)
        {
            generationChunks.Sort();
            foreach (var chunks in generationChunks)
            {
                chunks.Generate(progress);
            }
        }

        private void CreateChunk()
        {
            
            for (int i = 0; i < Main.maxTilesX; i += 200)
            {
                for (int j = 0; j < Main.maxTilesY; j += 150)
                {
                    Chunk.Chunk newChunk = new Chunk.Chunk(i, j);
                    newChunk.AddMissingModChunk();
                    mod.Logger.Info(newChunk.chunkInternalName);
                    
                    chunkList.Add(newChunk.chunkInternalName, newChunk);
                }
            }
        }
    }
}
