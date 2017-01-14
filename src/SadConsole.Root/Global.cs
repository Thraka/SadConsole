using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public static class Global
    {
        internal static string FontPathHint;

        public static Dictionary<string, FontMaster> Fonts { get; } = new Dictionary<string, FontMaster>();
        public static Font FontDefault;
        public static Microsoft.Xna.Framework.Graphics.GraphicsDevice Device;

        public static Screen ActiveScreen;

        /// <summary>
        /// Loads a font from a file and adds it to the <see cref="Fonts"/> collection.
        /// </summary>
        /// <param name="font">The font file to load.</param>
        /// <returns>A master font that you can generate a usable font from.</returns>
        public static FontMaster LoadFont(string font)
        {
            if (!System.IO.File.Exists(font))
                throw new Exception($"Font does not exist: {font}");

            FontPathHint = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(font));

            var masterFont = SadConsole.Serializer.Load<FontMaster>(font);

            if (Fonts.ContainsKey(masterFont.Name))
                Fonts.Remove(masterFont.Name);

            Fonts.Add(masterFont.Name, masterFont);
            return masterFont;
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
            
            //if (SadConsole.Game.Settings.DoDraw)
            // Do all the render things.

            Global.ActiveScreen?.Renderer.Render(Global.ActiveScreen.Surfaces[0], new Point());

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
        public static Game GameInstance { get; private set; }

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
            GameInstance = new Game(font, consoleWidth, consoleHeight, ctorCallback);
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

            //Engine.ResetRendering();
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
            Global.Device = GraphicsDevice;
            Global.FontDefault = Global.LoadFont(font).GetFont(Font.FontSizes.One);
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

            public bool DoUpdate = true;

            internal GameSettings()
            {

            }
        }
    }
}
