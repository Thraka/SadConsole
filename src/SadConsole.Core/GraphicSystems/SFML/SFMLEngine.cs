#if SFML
using SFML.Graphics;
using System.Collections.Generic;

namespace SadConsole
{
    public static partial class Engine
    {
        /// <summary>
        /// The window being rendered to.
        /// </summary>
        public static RenderWindow Device { get; private set; }
        
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
        /// <param name="frameLimit">Defaults to 60 fps. Use 0 to indicate unlimited.</param>
        /// <returns>The default active console.</returns>
        public static Consoles.Console Initialize(string font, int consoleWidth, int consoleHeight, int frameLimit = 60)
        {
            SetupFontAndEffects(font);

            var window = new RenderWindow(new SFML.Window.VideoMode((uint)(DefaultFont.Size.X * consoleWidth), (uint)(DefaultFont.Size.Y * consoleHeight)), "SadConsole Game", SFML.Window.Styles.Titlebar | SFML.Window.Styles.Close);
            WindowWidth = (int)window.Size.X;
            WindowHeight = (int)window.Size.Y;

            window.Closed += (o, e) =>
            {
                ShutdownEventArgs args = new ShutdownEventArgs();
                EngineShutdown?.Invoke(null, args);
                if (!args.BlockShutdown)
                    ((SFML.Window.Window)o).Close();
            };

            if (frameLimit != 0)
                window.SetFramerateLimit((uint)frameLimit);

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
            EngineStart?.Invoke(null, System.EventArgs.Empty);

            while (Device.IsOpen)
            {
                Device.Clear(ClearFrameColor);

                if (DoUpdate)
                    Update(Device.HasFocus());

                if (DoRender)
                    Draw();
                
                Device.Display();
                Device.DispatchEvents();
            }
        }

        public static void Draw()
        {
            GameTimeDraw.Update();
            GameTimeElapsedRender = GameTimeDraw.ElapsedGameTime.TotalSeconds;

            ConsoleRenderStack.Render();
            EngineDrawFrame?.Invoke(null, System.EventArgs.Empty);
        }

        public static void Update(bool windowIsActive)
        {
            GameTimeUpdate.Update();
            GameTimeElapsedUpdate = GameTimeUpdate.ElapsedGameTime.TotalSeconds;

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