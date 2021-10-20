using System;
using Boggle.Analysis;
using Boggle.BoggleData;
using Boggle.GameUI;
using Boggle.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boggle.Scenes
{
    /// <summary>
    /// The main scene when you are in the game
    /// </summary>
    public class GameScene : Scene
    {
        public GameBoard Board { get; private set; }

        public double Time { get; set; } = 0;
        public int StartTime { get; set; } = 0;
        public double TimeRatio => Time / (double)StartTime;

        public bool Quit { get; private set; } = false;

        public bool ShowBestWords { get; private set; } = false;
        private FoundWord _hovered = null;

        /// <summary>
        /// Initialize a GameScene using a particular board
        /// </summary>
        /// <param name="board">The game board</param>
        /// <param name="startTime">How much time you start with</param>
        /// <param name="graphics">The graphics object</param>
        public GameScene(GameBoard board, int startTime, GraphicsDeviceManager graphics) : base(graphics)
        {
            Board = board;
            Board.Analyze();
            StartTime = startTime;
            Time = StartTime;
            Board.GetLongWord(11);
        }
        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent() { }

        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="gameTime">time since last frame</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            UI.DrawRectangle(_spriteBatch, UI.FullScreenRect, UI.BackgroundColor);

            // draw the board
            Board.Draw(_spriteBatch, _hovered);

            Rectangle button;
            _hovered = null;

            if (ShowBestWords)
            {
                // display the best words found
                int yp = 20;
                int i = 0;
                while (yp < Global.SCREEN_HEIGHT - 100 && i < Board.Words.Count)
                {
                    button = new Rectangle(Global.SCREEN_HEIGHT + 10, yp, Global.SCREEN_WIDTH - Global.SCREEN_HEIGHT - 20, 40);
                    bool over = Input.MouseOver(button);
                    if (over) _hovered = Board.Words[i];
                    UI.DrawTextCentered(_spriteBatch, Board.Words[i].Word, button, over ? UI.HighlightColor : Color.White, 0.5f);
                    yp += 40;
                    i++;
                }
            }
            else
            {
                // display the timer and all the buttons
                // note that button handling is done here because the bounds of the buttons are determined here

                button = new Rectangle(Global.SCREEN_HEIGHT + 10, Global.SCREEN_HEIGHT - 140, Global.SCREEN_WIDTH - Global.SCREEN_HEIGHT - 20, 50);
                UI.DrawRectangle(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
                UI.DrawTextCentered(_spriteBatch, "ROTATE", button, Color.Black, 0.5f);
                if (Input.ClickedOn(button)) Board.TargetRotation += 3.14152 / 2.0;

                button = new Rectangle(Global.SCREEN_HEIGHT + 10, Global.SCREEN_HEIGHT - 200, Global.SCREEN_WIDTH - Global.SCREEN_HEIGHT - 20, 50);
                UI.DrawRectangle(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
                UI.DrawTextCentered(_spriteBatch, "SHUFFLE", button, Color.Black, 0.5f);
                if (Input.ClickedOn(button)) { Board.Shuffle(); Board.Analyze(); Time = StartTime; }

                if (StartTime > 0)
                {
                    Rectangle timer = new Rectangle(Global.SCREEN_HEIGHT + 10 + button.Width / 2 - 40, 10, 80, Global.SCREEN_HEIGHT - 320 - 20);
                    UI.DrawRectangle(_spriteBatch, timer, Time < 0 ? (Time < -0.5 ? new Color(47, 51, 150) : Color.Red) : new Color(29, 32, 105));
                    timer = new Rectangle(timer.Left, (int)(timer.Top + (1.0 - TimeRatio) * timer.Height), timer.Width, (int)(TimeRatio * timer.Height));
                    if (Time > 0) UI.DrawRectangle(_spriteBatch, timer, new Color(47, 51, 150));
                }

                UI.DrawTextCentered(_spriteBatch, UI.SecondsToTime((int)Time), new Rectangle(Global.SCREEN_HEIGHT + 10, 110, Global.SCREEN_WIDTH - Global.SCREEN_HEIGHT - 20, 50), Color.White, 1);

                button = new Rectangle(Global.SCREEN_HEIGHT + 10, Global.SCREEN_HEIGHT - 320, Global.SCREEN_WIDTH - Global.SCREEN_HEIGHT - 20, 50);
                UI.DrawRectangle(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
                if (Input.MouseOver(button))
                {
                    UI.DrawTextCentered(_spriteBatch, "BEST WORDS", button, Color.Black, 0.5f);
                }
                else
                {
                    UI.DrawTextCentered(_spriteBatch, $"{Board.Words.Count} Words Found", UI.MoveY(button, -13), Color.Black, 0.25f);
                    UI.DrawTextCentered(_spriteBatch, $"Longest Word: {Board.LongestWord.Length} Letters", UI.MoveY(button, 13), Color.Black, 0.25f);
                }
                if (Input.ClickedOn(button)) { ShowBestWords = true; }

                button = new Rectangle(Global.SCREEN_HEIGHT + 10, Global.SCREEN_HEIGHT - 260, Global.SCREEN_WIDTH - Global.SCREEN_HEIGHT - 20, 50);
                UI.DrawRectangle(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
                UI.DrawTextCentered(_spriteBatch, "RESET TIME", button, Color.Black, 0.5f);
                if (Input.ClickedOn(button)) Time = StartTime;
            }

            button = new Rectangle(Global.SCREEN_HEIGHT + 10, Global.SCREEN_HEIGHT - 80, Global.SCREEN_WIDTH - Global.SCREEN_HEIGHT - 20, 50);
            UI.DrawRectangle(_spriteBatch, button, Input.MouseOver(button) ? UI.HighlightColor : Color.White);
            UI.DrawTextCentered(_spriteBatch, ShowBestWords ? "BACK" : "QUIT", button, Color.Black, 0.5f);
            if (Input.ClickedOn(button))
            {
                if (ShowBestWords) { ShowBestWords = false; }
                else Quit = true;
            }

            _spriteBatch.End();
        }

        /// <summary>
        /// Update the timer
        /// </summary>
        /// <param name="gameTime">Time since last frame</param>
        public override void Update(GameTime gameTime)
        {
            if(StartTime <= 0)
            {
                Time += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                Time -= gameTime.ElapsedGameTime.TotalSeconds;
                if (Time < -1) Time = 0;
            }
        }

        
    }
}
