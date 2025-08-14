using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole.SerializedTypes;

/// <summary>
/// JSON converter for <see cref="ColoredGlyph"/> type.
/// </summary>
public class ColoredGlyphJsonConverter : JsonConverter<ColoredGlyph>
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, ColoredGlyph value, JsonSerializer serializer) =>
        serializer.Serialize(writer, (ColoredGlyphSerialized)value);

    /// <inheritdoc/>
    public override ColoredGlyph ReadJson(JsonReader reader, Type objectType, ColoredGlyph existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        serializer.Deserialize<ColoredGlyphSerialized>(reader);
}

/// <summary>
/// Serializable form of <see cref="ColoredGlyph"/>.
/// </summary>
public class ColoredGlyphSerialized
{
    /// <summary>
    /// Gets or sets the decorators for the glyph.
    /// </summary>
    public CellDecorator[]? Decorators;
    /// <summary>
    /// Gets or sets the foreground color.
    /// </summary>
    public Color Foreground;
    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public Color Background;
    /// <summary>
    /// Gets or sets the glyph index.
    /// </summary>
    public int Glyph;
    /// <summary>
    /// Gets or sets the mirroring of the glyph.
    /// </summary>
    public Mirror Mirror;
    /// <summary>
    /// Gets or sets whether the glyph is visible.
    /// </summary>
    public bool IsVisible;

    /// <summary>
    /// Implicitly converts a <see cref="ColoredGlyph"/> to a <see cref="ColoredGlyphSerialized"/>.
    /// </summary>
    /// <param name="cell">The cell to convert.</param>
    public static implicit operator ColoredGlyphSerialized(ColoredGlyph cell) => new ColoredGlyphSerialized()
    {
        Foreground = cell.Foreground,
        Background = cell.Background,
        Glyph = cell.Glyph,
        IsVisible = cell.IsVisible,
        Mirror = cell.Mirror,
        Decorators = cell.Decorators != null
                        ? (cell.Decorators.Count != 0
                           ? cell.Decorators.ToArray()
                           : null)
                        : null
    };

    /// <summary>
    /// Implicitly converts a <see cref="ColoredGlyphSerialized"/> to a <see cref="ColoredGlyph"/>.
    /// </summary>
    /// <param name="cell">The serialized cell to convert.</param>
    public static implicit operator ColoredGlyph(ColoredGlyphSerialized cell)
    {
        var newCell = new ColoredGlyph(cell.Foreground, cell.Background, cell.Glyph, cell.Mirror)
        {
            IsVisible = cell.IsVisible,
            Decorators = cell.Decorators != null ? new List<CellDecorator>(cell.Decorators) : null
        };

        return newCell;
    }
}
