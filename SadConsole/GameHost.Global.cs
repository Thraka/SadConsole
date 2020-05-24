using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;
using SadConsole.Renderers;
using System.IO;

namespace SadConsole
{
    /// <summary>
    /// Represents the SadConsole game engine.
    /// </summary>
    public abstract partial class GameHost : IDisposable
    {
        /// <summary>
        /// Collection of fonts. Used mainly by the deserialization system.
        /// </summary>
        public Dictionary<string, Font> Fonts { get; } = new Dictionary<string, Font>();

        /// <summary>
        /// The font automatically loaded by SadConsole. Standard IBM style font.
        /// </summary>
        public Font EmbeddedFont { get; internal set; }

        /// <summary>
        /// The font automatically loaded by SadConsole. Standard IBM style font. Extended with extra SadConsole characters.
        /// </summary>
        public Font EmbeddedFontExtended { get; internal set; }

        /// <summary>
        /// The default font for any type that does not provide a font.
        /// </summary>
        public Font DefaultFont { get; set; }

        /// <summary>
        /// The default font to use with <see cref="DefaultFont"/>.
        /// </summary>
        public Font.Sizes DefaultFontSize { get; set; } = Font.Sizes.One;

        /// <summary>
        /// Global keyboard object used by SadConsole during the update frame.
        /// </summary>
        public Input.Keyboard Keyboard { get; } = new Input.Keyboard();

        /// <summary>
        /// Global mouse object used by SadConsole during the update frame.
        /// </summary>
        public Input.Mouse Mouse { get; } = new Input.Mouse();

        /// <summary>
        /// The elapsed time between now and the last update call.
        /// </summary>
        public TimeSpan UpdateFrameDelta { get; set; }

        /// <summary>
        /// The elapsed time between now and the last draw call.
        /// </summary>
        public TimeSpan DrawFrameDelta { get; set; }

        /// <summary>
        /// The total time the game has been running.
        /// </summary>
        public TimeSpan GameRunningTotalTime { get; set; }

        /// <summary>
        /// The active screen processed by the game.
        /// </summary>
        public Console Screen { get; set; }

        /// <summary>
        /// The stack of focused consoles used by the mouse and keyboard.
        /// </summary>
        public FocusedScreenObjectStack FocusedScreenObjects { get; } = new FocusedScreenObjectStack();


        /// <summary>
        /// A global random number generator.
        /// </summary>
        public Random Random { get; set; } = new Random();
    }
}
