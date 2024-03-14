using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// A simple container for controls.
/// </summary>
public interface IContainer : IList<ControlBase>
{
    /// <summary>
    /// Gets the position of the container based on any parents position.
    /// </summary>
    Point AbsolutePosition { get; }

    /// <summary>
    /// The host owning this container.
    /// </summary>
    ControlHost? Host { get; }

    /// <summary>
    /// Gets a control by its <see cref="ControlBase.Name"/> property.
    /// </summary>
    /// <param name="name">The name of the control.</param>
    /// <returns>The control.</returns>
    ControlBase this[string name] { get; }

    /// <summary>
    /// Checks whether or not the container has a control registered with the given name. 
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns><see langword="true"/> when the control is found; otherwise <see langword="false"/>.</returns>
    bool HasNamedControl(string name);

    /// <summary>
    /// Checks whether or not the container has a control registered with the given name. If found, the instance is assigned to the <paramref name="control"/> parameter.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <param name="control">The control instance found.</param>
    /// <returns><see langword="true"/> when the control is found; otherwise <see langword="false"/>.</returns>
    bool HasNamedControl(string name, out ControlBase? control);

    /// <summary>
    /// Gets a control by its <see cref="ControlBase.Name"/> property.
    /// </summary>
    /// <param name="name">The name of the control.</param>
    /// <returns>The control.</returns>
    ControlBase GetNamedControl(string name);
}
