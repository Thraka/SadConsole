using Microsoft.Xna.Framework;
using System;

namespace SadConsole.Input
{
    /// <summary>
    /// Event arguments for mouse events.
    /// </summary>
    public class MouseEventArgs: EventArgs
    {
        /// <summary>
        /// The current console under the mouse.
        /// </summary>
        public SadConsole.Consoles.IConsole Console { get; private set; }
        /// <summary>
        /// The cell of the current console under the mouse.
        /// </summary>
        public Cell Cell { get; private set; }

        /// <summary>
        /// Which cell x,y the mouse is over on the console.
        /// </summary>
        public Point ConsoleLocation { get; private set; }

        /// <summary>
        /// Where the mouse is located on the screen.
        /// </summary>
        public Point ScreenLocation { get; private set; }

        /// <summary>
        /// What cell in the gameworld (top-left of window is 0,0) the mouse is located.
        /// </summary>
        public Point WorldLocation { get; private set; }

        public bool LeftButtonDown { get; private set; }
        public bool LeftButtonDoubleClicked { get; private set; }
        public bool LeftButtonClicked { get; private set; }
        public bool RightButtonDown { get; private set; }
        public bool RightButtonDoubleClicked { get; private set; }
        public bool RightButtonClicked { get; private set; }

        public int ScrollWheelValue { get; private set; }
        public int ScrollWheelValueChange { get; private set; }

        public MouseEventArgs(MouseInfo info)
        {
            this.Console = info.Console;
            this.Cell = info.Cell;
            this.ConsoleLocation = info.ConsoleLocation;
            this.ScreenLocation = info.ScreenLocation;
            this.WorldLocation = info.WorldLocation;

            this.LeftButtonDown = info.LeftButtonDown;
            this.RightButtonDown = info.RightButtonDown;
            this.LeftButtonClicked = info.LeftClicked;
            this.RightButtonClicked = info.RightClicked;
            this.LeftButtonDoubleClicked = info.LeftDoubleClicked;
            this.RightButtonDoubleClicked = info.RightDoubleClicked;

            this.ScrollWheelValue = info.ScrollWheelValue;
            this.ScrollWheelValueChange = info.ScrollWheelValueChange;
        }
    }
}
