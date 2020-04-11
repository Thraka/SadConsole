#if XNA
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SadConsole.DrawCalls;

namespace SadConsole
{
    /// <summary>
    /// A global state provider used by SadConsole.
    /// </summary>
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
        public static Font FontDefault { get; set; }

        /// <summary>
        /// The font automatically loaded by SadConsole. Standard IBM style font.
        /// </summary>
        public static Font FontEmbedded { get; internal set; }

        /// <summary>
        /// The font automatically loaded by SadConsole. Standard IBM style font. Extended with extra SadConsole characters.
        /// </summary>
        public static Font FontEmbeddedExtended { get; internal set; }

        /// <summary>
        /// The MonoGame graphics device.
        /// </summary>
        public static GraphicsDevice GraphicsDevice { get; set; }

        /// <summary>
        /// The MonoGame graphics device manager.
        /// </summary>
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        /// <summary>
        /// A global reusable sprite batch.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; set; }

        /// <summary>
        /// The active screen processed by the game.
        /// </summary>
        public static Console CurrentScreen { get; set; }

        /// <summary>
        /// The stack of consoles that will receive keyboard and mouse input.
        /// </summary>
        public static ConsoleStack FocusedConsoles = new ConsoleStack();

        /// <summary>
        /// A global random number generator.
        /// </summary>
        public static Random Random { get; set; } = new Random();

        /// <summary>
        /// The elapsed time from the update call. The value is from <see cref="GameTimeUpdate"/>.
        /// </summary>
        public static double GameTimeElapsedUpdate { get; set; }

        /// <summary>
        /// The elapsed time from the render call. The value is from <see cref="GameTimeRender"/>.
        /// </summary>
        public static double GameTimeElapsedRender { get; set; }


        /// <summary>
        /// The <see cref="GameTime"/> object from the update pass.
        /// </summary>
        public static GameTime GameTimeUpdate { get; set; }

        /// <summary>
        /// The <see cref="GameTime"/> object from the render pass.
        /// </summary>
        public static GameTime GameTimeRender { get; set; }

        /// <summary>
        /// Mouse state which is usually updated in the update pass.
        /// </summary>
        public static Input.Mouse MouseState { get; set; } = new Input.Mouse();

        /// <summary>
        /// Keyboard state which is usually updated in the update pass.
        /// </summary>
        public static Input.Keyboard KeyboardState { get; set; } = new Input.Keyboard();

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
        public static int WindowWidth => GraphicsDevice.PresentationParameters.BackBufferWidth;

        /// <summary>
        /// The current game window height.
        /// </summary>
        public static int WindowHeight => GraphicsDevice.PresentationParameters.BackBufferHeight;

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

        /// <summary>
        /// Loads a font from a file and adds it to the <see cref="Fonts"/> collection.
        /// </summary>
        /// <param name="font">The font file to load.</param>
        /// <returns>A master font that you can generate a usable font from.</returns>
        public static FontMaster LoadFont(string font)
        {
            //if (!File.Exists(font))
            //{
            //    font = Path.Combine(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(font)), "fonts"), Path.GetFileName(font));
            //    if (!File.Exists(font))
            //        throw new Exception($"Font does not exist: {font}");
            //}                    

            //FontPathHint = Path.GetDirectoryName(Path.GetFullPath(font));
            try
            {
                FontMaster masterFont = SadConsole.Serializer.Load<FontMaster>(font, false);

                if (Fonts.ContainsKey(masterFont.Name))
                {
                    Fonts.Remove(masterFont.Name);
                }

                Fonts.Add(masterFont.Name, masterFont);
                return masterFont;
            }
            catch (System.Runtime.Serialization.SerializationException)
            {

                throw;
            }

        }

