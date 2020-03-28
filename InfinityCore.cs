using System;
using System.Collections.Generic;
using System.Reflection;
using InfinityCore.API.Loader;
using InfinityCore.Helper.Injection;
using InfinityCore.Helper.ReflectionHelper;
using Terraria.ModLoader;

namespace InfinityCore
{
	partial class InfinityCore : Mod
	{
		internal static InfinityCore instance;

        public static bool EnableInfinityCoreStaticLoader = true;

		public InfinityCore()
		{
			instance = this;
		}

		public override void Load()
		{
            instance = this;
            foreach (Assembly allStaticLoaderEnabledMod in GetAllStaticLoaderEnabledMods())
            {
                ReflectionHelper.InitializeAllStaticLoad(allStaticLoaderEnabledMod);
            }
            LoadContent();
        }

        private List<Assembly> GetAllStaticLoaderEnabledMods()
        {
			List<Assembly> modList = new List<Assembly>();
            foreach (Mod mod in ModLoader.Mods)
            {
                FieldInfo staticLoaderEnabledField = mod.GetType().GetField("EnableInfinityCoreStaticLoader", BindingFlags.Static | BindingFlags.Public);
                if (staticLoaderEnabledField != null)
                {
                    modList.Add(mod.Code);
                }
            }
            return modList;
        }

        private void LoadContent()
        {
            foreach (Mod mod in ModLoader.Mods)
            {
                AutoloadComponent(mod);
            }
        }

        public override void Unload()
		{
			ReflectionHelper.InvokeAllStaticUnload();
			instance = null;
		}
    }
}