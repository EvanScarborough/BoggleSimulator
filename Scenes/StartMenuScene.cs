using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boggle.GameUI;
using Boggle.Helpers;

namespace Boggle.Scenes
{
    /// <summary>
    /// The start menu scene
    /// </summary>
    public class StartMenuScene : Scene
    {
        private Texture2D _logoTexture;

        private int _selected = 0;
        private List<string> _menuItems = new List<string>() { "START GAME", "OPTIONS", "QUIT" };

        public static readonly int OPTION_X = 100;
        public static readonly int OPTION_Y = 200;
        public static readonly int BOX_SIZE = 50;

        private float _phi = 0;

        public bool ShouldQuit { get; private set; } = false;

        private GameSetupScene _gameSetupScene = null;

        /// <summary>
        /// Initialize the scene
        /// </summary>
        /// <param name="graphics"></param>
        public StartMenuScene(GraphicsDeviceManager graphics) : base(graphics)
        {

        }
        /// <summary>
        /// Load textures and data needed by the scene
        /// </summary>
        protected override void LoadContent()
        {
            _logoTexture = TextureManager.Get("logo");
        }

        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="gameTime">Time since last frame</param>
        public override void Draw(GameTime gameTime)
        {
            // if you've gone to the setup scene, draw that scene, not this one.
            if (_gameSetupScene != null)
            {
                _gameSetupScene.Draw(gameTime);
                return;
            }

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            UI.DrawRectangle(_spriteBatch, UI.FullScreenRect, UI.BackgroundColor);
            // title
            UI.DrawText(_spriteBatch, "Boggle!", new Vector2(100, 50), Color.White, 1.5f);

            // draw the buttons
            _selected = -1;
            for (int i = 0; i < _menuItems.Count; i++)
            {
                // figure out which button you are hovering over
                if(Input.MouseOver(new Rectangle(OPTION_X,OPTION_Y + i * (BOX_SIZE + 10), BOX_SIZE * _menuItems[i].Length, BOX_SIZE)))
                {
                    _selected = i;
                }
                int xp = OPTION_X;
                foreach (char c in _menuItems[i])
                {
                    int yoff = 0;
                    if (_selected == i) { yoff = (int)(Math.Sin(_phi * 5f + (float)xp) * 8.0); }
                    UI.DrawTile(_spriteBatch, new Rectangle(xp + 1, OPTION_Y + i * (BOX_SIZE + 10) + 1 + yoff, BOX_SIZE - 2, BOX_SIZE - 2), "" + c, _selected == i ? UI.HighlightColor : Color.White, Color.Black, 0.5f);
                    xp += BOX_SIZE;
                }
            }
            _phi += 0.02f;
            _spriteBatch.End();
        }

        /// <summary>
        /// Update the scene
        /// </summary>
        /// <param name="gameTime">Time since last frame</param>
        public override void Update(GameTime gameTime)
        {
            // if you're on the setup scene, update that one instead of this
            if (_gameSetupScene != null)
            {
                _gameSetupScene.Update(gameTime);
                // check if the user exited the setup scene
                if (_gameSetupScene.Done) _gameSetupScene = null;
                else return;
            }
            // handle button interactions
            if (_selected >= 0 && Input.Clicked)
            {
                if (_selected == 0) { _gameSetupScene = new GameSetupScene(_graphics); }
                if (_selected == 1) { }
                if (_selected == 2) { ShouldQuit = true; }
                _selected = -1;
            }
        }

        
    }
}
