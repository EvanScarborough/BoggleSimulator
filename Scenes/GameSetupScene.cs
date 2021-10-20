using System;
using System.Collections.Generic;
using System.Linq;
using Boggle.BoggleData;
using Boggle.GameUI;
using Boggle.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boggle.Scenes
{
    /// <summary>
    /// The scene that allows you to select a game board and timer
    /// </summary>
    public class GameSetupScene : Scene
    {
        public int Seconds { get; private set; } = 3 * 60;

        public List<GameBoard> Boards;
        public int SelectedBoard { get; private set; } = 0;

        public static readonly int BOX_SIZE = 50;
        private float _phi = 0;
        private bool _startSelected = false;
        private bool _backSelected = false;

        private GameScene _gameScene = null;

        public bool Done { get; private set; }

        /// <summary>
        /// Initialize the scene
        /// </summary>
        /// <param name="graphics">The graphics object</param>
        public GameSetupScene(GraphicsDeviceManager graphics) : base(graphics)
        {
            // get a list of all the board types available
            Boards = BoardManager.Boards.Select(b => b.Value).ToList();
        }
        /// <summary>
        /// Load content for the scene
        /// </summary>
        protected override void LoadContent() { }


        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="gameTime">Time since last frame</param>
        public override void Draw(GameTime gameTime)
        {
            // if you've started the game, draw the game, not this menu
            if (_gameScene != null)
            {
                _gameScene.Draw(gameTime);
                return;
            }

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            UI.DrawRectangle(_spriteBatch, UI.FullScreenRect, UI.BackgroundColor);
            // title
            UI.DrawText(_spriteBatch, "Game Setup", new Rectangle(30, 30, 0, 0), Color.White, 0.5f);

            // Board selection option
            UI.DrawText(_spriteBatch, Boards[SelectedBoard].Name, new Rectangle(60, 80, 0, 0), Color.White, 0.5f);
            Rectangle button = new Rectangle(20, 80, 30, 30);
            UI.DrawTile(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
            _spriteBatch.Draw(UI.UITexture, button, new Rectangle(48, 48, 16, 16), Color.Gray);
            if (Input.ClickedOn(button)) { SelectedBoard--; if (SelectedBoard < 0) SelectedBoard = Boards.Count - 1; }

            // buttons to select a board type
            button = new Rectangle(300, 80, 30, 30);
            UI.DrawTile(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
            _spriteBatch.Draw(UI.UITexture, button, new Rectangle(64, 48, 16, 16), Color.Gray);
            if (Input.ClickedOn(button)) { SelectedBoard++; if (SelectedBoard >= Boards.Count) SelectedBoard = 0; }


            // timer selection
            UI.DrawText(_spriteBatch, "Time: " + (Seconds > 0 ? UI.SecondsToTime(Seconds) : "Untimed"), new Rectangle(60, 120, 0, 0), Color.White, 0.5f);
            button = new Rectangle(20, 120, 30, 30);
            UI.DrawTile(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
            _spriteBatch.Draw(UI.UITexture, button, new Rectangle(64, 64, 16, 16), Color.Gray);
            if (Input.ClickedOn(button)) { Seconds -= 15; if (Seconds < 0) Seconds = 0; }

            // buttons to increase or decrease the timer
            button = new Rectangle(300, 120, 30, 30);
            UI.DrawTile(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
            _spriteBatch.Draw(UI.UITexture, button, new Rectangle(48, 64, 16, 16), Color.Gray);
            if (Input.ClickedOn(button)) { Seconds += 15;}


            // start button
            int xp = 40;
            int yp = 200;
            _startSelected = Input.MouseOver(new Rectangle(xp, yp, BOX_SIZE * 5, BOX_SIZE));
            foreach (char c in "START")
            {
                int yoff = 0;
                if (_startSelected) { yoff = (int)(Math.Sin(_phi * 5f + (float)xp) * 8.0); }
                UI.DrawTile(_spriteBatch, new Rectangle(xp + 1, yp + 1 + yoff, BOX_SIZE - 2, BOX_SIZE - 2), "" + c, _startSelected ? UI.HighlightColor : Color.White, Color.Black, 0.5f);
                xp += BOX_SIZE;
            }
            if (_startSelected) _phi += 0.02f;

            // back button
            xp = 40;
            yp = Global.SCREEN_HEIGHT - BOX_SIZE - 40;
            _backSelected = Input.MouseOver(new Rectangle(xp, yp, BOX_SIZE * 5, BOX_SIZE));
            foreach (char c in "BACK")
            {
                int yoff = 0;
                if (_backSelected) { yoff = (int)(Math.Sin(_phi * 5f + (float)xp) * 8.0); }
                UI.DrawTile(_spriteBatch, new Rectangle(xp + 1, yp + 1 + yoff, BOX_SIZE - 2, BOX_SIZE - 2), "" + c, _backSelected ? UI.HighlightColor : Color.White, Color.Black, 0.5f);
                xp += BOX_SIZE;
            }
            if (_backSelected) _phi += 0.02f;

            _spriteBatch.End();
        }

        /// <summary>
        /// Update the scene
        /// </summary>
        /// <param name="gameTime">Time since last frame</param>
        public override void Update(GameTime gameTime)
        {
            // don't update this scene if you're in the game scene
            if (_gameScene != null)
            {
                _gameScene.Update(gameTime);
                if (_gameScene.Quit) _gameScene = null;
                else return;
            }
            if (_startSelected && Input.Clicked)
            {
                _gameScene = new GameScene(Boards[SelectedBoard], Seconds, _graphics);
                _startSelected = false;
            }
            if (_backSelected && Input.Clicked)
            {
                Done = true;
                _backSelected = false;
            }
        }

        
    }
}