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
}
