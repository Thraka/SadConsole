using System;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.Components;

/// <summary>
/// Enables dragging a scrollable surface around by mouse.
/// </summary>
public class MouseDrag : MouseConsoleComponent
{
    bool _isDragging = false;
    Point _grabWorldPosition = Point.Zero;
    Point _grabOriginalPosition = Point.Zero;
    bool _previousMouseExclusiveDrag;

    /// <summary>
    /// When true, enables this component.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc/>
    /// <exception cref="Exception">Raised when the host this component is added to doesn't implement <see cref="IScreenSurface"/>.</exception>
    public override void OnAdded(IScreenObject host)
    {
        if (host is not IScreenSurface)
            throw new Exception($"Component requires {nameof(IScreenSurface)}");
    }

    /// <inheritdoc/>
    public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
    {
        var localHost = (IScreenSurface)host;
        handled = false;

        // Disabled or surface can't even scroll
        if (!IsEnabled || !localHost.Surface.IsScrollable)
            return;

        // Stopped dragging
        else if (_isDragging && !state.Mouse.LeftButtonDown)
        {
            _isDragging = false;
            localHost.IsExclusiveMouse = _previousMouseExclusiveDrag;
            handled = true;
        }

        // Dragging
        else if (_isDragging)
        {
            localHost.Surface.ViewPosition = _grabOriginalPosition + (_grabWorldPosition - state.WorldCellPosition);
            handled = true;
        }

        // Not dragging, check if we should
        else if (state.IsOnScreenObject && !_isDragging && state.Mouse.LeftButtonDown && state.Mouse.LeftButtonDownDuration == TimeSpan.Zero)
        {
            _grabWorldPosition = state.WorldCellPosition;
            _grabOriginalPosition = localHost.Surface.ViewPosition;
            _isDragging = true;
            _previousMouseExclusiveDrag = localHost.IsExclusiveMouse;
            localHost.IsExclusiveMouse = true;
            handled = true;
        }
    }
}
