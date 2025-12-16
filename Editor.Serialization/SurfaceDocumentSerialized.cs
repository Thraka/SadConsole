using System.Collections.Generic;
using SadConsole.Entities;
using SadConsole.SerializedTypes;
using SadRogue.Primitives;

namespace SadConsole.Editor.Serialization;

public class SurfaceDocumentSerialized
{
    public string Title;
    public CellSurface Surface;
    public FontSerialized SurfaceFont;
    public Point SurfaceFontSize;
    public Point EditorFontSize;
    public DocumentOptions Options;
    public ZoneSerialized[]? Zones;
    public SimpleObjectDefinition[]? SimpleObjects;
    public Dictionary<string, string> Metadata = new();

    public ScreenSurface GetScreenSurface() =>
        new(Surface, FontSerialized.ToFont(SurfaceFont), SurfaceFontSize);

    public IEnumerable<Zone> GetZones()
    {
        if (Zones == null || Zones.Length == 0)
            throw new System.Exception("No zones defined in document.");

        foreach (var item in Zones)
            yield return item.ToZone();
    }
}
