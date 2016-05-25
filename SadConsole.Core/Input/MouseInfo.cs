#if !SHARPDX
using Microsoft.Xna.Framework;
#else
using SharpDX.DirectInput;
using SharpDX.Toolkit;
#endif
using SadConsole.Consoles;

namespace SadConsole.Input
{
    /// <summary>
    /// Processes mouse input data.
    /// </summary>
    public class MouseInfo
    {
        /// <summary>
        /// The current console under the mouse.
        /// </summary>
        public IConsole Console;
        /// <summary>
        /// The cell of the current console under the mouse.
        /// </summary>
        public Cell Cell;

        /// <summary>
        /// Which cell x,y the mouse is over on the console.
        /// </summary>
        public Point ConsoleLocation;

        /// <summary>
        /// Where the mouse is located on the screen.
        /// </summary>
        public Point ScreenLocation;

        /// <summary>
        /// What cell in the gameworld (top-left of window is 0,0) the mouse is located.
        /// </summary>
        public Point WorldLocation;

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

        private System.TimeSpan _leftLastClickedTime;
        private System.TimeSpan _rightLastClickedTime;

        //public static bool operator == (MouseInfo left, MouseInfo right)
        //{
        //    return left.Console == right.Console && left.X == right.X && left.Y == right.Y;
        //}

        //public static bool operator !=(MouseInfo left, MouseInfo right)
        //{
        //    return left.Console != right.Console || left.X != right.X || left.Y != right.Y;
        //}

        /// <summary>
        /// Fills out the state of the mouse.
        /// </summary>
        /// <param name="gameTime"></param>
        public void ProcessMouse(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Microsoft.Xna.Framework.Input.MouseState currentState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            bool leftDown = currentState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            bool rightDown = currentState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

            bool newLeftClicked = LeftButtonDown && !leftDown;
            bool newRightClicked = RightButtonDown && !rightDown;

            if (!newLeftClicked)
                LeftDoubleClicked = false;
            if (!newRightClicked)
                RightDoubleClicked = false;

            if (LeftClicked && newLeftClicked && gameTime.ElapsedGameTime.TotalMilliseconds < 1000)
                LeftDoubleClicked = true;
            if (RightClicked && newRightClicked && gameTime.ElapsedGameTime.TotalMilliseconds < 1000)
                RightDoubleClicked = true;

            LeftClicked = newLeftClicked;
            RightClicked = newRightClicked;
            _leftLastClickedTime = gameTime.ElapsedGameTime;
            _rightLastClickedTime = gameTime.ElapsedGameTime;
            LeftButtonDown = leftDown;
            RightButtonDown = rightDown;

            ScrollWheelValueChange = ScrollWheelValue - currentState.ScrollWheelValue;
            ScrollWheelValue = currentState.ScrollWheelValue;

            ScreenLocation = new Point(currentState.X, currentState.Y);
        }

        /// <summary>
        /// Sets the WorldLocation and ConsoleLocation properties based on the cell size of the provided console. If absolute positioning is used on the console, then the properties will represent pixels.
        /// </summary>
        /// <param name="data">The console to get the data from.</param>
        /// <remarks>This method alters the data of the mouse information based on the provided console. It </remarks>
        public void Fill(IConsole data)
        {
            if (data.UsePixelPositioning)
            {
                WorldLocation.X = ScreenLocation.X - data.Position.X;
                WorldLocation.Y = ScreenLocation.Y - data.Position.Y;
                ConsoleLocation = WorldLocation.WorldLocationToConsole(data.Data.Font.Size.X, data.Data.Font.Size.Y);

                if (WorldLocation.X < 0)
                    ConsoleLocation.X -= 1;
                if (WorldLocation.Y < 0)
                    ConsoleLocation.Y -= 1;
            }
            else
            {
                WorldLocation = ScreenLocation.WorldLocationToConsole(data.Data.Font.Size.X, data.Data.Font.Size.Y);
                ConsoleLocation = new Point(WorldLocation.X - data.Position.X, WorldLocation.Y - data.Position.Y);
            }

            //TODO: Need to translate mouse coords by the render transform used by the console!!
            
            // If the mouse is on a console, then we need to fill out the mouse information with the console information.
            if (ConsoleLocation.X >= 0 && ConsoleLocation.X <= data.Data.ViewArea.Width - 1 &&
                ConsoleLocation.Y >= 0 && ConsoleLocation.Y <= data.Data.ViewArea.Height - 1)
            {
                ConsoleLocation = new Point(ConsoleLocation.X + data.Data.ViewArea.Location.X, ConsoleLocation.Y + data.Data.ViewArea.Location.Y);
                Cell = data.Data[ConsoleLocation.X, ConsoleLocation.Y];
                Console = data;

                // Other console previously had mouse, we'll properly tell it that it has loss it.
                if (Engine.LastMouseConsole != data)
                {
                    if (Engine.LastMouseConsole != null)
                    {
                        var info = this.Clone();
                        Engine.LastMouseConsole.ProcessMouse(info);
                    }

                    Engine.LastMouseConsole = data;
                }
            }
        }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The clone.</returns>
        public virtual MouseInfo Clone()
        {
            var returnValue = new MouseInfo();

            returnValue.Cell = this.Cell;
            returnValue.Console = this.Console;
            returnValue.ConsoleLocation = this.ConsoleLocation;
            returnValue.LeftButtonDown = this.LeftButtonDown;
            returnValue.LeftClicked = this.LeftClicked;
            returnValue.RightButtonDown = this.RightButtonDown;
            returnValue.RightClicked = this.RightClicked;
            returnValue.ScreenLocation = this.ScreenLocation;
            returnValue.WorldLocation = this.WorldLocation;

            return returnValue;
        }
    }
}
