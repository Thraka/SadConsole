#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.SerializedTypes
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct PointSerialized
    {
        [DataMember]
        public int X;

        [DataMember]
        public int Y;

        public static implicit operator PointSerialized(Point point)
        {
            return new PointSerialized() { X = point.X, Y = point.Y };
        }

        public static implicit operator Point(PointSerialized point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
