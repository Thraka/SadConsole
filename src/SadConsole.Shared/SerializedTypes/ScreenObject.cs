using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkColor = Microsoft.Xna.Framework.Color;
using System.Linq;
using Newtonsoft.Json;

namespace SadConsole.SerializedTypes
{

    public class ScreenObjectJson : JsonConverter<ScreenObject>
    {
        public override void WriteJson(JsonWriter writer, ScreenObject value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (ScreenObjectSerialized)value);
        }

        public override ScreenObject ReadJson(JsonReader reader, Type objectType, ScreenObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<ScreenObjectSerialized>(reader);
        }
    }


    [DataContract]
    public class ScreenObjectSerialized
    {
        [DataMember] public PointSerialized Position;
        [DataMember] public bool IsVisible;
        [DataMember] public bool IsPaused;

        public static implicit operator ScreenObjectSerialized(ScreenObject screen)
        {
            return new ScreenObjectSerialized()
            {
                Position = screen.Position,
                IsVisible = screen.IsVisible,
                IsPaused = screen.IsPaused
            };
        }

        public static implicit operator ScreenObject(ScreenObjectSerialized screen)
        {
            return new ScreenObject()
            {
                Position = screen.Position,
                IsVisible = screen.IsVisible,
                IsPaused = screen.IsPaused
            };
        }
    }
}
