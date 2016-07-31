#if SFML
using Point = SFML.System.Vector2i;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SadConsole.Consoles;

namespace SadConsole
{
    public static class Engine
    {
        #region Constants
        public const int FontColumns = 16;
        #endregion

        #region Fields
        public static event EventHandler EngineUpdated;
        public static event EventHandler EngineDrawFrame;
        public static event EventHandler<ShutdownEventArgs> EngineShutdown;
        public static event EventHandler EngineStart;

        /// <summary>
        /// Clears the screen this color each frame.
        /// </summary>
        public static Color ClearFrameColor = Color.Black;

        private static Consoles.IConsole _activeConsole;
        private static List<Type> _cellEffects;

        internal static IConsole LastMouseConsole;

        /// <summary>
        /// The width of the game window.
        /// </summary>
        public static int WindowWidth { get; set; }

        /// <summary>
        /// The height of the game window.
        /// </summary>
        public static int WindowHeight { get; set; }

        /// <summary>
        /// Total seconds since the last time the update method was called.
        /// </summary>
        public static double GameTimeElapsedUpdate { get; private set; }

        /// <summary>
        /// Total seconds since the last time the render method was called.
        /// </summary>
        public static double GameTimeElapsedRender { get; private set; }

        /// <summary>
        /// The GameTime object that was last used in the Update method.
        /// </summary>
        public static GameTime GameTimeUpdate { get; private set; }

        /// <summary>
        /// The GameTime object that was last used in the Draw method.
        /// </summary>
        public static GameTime GameTimeDraw { get; private set; }

        /// <summary>
        /// A list of consoles that will be rendered.
        /// </summary>
        public static Consoles.ConsoleList ConsoleRenderStack;

        /// <summary>
        /// Sets the console that has focus. Active console receives keyboard and mouse events.
        /// </summary>
        public static Consoles.IConsole ActiveConsole
        {
            get { return _activeConsole; }
            set
            {
                if (_activeConsole != null)
                {
                    //if (_activeConsole.CanActiveBeTaken(value))
                        changeActiveConsole(_activeConsole, value);
                }
                else if (value != null)
                    changeActiveConsole(null, value);
            }
        }

        /// <summary>
        /// Provides access to the state of the keyboard when the engine was last updated.
        /// </summary>
        public static KeyboardInfo Keyboard { get; private set; }

        /// <summary>
        /// Provides access to the state of the mouse when the engine was last updated.
        /// </summary>
        public static MouseInfo Mouse { get; private set; }

        /// <summary>
        /// Gets or sets a value to enable the keyboard for use with SadConsole.
        /// </summary>
        public static bool UseKeyboard { get; set; }

