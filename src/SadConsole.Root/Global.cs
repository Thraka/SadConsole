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

        /// <summary>
        /// Collection of fonts. Used mainly by the deserialization system.
        /// </summary>
        public static Dictionary<string, FontMaster> Fonts { get; } = new Dictionary<string, FontMaster>();

        /// <summary>
        /// The default font for any type that does not provide a font.
        /// </summary>
        public static Font FontDefault;

        /// <summary>
        /// The MonoGame graphics device.
        /// </summary>
        public static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The MonoGame graphics device manager.
        /// </summary>
        public static GraphicsDeviceManager GraphicsDeviceManager;

        /// <summary>
        /// A global reusable sprite batch.
        /// </summary>
        public static SpriteBatch SpriteBatch;
        
        /// <summary>
        /// The active screen processed by the game.
        /// </summary>
        public static IScreen CurrentScreen = new Screen();

        /// <summary>
        /// A global random number generator.
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// The elapsed time from the update call. The value is from <see cref="GameTimeUpdate"/>.
        /// </summary>
        public static double GameTimeElapsedUpdate;

        /// <summary>
        /// The elapsed time from the render call. The value is from <see cref="GameTimeRender"/>.
        /// </summary>
        public static double GameTimeElapsedRender;


        /// <summary>
        /// The <see cref="GameTime"/> object from the update pass.
        /// </summary>
        public static GameTime GameTimeUpdate;

        /// <summary>
        /// The <see cref="GameTime"/> object from the render pass.
        /// </summary>
        public static GameTime GameTimeRender;

        #region Input
        /// <summary>
        /// Mouse state which is usually updated in the update pass.
        /// </summary>
        public static Input.MouseInfo MouseState = new Input.MouseInfo();

        /// <summary>
        /// Keyboard state which is usually updated in the update pass.
        /// </summary>
        public static Input.KeyboardInfo KeyboardState = new Input.KeyboardInfo();
        #endregion

        #region Rendering
        /// <summary>
        /// The render target of SadConsole. This is generally rendered to the screen as the final step of drawing.
        /// </summary>
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
        public static List<DrawCall> DrawCalls = new List<DrawCall>(5);
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

        /// <summary>
        /// Resets the <see cref="RenderOutput"/> target and determines the appropriate <see cref="RenderRect"/> and <see cref="RenderScale"/> based on the window or fullscreen state.
        /// </summary>
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
}