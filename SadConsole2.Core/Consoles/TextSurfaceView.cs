using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    public class TextSurfaceView : ITextSurfaceView
    {
        private ITextSurfaceView data;
        private Rectangle area;

        public Rectangle ViewArea
        {
            get { return area; }
            set
            {
                if (area.Width > data.ViewArea.Width)
                    throw new ArgumentOutOfRangeException("area", "The area is too wide for the surface.");
                if (area.Height > data.ViewArea.Height)
                    throw new ArgumentOutOfRangeException("area", "The area is too tall for the surface.");

                if (area.X < 0)
                    throw new ArgumentOutOfRangeException("area", "The left of the area cannot be less than 0.");
                if (area.Y < 0)
                    throw new ArgumentOutOfRangeException("area", "The top of the area cannot be less than 0.");

                if (area.X + area.Width > data.ViewArea.Width)
                    throw new ArgumentOutOfRangeException("area", "The area x + width is too wide for the surface.");
                if (area.Y + area.Height > data.ViewArea.Height)
                    throw new ArgumentOutOfRangeException("area", "The area y + height is too tal for the surface.");

                area = value;
                ResetArea();
            }
        }
        public Rectangle[] RenderRects { get; private set; }
        public Cell[] RenderCells { get; private set; }
        public Rectangle AbsoluteArea { get; private set; }

        public Font Font { get { return data.Font; } }
        public Color DefaultBackground { get { return data.DefaultBackground; } }
        public Color DefaultForeground { get { return data.DefaultForeground; } }

        public Color Tint { get; set; }


        public TextSurfaceView(ITextSurfaceView surface, Rectangle area)
        {
            data = surface;
            ViewArea = area;
            Tint = data.Tint;
        }

        private void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * data.Font.Size.X, y * data.Font.Size.Y, data.Font.Size.X, data.Font.Size.Y);
                    RenderCells[index] = data.RenderCells[(y + area.Top) * data.ViewArea.Width + (x + area.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, area.Width * data.Font.Size.X, area.Height * data.Font.Size.Y);
        }
    }
}
