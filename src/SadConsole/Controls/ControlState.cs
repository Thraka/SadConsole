using System;

namespace SadConsole.Controls
{
    /// <summary>
    /// Indicates the state of a control.
    /// </summary>
    [Flags]
    public enum ControlStates
    {
        /// <summary>
        /// Normal state. 
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The control is disabled.
        /// </summary>
        Disabled = 1 << 0,

        /// <summary>
        /// The control has focus.
        /// </summary>
        Focused = 1 << 1,

        /// <summary>
        /// The control is selected
        /// </summary>
        Clicked = 1 << 2,

        /// <summary>
        /// The mouse is over the control.
        /// </summary>
        MouseOver = 1 << 3,

        /// <summary>
        /// The left mouse button is down.
        /// </summary>
        MouseLeftButtonDown = 1 << 4,

        /// <summary>
        /// The Right mouse button is down.
        /// </summary>
        MouseRightButtonDown = 1 << 5,

        /// <summary>
        /// THe control is selected
        /// </summary>
        Selected = 1 << 6
    }
}
