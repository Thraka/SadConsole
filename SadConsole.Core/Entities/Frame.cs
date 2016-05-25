namespace SadConsole.Entities
{
    using Consoles;
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    [DataContract]
    public class Frame: TextSurface
    {
        //public Rectangle[] CellIndexRects { get; private set; }

        public Frame(int width, int height, Font font)
            : base(width, height, font)
        {
        }

        public new void Save(string file)
        {
            SadConsole.Serializer.Save<Frame>(this, file);
        }

        public new static Frame Load(string file)
        {
            return SadConsole.Serializer.Load<Frame>(file);
        }
    }
}
