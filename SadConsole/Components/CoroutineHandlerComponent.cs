using System;
using Coroutine;
using SadConsole.Input;

namespace SadConsole.Components;

/// <summary>
/// An implementation of <see cref="CoroutineHandlerInstance"/> that calls <see cref="CoroutineHandlerInstance.Tick(TimeSpan)"/> every time <see cref="IComponent.Update(SadConsole.IScreenObject, TimeSpan)"/> is called.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Coroutines")]
public class CoroutineHandlerComponent : CoroutineHandlerInstance, IComponent
{
    /// <summary>
    /// The sort order for this component.
    /// </summary>
    public uint SortOrder { get; set; }

    bool IComponent.IsUpdate => true;

    bool IComponent.IsRender => false;

    bool IComponent.IsMouse => false;

    bool IComponent.IsKeyboard => false;

    /// <summary>
    /// Creates a new instance of the coroutine handler.
    /// </summary>
    public CoroutineHandlerComponent() { }

    void IComponent.Update(IScreenObject host, TimeSpan delta)
    {
        if (host.IsEnabled)
            Tick(delta);
    }

    void IComponent.Render(IScreenObject host, TimeSpan delta) =>
        throw new NotImplementedException();

    void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
        throw new NotImplementedException();

    void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
        throw new NotImplementedException();

    void IComponent.OnAdded(IScreenObject host) { }

    void IComponent.OnRemoved(IScreenObject host) { }
}
