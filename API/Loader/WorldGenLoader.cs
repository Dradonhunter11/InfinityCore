﻿using InfinityCore.API.WorldGen;
using InfinityCore.Worlds;
using System;
using System.Collections.Generic;
using Terraria.World.Generation;

namespace InfinityCore.API.Loader
{
    public static class WorldGenLoader
    {
        private static IDictionary<string, ModWorldGen> worldGenDictionnary = new Dictionary<string, ModWorldGen>();

        public static int Count => worldGenDictionnary.Count;

        internal static void RegisterWorldGenOption(Type type)
        {
            ModWorldGen worldGen = (ModWorldGen)Activator.CreateInstance(type);
        }

        internal static List<GenPass> CreatePassList(List<GenPass> passList, float passWeight) => worldGenDictionnary[InfinityCoreWorld.worldGenType].CreateGenPassList();
    }
}
