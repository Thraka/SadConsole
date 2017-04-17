using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace SadConsole.SerializedTypes
{
    /// <summary>
    /// Serialized instance of a <see cref="GameObject"/>.
    /// </summary>
    [DataContract]
    public class GameObjectSerialized
    {
        [DataMember]
        public string AnimationName;
        [DataMember]
        public List<AnimatedSurfaceSerialized> Animations;
        [DataMember]
        public bool IsVisible;
        [DataMember]
        public PointSerialized Position;
        [DataMember]
        public PointSerialized PositionOffset;
        [DataMember]
        public bool UsePixelPositioning;
        [DataMember]
        public string Name;

        
        public static implicit operator GameObjectSerialized(GameHelpers.GameObject gameObject)
        {
            var serializedObject = new GameObjectSerialized()
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

        public static implicit operator GameHelpers.GameObject(GameObjectSerialized serializedObject)
        {
            var gameObject = new GameHelpers.GameObject(1, 1);

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
