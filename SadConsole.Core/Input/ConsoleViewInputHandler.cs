#if SFML
using Point = SFML.System.Vector2i;
using Keys = SFML.Window.Keyboard.Key;
using Rectangle = SFML.Graphics.IntRect;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endif

using SadConsole.Consoles;

namespace SadConsole.Input
{
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
                            Rectangle viewport = console.TextSurface.RenderArea;

#if SFML
                            viewport.Left += _mouseLastLocation.X - currentLocation.X;
                            viewport.Top += _mouseLastLocation.Y - currentLocation.Y;
#else
                            viewport.X += _mouseLastLocation.X - currentLocation.X;
                            viewport.Y += _mouseLastLocation.Y - currentLocation.Y;
#endif
                            _mouseLastLocation = currentLocation;

                            console.TextSurface.RenderArea = viewport;
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
                var view = console.TextSurface.RenderArea;

                if (info.IsKeyDown(MoveLeftKey))
                {
#if SFML
                    view.Left -= 1;
#else
                    view.X -= 1;
#endif
                    handled = true;
                }
                else if (info.IsKeyDown(MoveRightKey))
                {
#if SFML
                    view.Left += 1;
#else
                    view.X += 1;
#endif
                    handled = true;
                }
                if (info.IsKeyDown(MoveUpKey))
                {
#if SFML
                    view.Top -= 1;
#else
                    view.Y -= 1;
#endif
                    handled = true;
                }
                else if (info.IsKeyDown(MoveDownKey))
                {
#if SFML
                    view.Top += 1;
#else
                    view.Y += 1;
#endif
                    handled = true;
                }

                console.TextSurface.RenderArea = view;
            }
            return handled;
        }
    }
}
