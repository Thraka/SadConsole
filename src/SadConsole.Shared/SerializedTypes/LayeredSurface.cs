using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadConsole.Surfaces;

namespace SadConsole.SerializedTypes
{
    public class LayeredJsonConverter : JsonConverter<Layered>
    {
        public override void WriteJson(JsonWriter writer, Layered value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (LayeredSurfaceSerialized)value);
        }

        public override Layered ReadJson(JsonReader reader, Type objectType, Layered existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<LayeredSurfaceSerialized>(reader);
        }
    }

    [DataContract]
    public class LayeredSurfaceSerialized: ScreenObjectSerialized
    {
        [DataMember] public Basic[] Layers;

        public static implicit operator LayeredSurfaceSerialized(Layered screen)
        {
            return new LayeredSurfaceSerialized()
            {
                Position = screen.Position,
                IsVisible = screen.IsVisible,
                IsPaused = screen.IsPaused,
                Layers = screen.ToArray()
            };
        }

        public static implicit operator Layered(LayeredSurfaceSerialized screen)
        {
            var returnObject = new Layered()
            {
                Position = screen.Position,
                IsVisible = screen.IsVisible,
                IsPaused = screen.IsPaused
            };
            foreach (var screenLayer in screen.Layers)
                returnObject.Add(screenLayer);

            return returnObject;
        }
    }
}
