#nullable disable
using System;
using Newtonsoft.Json;
using SadRogue.Primitives;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.SerializedTypes;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer) =>
        serializer.Serialize(writer, (ColorSerialized)value);

    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        serializer.Deserialize<ColorSerialized>(reader);
}

public class ColorSerialized
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public static implicit operator ColorSerialized(Color color) =>
        new ColorSerialized()
        {
            R = color.R,
            G = color.G,
            B = color.B,
            A = color.A,
        };

    public static implicit operator Color(ColorSerialized color) =>
        new Color(color.R, color.G, color.B, color.A);
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
