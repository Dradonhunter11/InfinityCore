using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.API.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace InfinityCore.API.Loader
{
    class TreeLoader
    {
        private static IDictionary<int, ModTree> trees;

        internal static void Load()
        {
            trees = (IDictionary<int, ModTree>)typeof(TileLoader).GetField("trees", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            IL.Terraria.Main.DrawTiles += DrawTileIL;
        }

        internal static void DrawTileIL(ILContext il)
        {
            var c = new ILCursor(il);

            InjectionTreeTopHook(c);

            c = new ILCursor(il);
            
            InjectionTreeBranchHook(c);
        }

        private static void InjectionTreeBranchHook(ILCursor c)
        {
            c.Index = 0;
            int treeBranchType = 0;

            if(c.TryGotoNext(i => i.MatchCall(out _),
                i => i.MatchLdfld(out _),
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdcI4(out _),
                i => i.MatchLdloca(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchCall(out _),
                i => i.MatchStloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop()))
            {
                c.Index += 2;
                c.Emit(OpCodes.Dup);
                c.EmitDelegate<Action<int>>(i => { treeBranchType = i; });
            }

            if (c.TryGotoNext(
                i => i.MatchCallvirt(typeof(SpriteBatch), "Draw"),
                i => i.MatchBr(out _), 
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchCallvirt(out _),
                i => i.MatchLdcI4(out _),
                i => i.MatchBle(out _)))
            {
                c.Remove();
                c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, float, Vector2, float, SpriteEffects, float>>((spritebatch, texture, position, sourceRectangle, color, rotation, origin, scale, effect, layerDept) =>
                {
                    spritebatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effect, layerDept);

                    PostDrawTreeBranch(treeBranchType, spritebatch, position, sourceRectangle, origin);
                });
            }

            if(c.TryGotoNext(i => i.MatchCall(out _),
                i => i.MatchLdfld(out _),
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdcI4(out _),
                i => i.MatchLdloca(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchCall(out _),
                i => i.MatchStloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop()))
            {
                c.Index += 2;
                c.Emit(OpCodes.Dup);
                c.EmitDelegate<Action<int>>(i => { treeBranchType = i; });
            }

            if (c.TryGotoNext(
                i => i.MatchCallvirt(typeof(SpriteBatch), "Draw"),
                i => i.MatchBr(out _),
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchCallvirt(out _),
                i => i.MatchLdcI4(out _),
                i => i.MatchBle(out _)))
            {
                c.Remove();
                c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, float, Vector2, float, SpriteEffects, float>>((spritebatch, texture, position, sourceRectangle, color, rotation, origin, scale, effect, layerDept) =>
                {
                    spritebatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effect, layerDept);

                    PostDrawTreeBranch(treeBranchType, spritebatch, position, sourceRectangle, origin);
                });
            }


        }

        private static void InjectionTreeTopHook(ILCursor c)
        {
            int treeTopType = 0;

            if (c.TryGotoNext(
                i => i.MatchCall(out _),
                i => i.MatchLdfld(out _),
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloca(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloca(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloca(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloca(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchLdloca(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchCall(out _),
                i => i.MatchStloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop()))
            {
                c.Index += 2;
                c.Emit(OpCodes.Dup);
                c.EmitDelegate<Action<int>>(i => { treeTopType = i; });
            }

            if (c.TryGotoNext(
                i => i.MatchCallvirt(typeof(SpriteBatch), "Draw"),
                i => i.MatchBr(out _), 
                i => i.MatchLdloc(out _),
                i => i.MatchNop(),
                i => i.MatchNop(),
                i => i.MatchCallvirt(out _),
                i => i.MatchLdcI4(out _),
                i => i.MatchBle(out _)))
            {
                c.Remove();
                c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, float, Vector2, float, SpriteEffects, float>>((spritebatch, texture, position, sourceRectangle, color, rotation, origin, scale, effect, layerDept) =>
                {
                    spritebatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effect, layerDept);

                    PostDrawTreeTop(treeTopType, spritebatch, position, sourceRectangle, origin);
                    //Get tree type?
                });
            }
        }

        public static void PostDrawTreeTop(int type, SpriteBatch sb, Vector2 position, Rectangle? sourceRectangle, Vector2 origin)
        {
            if (trees.ContainsKey(type) && trees[type] is ITreeHook tree)
            {
                tree.PostDrawTreeTop(sb, position, sourceRectangle, origin);
            }
        }

        public static void PostDrawTreeBranch(int type, SpriteBatch sb, Vector2 position, Rectangle? sourceRectangle, Vector2 origin)
        {
            if (trees.ContainsKey(type) && trees[type] is ITreeHook tree)
            {
                tree.PostDrawTreeBranch(sb, position, sourceRectangle, origin);
            }
        }
    }
}
