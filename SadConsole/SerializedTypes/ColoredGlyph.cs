﻿#nullable disable
using System;
using System.Linq;
using Newtonsoft.Json;
using SadRogue.Primitives;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.SerializedTypes;

public class ColoredGlyphJsonConverter : JsonConverter<ColoredGlyphBase>
{
    public override void WriteJson(JsonWriter writer, ColoredGlyphBase value, JsonSerializer serializer) =>
        serializer.Serialize(writer, (ColoredGlyphSerialized)value);

    public override ColoredGlyphBase ReadJson(JsonReader reader, Type objectType, ColoredGlyphBase existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        serializer.Deserialize<ColoredGlyphSerialized>(reader);
}

public class ColoredGlyphSerialized
{
    public CellDecorator[] Decorators;
    public Color Foreground;
    public Color Background;
    public int Glyph;
    public Mirror Mirror;
    public bool IsVisible;

    public static implicit operator ColoredGlyphSerialized(ColoredGlyphBase cell) => new ColoredGlyphSerialized()
    {
        Foreground = cell.Foreground,
        Background = cell.Background,
        Glyph = cell.Glyph,
        IsVisible = cell.IsVisible,
        Mirror = cell.Mirror,
        Decorators = cell.Decorators
    };

    public static implicit operator ColoredGlyphBase(ColoredGlyphSerialized cell)
    {
        var newCell = new ColoredGlyph(cell.Foreground, cell.Background, cell.Glyph, cell.Mirror)
        {
            IsVisible = cell.IsVisible,
            Decorators = cell.Decorators.Length != 0 ? cell.Decorators.ToArray() : System.Array.Empty<CellDecorator>()
        };

        return newCell;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
