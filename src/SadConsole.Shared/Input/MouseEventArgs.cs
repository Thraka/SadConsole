using Microsoft.Xna.Framework;
using System;

namespace SadConsole.Input
{
    /// <summary>
    /// Event arguments for mouse events.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// The mouse state associated with a console.
        /// </summary>
        public MouseConsoleState MouseState;

        public MouseEventArgs(MouseConsoleState state)
        {
            MouseState = state;
        }
    }
}
