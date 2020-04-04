using InfinityCore.API.Chunks;
using InfinityCore.Players;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace InfinityCore.Worlds
{
    public partial class InfinityCoreWorld : ModWorld
    {

        internal static Dictionary<string, Chunk.Chunk> chunkList = new Dictionary<string, Chunk.Chunk>();

        internal static List<Chunk.Chunk> activeChunksList = new List<Chunk.Chunk>();

        internal static List<ModChunk> generationChunks = new List<ModChunk>();

        internal static bool preChunkGeneration = false;

        public static List<GenPass> specialPostChunkGenPasses = new List<GenPass>();

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

        private static void CreateChunk(GenerationProgress progress = null)
        {
            preChunkGeneration = false;
            progress.Message = "Dividing the world in section";
            CreateChunk();
        }

        private static void GenerateChunk(GenerationProgress progress = null)
        {
            //generationChunks.Sort();
            foreach (var chunks in generationChunks)
            {
                chunks.Generate(progress);
            }
        }

        private static void CreateChunk()
        {

            for (int i = 0; i < Main.maxTilesX; i += Chunk.Chunk.sizeX)
            {
                for (int j = 0; j < Main.maxTilesY; j += Chunk.Chunk.sizeY)
                {
                    Chunk.Chunk newChunk = new Chunk.Chunk(i, j);
                    newChunk.AddMissingModChunk();
                    chunkList.Add(newChunk.chunkInternalName, newChunk);
                }
            }
        }

        public static void SetChunkSize(int x, int y)
        {
            if (!WorldGen.gen)
            {
                throw new Exception("Attempt to modify world Chunk cannot be done outside of world generation");
            }

            if (!preChunkGeneration)
            {
                throw new Exception("Attempt to modify world Chunk after the chunk have been generated, ensure that you modify before the chunk generation. Insert your world gen phase before \"Creating Chunk\"");
            }
            Chunk.Chunk.sizeX = x;
            Chunk.Chunk.sizeY = y;
        }

        public static void PreSubworldGen(WorldGenerator generator)
        {
            preChunkGeneration = true;
            InfinityCore.instance.Logger.Info("Pre Subworld Gen hook work!");

        }

        public static void PostSubworldGen(WorldGenerator generator)
        {
            generator.Append(new PassLegacy("Creating Chunk", p => CreateChunk(p)));
            foreach (GenPass specialPostChunkGenPass in specialPostChunkGenPasses)
            {
                generator.Append(specialPostChunkGenPass);
            }
            generator.Append(new PassLegacy("ModChunck World Generation", p => GenerateChunk(p)));
            generator.Append(new PassLegacy("clear special post gen list", p => specialPostChunkGenPasses.Clear()));
        }
    }
}
