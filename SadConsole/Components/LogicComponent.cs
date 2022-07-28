using System;
using SadConsole.Input;

namespace SadConsole.Components;

/// <summary>
/// A base class that implements <see cref="IComponent.Update(IScreenObject, TimeSpan)"/> and <see cref="IComponent.Render(IScreenObject, TimeSpan)"/> of <see cref="IComponent"/>.
/// </summary>
public abstract class LogicComponent : IComponent
{
    /// <summary>
    /// Indicates priority to other components.
    /// </summary>
    public uint SortOrder { get; set; }

    /// <summary>
    /// Called by a host on the draw frame.
    /// </summary>
    /// <param name="host">The host calling the component.</param>
    /// <param name="delta">The time that has elapsed from the last call to this component.</param>
    public abstract void Render(IScreenObject host, TimeSpan delta);

    /// <summary>
    /// Called by a host on the update frame.
    /// </summary>
    /// <param name="host">The host calling the component.</param>
    /// <param name="delta">The time that has elapsed from the last call to this component.</param>
    public abstract void Update(IScreenObject host, TimeSpan delta);

    /// <inheritdoc />
    public virtual void OnAdded(IScreenObject host) { }

    /// <inheritdoc />
    public virtual void OnRemoved(IScreenObject host) { }

    uint IComponent.SortOrder => SortOrder;

    bool IComponent.IsUpdate => true;

    bool IComponent.IsRender => true;

    bool IComponent.IsMouse => false;

    bool IComponent.IsKeyboard => false;

    void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
        handled = false;

    void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
        handled = false;
}
