using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkColor = Microsoft.Xna.Framework.Color;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class BasicSurface
    {
        [DataMember]
        public Font Font;
        [DataMember]
        public Rectangle RenderArea;
        [DataMember]
        public Cell[] Cells;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;
        [DataMember]
        public Color DefaultForeground;
        [DataMember]
        public Color DefaultBackground;
        [DataMember]
        public Color Tint;

        public static implicit operator BasicSurface(Surfaces.BasicSurface surface)
        {
            return new BasicSurface()
            {
                Font = surface.Font,
                RenderArea = surface.RenderArea,
                Cells = surface.Cells,
                Width = surface.Width,
                Height = surface.Height,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint
            };
        }

        public static implicit operator Surfaces.BasicSurface(BasicSurface surface)
        {
            return new Surfaces.BasicSurface(surface.Width, surface.Height, surface.Cells, surface.Font, surface.RenderArea)
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
            };
        }
    }
}
