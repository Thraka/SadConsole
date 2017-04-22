using FrameworkPoint = Microsoft.Xna.Framework.Point;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class AnimatedSurfaceSerialized
    {
        [DataMember]
        public BasicSurfaceSerialized[] Frames;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;
        [DataMember]
        public float AnimationDuration;
        [DataMember]
        public FontSerialized Font;
        [DataMember]
        public string Name;
        [DataMember]
        public bool Repeat;
        [DataMember]
        public FrameworkPoint Center;

        public static implicit operator AnimatedSurfaceSerialized(Surfaces.AnimatedSurface surface)
        {
            return new AnimatedSurfaceSerialized()
            {
                Frames = surface.Frames.Select(s => (BasicSurfaceSerialized)s).ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                AnimationDuration = surface.AnimationDuration,
                Name = surface.Name,
                Font = surface.Font,
                Repeat = surface.Repeat,
                Center = surface.Center
            };
        }

        public static implicit operator Surfaces.AnimatedSurface(AnimatedSurfaceSerialized serializedObject)
        {
            var animationSurface = new Surfaces.AnimatedSurface(serializedObject.Name, serializedObject.Width, serializedObject.Height, serializedObject.Font);
            animationSurface.Frames = new List<Surfaces.NoDrawSurface>(serializedObject.Frames.Select(s => (Surfaces.NoDrawSurface)s).ToArray());
            animationSurface.CurrentFrameIndex = 0;
            animationSurface.AnimationDuration = serializedObject.AnimationDuration;
            animationSurface.Repeat = serializedObject.Repeat;
            animationSurface.Center = serializedObject.Center;
            return animationSurface;
        }
    }
}
