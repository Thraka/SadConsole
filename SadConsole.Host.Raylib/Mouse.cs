using System;
using Point = SadRogue.Primitives.Point;

namespace SadConsole.Host;

class Mouse : SadConsole.Input.IMouseState
{
    private int _mouseWheelValue;
    
    public Mouse()
    {
        //window.MouseWheelScrolled += Window_MouseWheelScrolled;
        //_window = window;
    }

    ~Mouse()
    {
        //_window.MouseWheelScrolled -= Window_MouseWheelScrolled;
        //_window = null;
    }

    //private void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
    //{
    //    _mouseWheelValue += (int)e.Delta;
    //}

    public bool IsLeftButtonDown => false;

    public bool IsRightButtonDown => false;

    public bool IsMiddleButtonDown => false;

    public Point ScreenPosition
    {
        get
        {
            return new Point(0, 0);
        }
    }

    public int MouseWheel => _mouseWheelValue;

    public void Refresh() =>
        throw new NotImplementedException("This method is not used by the host.");
}
