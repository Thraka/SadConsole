#if MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SadConsole
{
    public static partial class Engine
    {
        private static RenderTarget2D renderTarget;
        private static SpriteBatch renderBatch;

        /// <summary>
        /// Where on the screen the engine will be rendered.
        /// </summary>
        public static Rectangle RenderRect { get; set; }
        
        /// <summary>
        /// If the <see cref="RenderRect"/> is stretched, this is the ratio difference between unstretched.
        /// </summary>
        public static Vector2 RenderScale { get; set; }


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

            DeviceManager = deviceManager;

            DefaultFont.ResizeGraphicsDeviceManager(deviceManager, consoleWidth, consoleHeight, 0, 0);

            // Create the default console.
            ActiveConsole = new Consoles.Console(consoleWidth, consoleHeight);
            ActiveConsole.TextSurface.DefaultBackground = Color.Black;
            ActiveConsole.TextSurface.DefaultForeground = ColorAnsi.White;
            ((Consoles.Console)ActiveConsole).Clear();

            ConsoleRenderStack.Add(ActiveConsole);

            renderBatch = new SpriteBatch(Device);

            ResetRendering();

            return (Consoles.Console)ActiveConsole;
        }

        /// <summary>
        /// Resets the render target and render rect to the size of the <see cref="WindowWidth"/> and <see cref="WindowHeight"/>.
        /// </summary>
        public static void ResetRendering()
        {
            renderTarget = new RenderTarget2D(Device, WindowWidth, WindowHeight);
            

            if (MonoGameInstance.DisplayOptions == SadConsoleGame.WindowResizeOptions.Center)
            {
                RenderRect = new Rectangle((DeviceManager.PreferredBackBufferWidth - Engine.WindowWidth) / 2, (DeviceManager.PreferredBackBufferHeight - Engine.WindowHeight) / 2, Engine.WindowWidth, Engine.WindowHeight);
                RenderScale = new Vector2(1);
            }
            else if (MonoGameInstance.DisplayOptions == SadConsoleGame.WindowResizeOptions.Scale)
            {
                int multiple = 2;

                // Find the bounds
                while (true)
                {
                    if (Engine.WindowWidth * multiple > DeviceManager.PreferredBackBufferWidth || Engine.WindowHeight * multiple > DeviceManager.PreferredBackBufferHeight)
                    {
                        multiple--;
                        break;
                    }

                    multiple++;
                }

                RenderRect = new Rectangle((DeviceManager.PreferredBackBufferWidth - (Engine.WindowWidth * multiple)) / 2, (DeviceManager.PreferredBackBufferHeight - (Engine.WindowHeight * multiple)) / 2, Engine.WindowWidth * multiple, Engine.WindowHeight * multiple);
                RenderScale = new Vector2(WindowWidth / ((float)WindowWidth * multiple), WindowHeight / (float)(WindowHeight * multiple));
            }
            else
            {
                RenderRect = new Rectangle(0, 0, WindowWidth, WindowHeight);
                RenderScale = new Vector2((float)DeviceManager.PreferredBackBufferWidth / Engine.MonoGameInstance.Window.ClientBounds.Width, (float)DeviceManager.PreferredBackBufferHeight / Engine.MonoGameInstance.Window.ClientBounds.Height);
            }
        }

        /// <summary>
        /// Initializes SadConsole with only a <see cref="GraphicsDevice"/>.
        /// </summary>
        /// <param name="deviceManager">The graphics device manager from MonoGame.</param>
        /// <param name="font">The font to load as the <see cref="DefaultFont"/>.</param>
        /// <param name="consoleWidth">The width of the default root console (and game window).</param>
        /// <param name="consoleHeight">The height of the default root console (and game window).</param>
        /// <returns>The default active console.</returns>
        public static Consoles.Console Initialize(GraphicsDevice device, string font, int consoleWidth, int consoleHeight)
        {
            Device = device;

            SetupFontAndEffects(font);

            // Create the default console.
            ActiveConsole = new Consoles.Console(consoleWidth, consoleHeight);
            ActiveConsole.TextSurface.DefaultBackground = Color.Black;
            ActiveConsole.TextSurface.DefaultForeground = ColorAnsi.White;
            ((Consoles.Console)ActiveConsole).Clear();

            ConsoleRenderStack.Add(ActiveConsole);
            
            renderBatch = new SpriteBatch(Device);

            ResetRendering();

            return (Consoles.Console)ActiveConsole;
        }

        /// <summary>
        /// Prepares the engine for use. This must be the first method you call on the engine, then call <see cref="Run"/> to start SadConsole.
        /// </summary>
        /// <param name="font">The font to load as the <see cref="DefaultFont"/>.</param>
        /// <param name="consoleWidth">The width of the default root console (and game window).</param>
        /// <param name="consoleHeight">The height of the default root console (and game window).</param>
        /// <param name="ctorCallback">Optional callback from the MonoGame Game class constructor.</param>
        /// <returns>The default active console.</returns>
        public static void Initialize(string font, int consoleWidth, int consoleHeight, Action<SadConsoleGame> ctorCallback = null)
        {
            MonoGameInstance = new SadConsoleGame(font, consoleWidth, consoleHeight, ctorCallback);
            MonoGameInstance.Exiting += (s, e) => EngineShutdown?.Invoke(null, new ShutdownEventArgs() { });
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

            if (DoRender)
            {
                Device.SetRenderTarget(renderTarget);
                ConsoleRenderStack.Render();
                Device.SetRenderTarget(null);

                // Render based on full screen settings

                renderBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                renderBatch.Draw(renderTarget, RenderRect, Color.White);
                renderBatch.End();
            }

            EngineDrawFrame?.Invoke(null, System.EventArgs.Empty);
        }

        public static void Update(GameTime gameTime, bool windowIsActive)
        {
            GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;
            GameTimeUpdate = gameTime;

            if (DoUpdate)
            {
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
            }
            EngineUpdated?.Invoke(null, System.EventArgs.Empty);
        }
        
        /// <summary>
        /// Only call this if you steal away the render target during the render process.
        /// </summary>
        public static void RestoreRenderTarget()
        {
            Device.SetRenderTarget(renderTarget);
        }

        /// <summary>
        /// Toggles between fullscreen. This safely restores the original window size.
        /// </summary>
        public static void ToggleFullScreen()
        {
            // Coming back from fullscreen
            if (DeviceManager.IsFullScreen)
            {
                DeviceManager.PreferredBackBufferWidth = WindowWidth;
                DeviceManager.PreferredBackBufferHeight = WindowHeight;
            }

            DeviceManager.ToggleFullScreen();
        }
    }
}
#endif