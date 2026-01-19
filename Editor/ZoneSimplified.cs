using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Editor;

public class ZoneSimplified
{
    public string Name;
    public Area ZoneArea;
    public ColoredGlyph Appearance;
    public Dictionary<string, string> Settings = new();

    override public string ToString() => Name;

    public static implicit operator ZoneSimplified(Serialization.ZoneSerialized zone)
    {
        return new ZoneSimplified
        {
            Name = zone.Name,
            ZoneArea = new Area(zone.ZoneArea),
            Appearance = zone.Appearance,
            Settings = zone.Settings
        };
    }

    public static implicit operator Serialization.ZoneSerialized(ZoneSimplified zone)
    {
        return new Serialization.ZoneSerialized
        {
            Name = zone.Name,
            ZoneArea = zone.ZoneArea.ToArray(),
            Appearance = zone.Appearance,
            Settings = zone.Settings
        };
    }
}
