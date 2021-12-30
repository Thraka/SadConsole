using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace SadConsole.Host
{
    public static class Global
    {
        public static GraphicsDevice GraphicsDevice { get; set; }

        public static WpfGraphicsDeviceService GraphicsDeviceManager { get; set; }

        public static SpriteBatch SharedSpriteBatch { get; set; }

        public static RenderTarget2D RenderOutput { get; set; }

        public static RenderTarget2D GraphicsDeviceWpfControl { get; set; }

        public static void ResetGraphicsDevice() =>
            GraphicsDevice.SetRenderTarget(GraphicsDeviceWpfControl);
    }
}
