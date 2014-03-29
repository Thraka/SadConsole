namespace SadConsole.Entities
{
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    [DataContract]
    public class Frame: CellSurface
    {
        //public Rectangle[] CellIndexRects { get; private set; }

        public Frame(int width, int height)
            : base(width, height)
        {
        }

        protected override void OnResize()
        {
            //base.OnResize();

            //int index = 0;

            //CellIndexRects = new Rectangle[_width * _height];
            //for (int y = 0; y < _height; y++)
            //{
            //    for (int x = 0; x < _width; x++)
            //    {
            //        CellIndexRects[index] = new Rectangle(x * CellSize.X, y * CellSize.Y, CellSize.X, CellSize.Y);
            //        index++;
            //    }
            //}
            
        }
    }
}
