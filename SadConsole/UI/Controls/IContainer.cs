using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// A simple container for controls.
/// </summary>
public interface IContainer : IReadOnlyList<ControlBase>
{
    /// <summary>
    /// Gets the position of the container based on any parents position.
    /// </summary>
    Point AbsolutePosition { get; }

    /// <summary>
    /// The host owning this container.
    /// </summary>
    ControlHost Host { get; }

    /// <summary>
    /// Adds a control to this container.
    /// </summary>
    /// <param name="control">The control.</param>
    void Add(ControlBase control);

    /// <summary>
    /// Removes a control from this container.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns><see langword="true"/> if item was successfully removed; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if item is not found.</returns>
    bool Remove(ControlBase control);

    /// <summary>
    /// <see langword="true"/> when the control exists in the container; otherwise, <see langword="false"/>.
    /// </summary>
    /// <param name="control">The control to find.</param>
    /// <returns>A value to indicate if this control is in this container.</returns>
    bool Contains(ControlBase control);
}
