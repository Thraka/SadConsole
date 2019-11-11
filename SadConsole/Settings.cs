using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Various settings for SadConsole.
    /// </summary>
    public static class Settings
    {
        internal static bool IsExitingFullscreen = false;

        private static int _preFullScreenWidth;
        private static int _preFullScreenHeight;
        private static bool _handleResizeNone;

        /// <summary>
        /// Gets and sets the default value for <see cref="ScreenObject.UseKeyboard"/> when the console is created.
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
        /// When true, indicates that the game loop should call <see cref="ScreenObject.Draw"/> on each object in <see cref="Global.Screen"/>.
        /// </summary>
        public static bool DoDraw = true;

        /// <summary>
        /// When true, indicates that any game framework should render a composed image, of all consoles, to the screen.
        /// </summary>
        public static bool DoFinalDraw = true;

        /// <summary>
        /// When true, indicates that the game loop should call <see cref="ScreenObject.Update"/> on each object in <see cref="Global.Screen"/>.
        /// </summary>
        public static bool DoUpdate = true;

        /// <summary>
        /// When not set to (0,0) this property specifies the minimum size of the game window in pixels.
        /// </summary>
        public static Point WindowMinimumSize { get; set; } = new Point(0, 0);

        /// <summary>
        /// When set to true, all loading and saving performed by SadConsole uses GZIP. <see cref="Global.LoadFont(string)"/> does not use this setting and always runs uncompressed.
        /// </summary>
        public static bool SerializationIsCompressed { get; set; } = false;

        /// <summary>
        /// When set to true, and a font is not specified with the <see cref="Game.Create(string, int, int, Action{Game})"/> overload, the IBM 8x16 extended SadConsole font will be used.
        /// </summary>
        public static bool UseDefaultExtendedFont { get; set; } = false;

        /// <summary>
        /// Settings related to input.
        /// </summary>
        public static class Input
        {
            /// <summary>
            /// Not currently used
            /// </summary>
            public static bool ProcessMouseOffscreen = false;

            public static bool DoMouse = true;

            public static bool DoKeyboard = true;
        }

        /// <summary>
        /// Rendering options generally set by a game host.
        /// </summary>
        public static class Rendering
        {
            /// <summary>
            /// The width of the area to render on the game window.
            /// </summary>
            public static int RenderWidth { get; set; }

            /// <summary>
            /// The height of the area to render on the game window.
            /// </summary>
            public static int RenderHeight { get; set; }

            /// <summary>
            /// Where on the screen the engine will be rendered.
            /// </summary>
            public static Rectangle RenderRect { get; set; }

            /// <summary>
            /// If the <see cref="RenderRect"/> is stretched, this is the ratio difference between unstretched.
            /// </summary>
            public static System.Numerics.Vector2 RenderScale { get; set; }
        }

        /// <summary>
        /// Resize modes for the final SadConsole render pass.
        /// </summary>
        public enum WindowResizeOptions
        {
            /// <summary>
            /// Stretches the output to fit the window.
            /// </summary>
            Stretch,

            /// <summary>
            /// Centers output in the window.
            /// </summary>
            Center,

            /// <summary>
            /// Scales output to fit the window as best as possible while maintaining a good picture.
            /// </summary>
            Scale,

            /// <summary>
            /// Fits output to the window using padding to maintain aspect ratio.
            /// </summary>
            Fit,

            /// <summary>
            /// Output always matches the window.
            /// </summary>
            None,
        }
    }
}
