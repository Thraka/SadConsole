using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Represents a simple object definition with a visual, positions, and name.
/// </summary>
public class SimpleObjectDefinition
{
    /// <summary>
    /// Gets or sets the visual representation of the object.
    /// </summary>
    public ColoredGlyph Visual { get; set; } = new();
    /// <summary>
    /// Gets the list of positions for the object.
    /// </summary>
    public List<Point> Positions { get; } = new();
    /// <summary>
    /// Gets or sets the name of the object.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Returns the name of the object.
    /// </summary>
    /// <returns>The name of the object.</returns>
    public override string ToString() =>
        Name;
}
