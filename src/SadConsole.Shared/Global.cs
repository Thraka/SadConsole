namespace SadConsole
{
#if XNA
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
#endif

    using SadConsole.Content;
    using SadConsole.DrawCalls;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class Global
    {
        internal static string SerializerPathHint;

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
        public static Console CurrentScreen;

        /// <summary>
        /// The stack of consoles that will receive keyboard and mouse input.
        /// </summary>
        public static ConsoleStack FocusedConsoles = new ConsoleStack();

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

        /// <summary>
        /// Mouse state which is usually updated in the update pass.
        /// </summary>
        public static Input.Mouse MouseState = new Input.Mouse();

        /// <summary>
        /// Keyboard state which is usually updated in the update pass.
        /// </summary>
        public static Input.Keyboard KeyboardState = new Input.Keyboard();

#region Rendering
        /// <summary>
        /// The render target of SadConsole. This is generally rendered to the screen as the final step of drawing.
        /// </summary>
        public static RenderTarget2D RenderOutput;

        /// <summary>
        /// The width of the area to render on the game window.
        /// </summary>
        public static int RenderWidth { get; set; }

        /// <summary>
        /// The height of the area to render on the game window.
        /// </summary>
        public static int RenderHeight { get; set; }

        /// <summary>
        /// The current game window width.
        /// </summary>
        public static int WindowWidth { get; set; }

        /// <summary>
        /// The current game window height.
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
        public static List<IDrawCall> DrawCalls = new List<IDrawCall>(5);
        #endregion

        private static readonly IContentProvider ContentProvider;
        private static readonly TextureLoader TextureLoader;
        private static readonly FontLoader FontLoader;

        static Global()
        {
            ContentProvider = new ContentProvider();
            TextureLoader = new TextureLoader(ContentProvider);
            FontLoader = new FontLoader(ContentProvider, TextureLoader);
        }

        /// <summary>
        /// Loads a font from a file and adds it to the <see cref="Fonts"/> collection.
        /// </summary>
        /// <param name="name">The font file to load.</param>
        /// <returns>A master font that you can generate a usable font from.</returns>
        public static FontMaster LoadFont(string name)
        {
            var font = FontLoader.Load(name).GetAwaiter().GetResult();

            if (Fonts.ContainsKey(font.Name))
                Fonts.Remove(font.Name);

            Fonts.Add(font.Name, font);
            return font;
        }
        
        internal static void LoadEmbeddedFont()
        {
            var resourceNameFont = Settings.UseDefaultExtendedFont ?
                "embedded:SadConsole.Resources.IBM_ext.font" :
                "embedded:SadConsole.Resources.IBM.font";
            
            Settings.LoadingEmbeddedFont = true;
            Global.SerializerPathHint = "";

            var font = FontLoader.Load(resourceNameFont).GetAwaiter().GetResult();

            Fonts.Add(font.Name, font);
            FontDefault = font.GetFont(Font.FontSizes.One);

            Settings.LoadingEmbeddedFont = false;
        }


        /// <summary>
        /// Resets the <see cref="RenderOutput"/> target and determines the appropriate <see cref="RenderRect"/> and <see cref="RenderScale"/> based on the window or fullscreen state.
        /// </summary>
        public static void ResetRendering()
        {
            RenderOutput = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight);

            if (Settings.ResizeMode == Settings.WindowResizeOptions.Center)
            {
                RenderRect = new Rectangle((GraphicsDevice.PresentationParameters.BackBufferWidth - RenderWidth) / 2, (GraphicsDevice.PresentationParameters.BackBufferHeight - RenderHeight) / 2, RenderWidth, RenderHeight);
                RenderScale = new Vector2(1);
            }
            else if (Settings.ResizeMode == Settings.WindowResizeOptions.Scale)
            {
                int multiple = 2;

                // Find the bounds
                while (true)
                {
                    if (RenderWidth * multiple > GraphicsDevice.PresentationParameters.BackBufferWidth || RenderHeight * multiple > GraphicsDevice.PresentationParameters.BackBufferHeight)
                    {
                        multiple--;
                        break;
                    }

                    multiple++;
                }

                RenderRect = new Rectangle((GraphicsDevice.PresentationParameters.BackBufferWidth - (RenderWidth * multiple)) / 2, (GraphicsDevice.PresentationParameters.BackBufferHeight - (RenderHeight * multiple)) / 2, RenderWidth * multiple, RenderHeight * multiple);
                RenderScale = new Vector2(RenderWidth / ((float)RenderWidth * multiple), RenderHeight / (float)(RenderHeight * multiple));
            }
            else
            {
                RenderRect = GraphicsDevice.Viewport.Bounds;
                RenderScale = new Vector2(RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, RenderHeight / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
        }
    }
}