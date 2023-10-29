#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes;

public class FontJsonConverter : JsonConverter<IFont>
{
    public override void WriteJson(JsonWriter writer, IFont value, JsonSerializer serializer) =>
        serializer.Serialize(writer, FontSerialized.FromFont(value));

    public override IFont ReadJson(JsonReader reader, Type objectType, IFont existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        FontSerialized.ToFont(serializer.Deserialize<FontSerialized>(reader));
}

public class FontSerialized
{
    public string Name;

    public static FontSerialized FromFont(IFont font) =>
        new FontSerialized() { Name = font.Name };

    public static IFont ToFont(FontSerialized font)
    {
        if (font == null)
            return null;

        return GameHost.Instance.Fonts.ContainsKey(font.Name) ? GameHost.Instance.Fonts[font.Name] : GameHost.Instance.DefaultFont;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
