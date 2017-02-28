using FrameworkPoint = Microsoft.Xna.Framework.Point;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public struct Point
    {
        [DataMember]
        public int X;
        [DataMember]
        public int Y;

        public static implicit operator Point(FrameworkPoint point)
        {
            return new Point() { X = point.X, Y = point.Y };
        }

        public static implicit operator FrameworkPoint(Point point)
        {
            return new FrameworkPoint(point.X, point.Y);
        }
    }
}
