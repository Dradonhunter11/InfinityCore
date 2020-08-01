using System;
using InfinityCore.API.ModCompatibility;
using InfinityCore.Helper.ReflectionHelper;
using InfinityCore.Worlds;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Terraria;
using Terraria.ModLoader;

namespace InfinityCore
{
    public partial class InfinityCore : Mod
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

        public static Video LoadVideo(Mod mod, string path)
        {
            if (!IsTMLFNA64())
            {
                throw new Exception("Cannot load video on XNA tML, pls install tML 64bit to use this feature.");
            }

            Video result = null;
            string fileName = path.Split('/')[path.Split('/').Length - 1];

            try
            {
                if (File.Exists(Path.Combine(Main.SavePath, "video", mod.Name + "." + fileName + ".ogv")))
                {
                    ConstructorInfo info = typeof(Video).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] {typeof(string), typeof(GraphicsDevice)}, null);
                    object[] obj = new object[] {Path.Combine(Main.SavePath, "video", mod.Name + "." + fileName + ".ogv"), Main.graphics.GraphicsDevice};
                    return (Video) info.Invoke(obj);
                }
                byte[] stream = ModContent.GetFileBytes(path + ".ogv");
                if (!Directory.Exists(Path.Combine(Main.SavePath, "video")))
                {
                    Directory.CreateDirectory(Path.Combine(Main.SavePath, "video"));
                }
                File.WriteAllBytes(Path.Combine(Main.SavePath, "video", mod.Name + "." + fileName + ".ogv"), stream);
                return LoadVideo(mod, path);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }

        public override void PostAddRecipes()
        {
            foreach (Assembly allStaticLoaderEnabledMod in GetAllStaticLoaderEnabledMods())
            {
                ReflectionHelper.InitializeAllStaticPostLoad(allStaticLoaderEnabledMod);
                ReflectionHelper.CachesAllReflection(allStaticLoaderEnabledMod);
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

        public static bool IsTMLFNA64()
        {
            return typeof(Vector2).Assembly == typeof(SpriteBatch).Assembly;
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