using FrameworkPoint = Microsoft.Xna.Framework.Point;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using SadConsole.Surfaces;

namespace SadConsole.SerializedTypes
{
    public class AnimatedSurfaceConverterJson : JsonConverter<Surfaces.Animated>
    {
        public override void WriteJson(JsonWriter writer, Animated value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (AnimatedSurfaceSerialized)value);
        }

        public override Animated ReadJson(JsonReader reader, Type objectType, Animated existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<AnimatedSurfaceSerialized>(reader);
        }
    }

    [DataContract]
    public class AnimatedSurfaceSerialized : ScreenObjectSerialized
    {
        [DataMember] public BasicSurfaceSerialized[] Frames;
        [DataMember] public int Width;
        [DataMember] public int Height;
        [DataMember] public float AnimationDuration;
        [DataMember] public FontSerialized Font;
        [DataMember] public string Name;
        [DataMember] public bool Repeat;
        [DataMember] public FrameworkPoint Center;

        public static implicit operator AnimatedSurfaceSerialized(Surfaces.Animated surface)
        {
            return new AnimatedSurfaceSerialized()
            {
                Frames = surface.Frames.Select(s => (BasicSurfaceSerialized) s).ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                AnimationDuration = surface.AnimationDuration,
                Name = surface.Name,
                Font = surface.Font,
                Repeat = surface.Repeat,
                Center = surface.Center,
                Position = surface.Position,
                IsVisible = surface.IsVisible,
                IsPaused = surface.IsPaused
            };
        }

        public static implicit operator Surfaces.Animated(AnimatedSurfaceSerialized serializedObject)
        {
            return new Surfaces.Animated(serializedObject.Name, serializedObject.Width,
                                         serializedObject.Height, serializedObject.Font)
            {
                Frames = new List<Surfaces.BasicNoDraw>(serializedObject.Frames.Select(s => (Surfaces.BasicNoDraw) s).ToArray()),
                CurrentFrameIndex = 0,
                AnimationDuration = serializedObject.AnimationDuration,
                Repeat = serializedObject.Repeat,
                Center = serializedObject.Center,
                Position = serializedObject.Position,
                IsVisible = serializedObject.IsVisible,
                IsPaused = serializedObject.IsPaused
            };
        }
    }
}
