using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{
    public class ScrollingConsoleJsonConverter : JsonConverter<ScrollingConsole>
    {
        public override void WriteJson(JsonWriter writer, ScrollingConsole value, JsonSerializer serializer) => serializer.Serialize(writer, (ScrollingConsoleSerialized)value);

        public override ScrollingConsole ReadJson(JsonReader reader, Type objectType, ScrollingConsole existingValue, bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<ScrollingConsoleSerialized>(reader);
    }

    [DataContract]
    public class ScrollingConsoleSerialized : ConsoleSerialized
    {
        [DataMember] public RectangleSerialized ViewPort;


        public static implicit operator ScrollingConsoleSerialized(ScrollingConsole surface) => new ScrollingConsoleSerialized()
        {
            ViewPort = surface.ViewPort,
            Font = surface.Font,
            Cells = surface.Cells.Select(c => (CellSerialized)c).ToArray(),
            Width = surface.Width,
            Height = surface.Height,
            DefaultForeground = surface.DefaultForeground,
            DefaultBackground = surface.DefaultBackground,
            Tint = surface.Tint,
            UsePixelPositioning = surface.UsePixelPositioning,
            UseMouse = surface.UseMouse,
            UseKeyboard = surface.UseKeyboard,
            Position = surface.Position,
            IsVisible = surface.IsVisible,
            IsPaused = surface.IsPaused

        };

        public static implicit operator ScrollingConsole(ScrollingConsoleSerialized surface) => new ScrollingConsole(surface.Width, surface.Height, surface.Font ?? Global.FontDefault, surface.ViewPort, surface.Cells.Select(c => (Cell)c).ToArray())
        {
            Tint = surface.Tint,
            DefaultForeground = surface.DefaultForeground,
            DefaultBackground = surface.DefaultBackground,
            Position = surface.Position,
            IsVisible = surface.IsVisible,
            IsPaused = surface.IsPaused,
            UsePixelPositioning = surface.UsePixelPositioning,
            UseMouse = surface.UseMouse,
            UseKeyboard = surface.UseKeyboard,
        };
    }
}
