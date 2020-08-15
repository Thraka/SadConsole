#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    /// <summary>
    /// Various settings for SadConsole.
    /// </summary>
    public static class Settings
    {
        internal static bool LoadingEmbeddedFont = false;
        private static int _preFullScreenWidth;
        private static int _preFullScreenHeight;
        private static bool _handleResizeNone;

        /// <summary>
        /// Gets and sets the default value for <see cref="Console.UseKeyboard"/> when the console is created.
        /// </summary>
        public static bool DefaultConsoleUseKeyboard = true;

        /// <summary>
        /// The color to automatically clear the device with.
        /// </summary>
        public static Color ClearColor = Color.Black;

        /// <summary>
        /// The type of resizing options for the window.
        /// </summary>
        public static WindowResizeOptions ResizeMode = WindowResizeOptions.Scale;

        /// <summary>
        /// Allow the user to resize the window. Must be set before the game is created.
        /// </summary>
        public static bool AllowWindowResize = true;

        /// <summary>
        /// Unlimited FPS when rendering (normally limited to 60fps). Must be set before the game is created.
        /// </summary>
        public static bool UnlimitedFPS = false;

        /// <summary>
        /// When true, indicates that <see cref="SadConsole.Game.SadConsoleGameComponent.Draw(GameTime)"/> will run.
        /// </summary>
        public static bool DoDraw = true;

        /// <summary>
        /// When true, indicates that <see cref="SadConsole.Game.SadConsoleGameComponent.Draw(GameTime)"/> will render to the screen at the end.
        /// </summary>
        public static bool DoFinalDraw = true;

        /// <summary>
        /// When true, indicates that <see cref="SadConsole.Game.SadConsoleGameComponent.Update(GameTime)"/> will run.
        /// </summary>
        public static bool DoUpdate = true;

        /// <summary>
        /// The <see cref="Microsoft.Xna.Framework.Graphics.GraphicsProfile"/> value to use.
        /// </summary>
        public static Microsoft.Xna.Framework.Graphics.GraphicsProfile GraphicsProfile = Microsoft.Xna.Framework.Graphics.GraphicsProfile.Reach;

        internal static bool IsExitingFullscreen = false;

        /// <summary>
        /// Tells MonoGame to use a full screen resolution change instead of soft (quick) full screen. Must be set before the game is created.
        /// </summary>
        public static bool UseHardwareFullScreen { get; set; } = false;

        /// <summary>
        /// When not set to (0,0) this property specifies the minimum size of the game window in pixels.
        /// </summary>
        public static Point WindowMinimumSize { get; set; } = Point.Zero;

        /// <summary>
        /// When set to true, all loading and saving performed by SadConsole uses GZIP. <see cref="Global.LoadFont(string)"/> does not use this setting and always runs uncompressed.
        /// </summary>
        public static bool SerializationIsCompressed { get; set; } = false;

        /// <summary>
        /// When set to true, and a font is not specified with the <see cref="Game.Create(string, int, int, System.Action{Game})"/> overload, the IBM 8x16 extended SadConsole font will be used.
        /// </summary>
        public static bool UseDefaultExtendedFont { get; set; } = false;

        /// <summary>
        /// When <see langword="true"/> and <see cref="Settings.ResizeMode"/> is <see cref="WindowResizeOptions.None"/>, the <see cref="ToggleFullScreen"/> method will leave the <see cref="Settings.ResizeMode"/> alone.
        /// </summary>
        public static bool FullScreenPreventScaleChangeForNone { get; set; } = false;

        /// <summary>
        /// Toggles between windowed and fullscreen rendering for SadConsole.
        /// </summary>
        public static void ToggleFullScreen()
        {
            Global.GraphicsDeviceManager.ApplyChanges();

            // Coming back from fullscreen
            if (Global.GraphicsDeviceManager.IsFullScreen)
            {
                Global.GraphicsDeviceManager.IsFullScreen = !Global.GraphicsDeviceManager.IsFullScreen;

                Global.GraphicsDeviceManager.PreferredBackBufferWidth = _preFullScreenWidth;
                Global.GraphicsDeviceManager.PreferredBackBufferHeight = _preFullScreenHeight;
                Global.GraphicsDeviceManager.ApplyChanges();
            }

            // Going full screen
            else
            {
                _preFullScreenWidth = Global.GraphicsDevice.PresentationParameters.BackBufferWidth;
                _preFullScreenHeight = Global.GraphicsDevice.PresentationParameters.BackBufferHeight;

                if (Settings.ResizeMode == WindowResizeOptions.None && !FullScreenPreventScaleChangeForNone)
                {
                    _handleResizeNone = true;
                    Settings.ResizeMode = WindowResizeOptions.Scale;
                }

                Global.GraphicsDeviceManager.PreferredBackBufferWidth = Global.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                Global.GraphicsDeviceManager.PreferredBackBufferHeight = Global.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

                Global.GraphicsDeviceManager.IsFullScreen = !Global.GraphicsDeviceManager.IsFullScreen;
                Global.GraphicsDeviceManager.ApplyChanges();

                if (_handleResizeNone)
                {
                    _handleResizeNone = false;
                    Settings.ResizeMode = WindowResizeOptions.None;
                }
            }
        }

        /// <summary>
        /// Resizes the game window.
        /// </summary>
        /// <param name="width">The width of the window in pixels.</param>
        /// <param name="height">The height of the window in pixels.</param>
        public static void ResizeWindow(int width, int height)
        {
            Global.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            Global.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            Global.GraphicsDeviceManager.ApplyChanges();

            Global.ResetRendering();
        }

        /// <summary>
        /// Settings related to input.
        /// </summary>
        public static class Input
        {
            /// <summary>
            /// Not currently used
            /// </summary>
            public static bool ProcessMouseOffscreen = false;

            /// <summary>
            /// When true, the <see cref="Game"/> object updates the mouse state every update frame.
            /// </summary>
            public static bool DoMouse = true;

            /// <summary>
            /// When true, the <see cref="Game"/> object updates the keyboard state every update frame.
            /// </summary>
            public static bool DoKeyboard = true;
        }

        /// <summary>
        /// Resize modes for the final SadConsole render pass.
        /// </summary>
        public enum WindowResizeOptions
        {
            /// <summary>
            /// Stretches the <see cref="Global.RenderOutput"/> to fit the window.
            /// </summary>
            Stretch,

            /// <summary>
            /// Centers <see cref="Global.RenderOutput"/> in the window.
            /// </summary>
            Center,

            /// <summary>
            /// Scales <see cref="Global.RenderOutput"/> to fit the window as best as possible while maintaining a good picture.
            /// </summary>
            Scale,

            /// <summary>
            /// Fits <see cref="Global.RenderOutput"/> to the window using padding to maintain aspect ratio.
            /// </summary>
            Fit,

            /// <summary>
            /// <see cref="Global.RenderOutput"/> always matches the window.
            /// </summary>
            None,
        }
    }
}
