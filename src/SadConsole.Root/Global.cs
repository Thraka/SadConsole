using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SadConsole
{
    public static class Global
    {
        internal static string FontPathHint;

        public static Dictionary<string, FontMaster> Fonts { get; } = new Dictionary<string, FontMaster>();
        public static Font FontDefault;
        public static GraphicsDevice GraphicsDevice;
        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static SpriteBatch SpriteBatch;
        public static IScreen ActiveScreen;
        public static Random Random = new Random();
        public static double GameTimeElapsedUpdate;
        public static double GameTimeElapsedRender;

        public static GameTime GameTimeUpdate;
        public static GameTime GameTimeRender;

        #region Input
        public static Input.MouseInfo MouseState = new Input.MouseInfo();
        public static Input.KeyboardInfo KeyboardState = new Input.KeyboardInfo();
        #endregion

        #region Rendering
        public static RenderTarget2D RenderOutput;

        /// <summary>
        /// The width of the game window.
        /// </summary>
        public static int WindowWidth { get; set; }

        /// <summary>
        /// The height of the game window.
        /// </summary>
        public static int WindowHeight { get; set; }

        /// <summary>
        /// Where on the screen the engine will be rendered.
        /// </summary>
        public static Rectangle RenderRect { get; set; }

        /// <summary>
        /// If the <see cref="RenderRect"/> is stretched, this is the ratio difference between unstretched.
        /// </summary>
        public static Vector2 RenderScale { get; set; }

        /// <summary>
        /// Draw calls to render to <see cref="RenderOutput"/>.
        /// </summary>
        public static List<Tuple<Surfaces.ISurface, Microsoft.Xna.Framework.Point>> DrawCalls = new List<Tuple<Surfaces.ISurface, Microsoft.Xna.Framework.Point>>(5);
        #endregion

        /// <summary>
        /// Loads a font from a file and adds it to the <see cref="Fonts"/> collection.
        /// </summary>
        /// <param name="font">The font file to load.</param>
        /// <returns>A master font that you can generate a usable font from.</returns>
        public static FontMaster LoadFont(string font)
        {
            if (!File.Exists(font))
            {
                font = Path.Combine(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(font)), "fonts"), Path.GetFileName(font));
                if (!File.Exists(font))
                    throw new Exception($"Font does not exist: {font}");
            }                    

            FontPathHint = Path.GetDirectoryName(Path.GetFullPath(font));

            var masterFont = SadConsole.Serializer.Load<FontMaster>(font);

            if (Fonts.ContainsKey(masterFont.Name))
                Fonts.Remove(masterFont.Name);

            Fonts.Add(masterFont.Name, masterFont);
            return masterFont;
        }

        public static void ResetRendering()
        {
            RenderOutput = new RenderTarget2D(GraphicsDevice, WindowWidth, WindowHeight);

            if (Settings.ResizeMode == Settings.WindowResizeOptions.Center)
            {
                RenderRect = new Rectangle((GraphicsDeviceManager.PreferredBackBufferWidth - WindowWidth) / 2, (GraphicsDeviceManager.PreferredBackBufferHeight - WindowHeight) / 2, WindowWidth, WindowHeight);
                RenderScale = new Vector2(1);
            }
            else if (Settings.ResizeMode == Settings.WindowResizeOptions.Scale)
            {
                int multiple = 2;

                // Find the bounds
                while (true)
                {
                    if (WindowWidth * multiple > GraphicsDeviceManager.PreferredBackBufferWidth || WindowHeight * multiple > GraphicsDeviceManager.PreferredBackBufferHeight)
                    {
                        multiple--;
                        break;
                    }

                    multiple++;
                }

                RenderRect = new Rectangle((GraphicsDeviceManager.PreferredBackBufferWidth - (WindowWidth * multiple)) / 2, (GraphicsDeviceManager.PreferredBackBufferHeight - (WindowHeight * multiple)) / 2, WindowWidth * multiple, WindowHeight * multiple);
                RenderScale = new Vector2(WindowWidth / ((float)WindowWidth * multiple), WindowHeight / (float)(WindowHeight * multiple));
            }
            else
            {
                RenderRect = new Rectangle(0, 0, WindowWidth, WindowHeight);
                RenderScale = new Vector2((float)GraphicsDeviceManager.PreferredBackBufferWidth / Game.Instance.Window.ClientBounds.Width, (float)GraphicsDeviceManager.PreferredBackBufferHeight / Game.Instance.Window.ClientBounds.Height);
            }
        }
    }

    public static class Settings
    {
        /// <summary>
        /// The color to automatically clear the device with.
        /// </summary>
        public static Color ClearColor = Color.Black;

        /// <summary>
        /// The type of resizing options for the window.
        /// </summary>
        public static WindowResizeOptions ResizeMode = WindowResizeOptions.Scale;

        /// <summary>
        /// Allow the user to resize the window. Must be set before the game is created.
        /// </summary>
        public static bool AllowWindowResize = false;

        /// <summary>
        /// Unlimited FPS when rendering (normally limited to 60fps). Must be set before the game is created.
        /// </summary>
        public static bool UnlimitedFPS = false;

        public static bool DoDraw = true;
        public static bool DoFinalDraw = true;
        public static bool DoUpdate = true;

        public static class Input
        {
            public static bool ProcessMouseOffscreen = false;

            public static bool DoMouse = true;

            public static bool DoKeyboard = true;
        }

        public enum WindowResizeOptions
        {
            Stretch,
            Center,
            Scale,
        }
    }
}


namespace SadConsole
{
    using Microsoft.Xna.Framework;



    


}
