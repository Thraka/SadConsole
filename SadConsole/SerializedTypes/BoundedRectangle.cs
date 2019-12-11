using System;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole.SerializedTypes
{
    public class BoundedRectangleJsonConverter : JsonConverter<BoundedRectangle>
    {
        public override void WriteJson(JsonWriter writer, BoundedRectangle value, JsonSerializer serializer) =>
            serializer.Serialize(writer, (BoundedRectangleSerialized)value);

        public override BoundedRectangle ReadJson(JsonReader reader, Type objectType, BoundedRectangle existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            serializer.Deserialize<BoundedRectangleSerialized>(reader);
    }

    public struct BoundedRectangleSerialized
    {
        public RectangleSerialized Area;
        public RectangleSerialized Bounds;

        public static implicit operator BoundedRectangleSerialized(BoundedRectangle rect) =>
            new BoundedRectangleSerialized()
        {
            Area = rect.Area,
            Bounds = rect.BoundingBox
        };

        public static implicit operator BoundedRectangle(BoundedRectangleSerialized rect) =>
            new BoundedRectangle(rect.Area, rect.Bounds);
    }
}
