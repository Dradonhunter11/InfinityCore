using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.API.Chunks;
using InfinityCore.API.WorldGen;
using InfinityCore.Worlds;
using InfinityCore.Worlds.Chunk;
using Microsoft.Xna.Framework;

namespace InfinityCore.API.Loader
{
    class ChunkLoader
    {
        internal static IDictionary<string, ModChunk> chunkDictionnary;

        internal static void RegisterModChunk(Type type)
        {
            ModChunk chunk = (ModChunk) Activator.CreateInstance(type);
            chunkDictionnary.Add(type.FullName, chunk);
        }

        internal static void Unload()
        {
            chunkDictionnary.Clear();
            chunkDictionnary = null;
        }

        internal static void Load()
        {
            chunkDictionnary = new Dictionary<string, ModChunk>();
        }

        public static bool IsActive(Vector2 position)
        {
            return InfinityCoreWorld.chunkList[$"region{position.X / 200}{position.Y / 150}"].IsActive;
        }
    }
}
