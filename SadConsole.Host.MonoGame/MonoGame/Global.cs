using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if WPF
using MonoGame.Framework.WpfInterop;
#endif

namespace SadConsole.Host
{
    public static class Global
    {
        public static bool BlockSadConsoleInput { get; set; }

        public static GraphicsDevice GraphicsDevice { get; set; }

        public static SpriteBatch SharedSpriteBatch { get; set; }

        public static RenderTarget2D RenderOutput { get; set; }

        public static GameTime UpdateLoopGameTime { get; internal set; }

        public static GameTime RenderLoopGameTime { get; internal set; }

#if WPF

        public static RenderTarget2D GraphicsDeviceWpfControl { get; set; }

        public static WpfGraphicsDeviceService GraphicsDeviceManager { get; set; }

        public static void ResetGraphicsDevice() =>
            GraphicsDevice.SetRenderTarget(GraphicsDeviceWpfControl);

#else

        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        public static void ResetGraphicsDevice() =>
            GraphicsDevice.SetRenderTarget(null);
#endif

    }
}
