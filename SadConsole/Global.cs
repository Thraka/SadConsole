using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole
{
    public static class Global
    {
        /// <summary>
        /// The active screen processed by the game.
        /// </summary>
        public static Console Screen { get; set; }

        /// <summary>
        /// A global random number generator.
        /// </summary>
        public static Random Random { get; set; } = new Random();

        /// <summary>
        /// The elapsed time between now and the last update call.
        /// </summary>
        public static TimeSpan UpdateFrameDelta { get; set; }

        /// <summary>
        /// The elapsed time between now and the last draw call.
        /// </summary>
        public static TimeSpan DrawFrameDelta { get; set; }

        /// <summary>
        /// The total time the game has been running.
        /// </summary>
        public static TimeSpan GameRunningTotalTime { get; set; }

        /// <summary>
        /// Collection of fonts. Used mainly by the deserialization system.
        /// </summary>
        public static Dictionary<string, Font> Fonts { get; } = new Dictionary<string, Font>();

        /// <summary>
        /// The default font for any type that does not provide a font.
        /// </summary>
        public static Font DefaultFont { get; set; }

        /// <summary>
        /// The default font to use with <see cref="DefaultFont"/>.
        /// </summary>
        public static Font.Sizes DefaultFontSize { get; set; } = Font.Sizes.One;

        /// <summary>
        /// Global keyboard object used by SadConsole during the update frame.
        /// </summary>
        public static Input.Keyboard Keyboard { get; } = new Input.Keyboard();

        /// <summary>
        /// Global mouse object used by SadConsole during the update frame.
        /// </summary>
        public static Input.Mouse Mouse { get; } = new Input.Mouse();

        /// <summary>
        /// The stack of focused consoles used by the mouse and keyboard.
        /// </summary>
        public static FocusedScreenObjectStack FocusedScreenObjects { get; } = new FocusedScreenObjectStack();
    }
}
