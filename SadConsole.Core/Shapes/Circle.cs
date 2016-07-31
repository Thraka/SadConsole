#if SFML
using SFML.Graphics;
using Point = SFML.System.Vector2i;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using SadConsole.Consoles;

namespace SadConsole.Shapes
{
    public class Circle: IShape
    {
        public Point Center;
        public int Radius;
        public ICellAppearance BorderAppearance;
        //public ICellAppearance FillAppearance;
        //public bool Fill;

        public Circle()
        {
            
        }

        public void Draw(SurfaceEditor surface)
        {
            if (BorderAppearance == null)
                BorderAppearance = new CellAppearance(Color.Blue, Color.Black, 4);

            Algorithms.Circle(Center.X, Center.Y, Radius, (x, y) => { if (surface.IsValidCell(x, y)) surface.SetCellAppearance(x, y, BorderAppearance); });
        }
    }
}
