using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.Host;

/// <summary>
/// A mouse state populated by the client that embeds the Skia host.
/// </summary>
public sealed class Mouse : IMouseState
{
    /// <inheritdoc />
    public bool IsLeftButtonDown { get; private set; }

    /// <inheritdoc />
    public bool IsRightButtonDown { get; private set; }

    /// <inheritdoc />
    public bool IsMiddleButtonDown { get; private set; }

    /// <inheritdoc />
    public Point ScreenPosition { get; private set; }

    /// <inheritdoc />
    public int MouseWheel { get; private set; }

    /// <summary>
    /// Replaces the current mouse state.
    /// </summary>
    public void SetState(Point screenPosition, bool isLeftButtonDown = false,
                         bool isRightButtonDown = false, bool isMiddleButtonDown = false,
                         int mouseWheel = 0)
    {
        ScreenPosition = screenPosition;
        IsLeftButtonDown = isLeftButtonDown;
        IsRightButtonDown = isRightButtonDown;
        IsMiddleButtonDown = isMiddleButtonDown;
        MouseWheel = mouseWheel;
    }

    /// <inheritdoc />
    public void Refresh() { }
}
