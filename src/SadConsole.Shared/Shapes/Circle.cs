using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SadConsole.Surfaces;

namespace SadConsole.Shapes
{
    public class Circle: IShape
    {
        public Point Center;
        public int Radius;
        public Cell BorderAppearance;
        //public ICell FillAppearance;
        //public bool Fill;

        public Circle()
        {
            
        }

        public void Draw(SurfaceEditor surface)
        {
            if (BorderAppearance == null)
                BorderAppearance = new Cell(Color.Blue, Color.Black, 4);

            Algorithms.Circle(Center.X, Center.Y, Radius, (x, y) => { if (surface.IsValidCell(x, y)) surface.SetCell(x, y, BorderAppearance); });

            surface.TextSurface.IsDirty = true;
        }
    }
}
