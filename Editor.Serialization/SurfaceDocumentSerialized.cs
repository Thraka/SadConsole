#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
using SadConsole.Entities;
using SadConsole.SerializedTypes;
using SadRogue.Primitives;

namespace SadConsole.Editor.Serialization;

/// <summary>
/// Represents a serialized surface document containing cell data, fonts, zones, and metadata.
/// </summary>
public class SurfaceDocumentSerialized
{
    /// <summary>
    /// Gets or sets the title of the document.
    /// </summary>
    public string Title;

    /// <summary>
    /// Gets or sets the cell surface containing the visual data of the document.
    /// </summary>
    public CellSurface Surface;

    /// <summary>
    /// Gets or sets the font used for rendering the surface.
    /// </summary>
    public FontSerialized SurfaceFont;

    /// <summary>
    /// Gets or sets the font size used for the surface rendering.
    /// </summary>
    public Point SurfaceFontSize;

    /// <summary>
    /// Gets or sets the font size used in the editor.
    /// </summary>
    public Point EditorFontSize;

    /// <summary>
    /// Gets or sets the options for the document.
    /// </summary>
    public DocumentOptions Options;

    /// <summary>
    /// Gets or sets the array of zones defined in the document, or null if no zones are defined.
    /// </summary>
    public ZoneSerialized[]? Zones;

    /// <summary>
    /// Gets or sets the array of simple object definitions in the document, or null if none are defined.
    /// </summary>
    public SimpleObjectDefinition[]? SimpleObjects;

    /// <summary>
    /// Gets or sets metadata associated with the document.
    /// </summary>
    public Dictionary<string, string> Metadata = new();

    /// <summary>
    /// Creates a screen surface using the document's surface and the default font with the surface font size.
    /// </summary>
    /// <returns>A new screen surface instance.</returns>
    public ScreenSurface GetScreenSurface() =>
        new(Surface, GameHost.Instance.DefaultFont, SurfaceFontSize);

    /// <summary>
    /// Creates a screen surface using the document's surface and the serialized font with the surface font size.
    /// </summary>
    /// <returns>A new screen surface instance with the document's font.</returns>
    public ScreenSurface GetScreenSurfaceAndFont() =>
        new(Surface, FontSerialized.ToFont(SurfaceFont), SurfaceFontSize);

    /// <summary>
    /// Enumerates all zones defined in the document.
    /// </summary>
    /// <returns>An enumerable of zones converted from the serialized zone definitions.</returns>
    /// <exception cref="System.Exception">Thrown when no zones are defined in the document.</exception>
    public IEnumerable<Zone> GetZones()
    {
        if (Zones == null || Zones.Length == 0)
            throw new System.Exception("No zones defined in document.");

        foreach (var item in Zones)
            yield return item.ToZone();
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
