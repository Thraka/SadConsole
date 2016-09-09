#if MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SadConsole
{
    public static partial class Engine
    {
        /// <summary>
        /// A game instance for SadConsole used when calling <see cref="Initialize(string, int, int)"/>.
        /// </summary>
        public static SadConsoleGame MonoGameInstance;

        /// <summary>
        /// When true, does not lock at 60fps. Must be set before <see cref="Initialize(string, int, int)"/> is called.
        /// </summary>
        public static bool UnlimitedFPS;

        /// <summary>
        /// The graphics device used by SadConsole.
        /// </summary>
        public static GraphicsDevice Device { get; private set; }

        /// <summary>
        /// The graphics device used by SadConsole.
        /// </summary>
        public static GraphicsDeviceManager DeviceManager { get; private set; }

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

            SetupFontAndEffects(font);

            DefaultFont.ResizeGraphicsDeviceManager(deviceManager, consoleWidth, consoleHeight, 0, 0);

            // Create the default console.
            ActiveConsole = new Consoles.Console(consoleWidth, consoleHeight);
            ActiveConsole.TextSurface.DefaultBackground = Color.Black;
            ActiveConsole.TextSurface.DefaultForeground = ColorAnsi.White;
            ((Consoles.Console)ActiveConsole).Clear();

            ConsoleRenderStack.Add(ActiveConsole);
            
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

        internal static void InitializeCompleted()
        {
            EngineStart?.Invoke(null, System.EventArgs.Empty);
        }

        public static void Run()
        {
            MonoGameInstance.Run();
        }

        public static void Draw(GameTime gameTime)
        {
            GameTimeElapsedRender = gameTime.ElapsedGameTime.TotalSeconds;
            GameTimeDraw = gameTime;

            ConsoleRenderStack.Render();
            EngineDrawFrame?.Invoke(null, System.EventArgs.Empty);
        }

        public static void Update(GameTime gameTime, bool windowIsActive)
        {
            GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;
            GameTimeUpdate = gameTime;

            if (windowIsActive)
            {
                if (UseKeyboard)
                    Keyboard.ProcessKeys(GameTimeUpdate);

                if (UseMouse)
                {
                    Mouse.ProcessMouse(GameTimeUpdate);
                    if (ProcessMouseWhenOffScreen ||
                        (Mouse.ScreenLocation.X >= 0 && Mouse.ScreenLocation.Y >= 0 &&
                         Mouse.ScreenLocation.X < WindowWidth && Mouse.ScreenLocation.Y < WindowHeight))
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
            EngineUpdated?.Invoke(null, System.EventArgs.Empty);
        }

    }
}
#endif