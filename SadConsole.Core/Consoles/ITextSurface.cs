using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    public interface ITextSurface
    {
        Rectangle AbsoluteArea { get; set; }
        Rectangle[] RenderRects { get; }//set; }
        Cell[] Cells { get; }
        Cell[] RenderCells { get; }//set; }
        int Width { get; }
        int Height { get; }

        Font Font { get; set; }
        Color DefaultBackground { get; set; }
        Color DefaultForeground { get; set; }
        Color Tint { get; set; }
        Rectangle ViewArea { get; set; }

        bool IsValidCell(int x, int y);
        bool IsValidCell(int x, int y, out int index);
        Cell GetCell(int x, int y);
    }
}
