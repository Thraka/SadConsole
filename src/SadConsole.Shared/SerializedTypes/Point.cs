using FrameworkPoint = Microsoft.Xna.Framework.Point;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public struct PointSerialized
    {
        [DataMember]
        public int X;
        [DataMember]
        public int Y;

        public static implicit operator PointSerialized(FrameworkPoint point)
        {
            return new PointSerialized() { X = point.X, Y = point.Y };
        }

        public static implicit operator FrameworkPoint(PointSerialized point)
        {
            return new FrameworkPoint(point.X, point.Y);
        }
    }
}
