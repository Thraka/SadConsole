using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using SadConsole.Surfaces;

namespace SadConsole.SerializedTypes
{
    public class EntityJsonConverter : JsonConverter<Entity>
    {
        public override void WriteJson(JsonWriter writer, Entity value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (EntitySerialized)value);
        }

        public override Entity ReadJson(JsonReader reader, Type objectType, Entity existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<EntitySerialized>(reader);
        }
    }


    /// <summary>
    /// Serialized instance of a <see cref="Entity"/>.
    /// </summary>
    [DataContract]
    public class EntitySerialized
    {
        [DataMember] public string AnimationName;
        [DataMember] public List<AnimatedSurfaceSerialized> Animations;
        [DataMember] public bool IsVisible;
        [DataMember] public PointSerialized Position;
        [DataMember] public PointSerialized PositionOffset;
        [DataMember] public bool UsePixelPositioning;
        [DataMember] public string Name;

        
        public static implicit operator EntitySerialized(Entity gameObject)
        {
            var serializedObject = new EntitySerialized()
            {
                AnimationName = gameObject.Animation != null ? gameObject.Animation.Name : "",
                Animations = gameObject.Animations.Values.Select(a => (AnimatedSurfaceSerialized)a).ToList(),
                IsVisible = gameObject.IsVisible,
                Position = gameObject.Position,
                PositionOffset = gameObject.PositionOffset,
                UsePixelPositioning = gameObject.UsePixelPositioning,
                Name = gameObject.Name,
            };

            if (!gameObject.Animations.ContainsKey(serializedObject.AnimationName))
                serializedObject.Animations.Add(gameObject.Animation);

            return serializedObject;
        }

        public static implicit operator Entity(EntitySerialized serializedObject)
        {
            var gameObject = new Entity(1, 1);

            foreach (var item in serializedObject.Animations)
                gameObject.Animations[item.Name] = item;

            if (gameObject.Animations.ContainsKey(serializedObject.AnimationName))
                gameObject.Animation = gameObject.Animations[serializedObject.AnimationName];
            else
                gameObject.Animation = serializedObject.Animations[0];

            gameObject.IsVisible = serializedObject.IsVisible;
            gameObject.Position = serializedObject.Position;
            gameObject.PositionOffset = serializedObject.PositionOffset;
            gameObject.UsePixelPositioning = serializedObject.UsePixelPositioning;
            gameObject.Name = serializedObject.Name;

            return gameObject;
        }
    }
}
