using System;
using SFML.Graphics;
using SFML.Window;
using Point = SadRogue.Primitives.Point;
using SFMLMouse = SFML.Window.Mouse;

namespace SadConsole.Host
{
    public class Mouse : SadConsole.Input.IMouseState
    {
        private int _mouseWheelValue;
        private RenderWindow _window;

        public Mouse(RenderWindow window)
        {
            window.MouseWheelScrolled += Window_MouseWheelScrolled;
            _window = window;
        }

        ~Mouse()
        {
            _window.MouseWheelScrolled -= Window_MouseWheelScrolled;
            _window = null;
        }

        private void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            _mouseWheelValue += (int)e.Delta;
        }

        public bool IsLeftButtonDown => SFMLMouse.IsButtonPressed(SFMLMouse.Button.Left);

        public bool IsRightButtonDown => SFMLMouse.IsButtonPressed(SFMLMouse.Button.Right);

        public bool IsMiddleButtonDown => SFMLMouse.IsButtonPressed(SFMLMouse.Button.Middle);

        public Point ScreenPosition
        {
            get
            {
                SFML.System.Vector2i position = SFMLMouse.GetPosition(_window);
                return new Point(position.X, position.Y);
            }
        }

        public int MouseWheel => _mouseWheelValue;
    }
}
