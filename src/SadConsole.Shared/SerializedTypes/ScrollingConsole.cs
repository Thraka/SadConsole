using System;
using System.Runtime.Serialization;
using System.Linq;
using Newtonsoft.Json;
using SadConsole;

namespace SadConsole.SerializedTypes
{
    public class ScrollingConsoleJsonConverter: JsonConverter<ScrollingConsole>
    {
        public override void WriteJson(JsonWriter writer, ScrollingConsole value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (ScrollingConsoleSerialized)value);
        }

        public override ScrollingConsole ReadJson(JsonReader reader, Type objectType, ScrollingConsole existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<ScrollingConsoleSerialized>(reader);
        }
    }


    [DataContract]
    public class ScrollingConsoleSerialized
    {
        [DataMember] public int Width;
        [DataMember] public int Height;
        [DataMember] public bool IsVisible;
        [DataMember] public bool IsPaused;
        [DataMember] public PointSerialized Position;
        [DataMember] public FontSerialized Font;
        [DataMember] public RectangleSerialized ViewPort;
        [DataMember] public ColorSerialized Tint;
        [DataMember] public ColorSerialized DefaultForeground;
        [DataMember] public ColorSerialized DefaultBackground;
        [DataMember] public CellSerialized[] Cells;

        public static implicit operator ScrollingConsoleSerialized(ScrollingConsole surface)
        {

            return new ScrollingConsoleSerialized()
            {
                ViewPort = surface.ViewPort,
                Font = surface.Font,
                Cells = surface.Cells.Select(c => (CellSerialized) c).ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint,
                Position = surface.Position,
                IsVisible = surface.IsVisible,
                IsPaused = surface.IsPaused
                
            };
        }
        
        public static implicit operator ScrollingConsole(ScrollingConsoleSerialized surface)
        {
            return new ScrollingConsole(surface.Width, surface.Height, surface.Font ?? Global.FontDefault, surface.ViewPort, surface.Cells.Select(c => (Cell) c).ToArray())
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Position = surface.Position,
                IsVisible = surface.IsVisible,
                IsPaused = surface.IsPaused
            };
        }
    }
}
