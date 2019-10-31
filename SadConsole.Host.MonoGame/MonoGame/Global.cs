using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.MonoGame
{
    public static class Global
    {
        public static GraphicsDevice GraphicsDevice { get; set; }

        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        public static SpriteBatch SharedSpriteBatch { get; set; }

        public static RenderTarget2D RenderOutput { get; set; }
    }
}
