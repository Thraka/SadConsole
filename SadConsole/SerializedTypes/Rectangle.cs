using System;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole.SerializedTypes
{
    public class RectangleJsonConverter : JsonConverter<Rectangle>
    {
        public override void WriteJson(JsonWriter writer, Rectangle value, JsonSerializer serializer) =>
            serializer.Serialize(writer, (RectangleSerialized)value);

        public override Rectangle ReadJson(JsonReader reader, Type objectType, Rectangle existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            serializer.Deserialize<RectangleSerialized>(reader);
    }

    public struct RectangleSerialized
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public static implicit operator RectangleSerialized(Rectangle rect) => new RectangleSerialized()
        {
            X = rect.X,
            Y = rect.Y,
            Width = rect.Width,
            Height = rect.Height
        };

        public static implicit operator Rectangle(RectangleSerialized rect) => new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
