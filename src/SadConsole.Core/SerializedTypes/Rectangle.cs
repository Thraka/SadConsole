#if SFML
using FrameworkRect = SFML.Graphics.IntRect;
#elif MONOGAME
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public struct Rectangle
    {
        [DataMember]
        public int X;
        [DataMember]
        public int Y;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;

        public static Rectangle FromFramework(FrameworkRect rect)
        {
            return new Rectangle() { X = rect.Left, Y = rect.Top, Width = rect.Width, Height = rect.Height };
        }

        public static FrameworkRect ToFramework(Rectangle rect)
        {
            return new FrameworkRect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
