using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Draw(TextSurface surface)
        {
            if (BorderAppearance == null)
                BorderAppearance = new CellAppearance(Color.Blue, Color.Black, 4);

            Algorithms.Circle(Center.X, Center.Y, Radius, (x, y) => { if (surface.IsValidCell(x, y)) surface.SetCellAppearance(x, y, BorderAppearance); });
        }
    }
}
