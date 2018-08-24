using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using SadConsole.Entities;
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

        
        public static implicit operator EntitySerialized(Entity entity)
        {
            var serializedObject = new EntitySerialized()
            {
                AnimationName = entity.Animation != null ? entity.Animation.Name : "",
                Animations = entity.Animations.Values.Select(a => (AnimatedSurfaceSerialized)a).ToList(),
                IsVisible = entity.IsVisible,
                Position = entity.Position,
                PositionOffset = entity.PositionOffset,
                UsePixelPositioning = entity.UsePixelPositioning,
                Name = entity.Name,
            };

            if (!entity.Animations.ContainsKey(serializedObject.AnimationName))
                serializedObject.Animations.Add(entity.Animation);

            return serializedObject;
        }

        public static implicit operator Entity(EntitySerialized serializedObject)
        {
            var entity = new Entity(1, 1);

            foreach (var item in serializedObject.Animations)
                entity.Animations[item.Name] = item;

            if (entity.Animations.ContainsKey(serializedObject.AnimationName))
                entity.Animation = entity.Animations[serializedObject.AnimationName];
            else
                entity.Animation = serializedObject.Animations[0];

            entity.IsVisible = serializedObject.IsVisible;
            entity.Position = serializedObject.Position;
            entity.PositionOffset = serializedObject.PositionOffset;
            entity.UsePixelPositioning = serializedObject.UsePixelPositioning;
            entity.Name = serializedObject.Name;

            return entity;
        }
    }
}
