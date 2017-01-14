using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

#if MONOGAME
using Color = Microsoft.Xna.Framework.Color;
#endif

namespace SadConsole
{
    /// <summary>
    /// Represents an individual glyph on the screen with a foreground, background, and effect.
    /// </summary>
    public class Cell
    {
        public Color Foreground;
        public Color Background;
        public int Glyph;
        public SpriteEffects SpriteEffect;

        public Cell(Color foreground, Color background, int glyph)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
        }
    }


    public class TextSurface
    {
        public Color DefaultBackground;
        public Color DefaultForeground;
        public Color Tint;

        public Cell[] Cells { get; protected set; }

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public TextSurface(int width, int height)
        {
            Width = width;
            Height = height;
            InitializeCells();
        }

        public Rectangle[] RenderRects;
        public Cell[] RenderCells;

        public Rectangle AbsoluteArea;

        private Rectangle area;


        protected virtual void InitializeCells()
        {
            Cells = new Cell[Width * Height];
            area = new Rectangle(0, 0, Width, Height);

            for (int i = 0; i < Cells.Length; i++)
                Cells[i] = new Cell(Color.White, Color.Transparent, 0);

            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = Font.GetRenderRect(x, y);
                    RenderCells[index] = Cells[(y + area.Top) * Width + (x + area.Left)];
                    index++;
                }
            }

            // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);
        }

        public void FillWithRandomGarbage(bool useEffect = false)
        {
            Random random = new Random();
            //pulse.Reset();
            int charCounter = 0;
            for (int y = 0; y < Height * Width; y++)
            {
                Cells[y].Glyph = charCounter;
                Cells[y].Foreground = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 255);
                Cells[y].Background = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 255);
                charCounter++;
                if (charCounter > 255)
                    charCounter = 0;
            }
        }

        public Font Font = Global.FontDefault;
    }
    
}
