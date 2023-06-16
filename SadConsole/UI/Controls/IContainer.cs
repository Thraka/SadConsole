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
}
