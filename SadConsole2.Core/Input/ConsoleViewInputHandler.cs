namespace SadConsole.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using SadConsole.Consoles;

    public class ConsoleViewInputHandler
    {
        private bool _mouseDown;
        private Point _mouseLastLocation;

        /// <summary>
        /// Allows the right-mouse button to drag the view around.
        /// </summary>
        public bool CanMoveWithRightButton { get; set; }

        /// <summary>
        /// Allows the left-mouse button to drag the view around.
        /// </summary>
        public bool CanMoveWithLeftButton { get; set; }

        /// <summary>
        /// Allows the keyboard keys to move the view around.
        /// </summary>
        public bool CanMoveWithKeyboard { get; set; }

        /// <summary>
        /// Defines the key used to move the viewport up.
        /// </summary>
        public Keys MoveUpKey { get; set; }

        /// <summary>
        /// Defines the key used to move the viewport down.
        /// </summary>
        public Keys MoveDownKey { get; set; }

        /// <summary>
        /// Defines the key used to move the viewport left.
        /// </summary>
        public Keys MoveLeftKey { get; set; }

        /// <summary>
        /// Defines the key used to move the viewport right.
        /// </summary>
        public Keys MoveRightKey { get; set; }

        public ConsoleViewInputHandler()
        {
            CanMoveWithRightButton = false;
            CanMoveWithLeftButton = false;
            CanMoveWithKeyboard = false;
            MoveUpKey = Keys.Up;
            MoveDownKey = Keys.Down;
            MoveLeftKey = Keys.Left;
            MoveRightKey = Keys.Right;
            _mouseDown = false;
            _mouseLastLocation = new Point();
        }

        public bool HandlerMouse(IConsole console, MouseInfo info)
        {
            if (console.IsVisible && console.CanUseMouse)
            {
                info.Fill(console);

                bool doDrag = (info.LeftButtonDown && CanMoveWithLeftButton) || (info.RightButtonDown && CanMoveWithRightButton);

                if (info.Console == console && doDrag)
                {
                    // Mouse just went down on us.
                    if (!_mouseDown)
                    {
                        _mouseDown = true;
                        _mouseLastLocation = new Point(info.ConsoleLocation.X, info.ConsoleLocation.Y);
                        console.ExclusiveFocus = true;
                    }
                    else
                    {
                        // Mouse has been down, still is
                        Point currentLocation = new Point(info.ConsoleLocation.X, info.ConsoleLocation.Y);

                        if (currentLocation != _mouseLastLocation)
                        {
                            Rectangle viewport = console.ViewArea;

                            viewport.X += _mouseLastLocation.X - currentLocation.X;
                            viewport.Y += _mouseLastLocation.Y - currentLocation.Y;
                            _mouseLastLocation = currentLocation;

                            console.ViewArea = viewport;
                        }
                    }

                    return true;
                }

                if (!doDrag && _mouseDown)
                {
                    console.ExclusiveFocus = false;
                    _mouseDown = false;
                }
            }

            return false;
        }

        public bool HandlerKeyboard(IConsole console, KeyboardInfo info)
        {
            //TODO: This is dependent on how fast update is working... Make independent
            bool handled = false;
            if (console.CanUseKeyboard && CanMoveWithKeyboard)
            {
                var view = console.ViewArea;

                if (info.IsKeyDown(MoveLeftKey))
                {
                    view.X -= 1;
                    handled = true;
                }
                else if (info.IsKeyDown(MoveRightKey))
                {
                    view.X += 1;
                    handled = true;
                }
                if (info.IsKeyDown(MoveUpKey))
                {
                    view.Y -= 1;
                    handled = true;
                }
                else if (info.IsKeyDown(MoveDownKey))
                {
                    view.Y += 1;
                    handled = true;
                }

                console.ViewArea = view;
            }
            return handled;
        }
    }
}
