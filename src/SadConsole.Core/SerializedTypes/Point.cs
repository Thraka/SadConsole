#if SFML
using FrameworkPoint = SFML.System.Vector2i;
#elif MONOGAME
using FrameworkPoint = Microsoft.Xna.Framework.Point;
#endif

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

        public static Point FromFramework(FrameworkPoint point)
        {
            return new Point() { X = point.X, Y = point.Y };
        }

        public static FrameworkPoint ToFramework(Point point)
        {
            return new FrameworkPoint(point.X, point.Y);
        }
    }
}
