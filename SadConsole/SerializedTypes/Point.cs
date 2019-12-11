using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.SerializedTypes
{
    public struct PointSerialized
    {
        public int X;
        public int Y;

        public static implicit operator PointSerialized(Point point) => new PointSerialized() { X = point.X, Y = point.Y };

        public static implicit operator Point(PointSerialized point) => new Point(point.X, point.Y);
    }
}
