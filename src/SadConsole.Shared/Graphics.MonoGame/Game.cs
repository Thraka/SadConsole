using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

#if WPF
using MonoGame_Game = MonoGame.Framework.WpfInterop.WpfGame;
using GraphicsDeviceManager = MonoGame.Framework.WpfInterop.WpfGraphicsDeviceService;
#elif FORMS
//using MonoGame_Game = MonoGame.Forms.Controls.
using MonoGame_Game = Microsoft.Xna.Framework.Game;
#else
using MonoGame_Game = Microsoft.Xna.Framework.Game;
#endif


namespace SadConsole
{
    public partial class Game
    {
        /// <summary>
        /// Instance of the MonoGame game
        /// </summary>
        public static Game Instance { get; set; }

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

        #if !WPF
        /// <summary>
        /// Initializes and creates a new SadConsole game.
        /// </summary>
        /// <param name="font">The path to the font that will become the <see cref="Global.FontDefault"/>.</param>
        /// <param name="consoleWidth">The width in cells (based on the font) the initial console and screen will be.</param>
        /// <param name="consoleHeight">The height in cells (based on the font) the initial console and screen will be.</param>
        /// <param name="ctorCallback">Optional callback pre SadConsole init but after MonoGame has created the Game instance.</param>
        public static void Create(string font, int consoleWidth, int consoleHeight, Action<Game> ctorCallback = null)
        {
            Instance = new Game(font, consoleWidth, consoleHeight, ctorCallback);
        }
#endif

#if FORMS
        public static string WpfFont;
        public static int WpfConsoleWidth;
        public static int WpfConsoleHeight;
#endif
    }

    /// <summary>
    /// A MonoGame <see cref="Game"/> instance that runs SadConsole. This is used when you don't provide one and call <see cref="Engine.Initialize(string, int, int)"/>.
    /// </summary>
    public partial class Game : MonoGame_Game
    {
        private bool resizeBusy = false;
        private string font;
        private int consoleWidth;
        private int consoleHeight;

        public GraphicsDeviceManager GraphicsDeviceManager;

#if !WPF
        protected Game(string font, int consoleWidth, int consoleHeight, Action<Game> ctorCallback)
        {
            if (Instance == null)
                Instance = this;

            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            this.font = font;
            this.consoleHeight = consoleHeight;
            this.consoleWidth = consoleWidth;
#if MONOGAME
            GraphicsDeviceManager.HardwareModeSwitch = Settings.UseHardwareFullScreen;
#endif
            ctorCallback?.Invoke(this);
        }


        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            //if (!resizeBusy && Settings.IsExitingFullscreen)
            //{
            //    GraphicsDeviceManager.PreferredBackBufferWidth = Global.WindowWidth;
            //    GraphicsDeviceManager.PreferredBackBufferHeight = Global.WindowHeight;

            //    resizeBusy = true;
            //    GraphicsDeviceManager.ApplyChanges();
            //    resizeBusy = false;
            //    Settings.IsExitingFullscreen = false;
            //}

            //Global.WindowWidth = GraphicsDeviceManager.PreferredBackBufferWidth;
            //Global.WindowHeight = GraphicsDeviceManager.PreferredBackBufferHeight;
            //Global.WindowWidth = Global.RenderWidth = GraphicsDeviceManager.PreferredBackBufferWidth;
            //Global.WindowHeight = Global.RenderHeight = GraphicsDeviceManager.PreferredBackBufferHeight;

            Global.ResetRendering();

        }

#else
        public static string WpfFont;
        public static int WpfConsoleWidth;
        public static int WpfConsoleHeight;

        public MonoGame.Framework.WpfInterop.Input.WpfKeyboard WpfKeyboard;
        public MonoGame.Framework.WpfInterop.Input.WpfMouse WpfMouse;

        public Game() : base()
        {
            if (Instance == null)
                Instance = this;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            //if (sizeInfo.NewSize != sizeInfo.PreviousSize)
            //{
            //    Global.FontDefault.ResizeGraphicsDeviceManager(consoleWidth, consoleHeight, 0, 0);
            //    Global.ResetRendering();
            //}
        }
#endif


        protected override void Initialize()
        {
#if !WPF
            if (Settings.UnlimitedFPS)
            {
                GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }
#else
            GraphicsDeviceManager = new MonoGame.Framework.WpfInterop.WpfGraphicsDeviceService(this);
            
            font = WpfFont;
            consoleWidth = WpfConsoleWidth;
            consoleHeight = WpfConsoleHeight;

            WpfKeyboard = new MonoGame.Framework.WpfInterop.Input.WpfKeyboard(this);
            WpfMouse = new MonoGame.Framework.WpfInterop.Input.WpfMouse(this);

#endif

            // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
            Components.Add(new ClearScreenGameComponent(this));
            Components.Add(new SadConsoleGameComponent(this));

            // Call the default initialize of the base class.
            base.Initialize();

            // Let the XNA framework show the mouse.
#if !WPF
            IsMouseVisible = true;

            // Hook window change for resolution fixes
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            Window.AllowUserResizing = SadConsole.Settings.AllowWindowResize;
#endif
            Global.GraphicsDevice = GraphicsDevice;
            Global.GraphicsDeviceManager = GraphicsDeviceManager;
            Global.SpriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
            Global.FontDefault = Global.LoadFont(font).GetFont(Font.FontSizes.One);
            Global.FontDefault.ResizeGraphicsDeviceManager(consoleWidth, consoleHeight, 0, 0);
            Global.ResetRendering();

            // Tell the main engine we're ready
            OnInitialize?.Invoke();

            // After we've init, clear the graphics device so everything is ready to start
            Global.GraphicsDevice.SetRenderTarget(Global.OriginalRenderTarget);
        }
    }
}
