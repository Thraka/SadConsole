using System;
using SadConsole.Input;

namespace SadConsole.Components;

/// <summary>
/// A base class that implements <see cref="IComponent.ProcessMouse(IScreenObject, MouseScreenObjectState, out bool)"/> of <see cref="IComponent"/>.
/// </summary>
public abstract class MouseConsoleComponent : IComponent
{
    /// <summary>
    /// Indicates priority to other components.
    /// </summary>
    public uint SortOrder { get; set; }

    /// <summary>
    /// Called by a host when the mouse is being processed.
    /// </summary>
    /// <param name="host">The host calling the component.</param>
    /// <param name="state">The state of the mouse in relation to the console.</param>
    /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
    public abstract void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled);

    /// <inheritdoc />
    public virtual void OnAdded(IScreenObject host) { }

    /// <inheritdoc />
    public virtual void OnRemoved(IScreenObject host) { }

    uint IComponent.SortOrder => SortOrder;

    bool IComponent.IsUpdate => false;

    bool IComponent.IsRender => false;

    bool IComponent.IsMouse => true;

    bool IComponent.IsKeyboard => false;

    void IComponent.Render(IScreenObject host, TimeSpan delta) { }

    void IComponent.ProcessKeyboard(IScreenObject host, Keyboard info, out bool handled) =>
        handled = false;

    void IComponent.Update(IScreenObject host, TimeSpan delta) { }
}
