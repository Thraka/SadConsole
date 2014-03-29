
namespace SadConsole.Input
{
    using SadConsole.Consoles;
    using System;

    /// <summary>
    /// Represents an object that can handle the keyboard and mouse.
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Allows this console to accept keyboard input.
        /// </summary>
        bool CanUseKeyboard { get; set; }
        /// <summary>
        /// Allows this console to accept mouse input.
        /// </summary>
        bool CanUseMouse { get; set; }
        /// <summary>
        /// Allows this console to be focusable.
        /// </summary>
        bool CanFocus { get; set; }
        /// <summary>
        /// Gets or sets this console as the <see cref="Engine.ActiveConsole"/> value.
        /// </summary>
        bool IsFocused { get; set; }
        /// <summary>
        /// Gets or sets whether or not this console has exclusive access to the mouse events.
        /// </summary>
        bool ExclusiveFocus { get; set; }
        /// <summary>
        /// Processes the mouse. If the mosue is over this console and the left button is clicked, this console will move to the top and become active focus of the engine.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>True when the mouse is over this console.</returns>
        bool ProcessMouse(MouseInfo info);
        /// <summary>
        /// Called by the engine to process the keyboard. If the <see cref="KeyboardHandler"/> has been set, that will be called instead of this method.
        /// </summary>
        /// <param name="info">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        bool ProcessKeyboard(KeyboardInfo info);
    }
}
