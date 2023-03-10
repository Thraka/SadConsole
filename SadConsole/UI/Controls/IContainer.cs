using System.Collections.Generic;
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
}
