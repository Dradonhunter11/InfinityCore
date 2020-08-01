using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfinityCore.API.Interface;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;

namespace InfinityCore.Helper.ReflectionHelper
{
    internal delegate void UpdateWorldsList();

    static partial class ReflectionHelper
    {
        internal static bool InfinityCoreReflectionCache = true; 

        internal static Dictionary<Type, Dictionary<string, Delegate>> methodCache = new Dictionary<Type, Dictionary<string, Delegate>>();
        internal static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        internal static Dictionary<Type, Dictionary<string, FieldInfo>> fieldCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        /// <summary>
        /// For mod injection later on
        /// </summary>
        internal static Dictionary<Type, Dictionary<string, Assembly>> assemblyCache = new Dictionary<Type, Dictionary<string, Assembly>>();

        public static List<Delegate> staticUnload = new List<Delegate>();

        internal static void InitializeAllStaticLoad(Assembly asm)
        {

            foreach (Type type in asm.GetTypes())
            {
                MethodInfo info = type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic);
                if (info != null)
                {
                    info.CreateDelegate(typeof(Action)).DynamicInvoke();
                    info = type.GetMethod("Unload", BindingFlags.Static | BindingFlags.NonPublic);
                    if (info != null)
                    {
                        staticUnload.Add(info.CreateDelegate(typeof(Action)));
                    }
                }
            }
        }

        internal static void InitializeAllStaticPostLoad(Assembly asm)
        {
            foreach (Type type in asm.GetTypes())
            {
                MethodInfo info = type.GetMethod("PostLoad", BindingFlags.Static | BindingFlags.NonPublic);
                if (info != null)
                {
                    info.CreateDelegate(typeof(Action)).DynamicInvoke();
                }
            }
        }

        internal static void CachesAllReflection(Assembly asm)
        {
            if (asm.GetTypes().Any(t => t.Name == "ReflectionHelper" || t.Name == "ReflectionCaches"))
            {
                Type[] reflectionHelperType = asm.GetTypes().Where(t => t.Name == "ReflectionHelper" || t.Name == "ReflectionCaches").ToArray();
                foreach (Type type in reflectionHelperType)
                {
                    if (type.GetField("InfinityCoreReflectionCache", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) != null)
                    {
                        MethodInfo AddFieldInfoCaches = type.GetMethod("AddFieldInfoCaches", BindingFlags.Static | BindingFlags.NonPublic);
                        if (AddFieldInfoCaches != null)
                        {
                            AddFieldInfoCaches.Invoke(null, new[] {fieldCache});
                        }

                        MethodInfo AddMethodInfoCaches = type.GetMethod("AddMethodInfoCaches", BindingFlags.Static | BindingFlags.NonPublic);
                        if (AddMethodInfoCaches != null)
                        {
                            AddMethodInfoCaches.Invoke(null, new[] {methodCache});
                        }

                        MethodInfo AddPropertyInfoCaches = type.GetMethod("AddPropertyInfoCaches", BindingFlags.Static | BindingFlags.NonPublic);
                        if (AddPropertyInfoCaches != null)
                        {
                            AddPropertyInfoCaches.Invoke(null, new[] {propertyCache});
                        }
                    }
                }
            }
        }

        internal static void InvokeAllStaticUnload()
        {
            foreach (Delegate unload in staticUnload)
            {
                unload.DynamicInvoke();
            }

            staticUnload.Clear();
        }

        internal static void Load()
        {

        }

        internal static void PostLoad()
        {
            if (ModLoader.Mods.Any(i => i.Name == "SubworldLibrary"))
            {
                Mod mod = ModLoader.GetMod("SubworldLibrary");
                fieldCache.Add(mod.GetModWorld("SLWorld").GetType(), new Dictionary<string, FieldInfo>()
                {
                    ["generator"] = mod.GetModWorld("SLWorld").GetType().GetField("generator", BindingFlags.NonPublic | BindingFlags.Static)
                });
            }
        }

        internal static void Unload()
        {
            fieldCache.Clear();
            propertyCache.Clear();
            methodCache.Clear();
        }


