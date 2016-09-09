#if SFML
using FrameworkColor = SFML.Graphics.Color;
#elif MONOGAME
using FrameworkColor = Microsoft.Xna.Framework.Color;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public struct Color
    {
        [DataMember]
        public byte R;
        [DataMember]
        public byte G;
        [DataMember]
        public byte B;
        [DataMember]
        public byte A;

        public static Color FromFramework(FrameworkColor color)
        {
            return new Color() { R = color.R, G = color.G, B = color.B, A = color.A };
        }

        public static FrameworkColor ToFramework(Color color)
        {
            return new FrameworkColor(color.R, color.G, color.B, color.A);
        }
    }
}
