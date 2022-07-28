using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;

namespace SadConsole.Host
{
    public static class Global
    {
        public static bool BlockSadConsoleInput { get; set; }

        public static RenderWindow GraphicsDevice { get; set; }

        public static SpriteBatch SharedSpriteBatch { get; set; }

        public static RenderTexture RenderOutput { get; set; }

        public static SFML.System.Clock UpdateTimer { get; set; }
        public static SFML.System.Clock DrawTimer { get; set; }
    }
}
