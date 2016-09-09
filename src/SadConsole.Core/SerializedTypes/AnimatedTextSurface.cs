#if SFML
using FrameworkPoint = SFML.System.Vector2i;
#elif MONOGAME
using FrameworkPoint = Microsoft.Xna.Framework.Point;
#endif

using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class AnimatedTextSurface
    {
        [DataMember]
        public TextSurfaceBasic[] Frames;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;
        [DataMember]
        public float AnimationDuration;
        [DataMember]
        public SadConsole.Font Font;
        [DataMember]
        public string Name;
        [DataMember]
        public bool Repeat;
        [DataMember]
        public FrameworkPoint Center;

        public static AnimatedTextSurface FromFramework(Consoles.AnimatedTextSurface surface)
        {
            return new AnimatedTextSurface()
            {
                Frames = surface.Frames.ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                AnimationDuration = surface.AnimationDuration,
                Name = surface.Name,
                Font = surface.Font,
                Repeat = surface.Repeat,
                Center = surface.Center
            };
        }
        
        public static Consoles.AnimatedTextSurface ToFramework(AnimatedTextSurface serializedObject)
        {
            var animationSurface = new Consoles.AnimatedTextSurface(serializedObject.Name, serializedObject.Width, serializedObject.Height, serializedObject.Font);
            animationSurface.Frames = new List<TextSurfaceBasic>(serializedObject.Frames);
            animationSurface.CurrentFrameIndex = 0;
            animationSurface.AnimationDuration = serializedObject.AnimationDuration;
            animationSurface.Repeat = serializedObject.Repeat;
            animationSurface.Center = serializedObject.Center;
            return animationSurface;
        }
    }
}
