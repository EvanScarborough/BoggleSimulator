using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Boggle.GameData;

namespace Boggle.GameUI
{
    public class TextureManager
    {
        private class TextureSub
        {
            private Texture2D Texture { get; set; }
            private int _usecount = 0;

            public TextureSub(Texture2D t)
            {
                Texture = t;
            }

            public Texture2D CheckOut()
            {
                _usecount++;
                return Texture;
            }
            public bool Return()
            {
                _usecount--;
                return _usecount <= 0;
            }
        }


        private static GraphicsDeviceManager _graphics;
        private static ContentManager _content;

        private static Texture2D _missingTexture;
        private static Dictionary<string, TextureSub> _textures = new Dictionary<string, TextureSub>();

        public static bool Initialize(ContentManager content, GraphicsDeviceManager graphics)
        {
            _content = content;
            _graphics = graphics;
            try
            {
                using (var stream = TitleContainer.OpenStream("Content/missing.png"))
                {
                    _missingTexture = Texture2D.FromStream(_graphics.GraphicsDevice, stream);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Texture2D Get(string name)
        {
            return Get(name.Split(new char[] { '/', '\\' }));
        }
        public static Texture2D Get(params string[] path)
        {
            string file = Path.Combine(path);
            if (!_textures.ContainsKey(file))
            {
                Load(file);
            }
            if (!_textures.ContainsKey(file)) return _missingTexture;
            return _textures[file].CheckOut();
        }
        public static bool Return(string name)
        {
            return Return(name.Split(new char[] { '/', '\\' }));
        }
        public static bool Return(params string[] path)
        {
            string file = Path.Combine(path);
            if (!_textures.ContainsKey(file)) return false;
            if (_textures[file].Return())
            {
                _textures.Remove(file);
                return true;
            }
            return false;
        }


        private static bool Load(string file)
        {
            if (Load(file, Path.Combine(GameDataManager.RelativeContentDir, "Texture")))
            {
                return true;
            }
            return false;
        }
        private static bool Load(string file, string path)
        {
            string fullpath = Path.Combine(path, file);
            if (!fullpath.Contains(".png")) fullpath += ".png";
            if (!File.Exists(fullpath)) return false;
            try
            {
                using (var stream = TitleContainer.OpenStream(fullpath))
                {
                    Texture2D texture = Texture2D.FromStream(_graphics.GraphicsDevice, stream);
                    _textures.Add(file, new TextureSub(texture));
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
