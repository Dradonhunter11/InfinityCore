using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityCore.Helper.ReflectionHelper;
using InfinityCore.Helper.UIHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.UI;

namespace InfinityCore.UI.WorldSelectUI
{
	internal delegate bool UnlockCondition();

	class UIFolderItem : UIPanel
	{
		private UIImage _folderIcon;
		private UIImage _lockIcon;

		private event UnlockCondition condition;

		private string _path = "Testing";
		private string _name = "Testing";

        public UIFolderItem(string name)
        {
            this._name = name;
            this._path = name;
        }

		public override void OnInitialize()
		{
			InitializeAppearance();
			_folderIcon = new UIImage(GetFolderIcon());
			_lockIcon = new UIImage(GetLockIcon());
			_folderIcon.Left.Set(4f, 0f);
			_lockIcon.Top.Set(4f, 1f);
			_lockIcon.Left.Set(4f, 1f);

			base.Append(_lockIcon);
			base.Append(_folderIcon);
			this.OnClick += OnClickEvent;
		}

		private void InitializeAppearance()
		{
			this.Height.Set(76f, 0f);
			this.Width.Set(0f, 1f);
			base.SetPadding(6f);
			this.BorderColor = new Color(89, 116, 213) * 0.7f;
		}

		private Texture2D GetFolderIcon()
		{
			if (condition != null)
			{
				return InfinityCore.instance.GetTexture((condition.Invoke()) ? "Textures/UIWorldSelect/FolderLocked" : "Textures/UIWorldSelect/FolderUnlocked");
			}
			return InfinityCore.instance.GetTexture("Textures/UIWorldSelect/FolderUnlocked");
		}

		private Texture2D GetLockIcon()
		{
			if (condition != null)
			{
				return InfinityCore.instance.GetTexture((condition.Invoke()) ? "Textures/UIWorldSelect/LockUnlocked" : "Textures/UIWorldSelect/LockLocked");
			}
			return InfinityCore.instance.GetTexture("Textures/UIWorldSelect/LockUnlocked");
		}

		public void OnClickEvent(UIMouseEvent mouseEvent, UIElement respondingElement)
		{
			Main.WorldPath = Path.Combine(Main.SavePath, "worlds", this._path);
			WorldSelectionUIHelper.currentPath = Path.Combine(Main.SavePath, "worlds", this._path);
			Directory.CreateDirectory(Main.WorldPath);
			Main.LoadWorlds();
			ReflectionHelper.methodCache[typeof(UIWorldSelect)]["UpdateWorldList"].DynamicInvoke(new object[] { });
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle style = GetInnerDimensions();
            Utils.DrawBorderString(spriteBatch, _name, new Vector2(style.X + 69f, style.Y), Color.White, 1f);
			//spriteBatch.DrawString(Main.fontMouseText, _name, new Vector2(style.X + 80f, style.Y), Color.White);
		}

		public override int CompareTo(object obj)
		{
            if (obj is UIFolderItem folder)
            {
                return _name.CompareTo(folder._name);
            }

            return -1;
        }
	}
}
