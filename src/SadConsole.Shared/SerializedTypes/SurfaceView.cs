using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    class SurfaceView
    {
        [DataMember]
        public Font Font;
        [DataMember]
        public Rectangle ViewArea;
        [DataMember]
        public Color DefaultForeground;
        [DataMember]
        public Color DefaultBackground;
        [DataMember]
        public Color Tint;

        public static implicit operator SurfaceView(Surfaces.SurfaceView surface)
        {
            return new SurfaceView()
            {
                Font = surface.Font,
                ViewArea = surface.ViewArea,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint
            };
        }

        public Surfaces.SurfaceView ToSurfaceView(Surfaces.ISurface originalSurface)
        {
            return new Surfaces.SurfaceView(originalSurface, ViewArea) { Tint = Tint, DefaultForeground = DefaultForeground, DefaultBackground = DefaultBackground, Font = Font };
        }
    }
}
