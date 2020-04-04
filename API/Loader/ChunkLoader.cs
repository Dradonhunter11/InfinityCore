using InfinityCore.API.Chunks;
using InfinityCore.Worlds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace InfinityCore.API.Loader
{
    class ChunkLoader
    {
        internal static IDictionary<string, ModChunk> chunkDictionnary;

        internal static void RegisterModChunk(Type type)
        {
            ModChunk chunk = (ModChunk)Activator.CreateInstance(type);
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
