using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SadConsole.Surface;

namespace SadConsole.Shapes
{
    public class Ellipse : IShape
    {
        public Point StartingPoint;
        public Point EndingPoint;
        public Cell BorderAppearance;
        //public ICellAppearance FillAppearance;
        //public bool Fill;

        public void Draw(SurfaceEditor surface)
        {
            Algorithms.Ellipse(StartingPoint.X, StartingPoint.Y, EndingPoint.X, EndingPoint.Y, (x, y) => { if (surface.IsValidCell(x, y)) surface.SetCellAppearance(x, y, BorderAppearance); });
        }
    }
}