        /// <summary>
        /// Gets or sets a value to enable the mouse for use with SadConsole.
        /// </summary>
        public static bool UseMouse { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate that the mouse (when enabled) should be processed even if the mouse is not over the game surface.
        /// </summary>
        public static bool ProcessMouseWhenOffScreen { get; set; }

        /// <summary>
        /// Gets or sets the default font to be used with the console. There must always be a default font set.
        /// </summary>
        public static Font DefaultFont { get; set; }

        /// <summary>
        /// Gets a collection of effects currently registered with the engine.
        /// </summary>
        public static System.Collections.ObjectModel.ReadOnlyCollection<Type> RegisteredEffects
        {
            get
            {
                return _cellEffects.AsReadOnly();
            }
        }

        /// <summary>
        /// Centralized sudo random number generator used by SadConsole. Replace it with your own seed to replicate specific randomness.
        /// </summary>
        public static Random Random = new Random();

        private static System.Diagnostics.Stopwatch gameTimer;
        #endregion

        #region Constructors
        static Engine()
        {
            Keyboard = new KeyboardInfo();
            Mouse = new MouseInfo();

            UseKeyboard = true;
            UseMouse = true;
            ProcessMouseWhenOffScreen = false;

            _cellEffects = new List<Type>();

            GameTimeUpdate = new GameTime();
            GameTimeDraw = new GameTime();
        }
        #endregion

        #region Properties
#if SFML
        /// <summary>
        /// The window being rendered to.
        /// </summary>
        public static RenderWindow Device { get; private set; }
#elif MONOGAME
        /// <summary>
        /// The graphics device used by SadConsole.
        /// </summary>
        public static GraphicsDevice Device { get; private set; }
#endif
        /// <summary>
        /// A collection of fonts.
        /// </summary>
        public static Dictionary<string, FontMaster> Fonts { get; internal set; }
        #endregion

        #region Methods

        #region Initialization

#if SFML
        private static void SetupFontAndEffects(string font)
        {
            Fonts = new Dictionary<string, FontMaster>();
            ConsoleRenderStack = new Consoles.ConsoleList();
            RegisterCellEffect<Effects.Blink>();
            RegisterCellEffect<Effects.BlinkGlyph>();
            RegisterCellEffect<Effects.ConcurrentEffect>();
            RegisterCellEffect<Effects.Delay>();
            RegisterCellEffect<Effects.EffectsChain>();
            RegisterCellEffect<Effects.Fade>();
            RegisterCellEffect<Effects.Recolor>();

            // Load the default font and screen size
            DefaultFont = LoadFont(font).GetFont(Font.FontSizes.One);
        }
        private static void SetupInputsAndTimers()
        {
            Mouse.Setup(Device);
            Keyboard.Setup(Device);
            GameTimeDraw.Start();
            GameTimeUpdate.Start();
        }

        private static Consoles.Console SetupStartingConsole(int consoleWidth, int consoleHeight)
        {
            ActiveConsole = new Consoles.Console(consoleWidth, consoleHeight);
            ActiveConsole.TextSurface.DefaultBackground = Color.Black;
            ActiveConsole.TextSurface.DefaultForeground = ColorAnsi.White;
            ((Consoles.Console)ActiveConsole).Clear();

            ConsoleRenderStack.Add(ActiveConsole);

            return (Consoles.Console)ActiveConsole;
        }

        /// <summary>
        /// Prepares the engine for use by creating a window. This must be the first method you call on the engine.
        /// </summary>
        /// <param name="font">The font to load as the <see cref="DefaultFont"/>.</param>
        /// <param name="consoleWidth">The width of the default root console (and game window).</param>
        /// <param name="consoleHeight">The height of the default root console (and game window).</param>
        /// <returns>The default active console.</returns>
        public static Consoles.Console Initialize(string font, int consoleWidth, int consoleHeight)
        {
            SetupFontAndEffects(font);

            var window = new RenderWindow(new SFML.Window.VideoMode((uint)(DefaultFont.Size.X * consoleWidth), (uint)(DefaultFont.Size.Y * consoleHeight)), "SadConsole Game", SFML.Window.Styles.Titlebar | SFML.Window.Styles.Close);
            window.Closed += (o, e) =>
            {
                ShutdownEventArgs args = new ShutdownEventArgs();
                EngineShutdown?.Invoke(null, args);
                if (!args.BlockShutdown)
                    ((SFML.Window.Window)o).Close();
            };
            window.SetFramerateLimit(60);
            Device = window;
            
            SetupInputsAndTimers();

            // Create the default console.
            return SetupStartingConsole(consoleWidth, consoleHeight);
        }

        /// <summary>
        /// Prepares the engine for use. This must be the first method you call on the engine.
        /// </summary>
        /// <param name="window">The rendering window.</param>
        /// <param name="font">The font to load as the <see cref="DefaultFont"/>.</param>
        /// <param name="consoleWidth">The width of the default root console (and game window).</param>
        /// <param name="consoleHeight">The height of the default root console (and game window).</param>
        /// <returns>The default active console.</returns>
        public static Consoles.Console Initialize(RenderWindow window, string font, int consoleWidth, int consoleHeight)
        {
            Device = window;

            SetupInputsAndTimers();
            SetupFontAndEffects(font);

            DefaultFont.ResizeGraphicsDeviceManager(window, consoleWidth, consoleHeight, 0, 0);


            // Create the default console.
            return SetupStartingConsole(consoleWidth, consoleHeight);
        }

        public static void Run()
        {
            EngineStart?.Invoke(null, EventArgs.Empty);

            while (Device.IsOpen)
            {
                Device.Clear(ClearFrameColor);

                Update(Device.HasFocus());
                Draw();
                
                Device.Display();
                Device.DispatchEvents();
            }
        }

#elif MONOGAME
        /// <summary>
        /// Prepares the engine for use. This must be the first method you call on the engine when you provide your own <see cref="GraphicsDeviceManager"/>.
        /// </summary>
        /// <param name="deviceManager">The graphics device manager from MonoGame.</param>
        /// <param name="font">The font to load as the <see cref="DefaultFont"/>.</param>
        /// <param name="consoleWidth">The width of the default root console (and game window).</param>
        /// <param name="consoleHeight">The height of the default root console (and game window).</param>
        /// <returns>The default active console.</returns>
        public static Consoles.Console Initialize(GraphicsDeviceManager deviceManager, string font, int consoleWidth, int consoleHeight)
        {
            if (Device == null)
                Device = deviceManager.GraphicsDevice;
            Fonts = new Dictionary<string, FontMaster>();
            ConsoleRenderStack = new Consoles.ConsoleList();
            RegisterCellEffect<Effects.Blink>();
            RegisterCellEffect<Effects.BlinkGlyph>();
            RegisterCellEffect<Effects.ConcurrentEffect>();
            RegisterCellEffect<Effects.Delay>();
            RegisterCellEffect<Effects.EffectsChain>();
            RegisterCellEffect<Effects.Fade>();
            RegisterCellEffect<Effects.Recolor>();

            // Load the default font and screen size
            DefaultFont = LoadFont(font).GetFont(Font.FontSizes.One);
            DefaultFont.ResizeGraphicsDeviceManager(deviceManager, consoleWidth, consoleHeight, 0, 0);

            // Create the default console.
            ActiveConsole = new Consoles.Console(consoleWidth, consoleHeight);
            ActiveConsole.TextSurface.DefaultBackground = Color.Black;
            ActiveConsole.TextSurface.DefaultForeground = ColorAnsi.White;
            ((Consoles.Console)ActiveConsole).Clear();

            ConsoleRenderStack.Add(ActiveConsole);

            EngineStart?.Invoke(null, EventArgs.Empty);

            return (Consoles.Console)ActiveConsole;
        }

        /// <summary>
        /// Prepares the engine for use. This must be the first method you call on the engine, then call <see cref="Run"/> to start SadConsole.
        /// </summary>
        /// <param name="font">The font to load as the <see cref="DefaultFont"/>.</param>
        /// <param name="consoleWidth">The width of the default root console (and game window).</param>
        /// <param name="consoleHeight">The height of the default root console (and game window).</param>
        /// <returns>The default active console.</returns>
        public static void Initialize(string font, int consoleWidth, int consoleHeight)
        {
            MonoGameInstance = new SadConsoleGame(font, consoleWidth, consoleHeight);
        }

        public static void Run()
        {
            MonoGameInstance.Run();
        }

        public static SadConsoleGame MonoGameInstance;
#endif
        #endregion

        #region Cell Effects
        /// <summary>
        /// Informs the engine of the cell effect. Helps with serialization.
        /// </summary>
        /// <typeparam name="TEffectType">The effect type to register.</typeparam>
        public static void RegisterCellEffect<TEffectType>()
            where TEffectType : Effects.ICellEffect
        {
            _cellEffects.Add(typeof(TEffectType));
        }

#endregion

        /// <summary>
        /// Loads a font from a file and adds it to the <see cref="Fonts"/> collection.
        /// </summary>
        /// <param name="font">The font file to load.</param>
        /// <returns>A master font that you can generate a usable font from.</returns>
        public static FontMaster LoadFont(string font)
        {
            if (!System.IO.File.Exists(font))
                throw new Exception($"Font does not exist: {font}");

            using (var stream = System.IO.File.OpenRead(font))
            {
                var masterFont = SadConsole.Serializer.Deserialize<FontMaster>(stream);

                if (Fonts.ContainsKey(masterFont.Name))
                    Fonts.Remove(masterFont.Name);

                Fonts.Add(masterFont.Name, masterFont);
                return masterFont;
            }
        }

        private static void changeActiveConsole(IConsole oldConsole, IConsole newConsole)
        {
            if (oldConsole != newConsole)
            {
                _activeConsole = newConsole;

                if (oldConsole != null)
                    oldConsole.IsFocused = false;

                if (_activeConsole != null)
                    _activeConsole.IsFocused = true;
            }
        }
#if SFML
        public static void Draw()
        {
            GameTimeDraw.Update();
            GameTimeElapsedRender = GameTimeDraw.ElapsedGameTime.TotalSeconds;

#elif MONOGAME
        public static void Draw(GameTime gameTime)
        {
            GameTimeElapsedRender = gameTime.ElapsedGameTime.TotalSeconds;
            GameTimeDraw = gameTime;
#endif

            ConsoleRenderStack.Render();
            EngineDrawFrame?.Invoke(null, EventArgs.Empty);
        }

#if SFML
        public static void Update(bool windowIsActive)
        {
            GameTimeUpdate.Update();
            GameTimeElapsedUpdate = GameTimeUpdate.ElapsedGameTime.TotalSeconds;

#elif MONOGAME
        public static void Update(GameTime gameTime, bool windowIsActive)
        {
            GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;
            GameTimeUpdate = gameTime;
#endif
            if (windowIsActive)
            {
                if (UseKeyboard)
                    Keyboard.ProcessKeys(GameTimeUpdate);

                if (UseMouse)
                {
                    Mouse.ProcessMouse(GameTimeUpdate);
                    //Device.DisplayMode.
                    if (ProcessMouseWhenOffScreen ||
                        (Mouse.ScreenLocation.X >= 0 && Mouse.ScreenLocation.Y >= 0 &&
#if SFML
                         Mouse.ScreenLocation.X < Device.Size.X && Mouse.ScreenLocation.Y < Device.Size.Y))
#elif MONOGAME
                         Mouse.ScreenLocation.X < Device.Viewport.Width && Mouse.ScreenLocation.Y < Device.Viewport.Height))
#endif
                    {
                        if (_activeConsole != null && _activeConsole.ExclusiveFocus)
                            _activeConsole.ProcessMouse(Mouse);
                        else
                            ConsoleRenderStack.ProcessMouse(Mouse);
                    }
                    else
                    {
                        // DOUBLE CHECK if mouse left screen then we should stop kill off lastmouse
                        if (LastMouseConsole != null)
                        {
                            Engine.LastMouseConsole.ProcessMouse(Mouse);
                            Engine.LastMouseConsole = null;
                        }
                    }
                }

                if (_activeConsole != null)
                    _activeConsole.ProcessKeyboard(Keyboard);
            }
            ConsoleRenderStack.Update();
            EngineUpdated?.Invoke(null, EventArgs.Empty);
        }

#endregion

        /// <summary>
        /// Returns the amount of cells (X,Y) given the specified <see cref="Font"/> and current <see cref="Engine.WindowWidth"/> and <see cref="Engine.WindowHeight"/> properties.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <returns>The amount of cells along the X and Y axis.</returns>
        public static Point GetScreenSizeInCells(Font font)
        {
            return new Point(WindowWidth / font.Size.X, WindowHeight / font.Size.Y);
        }

        /// <summary>
        /// Returns the amount of cells (X,Y) given the specified <see cref="TextSurface"/> and current <see cref="Engine.WindowWidth"/> and <see cref="Engine.WindowHeight"/> properties.
        /// </summary>
        /// <param name="surface">The cell surface.</param>
        /// <returns>The amount of cells along the X and Y axis.</returns>
        public static Point GetScreenSizeInCells(TextSurface surface)
        {
            return new Point(WindowWidth / surface.Font.Size.X, WindowHeight / surface.Font.Size.Y);
        }

        /// <summary>
        /// Sent with the <see cref="EngineShutdown" /> event.
        /// </summary>
        public class ShutdownEventArgs : EventArgs
        {
            /// <summary>
            /// When true, prevents the engine from shutting down.
            /// </summary>
            public bool BlockShutdown;
        }

    }

#if MONOGAME
    /// <summary>
    /// A game component to handle the SadConsole engine initialization, update, and drawing.
    /// </summary>
    public class EngineGameComponent: DrawableGameComponent
    {
        private Action initializationCallback;
        private string font;
        private int screenWidth;
        private int screenHeight;
        private GraphicsDeviceManager manager;

