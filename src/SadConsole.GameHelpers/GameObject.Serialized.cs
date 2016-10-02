#if SFML
using Point = SFML.System.Vector2i;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Game
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
        public List<Consoles.AnimatedTextSurface> Animations;
        [DataMember]
        public Font Font;
        [DataMember]
        public bool IsVisible;
        [DataMember]
        public Point Position;
        [DataMember]
        public bool RepositionRects;
        [DataMember]
        public bool UsePixelPositioning;
        [DataMember]
        public string Name;
        [DataMember]
        public Point RenderOffset;

        public static GameObjectSerialized FromFramework(GameObject gameObject)
        {
            var serializedObject = new GameObjectSerialized()
            {
                AnimationName = gameObject.Animation != null ? gameObject.Animation.Name : "",
                Animations = new List<Consoles.AnimatedTextSurface>(gameObject.Animations.Values),
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

        public static GameObject ToFramework(GameObjectSerialized serializedObject)
        {
            var gameObject = new GameObject(serializedObject.Font);

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
