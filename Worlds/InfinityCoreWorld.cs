using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfinityCore.API.Loader;
using InfinityCore.API.WorldGen;
using InfinityCore.Worlds.Chunk;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace InfinityCore.Worlds
{
	partial class InfinityCoreWorld : ModWorld
	{
        internal static string worldGenType = "default";

        internal string currentSavePath;

        internal static bool isSaving = false;

        internal Chunk.Chunk this[int x, int y]
        {
            get => chunkList[$"region{x / 200}{y / 150}"];
        }

        

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{   
            tasks.Add(new PassLegacy("Creating Chunk", p => CreateChunk(p)));
			if (worldGenType == "default")
			{
				return;
			}
			tasks.Clear();

			WorldGenLoader.CreatePassList(tasks, totalWeight);

            tasks.Add(new PassLegacy("Creating Chunk", p => CreateChunk(p)));
            tasks.Add(new PassLegacy("ModChunck World Generation", p => GenerateChunk(p)));
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
			tag.Add("chunkData",SaveChunk());
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
