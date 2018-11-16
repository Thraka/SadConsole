using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SadConsole
{
    static class RectangleExtensions
    {
        public static IEnumerable<Point> GetPoints(this Rectangle rectangle)
        {
            for (var y = 0; y < rectangle.Height; y++)
            {
                for (var x = 0; x < rectangle.Width; x++)
                {
                    yield return new Point(x + rectangle.X, y + rectangle.Y);
                }
            }
        }
    }
}
