using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace SadConsole.Editor.Serialization;

public class ZoneSerialized
{
    public string Name;
    public Point[] ZoneArea;
    public ColoredGlyph Appearance;
    public Dictionary<string, string> Settings = new();

    override public string ToString() => Name;

    public Zone ToZone()
    {
        Zone zone = new(ZoneArea);
        zone.Name = Name;
        zone.Appearance = Appearance.Clone();
        zone.Settings = new Dictionary<string, string>(Settings);

        return zone;
    }
}
