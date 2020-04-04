﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;

namespace InfinityCore.Helper.ReflectionHelper
{
    internal delegate void UpdateWorldsList();

    static partial class ReflectionHelper
    {
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
            LoadWorldSelectionReflection();
        }

        internal static void PostLoad()
        {
            if (ModLoader.Mods.Any(i => i.Name == "SubworldLibrary"))
            {
                Mod mod = ModLoader.GetMod("SubworldLibrary");
                ReflectionHelper.fieldCache.Add(mod.GetModWorld("SLWorld").GetType(), new Dictionary<string, FieldInfo>()
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

        private static void LoadWorldSelectionReflection()
        {
            AddFieldInfoCaches();
            AddMethodInfoCaches();
        }

        private static void AddMethodInfoCaches()
        {
            methodCache.Add(typeof(UIWorldSelect), new Dictionary<string, Delegate>()
            {
                ["UpdateWorldList"] = typeof(UIWorldSelect).GetMethod("UpdateWorldsList", BindingFlags.NonPublic | BindingFlags.Instance).CreateDelegate(typeof(Action), fieldCache[typeof(Main)]["_worldSelectMenu"].GetValue(null))
            });
        }

        private static void AddFieldInfoCaches()
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
        }
    }
}
