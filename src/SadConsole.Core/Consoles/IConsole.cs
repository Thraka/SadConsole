#if SFML
using Point = SFML.System.Vector2i;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif

using SadConsole.Input;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Supports basic console management and input functionallity.
    /// </summary>
    public interface IConsole : IInput, IDraw
    {
        /// <summary>
        /// The top-left coordinate of the screen where the console is located.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// The surface of the console.
        /// </summary>
        ITextSurfaceRendered TextSurface { get; set; }

        /// <summary>
        /// A controllable cursor for the console.
        /// </summary>
        Cursor VirtualCursor { get; }

        /// <summary>
        /// A parent list containing the console. Optional.
        /// </summary>
        IConsoleList Parent { get; set; }

        /// <summary>
        /// When true, changes the <see cref="Position"/> to be in pixels rather than cell coordinates.
        /// </summary>
        bool UsePixelPositioning { get; set; }

        /// <summary>
        /// Toggles the VirtualCursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        bool AutoCursorOnFocus { get; set; }
    }
}