        internal static void LoadEmbeddedFont()
        {
            //var auxList = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
#if WINDOWS_UWP || WINDOWS_UAP
            var assembly = new ColoredString().GetType().GetTypeInfo().Assembly;
#else
            var assembly = Assembly.GetExecutingAssembly();
#endif
            
            Font LoadFont(string fontName, string imageName)
            {
                using (Stream stream = assembly.GetManifestResourceStream(fontName))
                using (StreamReader sr = new StreamReader(stream))
                {
                    Settings.LoadingEmbeddedFont = true;
                    Global.SerializerPathHint = "";
                    var masterFont = (FontMaster)Newtonsoft.Json.JsonConvert.DeserializeObject(
                        sr.ReadToEnd(),
                        typeof(FontMaster),
                        new Newtonsoft.Json.JsonSerializerSettings()
                        {
                            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                        });

                    using (Stream fontStream = assembly.GetManifestResourceStream(imageName))
                    {
                        masterFont.Image = Texture2D.FromStream(Global.GraphicsDevice, fontStream);
                    }

                    masterFont.ConfigureRects();
                    Fonts.Add(masterFont.Name, masterFont);

                    Settings.LoadingEmbeddedFont = false;

                    return masterFont.GetFont(Font.FontSizes.One);
                }
            }

            FontEmbedded = LoadFont("SadConsole.Resources.IBM.font", "SadConsole.Resources.IBM8x16.png");
            FontEmbeddedExtended = LoadFont("SadConsole.Resources.IBM_ext.font", "SadConsole.Resources.IBM8x16_NoPadding_extended.png");
        }


        /// <summary>
        /// Resets the <see cref="RenderOutput"/> target and determines the appropriate <see cref="RenderRect"/> and <see cref="RenderScale"/> based on the window or fullscreen state.
        /// </summary>
        public static void ResetRendering()
        {
            if (Settings.ResizeMode == Settings.WindowResizeOptions.Center)
            {
                RenderOutput = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight);
                RenderRect = new Rectangle((GraphicsDevice.PresentationParameters.BackBufferWidth - RenderWidth) / 2, (GraphicsDevice.PresentationParameters.BackBufferHeight - RenderHeight) / 2, RenderWidth, RenderHeight);
                RenderScale = new Vector2(1);
            }
            else if (Settings.ResizeMode == Settings.WindowResizeOptions.Scale)
            {
                RenderOutput = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight);
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
            else if (Settings.ResizeMode == Settings.WindowResizeOptions.Fit)
            {
                RenderOutput = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight);
                float heightRatio = GraphicsDevice.PresentationParameters.BackBufferHeight / (float)RenderHeight;
                float widthRatio = GraphicsDevice.PresentationParameters.BackBufferWidth / (float)RenderWidth;

                float fitHeight = RenderHeight * widthRatio;
                float fitWidth = RenderWidth * heightRatio;

                if (fitHeight <= GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    // Render width = window width, pad top and bottom

                    RenderRect = new Rectangle(0, (int)((GraphicsDevice.PresentationParameters.BackBufferHeight - fitHeight) / 2), GraphicsDevice.PresentationParameters.BackBufferWidth, (int)fitHeight);

                    RenderScale = new Vector2(RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, RenderHeight / fitHeight);
                }
                else
                {
                    // Render height = window height, pad left and right

                    RenderRect = new Rectangle((int)((GraphicsDevice.PresentationParameters.BackBufferWidth - fitWidth) / 2), 0, (int)fitWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

                    RenderScale = new Vector2(RenderWidth / fitWidth, RenderHeight / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
                }
            }
            else if (Settings.ResizeMode == Settings.WindowResizeOptions.None)
            {
                RenderWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
                RenderHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
                RenderOutput = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight);
                RenderRect = GraphicsDevice.Viewport.Bounds;
                RenderScale = new Vector2(1);
            }
            else
            {
                RenderOutput = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight);
                RenderRect = GraphicsDevice.Viewport.Bounds;
                RenderScale = new Vector2(RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, RenderHeight / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
        }
    }
}
