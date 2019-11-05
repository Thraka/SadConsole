using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{
    public class ConsoleJsonConverter : JsonConverter<Console>
    {
        public override void WriteJson(JsonWriter writer, Console value, JsonSerializer serializer) => serializer.Serialize(writer, (ConsoleSerialized)value);

        public override Console ReadJson(JsonReader reader, Type objectType, Console existingValue,
                                         bool hasExistingValue, JsonSerializer serializer) => serializer.Deserialize<ConsoleSerialized>(reader);
    }

    [DataContract]
    public class ConsoleSerialized
    {
        [DataMember] public int Width;
        [DataMember] public int Height;
        [DataMember] public bool IsVisible;
        [DataMember] public bool IsPaused;
        [DataMember] public bool UsePixelPositioning;
        [DataMember] public bool UseMouse;
        [DataMember] public bool UseKeyboard;
        [DataMember] public PointSerialized Position;
        [DataMember] public FontSerialized Font;
        [DataMember] public ColorSerialized Tint;
        [DataMember] public ColorSerialized DefaultForeground;
        [DataMember] public ColorSerialized DefaultBackground;
        [DataMember] public CellSerialized[] Cells;

        public static implicit operator ConsoleSerialized(Console surface) => new ConsoleSerialized()
        {
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

        public static implicit operator Console(ConsoleSerialized surface) => new Console(surface.Width, surface.Height, surface.Font ?? Global.FontDefault, surface.Cells.Select(c => (Cell)c).ToArray())
        {
            Tint = surface.Tint,
            DefaultForeground = surface.DefaultForeground,
            DefaultBackground = surface.DefaultBackground,
            Position = surface.Position,
            IsVisible = surface.IsVisible,
            IsPaused = surface.IsPaused,
            UsePixelPositioning = surface.UsePixelPositioning,
            UseKeyboard = surface.UseKeyboard,
            UseMouse = surface.UseMouse,
        };
    }
}
