using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SFML.Graphics;

namespace SadConsole.Host
{
    class GameTexture : ITexture
    {
        private Texture _texture;
        private string _resourcePath;

        public Texture Texture => _texture;

        public string ResourcePath => _resourcePath;

        public int Height => (int)_texture.Size.Y;

        public int Width => (int)_texture.Size.X;

        public GameTexture(string path)
        {
            using (Stream fontStream = new FileStream(path, FileMode.Open))
                _texture = new Texture(fontStream);

            _resourcePath = path;
        }

        public GameTexture(Stream stream) =>
            _texture = new Texture(stream);

        public void Dispose()
        {
            _texture.Dispose();
            _texture = null;
        }
    }
}
