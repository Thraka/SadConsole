using Microsoft.Xna.Framework;

using SadConsole.Input;

namespace SadConsole
{
    /// <summary>
    /// Supports basic console management and input functionallity.
    /// </summary>
    public interface IConsole : IInput, IScreen
    {
        /// <summary>
        /// The surface of the console.
        /// </summary>
        Surfaces.ISurface TextSurface { get; set; }

        /// <summary>
        /// A controllable cursor for the console.
        /// </summary>
        Cursor VirtualCursor { get; }
        
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
