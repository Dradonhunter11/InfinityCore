using InfinityCore.API.Loader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        internal static string worldGenType = "default";

        internal string currentSavePath;

        internal static bool isSaving = false;

        public Chunk.Chunk this[int x, int y]
        {
            get
            {
                mod.Logger.Info($"region-{(int)(x / Chunk.Chunk.sizeX)}-{(int)(y / Chunk.Chunk.sizeY)}");
                return chunkList[$"region-{(int)(x / Chunk.Chunk.sizeX)}-{(int)(y / Chunk.Chunk.sizeY)}"];
            }
        }


        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int pots = tasks.FindIndex(i => i.Name == "pots");

            if (pots != -1)
            {
                tasks[pots] = new PassLegacy("pots", delegate (GenerationProgress progress)
                {
                    Main.tileSolid[137] = true;
                    Main.tileSolid[130] = true;
                    progress.Message = Lang.gen[35].Value;
                    for (int num262 = 0; num262 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num262++)
                    {
                        float num263 = (float)((double)num262 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));
                        progress.Set(num263);
                        bool flag18 = false;
                        int num264 = 0;
                        while (!flag18)
                        {
                            int num265 = WorldGen.genRand.Next((int)WorldGen.worldSurfaceHigh, Main.maxTilesY - 10);
                            if ((double)num263 > 0.93)
                                num265 = Main.maxTilesY - 150;
                            else if ((double)num263 > 0.75)
                                num265 = (int)WorldGen.worldSurfaceLow;

                            int num266 = WorldGen.genRand.Next(1, Main.maxTilesX);
                            bool flag19 = false;
                            for (int num267 = num265; num267 < Main.maxTilesY; num267++)
                            {
                                if (!flag19)
                                {
                                    if (Main.tile[num266, num267].active() && Main.tileSolid[Main.tile[num266, num267].type] && !Main.tile[num266, num267 - 1].lava())
                                        flag19 = true;
                                }
                                else
                                {
                                    int style = WorldGen.genRand.Next(0, 4);
                                    int tileType = 0;
                                    if (num267 < Main.maxTilesY - 5)
                                        tileType = Main.tile[num266, num267 + 1].type;

                                    if (tileType == 147 || tileType == 161 || tileType == 162)
                                        style = WorldGen.genRand.Next(4, 7);

                                    if (tileType == 60)
                                        style = WorldGen.genRand.Next(7, 10);

                                    if (Main.wallDungeon[Main.tile[num266, num267].wall])
                                        style = WorldGen.genRand.Next(10, 13);

                                    if (tileType == 41 || tileType == 43 || tileType == 44)
                                        style = WorldGen.genRand.Next(10, 13);

                                    if (tileType == 22 || tileType == 23 || tileType == 25)
                                        style = WorldGen.genRand.Next(16, 19);

                                    if (tileType == 199 || tileType == 203 || tileType == 204 || tileType == 200)
                                        style = WorldGen.genRand.Next(22, 25);

                                    if (tileType == 367)
                                        style = WorldGen.genRand.Next(31, 34);

                                    if (tileType == 226)
                                        style = WorldGen.genRand.Next(28, 31);

                                    if (num267 > Main.maxTilesY - 200)
                                        style = WorldGen.genRand.Next(13, 16);

                                    if (WorldGen.PlacePot(num266, num267, 28, style))
                                    {
                                        flag18 = true;
                                        break;
                                    }

                                    num264++;
                                    if (num264 >= 10000)
                                    {
                                        flag18 = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
            }

            preChunkGeneration = true;
            specialPostChunkGenPasses.Clear();
            if (worldGenType == "default")
            {
                tasks.Add(new PassLegacy("Creating Chunk", p => CreateChunk(p)));
                tasks.Add(new PassLegacy("ModChunk World Generation", p => GenerateChunk(p)));
                return;
            }
            tasks.Clear();

            WorldGenLoader.CreatePassList(tasks, totalWeight);

            tasks.Add(new PassLegacy("Creating Chunk", p => CreateChunk(p)));
            foreach (GenPass specialPostChunkGenPass in specialPostChunkGenPasses)
            {
                tasks.Add(specialPostChunkGenPass);
            }
            tasks.Add(new PassLegacy("ModChunk World Generation", p => GenerateChunk(p)));
        }

        public override void Initialize()
        {
            chunkList.Clear();
            currentSavePath = Main.ActiveWorldFileData.Path;
            ThreadPool.QueueUserWorkItem(UpdateChunkLoop);
        }

        public override TagCompound Save()
        {
            isSaving = true;
            TagCompound tag = new TagCompound();
            tag.Add("chunkData", SaveChunk());
            isSaving = false;

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            mod.Logger.Info("Mod World Load start");
            if (tag.ContainsKey("chunkData") && tag.GetCompound("chunkData").Count != 0)
            {
                LoadChunk(tag.GetCompound("chunkData"));
            }
            else
            {
                CreateChunk();
            }
            mod.Logger.Info("Mod World Load end");
        }



        //TODO: Put this in InfinityCoreWorld.Chunk.cs
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin();
            List<Chunk.Chunk> ActiveChunkList = chunkList.Values.Where(i => i.IsActive).ToList();
            foreach (Chunk.Chunk chunk in ActiveChunkList)
            {
                chunk.Draw(Main.spriteBatch);
            }
            Main.spriteBatch.End();
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
        }
    }
}
