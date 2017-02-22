using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Input
{
    /// <summary>
    /// The state of the mouse.
    /// </summary>
    public class Mouse
    {
        private System.TimeSpan _leftLastClickedTime;
        private System.TimeSpan _rightLastClickedTime;
        private IConsole lastMouseConsole;

        /// <summary>
        /// The pixel position of the mouse on the screen.
        /// </summary>
        public Point ScreenPosition { get; set; }

        /// <summary>
        /// Indicates the left mouse button is currently being pressed.
        /// </summary>
        public bool LeftButtonDown { get; set; }

        /// <summary>
        /// Indicates the left mouse button was clicked. (Held and then released)
        /// </summary>
        public bool LeftClicked { get; set; }

        /// <summary>
        /// Inidcates the left mouse button was double-clicked within one second.
        /// </summary>
        public bool LeftDoubleClicked { get; set; }

        /// <summary>
        /// Indicates the right mouse button is currently being pressed.
        /// </summary>
        public bool RightButtonDown { get; set; }

        /// <summary>
        /// Indicates the right mouse button was clicked. (Held and then released)
        /// </summary>
        public bool RightClicked { get; set; }

        /// <summary>
        /// Indicates the right mouse buttion was double-clicked within one second.
        /// </summary>
        public bool RightDoubleClicked { get; set; }

        /// <summary>
        /// The cumulative value of the scroll wheel. 
        /// </summary>
        public int ScrollWheelValue { get; set; }

        /// <summary>
        /// The scroll wheel value change between frames.
        /// </summary>
        public int ScrollWheelValueChange { get; set; }

        /// <summary>
        /// Indicates that the mouse is currently within the bounds of the rendering area.
        /// </summary>
        public bool IsOnScreen { get { return Global.RenderRect.Contains(ScreenPosition); } }

        /// <summary>
        /// Updates the state of the mouse.
        /// </summary>
        /// <param name="gameTime">Delta from last update.</param>
        public void Update(GameTime gameTime)
        {
            MouseState currentState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            // Update local state
            bool leftDown = currentState.LeftButton == ButtonState.Pressed;
            bool rightDown = currentState.RightButton == ButtonState.Pressed;

            ScrollWheelValueChange = ScrollWheelValue - currentState.ScrollWheelValue;
            ScrollWheelValue = currentState.ScrollWheelValue;

            ScreenPosition = new Point((int)(currentState.X * Global.RenderScale.X), (int)(currentState.Y * Global.RenderScale.Y)) - new Point((int)(Global.RenderRect.X * Global.RenderScale.X), (int)(Global.RenderRect.Y * Global.RenderScale.Y));
            bool newLeftClicked = LeftButtonDown && !leftDown;
            bool newRightClicked = RightButtonDown && !rightDown;

            if (!newLeftClicked)
                LeftDoubleClicked = false;
            if (!newRightClicked)
                RightDoubleClicked = false;

            if (LeftClicked && newLeftClicked && gameTime.ElapsedGameTime.TotalSeconds < 1000)
                LeftDoubleClicked = true;
            if (RightClicked && newRightClicked && gameTime.ElapsedGameTime.TotalSeconds < 1000)
                RightDoubleClicked = true;

            LeftClicked = newLeftClicked;
            RightClicked = newRightClicked;
            _leftLastClickedTime = gameTime.ElapsedGameTime;
            _rightLastClickedTime = gameTime.ElapsedGameTime;
            LeftButtonDown = leftDown;
            RightButtonDown = rightDown;
        }

        public virtual void Process()
        {
            // Scan through each "console" in the current screen, including children.
            // Check if mouse is over console. Check if console can process mouse while not focused


            // NEW

            // Global.InputTarget : IConsole-Stack
            // Kill IConsole.ExclusiveFocus and Focused/LostFocus
            // IInput.ExclusiveMouse works with Global.InputTarget.
            // If Global.InputTarget.ExclusiveMouse, then Global.InputTarget.ProcessMouse.
            // else loop each Global.CurrentScreen.ProcessMouse until true.

            // Basically the mouse will always be processed on the CurrentScreen unless
            // The GLobal.InputTargets top target has ExclusiveMouse = true.
            // If False, all consoles can be used by the mouse if UseMouse is true.
            
            if (Global.FocusedConsoles.Console != null && Global.FocusedConsoles.Console.IsExclusiveMouse)
            {
                var state = new MouseConsoleState(Global.FocusedConsoles.Console, this);

                if (lastMouseConsole != null && lastMouseConsole != Global.FocusedConsoles.Console)
                {
                    lastMouseConsole.LostMouse(state);
                    lastMouseConsole = null;
                }

                Global.FocusedConsoles.Console.ProcessMouse(state);

                lastMouseConsole = Global.FocusedConsoles.Console;
            }
            else if (Global.CurrentScreen != null)
            {
                if (Global.CurrentScreen is IConsole)
                {
                    bool foundMouseTarget = false;

                    // Build a list of all consoles
                    var consoles = new List<IConsole>();
                    GetConsoles((IConsole)Global.CurrentScreen, ref consoles);

                    consoles.Reverse();

                    for (int i = 0; i < consoles.Count; i++)
                    {
                        var state = new MouseConsoleState(Global.FocusedConsoles.Console, this);

                        var relativePixel = ScreenPosition - consoles[i].Position.ConsoleLocationToPixel(consoles[i].TextSurface.Font);

                        if (consoles[i].ProcessMouse(state))
                        {
                            if (lastMouseConsole != null && lastMouseConsole != consoles[i])
                            {
                                lastMouseConsole.LostMouse(state);
                                lastMouseConsole = consoles[i];
                            }

                            foundMouseTarget = true;
                        }
                    }

                    if (!foundMouseTarget)
                        lastMouseConsole?.LostMouse(new MouseConsoleState(null, this));
                }
            }

        }

        private void GetConsoles(IConsole console, ref List<IConsole> list)
        {
            if (console.UseMouse && console.TextSurface != null)
                list.Add(console);

            foreach (var child in console.Children)
            {
                if (child is IConsole)
                    GetConsoles((IConsole)child, ref list);
            }
        }
        

        public bool IsMouseOverConsole(IConsole console)
        {
            if (console.UsePixelPositioning)
            {
                var worldPos = ScreenPosition - console.Position;
                //var consolePos = worldPos.WorldLocationToConsole(console.TextSurface.Font);

                return console.TextSurface.AbsoluteArea.Contains(worldPos);
            }
            else
            {
                var worldPos = ScreenPosition.PixelLocationToConsole(console.TextSurface.Font);
                var consolePos = worldPos - console.Position + console.TextSurface.RenderArea.Location;

                return console.TextSurface.RenderArea.Contains(console.Position);
            }
        }

        public Mouse Clone()
        {
            return new Mouse()
            {
                ScreenPosition = this.ScreenPosition,
                LeftButtonDown = this.LeftButtonDown,
                LeftClicked = this.LeftClicked,
                LeftDoubleClicked = this.LeftDoubleClicked,
                RightButtonDown = this.RightButtonDown,
                RightClicked = this.RightClicked,
                RightDoubleClicked = this.RightDoubleClicked,
                ScrollWheelValue = this.ScrollWheelValue,
                ScrollWheelValueChange = this.ScrollWheelValueChange
            };
        }
    }
}
