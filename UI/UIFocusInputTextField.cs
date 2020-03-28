using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace InfinityCore.UI
{
    /// <summary>
    /// Taken directly from tmodloader
    /// TODO: Make the tmodloader one public move it to Terraria.ModLoader.UI.UIElements
    /// </summary>
    internal class UIFocusInputTextField : UIPanel
    {
        internal bool Focused = false;
        internal string CurrentString = "";

        private readonly string _hintText;
        private int _textBlinkerCount;
        private int _textBlinkerState;
        public bool UnfocusOnTab { get; internal set; } = false;

        public delegate void EventHandler(UIElement listeningElement);

        public event EventHandler OnTextChange;
        public event EventHandler OnUnfocus;
        public event EventHandler OnTab;

        public UIFocusInputTextField(string hintText)
        {
            _hintText = hintText;
        }

        public void SetText(string text)
        {
            if (text == null)
                text = "";

            if (CurrentString != text)
            {
                CurrentString = text;
                OnTextChange?.Invoke(this);
            }
        }

        public override void Click(UIMouseEvent evt)
        {
            Main.clrInput();
            Focused = true;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (!ContainsPoint(MousePosition) && Main.mouseLeft) // TODO: && focused maybe?
            {
                Focused = false;
                OnUnfocus?.Invoke(this);
            }
            base.Update(gameTime);
        }
        private static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            //Rectangle hitbox = GetInnerDimensions().ToRectangle();
            //Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Red * 0.6f);

            if (Focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newString = Main.GetInputText(CurrentString);
                if (!newString.Equals(CurrentString))
                {
                    CurrentString = newString;
                    OnTextChange?.Invoke(this);
                }
                else
                {
                    CurrentString = newString;
                }
                if (JustPressed(Keys.Tab))
                {
                    if (UnfocusOnTab)
                    {
                        Focused = false;
                        OnUnfocus?.Invoke(this);
                    }
                    OnTab?.Invoke(this);
                }
                if (++_textBlinkerCount >= 20)
                {
                    _textBlinkerState = (_textBlinkerState + 1) % 2;
                    _textBlinkerCount = 0;
                }
            }
            string displayString = CurrentString;
            if (_textBlinkerState == 1 && Focused)
            {
                displayString += "|";
            }
            CalculatedStyle space = GetDimensions();
            if (CurrentString.Length == 0 && !Focused)
            {
                Vector2 vector = Main.fontMouseText.MeasureString(_hintText);
                Utils.DrawBorderString(spriteBatch, _hintText, new Vector2(space.X + 6, space.Y + Height.Pixels - vector.Y - 5), Color.Gray);
            }
            else
            {
                Vector2 vector = Main.fontMouseText.MeasureString(displayString);
                Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X + 6, space.Y + Height.Pixels - vector.Y - 5), Color.White);
            }
        }
    }
}
