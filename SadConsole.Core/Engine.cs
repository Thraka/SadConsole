#if !SHARPDX
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using SharpDX.Toolkit.Graphics;
using SharpDX;
using SharpDX.Toolkit;
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
        /// <summary>
        /// The graphics device used by SadConsole.
        /// </summary>
        public static GraphicsDevice Device { get; private set; }

        /// <summary>
        /// A collection of fonts.
        /// </summary>
        public static Dictionary<string, FontMaster> Fonts { get; internal set; }
        #endregion

        #region Methods

        #region Initialization
        /// <summary>
        /// Prepares the engine for use. This must be the first method you call on the engine.
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
            RegisterCellEffect<Effects.BlinkCharacter>();
            RegisterCellEffect<Effects.ConcurrentEffect>();
            RegisterCellEffect<Effects.Delay>();
            RegisterCellEffect<Effects.EffectsChain>();
            RegisterCellEffect<Effects.Fade>();
            RegisterCellEffect<Effects.Recolor>();

            // Load the default font and screen size
            DefaultFont = LoadFont(font).GetFont(1);
            DefaultFont.ResizeGraphicsDeviceManager(deviceManager, consoleWidth, consoleHeight, 0, 0);

            // Create the default console.
            ActiveConsole = new Consoles.Console(consoleWidth, consoleHeight);
            ActiveConsole.Data.DefaultBackground = Color.Black;
            ActiveConsole.Data.DefaultForeground = ColorAnsi.White;
            ((Consoles.Console)ActiveConsole).Clear();

            ConsoleRenderStack.Add(ActiveConsole);

            return (Consoles.Console)ActiveConsole;
        }
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

        public static void Draw(GameTime gameTime)
        {
            GameTimeElapsedRender = gameTime.ElapsedGameTime.TotalSeconds;
            GameTimeDraw = gameTime;
            ConsoleRenderStack.Render(); 
        }

        public static void Update(GameTime gameTime, bool windowIsActive)
        {
            GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;
            GameTimeUpdate = gameTime;

            if (windowIsActive)
            {
                if (UseKeyboard)
                    Keyboard.ProcessKeys(gameTime);

                if (UseMouse)
                {
                    Mouse.ProcessMouse(gameTime);
                    //Device.DisplayMode.
                    if (ProcessMouseWhenOffScreen ||
                        (Mouse.ScreenLocation.X >= 0 && Mouse.ScreenLocation.Y >= 0 &&
                         Mouse.ScreenLocation.X < Device.Viewport.Width && Mouse.ScreenLocation.Y < Device.Viewport.Height))
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
    }

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

        public EngineGameComponent(Game game, GraphicsDeviceManager manager, Action initializeCallback, string font, int screenWidth, int screenHeight): base(game)
        {
            this.initializationCallback = initializeCallback;
            this.font = font;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public override void Initialize()
        {
            SadConsole.Engine.Initialize(manager, font, screenWidth, screenHeight);

            manager = null; // no need to hang on to this.

            if (initializationCallback != null)
                initializationCallback();
        }

        public override void Update(GameTime gameTime)
        {
            SadConsole.Engine.Update(gameTime, this.Game.IsActive);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SadConsole.Engine.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
