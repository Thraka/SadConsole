using FrameworkPoint = Microsoft.Xna.Framework.Point;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

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
        public Font Font;
        [DataMember]
        public string Name;
        [DataMember]
        public bool Repeat;
        [DataMember]
        public FrameworkPoint Center;

        public static implicit operator AnimatedSurface(Surfaces.AnimatedSurface surface)
        {
            return new AnimatedSurface()
            {
                Frames = surface.Frames.Select(s => (BasicSurface)s).ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                AnimationDuration = surface.AnimationDuration,
                Name = surface.Name,
                Font = surface.Font,
                Repeat = surface.Repeat,
                Center = surface.Center
            };
        }

        public static implicit operator Surfaces.AnimatedSurface(AnimatedSurface serializedObject)
        {
            var animationSurface = new Surfaces.AnimatedSurface(serializedObject.Name, serializedObject.Width, serializedObject.Height, serializedObject.Font);
            animationSurface.Frames = new List<Surfaces.BasicSurface>(serializedObject.Frames.Select(s => (Surfaces.BasicSurface)s));
            animationSurface.CurrentFrameIndex = 0;
            animationSurface.AnimationDuration = serializedObject.AnimationDuration;
            animationSurface.Repeat = serializedObject.Repeat;
            animationSurface.Center = serializedObject.Center;
            return animationSurface;
        }
    }
}
