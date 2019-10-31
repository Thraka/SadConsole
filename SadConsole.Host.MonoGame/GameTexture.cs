using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SadConsole.MonoGame
{
    class GameTexture : ITexture
    {
        private Microsoft.Xna.Framework.Graphics.Texture2D _texture;
        private string _resourcePath;

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture => _texture;

        public string ResourcePath => _resourcePath;

        public int Height => _texture.Height;

        public int Width => _texture.Width;

        public GameTexture(string path)
        {
            using (Stream fontStream = Microsoft.Xna.Framework.TitleContainer.OpenStream(path))
                _texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Global.GraphicsDevice, fontStream);

            _resourcePath = path;
        }

        public GameTexture(Stream stream) =>
            _texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Global.GraphicsDevice, stream);

        public void Dispose()
        {
            _texture.Dispose();
            _texture = null;
        }
    }
}
