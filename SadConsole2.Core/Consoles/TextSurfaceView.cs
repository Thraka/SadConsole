using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    public class TextSurfaceView : TextSurface
    {
        private ITextSurface data;
        
        public TextSurfaceView(ITextSurface surface, Rectangle area)
        {
            data = surface;
            DefaultBackground = surface.DefaultBackground;
            DefaultForeground = surface.DefaultForeground;
            base.font = surface.Font;
            base.width = surface.Width;
            base.height = surface.Height;

            
            this.area = area;
            this.cells = data.Cells;
            ResetArea();
            this.area = new Rectangle(0, 0, area.Width, area.Height);
            this.cells = this.renderCells;
            base.width = area.Width;
            base.height = area.Height;
            ResetArea();
            
        }
        
        protected override void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            renderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * Font.Size.X, y * Font.Size.Y, Font.Size.X, Font.Size.Y);
                    renderCells[index] = base.cells[(y + area.Top) * width + (x + area.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);
        }
    }
}
