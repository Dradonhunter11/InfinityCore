using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.API.Chunks;
using InfinityCore.API.Interface;
using InfinityCore.API.Loader;
using InfinityCore.API.WorldGen;
using Terraria.ModLoader;

namespace InfinityCore
{
	partial class InfinityCore : Mod
	{

        private static void AutoloadComponent(Mod mod)
		{
			if (mod.Code == null)
			{
				return;
			}

			foreach (Type component in mod.Code.GetTypes())
			{
				if (component.IsSubclassOf(typeof(ModWorldGen)))
				{
					WorldGenLoader.RegisterWorldGenOption(component);
				}

                if (component.IsSubclassOf(typeof(ModChunk)))
                {
					ChunkLoader.RegisterModChunk(component);
                }
			}
		}

		internal static void AutoLoadILoadable()
		{

		}

		internal static void AutoUnloadILoadable()
		{

		}

		internal static string GetInternalName(Mod mod, Type type) => mod.Name + ':' + type.Name;

		internal static string GetInternalName(Mod mod, string typeName) => mod.Name + ':' + typeName;
	}
}
