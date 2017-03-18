using FrameworkRect = Microsoft.Xna.Framework.Rectangle;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public struct RectangleSerialized
    {
        [DataMember]
        public int X;
        [DataMember]
        public int Y;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;

        public static implicit operator RectangleSerialized(FrameworkRect rect)
        {
            return new RectangleSerialized() { X = rect.Left, Y = rect.Top, Width = rect.Width, Height = rect.Height };
        }

        public static implicit operator FrameworkRect(RectangleSerialized rect)
        {
            return new FrameworkRect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
