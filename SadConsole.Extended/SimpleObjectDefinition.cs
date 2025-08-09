using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole;

public class SimpleObjectDefinition
{
    public ColoredGlyph Visual { get; set; } = new();
    public List<Point> Positions { get; } = new();
    public string Name { get; set; }

    public override string ToString() =>
        Name;
}
