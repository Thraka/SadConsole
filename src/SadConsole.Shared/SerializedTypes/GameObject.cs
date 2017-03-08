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
        public FontSerialized Font;
        [DataMember]
        public bool IsVisible;
        [DataMember]
        public PointSerialized Position;
        [DataMember]
        public bool RepositionRects;
        [DataMember]
        public bool UsePixelPositioning;
        [DataMember]
        public string Name;
        [DataMember]
        public PointSerialized RenderOffset;


        
        public static implicit operator GameObjectSerialized(GameHelpers.GameObject gameObject)
        {
            var serializedObject = new GameObjectSerialized()
            {
                AnimationName = gameObject.Animation != null ? gameObject.Animation.Name : "",
                Animations = gameObject.Animations.Values.Select(a => (AnimatedSurfaceSerialized)a).ToList(),
                Font = gameObject.Font,
                IsVisible = gameObject.IsVisible,
                Position = gameObject.Position,
                RepositionRects = gameObject.RepositionRects,
                UsePixelPositioning = gameObject.UsePixelPositioning,
                Name = gameObject.Name,
                RenderOffset = gameObject.RenderOffset
            };

            if (!gameObject.Animations.ContainsKey(serializedObject.AnimationName))
                serializedObject.Animations.Add(gameObject.Animation);

            return serializedObject;
        }

        public static implicit operator GameHelpers.GameObject(GameObjectSerialized serializedObject)
        {
            var gameObject = new GameHelpers.GameObject(serializedObject.Font);

            foreach (var item in serializedObject.Animations)
                gameObject.Animations.Add(item.Name, item);

            if (gameObject.Animations.ContainsKey(serializedObject.AnimationName))
                gameObject.Animation = gameObject.Animations[serializedObject.AnimationName];
            else
                gameObject.Animation = serializedObject.Animations[0];

            gameObject.IsVisible = serializedObject.IsVisible;
            gameObject.Position = serializedObject.Position;
            gameObject.UsePixelPositioning = serializedObject.UsePixelPositioning;
            gameObject.Name = serializedObject.Name;
            gameObject.RenderOffset = serializedObject.RenderOffset;
            gameObject.RepositionRects = serializedObject.RepositionRects;

            return gameObject;
        }
    }
}
