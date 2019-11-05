#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{
#pragma warning disable 1591
    public class AnimatedConsoleConverterJson : JsonConverter<AnimatedConsole>
    {
        public override void WriteJson(JsonWriter writer, AnimatedConsole value, JsonSerializer serializer) => serializer.Serialize(writer, (AnimatedConsoleSerialized)value);

        public override AnimatedConsole ReadJson(JsonReader reader, Type objectType,
            AnimatedConsole existingValue, bool hasExistingValue, JsonSerializer serializer) => serializer.Deserialize<AnimatedConsoleSerialized>(reader);
    }

    [DataContract]
    public class AnimatedConsoleSerialized : ConsoleSerialized
    {
        [DataMember] public CellSurfaceSerialized[] Frames;
        [DataMember] public float AnimationDuration;
        [DataMember] public string Name;
        [DataMember] public bool Repeat;
        [DataMember] public Point Center;

        public static implicit operator AnimatedConsoleSerialized(AnimatedConsole surface) => new AnimatedConsoleSerialized()
        {
            Frames = surface.Frames.Select(s => (CellSurfaceSerialized)s).ToArray(),
            Width = surface.Width,
            Height = surface.Height,
            AnimationDuration = surface.AnimationDuration,
            Name = surface.Name,
            Font = surface.Font,
            Repeat = surface.Repeat,
            Center = surface.Center,
            Position = surface.Position,
            UsePixelPositioning = surface.UsePixelPositioning,
            UseMouse = surface.UseMouse,
            UseKeyboard = surface.UseKeyboard,
            IsVisible = surface.IsVisible,
            IsPaused = surface.IsPaused,

            //Cells = surface.Cells.Select(c => (CellSerialized)c).ToArray(),
            DefaultForeground = surface.DefaultForeground,
            DefaultBackground = surface.DefaultBackground,
            Tint = surface.Tint,
        };

        public static implicit operator AnimatedConsole(AnimatedConsoleSerialized serializedObject) => new AnimatedConsole(serializedObject.Name, serializedObject.Width, serializedObject.Height, serializedObject.Font)
        {
            FramesList = new List<CellSurface>(serializedObject.Frames.Select(s => (CellSurface)s).ToArray()),
            CurrentFrameIndex = 0,
            AnimationDuration = serializedObject.AnimationDuration,
            Repeat = serializedObject.Repeat,
            Center = serializedObject.Center,
            Position = serializedObject.Position,
            UsePixelPositioning = serializedObject.UsePixelPositioning,
            UseMouse = serializedObject.UseMouse,
            UseKeyboard = serializedObject.UseKeyboard,
            IsVisible = serializedObject.IsVisible,
            IsPaused = serializedObject.IsPaused,
            Font = serializedObject.Font,

            DefaultForeground = serializedObject.DefaultForeground,
            DefaultBackground = serializedObject.DefaultBackground,
            Tint = serializedObject.Tint,
        };
    }
#pragma warning restore 1591
}
