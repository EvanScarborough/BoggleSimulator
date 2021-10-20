using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boggle.GameUI;
using Boggle.Helpers;
using Boggle.Scenes;
using Boggle.GameData;
using Boggle.BoggleData;
using Boggle.Analysis;

namespace Boggle
{
    public class Boggle : Game
    {
        private GraphicsDeviceManager _graphics;
        private StartMenuScene _scene;

        private RenderTarget2D _renderTarget;
        private SpriteBatch _spriteBatch;

        Exception e = null;

        public Boggle()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// The Initialize method is called once at the start
        /// </summary>
        protected override void Initialize()
        {
            // set screen size
            _graphics.PreferredBackBufferWidth = Global.SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = Global.SCREEN_HEIGHT;

            // creating the spritebatch that will draw sprites
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // a render target other than the window can be used to preserve hard edges even with screen resizing. good for pixel art!
            _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, Global.SCREEN_WIDTH, Global.SCREEN_HEIGHT);

            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            _graphics.ApplyChanges();
            // initialize the input handler and game data handler
            if (!Input.Initialize(Window) || !GameDataManager.Initialize())
            {
                Exit();
            }
            base.Initialize();
        }

        /// <summary>
        /// Load content is the best place to load any textures and data associated with the game
        /// </summary>
        protected override void LoadContent()
        {
            if(!TextureManager.Initialize(Content, _graphics) || !UI.Initialize(Content, _graphics) || !BoardManager.Initialize())
            {
                Exit();
            }
            WordBank.Initialize();
            // set the starting scene
            _scene = new StartMenuScene(_graphics);
        }

        /// <summary>
        /// Update the game data
        /// </summary>
        /// <param name="gameTime">the time elapsed since last frame</param>
        protected override void Update(GameTime gameTime)
        {
            // just update the scene. the base scene is responsible for updating any child scenes
            _scene.Update(gameTime);
            if (_scene.ShouldQuit) Exit();

            base.Update(gameTime);
            Input.Update(GraphicsDevice.PresentationParameters.Bounds);
        }

        /// <summary>
        /// Draw the game to the screen
        /// </summary>
        /// <param name="gameTime">time since last frame</param>
        protected override void Draw(GameTime gameTime)
        {
            // draw to the special render target
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.Black);

            _scene.Draw(gameTime);

            // then draw the render target to the window, preserving hard edges
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_renderTarget, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
