using Microsoft.Xna.Framework;
using Point = SadRogue.Primitives.Point;

namespace SadConsole.Host;

class Mouse : SadConsole.Input.IMouseState
{
    Microsoft.Xna.Framework.Input.MouseState _mouse;

    public Mouse()
    {
#if !WPF
        Refresh();
#endif
    }

    public bool IsLeftButtonDown => _mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

    public bool IsRightButtonDown => _mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

    public bool IsMiddleButtonDown => _mouse.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

    public Point ScreenPosition => new(_mouse.X, _mouse.Y);

    public int MouseWheel => _mouse.ScrollWheelValue;

    public void Refresh() =>
#if WPF
        _mouse = SadConsole.Game.Instance.MonoGameInstance.Mouse.GetState();
#else
        _mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
#endif

}
