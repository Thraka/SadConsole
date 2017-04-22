using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkColor = Microsoft.Xna.Framework.Color;
using System.Linq;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class BasicSurfaceSerialized
    {
        [DataMember]
        public FontSerialized Font;
        [DataMember]
        public RectangleSerialized RenderArea;
        [DataMember]
        public CellSerialized[] Cells;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;
        [DataMember]
        public ColorSerialized DefaultForeground;
        [DataMember]
        public ColorSerialized DefaultBackground;
        [DataMember]
        public ColorSerialized Tint;

        public static implicit operator BasicSurfaceSerialized(Surfaces.BasicSurface surface)
        {
            return new BasicSurfaceSerialized()
            {
                Font = surface.Font,
                RenderArea = surface.RenderArea,
                Cells = surface.Cells.Select(c => (CellSerialized)c).ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint
            };
        }

        public static implicit operator Surfaces.BasicSurface(BasicSurfaceSerialized surface)
        {
            return new Surfaces.BasicSurface(surface.Width, surface.Height, surface.Cells.Select(c => (Cell)c).ToArray(), surface.Font, surface.RenderArea)
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
            };
        }

        public static implicit operator Surfaces.NoDrawSurface(BasicSurfaceSerialized surface)
        {
            return new Surfaces.NoDrawSurface(surface.Width, surface.Height, surface.Font, surface.RenderArea, surface.Cells.Select(c => (Cell)c).ToArray())
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
            };
        }
    }
}