        private static void AddMethodInfoCaches(Dictionary<Type, Dictionary<string, Delegate>> methodCache)
        {
            methodCache.Add(typeof(UIWorldSelect), new Dictionary<string, Delegate>()
            {
                ["UpdateWorldList"] = typeof(UIWorldSelect).GetMethod("UpdateWorldsList", BindingFlags.NonPublic | BindingFlags.Instance).CreateDelegate(typeof(Action), fieldCache[typeof(Main)]["_worldSelectMenu"].GetValue(null))
            });
        }

        private static void AddFieldInfoCaches(Dictionary<Type, Dictionary<string, FieldInfo>> fieldCache)
        {
            fieldCache.Add(typeof(UIWorldSelect), new Dictionary<string, FieldInfo>()
            {
                ["_worldList"] = typeof(UIWorldSelect).GetField("_worldList", BindingFlags.Instance | BindingFlags.NonPublic),
                ["_backPanel"] = typeof(UIWorldSelect).GetField("_backPanel", BindingFlags.NonPublic | BindingFlags.Instance)
            });

            fieldCache.Add(typeof(Main), new Dictionary<string, FieldInfo>()
            {
                ["_worldSelectMenu"] = typeof(Main).GetField("_worldSelectMenu", BindingFlags.NonPublic | BindingFlags.Static)
            });

            fieldCache.Add(typeof(BuffLoader), new Dictionary<string, FieldInfo>()
            {
                ["buffs"] = typeof(BuffLoader).GetField("buffs", BindingFlags.Static | BindingFlags.NonPublic),
                ["globalBuffs"] = typeof(BuffLoader).GetField("globalBuffs", BindingFlags.Static | BindingFlags.NonPublic)
            });

            fieldCache.Add(typeof(ItemLoader), new Dictionary<string, FieldInfo>()
            {
                ["items"] = typeof(ItemLoader).GetField("items", BindingFlags.Static | BindingFlags.NonPublic),
                ["globalItems"] = typeof(ItemLoader).GetField("globalItems", BindingFlags.Static | BindingFlags.NonPublic)
            });

            fieldCache.Add(typeof(NPCLoader), new Dictionary<string, FieldInfo>()
            {
                ["npcs"] = typeof(NPCLoader).GetField("npcs", BindingFlags.Static | BindingFlags.NonPublic),
                ["globalNPCs"] = typeof(NPCLoader).GetField("globalItems", BindingFlags.Static | BindingFlags.NonPublic)
            });

            fieldCache.Add(typeof(ProjectileLoader), new Dictionary<string, FieldInfo>()
            {
                ["projectiles"] = typeof(ProjectileLoader).GetField("projectiles", BindingFlags.Static | BindingFlags.NonPublic),
                ["globalProjectiles"] = typeof(ProjectileLoader).GetField("globalProjectiles", BindingFlags.Static | BindingFlags.NonPublic)
            });

            fieldCache.Add(typeof(TileLoader), new Dictionary<string, FieldInfo>()
            {
                ["tiles"] = typeof(TileLoader).GetField("tiles", BindingFlags.Static | BindingFlags.NonPublic),
                ["globalTiles"] = typeof(TileLoader).GetField("globalTiles", BindingFlags.Static | BindingFlags.NonPublic),
                ["trees"] = typeof(TileLoader).GetField("trees", BindingFlags.Static | BindingFlags.NonPublic),
                ["palmTrees"] = typeof(TileLoader).GetField("palmTrees", BindingFlags.Static | BindingFlags.NonPublic),
                ["cacti"] = typeof(TileLoader).GetField("cacti", BindingFlags.Static | BindingFlags.NonPublic)
            });

            fieldCache.Add(typeof(WallLoader), new Dictionary<string, FieldInfo>()
            {
                ["walls"] = typeof(WallLoader).GetField("items", BindingFlags.Static | BindingFlags.NonPublic),
                ["globalWalls"] = typeof(WallLoader).GetField("globalItems", BindingFlags.Static | BindingFlags.NonPublic)
            });

            fieldCache.Add(typeof(ModDust), new Dictionary<string, FieldInfo>()
            {
                ["dusts"] = typeof(ModDust).GetField("dusts", BindingFlags.Static | BindingFlags.NonPublic)
            });

            fieldCache.Add(typeof(ModTileEntity), new Dictionary<string, FieldInfo>()
            {
                ["tileEntities"] = typeof(ModTileEntity).GetField("tileEntities", BindingFlags.Static | BindingFlags.NonPublic)
            });
        }
    }
}
