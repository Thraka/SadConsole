using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{
    public class CellSurfaceJsonConverter : JsonConverter<CellSurface>
    {
        public override void WriteJson(JsonWriter writer, CellSurface value, JsonSerializer serializer) => serializer.Serialize(writer, (CellSurfaceSerialized)value);

        public override CellSurface ReadJson(JsonReader reader, Type objectType, CellSurface existingValue, bool hasExistingValue, JsonSerializer serializer) => serializer.Deserialize<CellSurfaceSerialized>(reader);
    }

    [DataContract]
    public class CellSurfaceSerialized
    {
        [DataMember] public int Width;
        [DataMember] public int Height;
        [DataMember] public ColorSerialized DefaultForeground;
        [DataMember] public ColorSerialized DefaultBackground;
        [DataMember] public CellSerialized[] Cells;

        public static implicit operator CellSurfaceSerialized(CellSurface screen) => new CellSurfaceSerialized()
        {
            Cells = screen.Cells.Select(c => (CellSerialized)c).ToArray(),
            Width = screen.Width,
            Height = screen.Height,
            DefaultForeground = screen.DefaultForeground,
            DefaultBackground = screen.DefaultBackground,
        };

        public static implicit operator CellSurface(CellSurfaceSerialized screen) => new CellSurface(screen.Width, screen.Height, screen.Cells.Select(c => (Cell)c).ToArray())
        {
            DefaultBackground = screen.DefaultBackground,
            DefaultForeground = screen.DefaultForeground,
        };
    }
}
