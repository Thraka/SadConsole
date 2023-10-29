#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SadRogue.Primitives;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.SerializedTypes;

public class ColoredGlyphJsonConverter : JsonConverter<ColoredGlyph>
{
    public override void WriteJson(JsonWriter writer, ColoredGlyph value, JsonSerializer serializer) =>
        serializer.Serialize(writer, (ColoredGlyphSerialized)value);

    public override ColoredGlyph ReadJson(JsonReader reader, Type objectType, ColoredGlyph existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        serializer.Deserialize<ColoredGlyphSerialized>(reader);
}

public class ColoredGlyphSerialized
{
    public CellDecorator[]? Decorators;
    public Color Foreground;
    public Color Background;
    public int Glyph;
    public Mirror Mirror;
    public bool IsVisible;

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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
