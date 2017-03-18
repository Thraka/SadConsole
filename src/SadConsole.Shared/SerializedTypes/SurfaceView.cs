using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    class SurfaceViewSerialized
    {
        [DataMember]
        public FontSerialized Font;
        [DataMember]
        public RectangleSerialized ViewArea;
        [DataMember]
        public ColorSerialized DefaultForeground;
        [DataMember]
        public ColorSerialized DefaultBackground;
        [DataMember]
        public ColorSerialized Tint;

        public static implicit operator SurfaceViewSerialized(Surfaces.SurfaceView surface)
        {
            return new SurfaceViewSerialized()
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
