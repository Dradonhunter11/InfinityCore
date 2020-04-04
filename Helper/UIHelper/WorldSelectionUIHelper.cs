using InfinityCore.UI;
using InfinityCore.UI.WorldSelectUI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.UI;

namespace InfinityCore.Helper.UIHelper
{
    class WorldSelectionUIHelper
    {
        internal static string currentPath = Main.WorldPath;

        internal static string originalPath = Path.Combine(Main.SavePath, "Worlds");

        internal static UIWorldSelect uiWorldSelect;

        private static AdvancedUIPanel panel;
        private static UIFocusInputTextField textField;

        internal static bool needUpdate = false;

        internal static readonly List<UIElement> worldListElements = new List<UIElement>();

        internal static void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(11, -1, -1, 1, 1f, 0f);
            if (currentPath != originalPath)
            {
                currentPath = originalPath;
                Main.WorldPath = originalPath;
                Main.LoadWorlds();
                ReflectionHelper.ReflectionHelper.methodCache[typeof(UIWorldSelect)]["UpdateWorldList"].DynamicInvoke(new object[] { });
                needUpdate = true;
                return;
            }
            Main.menuMode = (Main.menuMultiplayer ? 12 : 1);
        }

        internal static int SortElement(UIElement element1, UIElement element2)
        {
            if (element1 is UIFolderItem && element2 is UIWorldListItem)
            {
                return -1;
            }
            if (element1 is UIWorldListItem && element2 is UIFolderItem)
            {
                return +1;
            }
            return element1.CompareTo(element2);
        }

        internal static void NewFolderClick(UIMouseEvent evt, UIElement listeningElement)
        {
            panel.isVisible = !panel.isVisible;
            textField.SetText("");
        }

        internal static AdvancedUIPanel CreateTextInputField(Vector2 position)
        {
            panel = new AdvancedUIPanel();
            panel.Height.Set(50, 0);
            panel.Top.Set(position.Y + 65, 0);
            panel.Left.Set(position.X, 0);
            panel.Width.Set(250, 0);

            textField = new UIFocusInputTextField("Enter folder name here");
            textField.Left.Set(0, 0);
            textField.Width.Set(180, 0);
            textField.Height.Set(40, 0);
            textField.Top.Set(0, 0);

            UIText text = new UIText("Create Folder", 0.5f, false);
            text.Left.Set(10, 0);
            text.HAlign = 1;
            text.OnClick += CreateFolderClick;
            panel.Append(textField);
            panel.Append(text);
            return panel;
        }

        internal static void CreateFolderClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (string.IsNullOrEmpty(textField.CurrentString) || !CheckForIllegalCharacter())
            {
                return;
            }

            string newDirectoryPath = Path.Combine(Main.WorldPath, textField.CurrentString);
            if (Directory.Exists(newDirectoryPath))
            {
                return;
            }

            Directory.CreateDirectory(newDirectoryPath);
            ReflectionHelper.ReflectionHelper.methodCache[typeof(UIWorldSelect)]["UpdateWorldList"].DynamicInvoke(new object[] { });
            needUpdate = true;
        }

        internal static bool CheckForIllegalCharacter()
        {
            return !textField.CurrentString.Contains("<") ||
                   !textField.CurrentString.Contains(">") ||
                   !textField.CurrentString.Contains(":") ||
                   !textField.CurrentString.Contains("\"") ||
                   !textField.CurrentString.Contains("/") ||
                   !textField.CurrentString.Contains("\\") ||
                   !textField.CurrentString.Contains("|") ||
                   !textField.CurrentString.Contains("?") ||
                   !textField.CurrentString.Contains("*");
        }

        //public static void AddFolderToTheList
    }
}
