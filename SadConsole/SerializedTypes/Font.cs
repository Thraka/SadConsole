using System;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{
    public class FontJsonConverter : JsonConverter<Font>
    {
        public override void WriteJson(JsonWriter writer, Font value, JsonSerializer serializer) =>
            serializer.Serialize(writer, (FontSerialized)value);

        public override Font ReadJson(JsonReader reader, Type objectType, Font existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            serializer.Deserialize<FontSerialized>(reader);
    }

    public class FontSerialized
    {
        public string Name;

        public static implicit operator FontSerialized(Font font) =>
            new FontSerialized() { Name = font.Name };

        public static implicit operator Font(FontSerialized font)
        {
            if (font == null)
                return null;

            return GameHost.Instance.Fonts.ContainsKey(font.Name) ? GameHost.Instance.Fonts[font.Name] : GameHost.Instance.DefaultFont;
        }
    }
}
