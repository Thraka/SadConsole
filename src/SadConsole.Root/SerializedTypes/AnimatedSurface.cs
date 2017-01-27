using FrameworkPoint = Microsoft.Xna.Framework.Point;

using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class AnimatedSurface
    {
        [DataMember]
        public BasicSurface[] Frames;
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

        public static AnimatedSurface FromFramework(Surfaces.AnimatedSurface surface)
        {
            return new AnimatedSurface()
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
        
        public static Surfaces.AnimatedSurface ToFramework(AnimatedSurface serializedObject)
        {
            var animationSurface = new Surfaces.AnimatedSurface(serializedObject.Name, serializedObject.Width, serializedObject.Height, serializedObject.Font);
            animationSurface.Frames = new List<BasicSurface>(serializedObject.Frames);
            animationSurface.CurrentFrameIndex = 0;
            animationSurface.AnimationDuration = serializedObject.AnimationDuration;
            animationSurface.Repeat = serializedObject.Repeat;
            animationSurface.Center = serializedObject.Center;
            return animationSurface;
        }
    }
}
