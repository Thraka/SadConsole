using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole.SerializedTypes
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class LayeredJsonConverter : JsonConverter<LayeredScreenSurface>
    {
        public override void WriteJson(JsonWriter writer, LayeredScreenSurface value, JsonSerializer serializer) => serializer.Serialize(writer, (LayeredScreenSurfaceSerialized)value);

        public override LayeredScreenSurface ReadJson(JsonReader reader, Type objectType, LayeredScreenSurface existingValue, bool hasExistingValue, JsonSerializer serializer) => serializer.Deserialize<LayeredScreenSurfaceSerialized>(reader);
    }

    [DataContract]
    public class LayeredScreenSurfaceSerialized
    {
        [DataMember] public int RenderClippedWidth;
        [DataMember] public int RenderClippedHeight;
        [DataMember] public bool RenderClipped;
        [DataMember] public LayeredScreenSurface.Layer[] Layers;
        [DataMember] public Color Tint;
        [DataMember] public bool UsePixelPositioning;
        [DataMember] public bool TintBeforeDrawCall;
        [DataMember] public Point Position;
        [DataMember] public bool UseKeyboard;
        [DataMember] public bool UseMouse;
        [DataMember] public bool FocusOnMouseClick;
        [DataMember] public bool MoveToFrontOnMouseClick;
        

        public static implicit operator LayeredScreenSurfaceSerialized(LayeredScreenSurface surface) => new LayeredScreenSurfaceSerialized()
        {
            RenderClipped = surface.RenderClipped,
            RenderClippedWidth = surface.RenderClippedWidth,
            RenderClippedHeight = surface.RenderClippedHeight,
            Layers = surface.Layers.ToArray(),
            Tint = surface.Tint,
            UsePixelPositioning = surface.UsePixelPositioning,
            TintBeforeDrawCall = surface.TintBeforeDrawCall,
            UseKeyboard = surface.UseKeyboard,
            UseMouse = surface.UseMouse,
            FocusOnMouseClick = surface.FocusOnMouseClick,
            MoveToFrontOnMouseClick = surface.MoveToFrontOnMouseClick
        };

        public static implicit operator LayeredScreenSurface(LayeredScreenSurfaceSerialized surface) =>
            new LayeredScreenSurface(surface.Layers, surface.RenderClipped, surface.RenderClippedWidth, surface.RenderClippedHeight);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
