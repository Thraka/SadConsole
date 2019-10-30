
namespace SadConsole.SerializedTypes
{
    using System.Runtime.Serialization;
    using SadRogue.Primitives;

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

        public static implicit operator RectangleSerialized(Rectangle rect) => new RectangleSerialized()
        {
            X = rect.X,
            Y = rect.Y,
            Width = rect.Width,
            Height = rect.Height
        };

        public static implicit operator Rectangle(RectangleSerialized rect) => new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
