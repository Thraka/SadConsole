#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.SerializedTypes
{
    using System.Runtime.Serialization;

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

        public static implicit operator ColorSerialized(Color color) => new ColorSerialized() { R = color.R, G = color.G, B = color.B, A = color.A };

        public static implicit operator Color(ColorSerialized color) => new Color(color.R, color.G, color.B, color.A);
    }
}
