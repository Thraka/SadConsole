using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
