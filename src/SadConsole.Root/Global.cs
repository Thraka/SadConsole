using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SadConsole.Text;

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
        public static Screen ActiveScreen;

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
        public static List<Tuple<Text.ITextSurfaceRendered, Microsoft.Xna.Framework.Point>> DrawCalls = new List<Tuple<Text.ITextSurfaceRendered, Microsoft.Xna.Framework.Point>>(5);
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

            if (Game.Settings.ResizeMode == Game.WindowResizeOptions.Center)
            {
                RenderRect = new Rectangle((GraphicsDeviceManager.PreferredBackBufferWidth - WindowWidth) / 2, (GraphicsDeviceManager.PreferredBackBufferHeight - WindowHeight) / 2, WindowWidth, WindowHeight);
                RenderScale = new Vector2(1);
            }
            else if (Game.Settings.ResizeMode == Game.WindowResizeOptions.Scale)
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


namespace SadConsole
{
    using Microsoft.Xna.Framework;

    public class SadConsoleGameComponent: DrawableGameComponent
    {
        internal SadConsoleGameComponent(Game game): base(game)
        {
            DrawOrder = 5;
            UpdateOrder = 5;
        }

        public override void Draw(GameTime gameTime)
        {
            if (SadConsole.Game.Settings.DoDraw)
            {
                // Make sure all items in the screen are drawn. (Build a list of draw calls)
                SadConsole.Global.ActiveScreen.Draw(gameTime.ElapsedGameTime);

                // Render to the global output texture
                GraphicsDevice.SetRenderTarget(Global.RenderOutput);

                // Render each draw call
                Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                foreach (var call in SadConsole.Global.DrawCalls)
                {
                    Global.SpriteBatch.Draw(call.Item1.LastRenderResult, call.Item2.ToVector2(), Color.White);

                }
                Global.SpriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);

                // Clear draw calls for next run
                SadConsole.Global.DrawCalls.Clear();

                // If we're going to draw to the screen, do it.
                if (SadConsole.Game.Settings.DoFinalDraw)
                {
                    Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                    Global.SpriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
                    Global.SpriteBatch.End();
                }
            }

            SadConsole.Game.OnDraw?.Invoke(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            //if (SadConsole.Game.Settings.DoUpdate)
                // Do all the update things.

            SadConsole.Game.OnUpdate?.Invoke(gameTime);
        }
    }

    public class ClearScreenGameComponent: DrawableGameComponent
    {
        internal ClearScreenGameComponent(Game game): base(game)
        {
            DrawOrder = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(SadConsole.Game.Settings.ClearColor);
        }
    }

    /// <summary>
    /// A MonoGame <see cref="Game"/> instance that runs SadConsole. This is used when you don't provide one and call <see cref="Engine.Initialize(string, int, int)"/>.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Static
        public static GameSettings Settings { get; } = new GameSettings();
        public static Game Instance { get; private set; }

        /// <summary>
        /// Called after each frame of update logic has happened.
        /// </summary>
        public static Action<GameTime> OnUpdate;

        /// <summary>
        /// Called after a frame has been drawn.
        /// </summary>
        public static Action<GameTime> OnDraw;

        /// <summary>
        /// Called when the device is created.
        /// </summary>
        public static Action OnInitialize;

        public static void Create(string font, int consoleWidth, int consoleHeight, Action<Game> ctorCallback = null)
        {
            Instance = new Game(font, consoleWidth, consoleHeight, ctorCallback);
        }
        #endregion
        
        private bool resizeBusy = false;
        private string font;
        private int consoleWidth;
        private int consoleHeight;
        public GraphicsDeviceManager GraphicsDeviceManager;

        // public stuff from Engine (like defaultfont)

        private Game(string font, int consoleWidth, int consoleHeight, Action<Game> ctorCallback)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.font = font;
            this.consoleHeight = consoleHeight;
            this.consoleWidth = consoleWidth;
            GraphicsDeviceManager.HardwareModeSwitch = false;

            ctorCallback?.Invoke(this);
        }


        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (!resizeBusy)
            {
                if (Settings.ResizeMode != WindowResizeOptions.Stretch)
                {
                    GraphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
                    GraphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Height;

                    resizeBusy = true;
                    GraphicsDeviceManager.ApplyChanges();
                    resizeBusy = false;
                }
            }

            Global.ResetRendering();
        }


        protected override void Initialize()
        {
            //if (Engine.UnlimitedFPS)
            //{
            //    GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            //    IsFixedTimeStep = false;
            //}

            // Let the XNA framework show the mouse.
            IsMouseVisible = true;

            // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
            Components.Add(new ClearScreenGameComponent(this));
            Components.Add(new SadConsoleGameComponent(this));
                        
            // Call the default initialize of the base class.
            base.Initialize();

            // Hook window change for resolution fixes
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            Window.AllowUserResizing = Game.Settings.AllowWindowResize;

            // Tell the main engine we're ready
            //Engine.InitializeCompleted();
            Global.GraphicsDevice = GraphicsDevice;
            Global.GraphicsDeviceManager = GraphicsDeviceManager;
            Global.SpriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
            Global.FontDefault = Global.LoadFont(font).GetFont(Font.FontSizes.One);
            Global.FontDefault.ResizeGraphicsDeviceManager(GraphicsDeviceManager, consoleWidth, consoleHeight, 0, 0);
            Global.ResetRendering();
            OnInitialize?.Invoke();
        }

        public enum WindowResizeOptions
        {
            Stretch,
            Center,
            Scale,
        }

        public class GameSettings
        {
            /// <summary>
            /// The color to automatically clear the device with.
            /// </summary>
            public Color ClearColor = Color.Black;

            /// <summary>
            /// The type of resizing options for the window.
            /// </summary>
            public WindowResizeOptions ResizeMode = WindowResizeOptions.Scale;

            /// <summary>
            /// Allow the user to resize the window. Must be set before the game is created.
            /// </summary>
            public bool AllowWindowResize = false;
            
            /// <summary>
            /// Unlimited FPS when rendering (normally limited to 60fps). Must be set before the game is created.
            /// </summary>
            public bool UnlimitedFPS = false;

            public bool DoDraw = true;
            public bool DoFinalDraw = true;

            public bool DoUpdate = true;

            internal GameSettings()
            {

            }
        }
    }
}
