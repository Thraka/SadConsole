using System;
using System.Collections.Generic;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.Components;

/// <summary>
/// Tints a surface when that surface would use the mouse. Helps debug which object is receiving mouse input as you move the mouse around.
/// </summary>
public class MouseTint : UpdateComponent
{
    IScreenObject? _previous;
    Mouse? _mouse;

    /// <inheritdoc/>
    public override void OnAdded(IScreenObject host)
    {
        _mouse = new();
        _mouse.Update(TimeSpan.Zero);
    }

    /// <inheritdoc/>
    public override void Update(IScreenObject host, TimeSpan delta)
    {
        _mouse!.Update(delta);

        // Build a list of all screen objects
        var screenObjects = new List<IScreenObject>();
        GetConsoles(GameHost.Instance.Screen!, ref screenObjects);

        // Process top-most screen objects first.
        screenObjects.Reverse();

        for (int i = 0; i < screenObjects.Count; i++)
        {
            var state = new MouseScreenObjectState(screenObjects[i], _mouse);

            if (screenObjects[i].IsVisible && screenObjects[i].UseMouse && state.IsOnScreenObject)
            {
                if (_previous != null && _previous != screenObjects[i])
                    if (_previous is IScreenSurface prevSurface)
                        prevSurface.Tint = Color.Transparent;

                _previous = screenObjects[i];

                if (_previous is IScreenSurface newSurface)
                    newSurface.Tint = Color.Purple.SetAlpha(128);

                break;
            }
        }
    }

    private static void GetConsoles(IScreenObject screen, ref List<IScreenObject> list)
    {
        if (!screen.IsVisible) return;

        if (screen.UseMouse)
            list.Add(screen);

        for (int i = 0; i < screen.Children.Count; i++)
        {
            IScreenObject child = screen.Children[i];
            GetConsoles(child, ref list);
        }
    }
}
