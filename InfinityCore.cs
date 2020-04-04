using InfinityCore.API.ModCompatibility;
using InfinityCore.Helper.ReflectionHelper;
using InfinityCore.Worlds;
using System.Collections.Generic;
using System.Reflection;
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

        public override void PostAddRecipes()
        {
            foreach (Assembly allStaticLoaderEnabledMod in GetAllStaticLoaderEnabledMods())
            {
                ReflectionHelper.InitializeAllStaticPostLoad(allStaticLoaderEnabledMod);
            }

            SubworldLibraryInjection.PreSubworldAddGenerationPass += InfinityCoreWorld.PreSubworldGen;
            SubworldLibraryInjection.PostSubworldAddGenerationPass += InfinityCoreWorld.PostSubworldGen;
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

        public override object Call(params object[] args)
        {
            string command = args[0] as string;
            switch (command)
            {
                case "SetCustomChunkSize":
                    InfinityCoreWorld.SetChunkSize((int)args[1], (int)args[2]);
                    break;
            }
            return base.Call(args);
        }
    }
}