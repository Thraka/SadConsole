using FrameworkColor = Microsoft.Xna.Framework.Color;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public struct ColorSerialized
    {
        [DataMember]
        public byte R;
        [DataMember]
        public byte G;
        [DataMember]
        public byte B;
        [DataMember]
        public byte A;

        public static implicit operator ColorSerialized(FrameworkColor color)
        {
            return new ColorSerialized() { R = color.R, G = color.G, B = color.B, A = color.A };
        }

        public static implicit operator FrameworkColor(ColorSerialized color)
        {
            return new FrameworkColor(color.R, color.G, color.B, color.A);
        }
    }
}
