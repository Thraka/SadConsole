#if SFML
using SFML.Graphics;
using Point = SFML.System.Vector2i;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using SadConsole.Consoles;

namespace SadConsole.Shapes
{
    public class Ellipse : IShape
    {
        public Point StartingPoint;
        public Point EndingPoint;
        public ICellAppearance BorderAppearance;
        //public ICellAppearance FillAppearance;
        //public bool Fill;

        public void Draw(SurfaceEditor surface)
        {
            Algorithms.Ellipse(StartingPoint.X, StartingPoint.Y, EndingPoint.X, EndingPoint.Y, (x, y) => { if (surface.IsValidCell(x, y)) surface.SetCellAppearance(x, y, BorderAppearance); });
        }
    }
}
