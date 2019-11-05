using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{
    public class CellSurfaceLayerJson : JsonConverter<CellSurfaceLayer>
    {
        public override void WriteJson(JsonWriter writer, CellSurfaceLayer value, JsonSerializer serializer) => serializer.Serialize(writer, (CellSurfaceLayerSerialized)value);

        public override CellSurfaceLayer ReadJson(JsonReader reader, Type objectType, CellSurfaceLayer existingValue, bool hasExistingValue, JsonSerializer serializer) => serializer.Deserialize<CellSurfaceLayerSerialized>(reader);
    }

    [DataContract]
    public class CellSurfaceLayerSerialized
    {
        [DataMember] public string Name;
        [DataMember] public bool IsVisible;
        [DataMember] public int Width;
        [DataMember] public int Height;
        [DataMember] public ColorSerialized DefaultForeground;
        [DataMember] public ColorSerialized DefaultBackground;
        [DataMember] public CellSerialized[] Cells;

        public static implicit operator CellSurfaceLayerSerialized(CellSurfaceLayer screen) => new CellSurfaceLayerSerialized()
        {
            Cells = screen.Cells.Select(c => (CellSerialized)c).ToArray(),
            Width = screen.Width,
            Height = screen.Height,
            DefaultForeground = screen.DefaultForeground,
            DefaultBackground = screen.DefaultBackground,
            IsVisible = screen.IsVisible,
            Name = screen.Name
        };

        public static implicit operator CellSurfaceLayer(CellSurfaceLayerSerialized screen) => new CellSurfaceLayer(screen.Width, screen.Height, screen.Cells.Select(c => (Cell)c).ToArray())
        {
            DefaultBackground = screen.DefaultBackground,
            DefaultForeground = screen.DefaultForeground,
            Name = screen.Name
        };
    }
}
