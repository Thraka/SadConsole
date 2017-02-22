using Microsoft.Xna.Framework;

using SadConsole.Input;

namespace SadConsole
{
    /// <summary>
    /// Supports basic console management and input functionallity.
    /// </summary>
    public interface IConsole : IScreen
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
        /// Allows this console to accept keyboard input.
        /// </summary>
        bool UseKeyboard { get; set; }

        /// <summary>
        /// Allows this console to accept mouse input.
        /// </summary>
        bool UseMouse { get; set; }

        /// <summary>
        /// Gets or sets whether or not this console has exclusive access to the mouse events.
        /// </summary>
        bool IsExclusiveMouse { get; set; }

        /// <summary>
        /// Called when the console had the mouse last frame but no longer has it.
        /// </summary>
        void LostMouse(MouseConsoleState state);

        /// <summary>
        /// Processes the mouse. If the mosue is over this console and the left button is clicked, this console will move to the top and become active focus of the engine.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>True when the mouse is over this console.</returns>
        bool ProcessMouse(MouseConsoleState state);

        /// <summary>
        /// Called by the engine to process the keyboard. If the <see cref="KeyboardHandler"/> has been set, that will be called instead of this method.
        /// </summary>
        /// <param name="state">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        bool ProcessKeyboard(Keyboard state);

        /// <summary>
        /// Sets or gets if the console has input focus.
        /// </summary>
        bool IsFocused { get; set; }
    }
}
