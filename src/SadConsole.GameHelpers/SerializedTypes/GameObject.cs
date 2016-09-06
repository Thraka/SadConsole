using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Game
{
    public partial class GameObject
    {
        /// <summary>
        /// Saves this <see cref="GameObject"/> to a file.
        /// </summary>
        /// <param name="file">The file to save.</param>
        public void Save(string file)
        {
            Serialized.Save(this, file);
        }

        /// <summary>
        /// Loads a <see cref="GameObject"/> from a file.
        /// </summary>
        /// <param name="file">The file to load.</param>
        /// <returns></returns>
        public static GameObject Load(string file)
        {
            return Serialized.Load(file);
        }

        /// <summary>
        /// Serialized instance of a <see cref="GameObject"/>.
        /// </summary>
        [DataContract]
        public class Serialized
        {
            [DataMember]
            public string storedAnimationName;
            [DataMember]
            public bool storedAsName;
            [DataMember]
            public Consoles.AnimatedTextSurface.Serialized Animation;
            [DataMember]
            public List<Consoles.AnimatedTextSurface.Serialized> Animations;
            [DataMember]
            public string FontName;
            [DataMember]
            public Font.FontSizes FontSize;
            [DataMember]
            public bool IsVisible;
            [DataMember]
            public SerializedTypes.Point Position;
            [DataMember]
            public bool RepositionRects;
            [DataMember]
            public bool UsePixelPositioning;
            [DataMember]
            public string Name;
            [DataMember]
            public SerializedTypes.Point RenderOffset;

            public Serialized(GameObject gameObject)
            {
                Animations = new List<Consoles.AnimatedTextSurface.Serialized>();

                foreach (var item in gameObject.Animations)
                    Animations.Add(new Consoles.AnimatedTextSurface.Serialized(item.Value));

                IsVisible = gameObject.IsVisible;
                Position = SerializedTypes.Point.FromFramework(gameObject.position);
                RepositionRects = gameObject.repositionRects;
                UsePixelPositioning = gameObject.usePixelPositioning;
                Name = gameObject.Name;
                FontName = gameObject.font.Name;
                FontSize = gameObject.font.SizeMultiple;
                RenderOffset = SerializedTypes.Point.FromFramework(gameObject.renderOffset);

                if (gameObject.Animations.ContainsValue(gameObject.animation) && gameObject.Animations.ContainsKey(gameObject.animation.Name))
                {
                    storedAsName = true;
                    storedAnimationName = gameObject.animation.Name;
                }
                else
                {
                    storedAsName = false;
                    storedAnimationName = gameObject.animation.Name;
                    Animation = new Consoles.AnimatedTextSurface.Serialized(gameObject.animation);
                    
                }
            }

            public static void Save(GameObject gameObject, string file)
            {
                var animation = new Serialized(gameObject);
                Serializer.Save(animation, file, new Type[] { typeof(Consoles.AnimatedTextSurface[]), typeof(Consoles.AnimatedTextSurface) });
            }

            public static GameObject Load(string file)
            {
                var loadedGameObject = Serializer.Load<Serialized>(file, new Type[] { typeof(Consoles.AnimatedTextSurface[]), typeof(Consoles.AnimatedTextSurface) });
                return Get(loadedGameObject);

            }

            public static GameObject Get(Serialized serializedObject)
            {
                Font font;

                // Try to find font
                if (Engine.Fonts.ContainsKey(serializedObject.FontName))
                    font = Engine.Fonts[serializedObject.FontName].GetFont(serializedObject.FontSize);
                else
                    font = Engine.DefaultFont;

                var gameObject = new GameObject(font);

                gameObject.Animations = new Dictionary<string, Consoles.AnimatedTextSurface>();

                foreach (var item in serializedObject.Animations)
                {
                    var animation = Consoles.AnimatedTextSurface.Serialized.Get(item);
                    gameObject.Animations.Add(animation.Name, animation);
                }

                gameObject.IsVisible = serializedObject.IsVisible;
                gameObject.position = SerializedTypes.Point.ToFramework(serializedObject.Position);
                gameObject.usePixelPositioning = serializedObject.UsePixelPositioning;
                gameObject.Name = serializedObject.Name;
                gameObject.font = font;
                gameObject.repositionRects = serializedObject.RepositionRects;
                gameObject.renderOffset = SerializedTypes.Point.ToFramework(serializedObject.RenderOffset);

                if (serializedObject.storedAsName)
                    gameObject.animation = gameObject.Animations[serializedObject.storedAnimationName];
                else
                    gameObject.animation = Consoles.AnimatedTextSurface.Serialized.Get(serializedObject.Animation);

                gameObject.animation.Font = gameObject.font;
                gameObject.UpdateAnimationRectangles();

                return gameObject;
            }
        }
    }
}
