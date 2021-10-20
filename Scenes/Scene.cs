using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boggle.Scenes
{
    /// <summary>
    /// A scene handles display and interaction. Contains its own spritebatch.
    /// </summary>
    public abstract class Scene
    {
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;

        /// <summary>
        /// Initialize the scene
        /// </summary>
        /// <param name="graphics">The graphics manager used by the spritebatch</param>
        public Scene(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            LoadContent();
        }
        /// <summary>
        /// Load any content needed for the scene
        /// </summary>
        protected abstract void LoadContent();
        /// <summary>
        /// Update the scene
        /// </summary>
        /// <param name="gameTime">Time since last frame</param>
        public abstract void Update(GameTime gameTime);
        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="gameTime">Time since last frame</param>
        public abstract void Draw(GameTime gameTime);
    }
}
