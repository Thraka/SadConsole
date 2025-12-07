using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Editor.Serialization;

public class ZoneSerialized
{
    public string Name;
    public Point[] ZoneArea;
    public ColoredGlyph Appearance;
    public Dictionary<string, string> Settings = new();

    override public string ToString() => Name;
}
