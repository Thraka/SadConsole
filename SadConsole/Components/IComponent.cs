using System;
using SadConsole.Input;

namespace SadConsole.Components;

/// <summary>
/// A component that can be added to a <see cref="IScreenObject"/>.
/// </summary>
public interface IComponent
{
    /// <summary>
    /// Indicates priority to other components.
    /// </summary>
    uint SortOrder { get; }

    /// <summary>
    /// When <see langword="true"/>, indicates that this component calls the <see cref="Update(IScreenObject, TimeSpan)"/> method.
    /// </summary>
    bool IsUpdate { get; }

    /// <summary>
    /// When <see langword="true"/>, indicates that this component calls the <see cref="Render(IScreenObject, TimeSpan)"/> method.
    /// </summary>
    bool IsRender { get; }

    /// <summary>
    /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessMouse(IScreenObject, MouseScreenObjectState, out bool)"/> method.
    /// </summary>
    bool IsMouse { get; }

    /// <summary>
    /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessKeyboard(IScreenObject, Keyboard, out bool)"/> method.
    /// </summary>
    bool IsKeyboard { get; }

    /// <summary>
    /// Called by a host on the update frame.
    /// </summary>
    /// <param name="host">The host calling the component.</param>
    /// <param name="delta">The time that has elapsed from the last call to this component.</param>
    void Update(IScreenObject host, TimeSpan delta);

    /// <summary>
    /// Called by a host on the render frame.
    /// </summary>
    /// <param name="host">The host calling the component.</param>
    /// <param name="delta">The time that has elapsed from the last call to this component.</param>
    void Render(IScreenObject host, TimeSpan delta);

    /// <summary>
    /// Called by a host when the mouse is being processed.
    /// </summary>
    /// <param name="host">The host console.</param>
    /// <param name="state">The mouse state.</param>
    /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
    void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled);

    /// <summary>
    /// Called by a host when the keyboard is being processed.
    /// </summary>
    /// <param name="host">The host that added this component.</param>
    /// <param name="keyboard">The keyboard state.</param>
    /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
    void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled);

    /// <summary>
    /// Called when the component is added to a host.
    /// </summary>
    /// <param name="host">The host that added the component.</param>
    void OnAdded(IScreenObject host);

    /// <summary>
    /// Called when the component is removed from the host.
    /// </summary>
    /// <param name="host">The host that removed the component.</param>
    void OnRemoved(IScreenObject host);

    /// <summary>
    /// Called when various states in the host change.
    /// </summary>
    /// <param name="host">The host that uses this component.</param>
    public void OnHostUpdated(IScreenObject host) { }
}
