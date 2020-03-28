using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.Helper.UIHelper;
using InfinityCore.UI.WorldSelectUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace InfinityCore.Helper.Injection
{
	static partial class Injection
	{
        //To move in WorldSelectionUIHelper.cs?
        internal static UIElement newFolderElement;
        internal static Texture2D newFolderIcon;
        internal static bool showCreateMenu;
        internal static bool needUpdate = false;
        internal static int updateUILoop = 0; //Value used to force refresh the UI

        internal static UIPanel newFolderTextPanel;

        internal delegate void orig_UpdateUIWorldSelect(UIElement self, GameTime gameTime);
        internal delegate void hook_UpdateUIWorldSelect(orig_UpdateUIWorldSelect orig, UIElement instance, GameTime gameTime);

        internal static event hook_UpdateUIWorldSelect UpdateWorldUISelect_Hook
        {
            add => HookEndpointManager.Add(typeof(UIWorldSelect).GetMethod("Update"), value);
            remove => HookEndpointManager.Remove(typeof(UIWorldSelect).GetMethod("Update"), value);
        }

        internal static void Detour_OnInitializeWorldSelectUI(On.Terraria.GameContent.UI.States.UIWorldSelect.orig_OnInitialize orig, UIWorldSelect instance)
        {
            orig(instance);

            WorldSelectionUIHelper.uiWorldSelect = instance;

            UIPanel reference = (UIPanel) ReflectionHelper.ReflectionHelper.fieldCache[typeof(UIWorldSelect)]["_backPanel"].GetValue(instance);

            var del = (UIElement.MouseEvent) Delegate.CreateDelegate(typeof(UIElement.MouseEvent), ReflectionHelper.ReflectionHelper.fieldCache[typeof(Main)]["_worldSelectMenu"].GetValue(null), "GoBackClick");

            reference.OnClick -= del;

            reference.OnClick += WorldSelectionUIHelper.GoBackClick;

            newFolderIcon = InfinityCore.instance.GetTexture("Textures/NewFolder");

            showCreateMenu = false;

            newFolderElement = new UIElement();
            newFolderElement.Width.Set(newFolderIcon.Width, 0);
            newFolderElement.Height.Set(newFolderIcon.Height, 0);
            newFolderElement.HAlign = 0.5f;
            newFolderElement.VAlign = 0.5f;
            newFolderElement.Left.Set(350, 0);
            newFolderElement.Top.Set(-180, 0);
            WorldSelectionUIHelper.needUpdate = true;
            newFolderElement.OnClick += WorldSelectionUIHelper.NewFolderClick;
            instance.Append(WorldSelectionUIHelper.CreateTextInputField(newFolderElement.GetInnerDimensions().Position()));
            instance.Append(newFolderElement);
        }

		internal static void Detour_UpdateWorldList(On.Terraria.GameContent.UI.States.UIWorldSelect.orig_UpdateWorldsList orig, UIWorldSelect instance)
		{
			orig(instance);
            if (WorldSelectionUIHelper.currentPath == WorldSelectionUIHelper.originalPath)
            {
                UIList reference = (UIList)ReflectionHelper.ReflectionHelper.fieldCache[typeof(UIWorldSelect)]["_worldList"].GetValue(instance);
                List<string> directoryList = Directory.EnumerateDirectories(Main.WorldPath).ToList();
                foreach (var directory in directoryList)
                {
                    reference.Add(new UIFolderItem(directory.Replace(Main.WorldPath + "\\", "")));
                    reference._items.Sort(new Comparison<UIElement>(WorldSelectionUIHelper.SortElement));
                }
            }
        }

        private static void Detour_UIWorldSelectDraw(On.Terraria.GameContent.UI.States.UIWorldSelect.orig_Draw orig, UIWorldSelect self, SpriteBatch spritebatch)
        {
            orig(self, spritebatch);

            CalculatedStyle panelPosition = newFolderElement.GetInnerDimensions();

            spritebatch.Draw(newFolderIcon, panelPosition.Position(), Color.White);
        }


        private static void Detour_UIWorldSelectUpdate(orig_UpdateUIWorldSelect orig, UIElement instance, GameTime gametime)
        {
            orig(instance, gametime);
            if (!(instance is UIWorldSelect))
            {
                return;
            }

            if (!WorldSelectionUIHelper.needUpdate)
            {
                return;
            }
            WorldSelectionUIHelper.needUpdate = false;
            UIList reference = (UIList) ReflectionHelper.ReflectionHelper.fieldCache[typeof(UIWorldSelect)]["_worldList"].GetValue(instance as UIWorldSelect);
            reference.Clear();
            reference.AddRange(WorldSelectionUIHelper.worldListElements);
            instance.Recalculate();
            Main.MenuUI.SetState(instance as UIWorldSelect);
        }
    }
}
