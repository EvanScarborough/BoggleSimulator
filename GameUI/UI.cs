using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Boggle.Helpers;

namespace Boggle.GameUI
{
    public class UI
    {
        private static GraphicsDeviceManager _graphics;
        private static ContentManager _content;

        public static Color HighlightColor => new Color(252, 232, 141);
        public static Color BackgroundColor => new Color(45, 127, 181);

        public static Texture2D UITexture { get; private set; }

        private static SpriteFont _font;

        public static bool TextInput => Input.Typing();

        public static Rectangle FullScreenRect => new Rectangle(0, 0, Global.SCREEN_WIDTH, Global.SCREEN_HEIGHT);

        public static string SecondsToTime(int seconds)
        {
            return ((seconds / 60) < 10 ? "0" : "") + (seconds / 60).ToString() + ":" + (seconds % 60 < 10 ? "0" : "") + (seconds % 60);
        }

        public static bool Initialize(ContentManager content, GraphicsDeviceManager graphics)
        {
            _content = content;
            _graphics = graphics;
            try
            {
                _font = _content.Load<SpriteFont>("Roboto");
                UITexture = TextureManager.Get("ui");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void DrawText(SpriteBatch spriteBatch, string text, Vector2 pos, Color color, float size)
        {
            spriteBatch.DrawString(_font, text, pos, color, 0, new Vector2(0, 0), size, SpriteEffects.None, 1);
        }
        public static void DrawText(SpriteBatch spriteBatch, string text, Rectangle rect, Color color, float size)
        {
            spriteBatch.DrawString(_font, text, new Vector2(rect.Left, rect.Top), color, 0, new Vector2(0, 0), size, SpriteEffects.None, 1);
        }

        public static void DrawTextTopCentered(SpriteBatch spriteBatch, string text, Vector2 pos, Color color, float size)
        {
            spriteBatch.DrawString(_font, text, new Vector2(pos.X - GetTextWidth(text, size) / 2, pos.Y), color, 0, new Vector2(0, 0), size, SpriteEffects.None, 1);
        }
        public static void DrawTextTopCentered(SpriteBatch spriteBatch, string text, Rectangle rect, Color color, float size)
        {
            spriteBatch.DrawString(_font, text, new Vector2(rect.Left + rect.Width / 2 - GetTextWidth(text, size) / 2, rect.Top), color, 0, new Vector2(0, 0), size, SpriteEffects.None, 1);
        }
        public static void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle rect, Color color, float size)
        {
            spriteBatch.DrawString(_font, text, new Vector2(rect.Left + rect.Width / 2 - GetTextWidth(text, size) / 2, rect.Top + rect.Height / 2 - GetTextHeight(text, size) / 2), color, 0, new Vector2(0, 0), size, SpriteEffects.None, 1);
        }


        public static float GetTextWidth(string text, float size)
        {
            return _font.MeasureString(text).X * size;
        }
        public static float GetTextHeight(string text, float size)
        {
            return _font.MeasureString(text).Y * size;
        }



        public static void DrawUIBox(SpriteBatch spriteBatch, Rectangle area, int border = 50)
        {
            // main area
            spriteBatch.Draw(UITexture, area, new Rectangle(32, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            // corners
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Top - border, border, border), new Rectangle(16, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Top - border, border, border), new Rectangle(48, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Bottom, border, border), new Rectangle(48, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Bottom, border, border), new Rectangle(16, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            // sides
            spriteBatch.Draw(UITexture, new Rectangle(area.Left, area.Top - border, area.Width, border), new Rectangle(32, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Top, border, area.Height), new Rectangle(48, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left, area.Bottom, area.Width, border), new Rectangle(32, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Top, border, area.Height), new Rectangle(16, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
        }
        public static void DrawSimpleUIBox(SpriteBatch spriteBatch, Rectangle area, int border = 50)
        {
            // main area
            spriteBatch.Draw(UITexture, area, new Rectangle(80, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            // corners
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Top - border, border, border), new Rectangle(64, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Top - border, border, border), new Rectangle(96, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Bottom, border, border), new Rectangle(96, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Bottom, border, border), new Rectangle(64, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            // sides
            spriteBatch.Draw(UITexture, new Rectangle(area.Left, area.Top - border, area.Width, border), new Rectangle(80, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Top, border, area.Height), new Rectangle(96, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left, area.Bottom, area.Width, border), new Rectangle(80, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Top, border, area.Height), new Rectangle(64, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
        }
        public static void DrawEmptySimpleUIBox(SpriteBatch spriteBatch, Rectangle area, int border = 50)
        {
            // corners
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Top - border, border, border), new Rectangle(64, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Top - border, border, border), new Rectangle(96, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Bottom, border, border), new Rectangle(96, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Bottom, border, border), new Rectangle(64, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            // sides
            spriteBatch.Draw(UITexture, new Rectangle(area.Left, area.Top - border, area.Width, border), new Rectangle(80, 0, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Right, area.Top, border, area.Height), new Rectangle(96, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left, area.Bottom, area.Width, border), new Rectangle(80, 32, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(UITexture, new Rectangle(area.Left - border, area.Top, border, area.Height), new Rectangle(64, 16, 16, 16), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
        }
        public static void DrawBorderlessSimpleUIBox(SpriteBatch spriteBatch, Rectangle area, int alpha = 255)
        {
            // main area
            spriteBatch.Draw(UITexture, area, new Rectangle(80, 16, 16, 16), new Color(255, 255, 255, alpha), 0, Vector2.Zero, SpriteEffects.None, 0.999f);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle area, Color color, float z = 0.999f)
        {
            spriteBatch.Draw(UITexture, area, new Rectangle(0, 0, 16, 16), color, 0, Vector2.Zero, SpriteEffects.None, z);
        }
        public static void DrawTile(SpriteBatch spriteBatch, Rectangle area, Color color, float z = 0.999f)
        {
            spriteBatch.Draw(UITexture, area, new Rectangle(0, 48, 48, 48), color, 0, Vector2.Zero, SpriteEffects.None, z);
        }
        public static void DrawTile(SpriteBatch spriteBatch, Rectangle area, string letter, Color color, Color textColor, float fontsize, float z = 0.999f)
        {
            spriteBatch.Draw(UITexture, area, new Rectangle(0, 48, 48, 48), color, 0, Vector2.Zero, SpriteEffects.None, z);
            DrawTextCentered(spriteBatch, letter, area, textColor, fontsize);
        }
        public static void DimScreen(SpriteBatch spriteBatch, float z = 0.999f)
        {
            DrawRectangle(spriteBatch, new Rectangle(0, 0, Global.SCREEN_WIDTH, Global.SCREEN_HEIGHT), new Color(0, 0, 0, 180), z);
        }

        public static string GetFittedString(string text, Rectangle rect, float size)
        {
            string ret = "";
            int pos = 0;
            while(pos < text.Length)
            {
                ret += text[pos];
                pos++;
                if (GetTextWidth(ret, size) > rect.Width)
                {
                    int lastSpace = ret.Length - 1;
                    while (lastSpace >= 0 && ret[lastSpace] != ' ' && ret[lastSpace] != '-')
                    {
                        if (ret[lastSpace] == '\n') { lastSpace = -1; break; }
                        lastSpace--;
                    }
                    if (lastSpace >= ret.Length - 1) ret = ret.Substring(0, ret.Length - 1) + "\n";
                    else if (lastSpace <= 0) ret = ret.Substring(0, ret.Length - 2) + "-\n" + ret.Substring(ret.Length - 2);
                    else ret = ret.Substring(0, lastSpace + 1) + "\n" + ret.Substring(lastSpace + 1);
                }
            }
            
            return ret;
        }

        public static void StartTextEntry(string s = "", int maxChars = 20)
        {
            Input.EnableTyping();
            Input.TextInput = s;
            Input.TextMaxChars = maxChars;
        }

        public static readonly Rectangle TextEntryRect = new Rectangle(150, 150, Global.SCREEN_WIDTH - 300, 200);
        private static int _textCursorBlink = 0;
        public static void DrawTextEntry(SpriteBatch spriteBatch, string prompt = "")
        {
            _textCursorBlink++; if (_textCursorBlink >= 40) _textCursorBlink = -40;
            DrawUIBox(spriteBatch, TextEntryRect);
            DrawText(spriteBatch, Input.TextInput + (_textCursorBlink > 0 ? "_" : ""), TextEntryRect, Color.White, 4);
            if (!string.IsNullOrEmpty(prompt)) DrawText(spriteBatch, prompt, new Vector2(TextEntryRect.Left, TextEntryRect.Top - 100), Color.White, 4);
        }





        public static Rectangle AddMargin(Rectangle r, int m)
        {
            return new Rectangle(r.Left - m, r.Top - m, r.Width + 2 * m, r.Height + 2 * m);
        }
        public static Rectangle MoveX(Rectangle r, int m)
        {
            return new Rectangle(r.Left + m, r.Top, r.Width, r.Height);
        }
        public static Rectangle MoveY(Rectangle r, int m)
        {
            return new Rectangle(r.Left, r.Top + m, r.Width, r.Height);
        }
    }
}
