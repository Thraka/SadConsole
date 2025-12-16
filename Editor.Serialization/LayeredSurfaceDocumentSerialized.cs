using System.Collections.Generic;
using SadConsole.Entities;
using SadConsole.SerializedTypes;
using SadRogue.Primitives;

namespace SadConsole.Editor.Serialization;

public class LayeredSurfaceDocumentSerialized
{
    public string Title;
    public CellSurface[] Layers;
    public bool[] LayerVisibility;
    public FontSerialized SurfaceFont;
    public Point SurfaceFontSize;
    public Point EditorFontSize;
    public DocumentOptions Options;
    public ZoneSerialized[]? Zones;
    public SimpleObjectDefinition[]? SimpleObjects;
    public Dictionary<string, string> Metadata = new();

    public LayeredScreenSurface GetLayeredScreenSurface()
    {
        LayeredScreenSurface layeredScreenSurface = new(Layers[0], FontSerialized.ToFont(SurfaceFont), SurfaceFontSize);
        for (int i = 1; i < Layers.Length; i++)
        {
            layeredScreenSurface.Layers.Add(Layers[i]);
        }

        return layeredScreenSurface;
    }
    
    public IEnumerable<Zone> GetZones()
    {
        if (Zones == null || Zones.Length == 0)
            throw new System.Exception("No zones defined in document.");

        foreach (var item in Zones)
            yield return item.ToZone();
    }
}
