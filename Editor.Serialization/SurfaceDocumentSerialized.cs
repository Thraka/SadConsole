using System.Collections.Generic;
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
}
