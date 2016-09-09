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
    public static partial class Engine
    {
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
        /// <summary>
        /// A collection of fonts.
        /// </summary>
        public static Dictionary<string, FontMaster> Fonts { get; internal set; }
        #endregion

        #region Methods
        
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

            var masterFont = SadConsole.Serializer.Load<FontMaster>(font);

            if (Fonts.ContainsKey(masterFont.Name))
                Fonts.Remove(masterFont.Name);

            Fonts.Add(masterFont.Name, masterFont);
            return masterFont;
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

        #endregion

        #region Initalize
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
    
}
