using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.Helper.ReflectionHelper;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace InfinityCore.API.ModCompatibility
{
    public static class SubworldLibraryInjection
    {
        internal static Mod SubWorldLibrary; 

        internal delegate void orig_LoadSubworld(string path, bool fromCloud);
        internal delegate void hook_LoadSubworld(orig_LoadSubworld orig, string path, bool fromCloud);

        public delegate void ModifySubworldTask(WorldGenerator generator);

        public static event ModifySubworldTask PreSubworldAddGenerationPass;
        public static event ModifySubworldTask PostSubworldAddGenerationPass;

        public static event ILContext.Manipulator LoadSubworld
        {
            add
            {
                if (SubWorldLibrary == null)
                {
                    return;
                }
                HookEndpointManager.Modify<hook_LoadSubworld>(MethodBase.GetMethodFromHandle(SubWorldLibrary.GetModWorld("SLWorld").GetType().GetMethod("LoadSubworld", BindingFlags.Static | BindingFlags.NonPublic).MethodHandle), value);
            }
            remove
            {
                if (SubWorldLibrary == null)
                {
                    return;
                }
                HookEndpointManager.Unmodify<hook_LoadSubworld>(MethodBase.GetMethodFromHandle(SubWorldLibrary.GetModWorld("SLWorld").GetType().GetMethod("LoadSubworld", BindingFlags.Static | BindingFlags.NonPublic).MethodHandle), value);
            }
        }

        internal static void Load()
        {

        }

        internal static void PostLoad()
        {
            if (ModLoader.Mods.Any(i => i.Name == "SubworldLibrary"))
            {
                InfinityCore.instance.Logger.Info("Subworld Library detected, starting IL injection");
                SubWorldLibrary = ModLoader.GetMod("SubworldLibrary");
                LoadSubworld += LoadSubworldIL;
            }
        }

        internal static void Unload()
        {
            SubWorldLibrary = null;
        }

        private static void LoadSubworldIL(ILContext il)
        {
            var c = new ILCursor(il);
            foreach (Instruction instruction in c.Instrs)
            {
                object obj = (instruction.Operand == null) ? "" : instruction.Operand.ToString();
                InfinityCore.instance.Logger.Info($"{instruction.Offset} | {instruction.OpCode} | {obj}");
            }
            PreSubworldAddGenerationPassHook(c);
            PostSubworldAddGenerationPassHook(c);
        }

        

        private static void PreSubworldAddGenerationPassHook(ILCursor c)
        {
            c.Index = 0;

            if (c.TryGotoNext(i => i.MatchLdcI4(out _),
                i => i.MatchStloc(out _),
                i => i.MatchBr(out _)))
            {
                InfinityCore.instance.Logger.Info("PreSubworldAddGenerationPassHook added");
                c.EmitDelegate<Action>(() =>
                {
                    
                    WorldGenerator generator = (WorldGenerator) ReflectionHelper.fieldCache[SubWorldLibrary.GetModWorld("SLWorld").GetType()]["generator"].GetValue(null);
                    PreSubworldAddGenerationPass(generator);
                });
            }
        }

        private static void PostSubworldAddGenerationPassHook(ILCursor c)
        {
            if (c.TryGotoNext(i => i.MatchLdnull()))
            {
                c.Index -= 1;
                InfinityCore.instance.Logger.Info("PostSubworldAddGenerationPassHook added");
                c.EmitDelegate<Action>(() =>
                {
                    WorldGenerator generator = (WorldGenerator) ReflectionHelper.fieldCache[SubWorldLibrary.GetModWorld("SLWorld").GetType()]["generator"].GetValue(null);
                    PostSubworldAddGenerationPass(generator);
                });
            }
        }
    }
}