        public EngineGameComponent(Game game, GraphicsDeviceManager manager, string font, int screenWidth, int screenHeight, Action initializeCallback): base(game)
        {
            this.initializationCallback = initializeCallback;
            this.font = font;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.manager = manager;
        }

        public override void Initialize()
        {
            Engine.Initialize(manager, font, screenWidth, screenHeight);

            manager = null; // no need to hang on to this.

            initializationCallback?.Invoke();
        }

        public override void Update(GameTime gameTime)
        {
            Engine.Update(gameTime, this.Game.IsActive);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.Draw(gameTime);

            base.Draw(gameTime);
        }
    }

    /// <summary>
    /// A MonoGame <see cref="Game"/> instance that runs SadConsole. This is used when you don't provide one and call <see cref="Engine.Initialize(string, int, int)"/>.
    /// </summary>
    public class SadConsoleGame: Game
    {
        private string font;
        private int consoleWidth;
        private int consoleHeight;
        public GraphicsDeviceManager GraphicsDeviceManager;

        internal SadConsoleGame(string font, int consoleWidth, int consoleHeight)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.font = font;
            this.consoleHeight = consoleHeight;
            this.consoleWidth = consoleWidth;
        }

        protected override void Initialize()
        {
            // Let the XNA framework show the mouse.
            IsMouseVisible = true;

            // Uncomment these two lines to run as fast as possible
            //GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;

            // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
            Components.Add(new EngineGameComponent(this, GraphicsDeviceManager, font, consoleWidth, consoleHeight, () => { }));

            // Call the default initialize of the base class.
            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Engine.ClearFrameColor);

            base.Draw(gameTime);
        }
    }

    /// <summary>
    /// A component to draw how many frames per second the engine is performing at.
    /// </summary>
    public class FPSCounterComponent : DrawableGameComponent
    {
        TextSurfaceRenderer consoleRender;
        TextSurface console;
        SurfaceEditor editor;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public FPSCounterComponent(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            console = new TextSurface(30, 1, Engine.DefaultFont);
            editor = new SurfaceEditor(console);
            console.DefaultBackground = Color.Black;
            editor.Clear();
            consoleRender = new TextSurfaceRenderer();
        }


        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);
            editor.Clear();
            editor.Print(0, 0, fps);
            consoleRender.Render(console, Point.Zero);
        }
    }

#endif
}
