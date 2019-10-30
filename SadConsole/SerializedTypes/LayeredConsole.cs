using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{
    public class LayeredJsonConverter : JsonConverter<LayeredConsole>
    {
        public override void WriteJson(JsonWriter writer, LayeredConsole value, JsonSerializer serializer) => serializer.Serialize(writer, (LayeredConsoleSerialized)value);

        public override LayeredConsole ReadJson(JsonReader reader, Type objectType, LayeredConsole existingValue, bool hasExistingValue, JsonSerializer serializer) => serializer.Deserialize<LayeredConsoleSerialized>(reader);
    }

    [DataContract]
    public class LayeredConsoleSerialized : ScrollingConsoleSerialized
    {
        [DataMember] public CellSurfaceLayerSerialized[] Layers;

        public static implicit operator LayeredConsoleSerialized(LayeredConsole surface) => new LayeredConsoleSerialized()
        {
            ViewPort = surface.ViewPort,
            Font = surface.Font,
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
            IsPaused = surface.IsPaused,

            Layers = surface._layers.Count != 0 ? surface._layers.Select(c => (CellSurfaceLayerSerialized)c).ToArray() : null,
        };

        public static implicit operator LayeredConsole(LayeredConsoleSerialized surface)
        {
            System.Collections.Generic.IEnumerable<CellSurfaceLayer> layers = surface.Layers != null ? surface.Layers.Select(l => (CellSurfaceLayer)l) : null;

            return new LayeredConsole(surface.Width, surface.Height, layers, surface.Font, surface.ViewPort)
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
}
