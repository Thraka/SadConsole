using Microsoft.Xna.Framework;
using SadConsole.MonoGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// A MonoGame <see cref="Game"/> instance that runs SadConsole. This is used when you don't provide one and call <see cref="Engine.Initialize(string, int, int)"/>.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Static
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
                if (Settings.ResizeMode != Settings.WindowResizeOptions.Stretch)
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
            Window.AllowUserResizing = SadConsole.Settings.AllowWindowResize;

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


    }
}
