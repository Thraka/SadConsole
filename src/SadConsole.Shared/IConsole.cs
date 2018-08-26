using Microsoft.Xna.Framework;
using SadConsole.Input;

namespace SadConsole
{
    /// <summary>
    /// Defines a console object.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// The private virtual curser reference.
        /// </summary>
        Cursor Cursor { get; }

        /// <summary>
        /// Gets or sets whether or not this console has exclusive access to the mouse events.
        /// </summary>
        bool IsExclusiveMouse { get; set; }

        /// <summary>
        /// Gets or sets this console as the active input target.
        /// </summary>
        bool IsFocused { get; set; }

        /// <summary>
        /// Allows this console to accept keyboard input.
        /// </summary>
        bool UseKeyboard { get; set; }

        /// <summary>
        /// Allows this console to accept mouse input.
        /// </summary>
        bool UseMouse { get; set; }

        /// <summary>
        /// How many cells wide the surface is.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// How many cells high the surface is.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Font used with rendering.
        /// </summary>
        Font Font { get; set; }

        /// <summary>
        /// The poistion of the screen object.
        /// </summary>
        /// <remarks>This position has no substance.</remarks>
        Point Position { get; set; }

        /// <summary>
        /// A position that is based on the <see cref="Parent"/> position.
        /// </summary>
        Point CalculatedPosition { get; }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        bool UsePixelPositioning { get; set; }

        /// <summary>
        /// The child objects of this instance.
        /// </summary>
        ScreenObjectCollection Children { get; }

        /// <summary>
        /// The parent object that this instance is a child of.
        /// </summary>
        ScreenObject Parent { get; set; }

        /// <summary>
        /// Sets the area of the console surface that should be rendered.
        /// </summary>
        Rectangle ViewPort { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the console.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Processes the mouse.
        /// </summary>
        /// <param name="state">The mouse state related to this console.</param>
        /// <returns>True when the mouse is over this console and processing should stop.</returns>
        bool ProcessMouse(MouseConsoleState state);

        /// <summary>
        /// Called by the engine to process the keyboard.
        /// </summary>
        /// <param name="info">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        bool ProcessKeyboard(Input.Keyboard info);

        /// <summary>
        /// Tells the console it should not consider the mouse over it anymore.
        /// </summary>
        /// <param name="state"></param>
        void LostMouse(MouseConsoleState state);

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        Cell this[int x, int y] { get; }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        Cell this[int index] { get; }
    }
}