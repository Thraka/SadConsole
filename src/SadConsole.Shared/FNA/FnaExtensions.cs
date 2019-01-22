using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    public static class FnaExtensions
    {
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static void Deconstruct(this Point source, out int X, out int Y)
        {
            X = source.X;
            Y = source.Y;
        }
    }
}
