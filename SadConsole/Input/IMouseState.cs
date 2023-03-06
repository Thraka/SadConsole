using SadRogue.Primitives;

namespace SadConsole.Input;

/// <summary>
/// Reports the state of the mouse.
/// </summary>
public interface IMouseState
{
    /// <summary>
    /// <see langword="true"/>  when the left mouse button is pressed; otherwise, <see langword="false"/>.
    /// </summary>
    bool IsLeftButtonDown { get; }

    /// <summary>
    /// <see langword="true"/>  when the right mouse button is pressed; otherwise, <see langword="false"/>.
    /// </summary>
    bool IsRightButtonDown { get; }

    /// <summary>
    /// <see langword="true"/>  when the middle mouse button is pressed; otherwise, <see langword="false"/>.
    /// </summary>
    bool IsMiddleButtonDown { get; }

    /// <summary>
    /// The pixel position of the mouse on the screen relative to the game window.
    /// </summary>
    Point ScreenPosition { get; }

    /// <summary>
    /// The value of the mouse wheel.
    /// </summary>
    int MouseWheel { get; }

    /// <summary>
    /// If applicable to the host implementation, refreshes the mouse state.
    /// </summary>
    void Refresh();
}
