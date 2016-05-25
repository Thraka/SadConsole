using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    public interface ITextSurface
    {
        Rectangle AbsoluteArea { get; }
        Rectangle[] RenderRects { get; }
        Cell[] Cells { get; }
        Cell[] RenderCells { get; }
        int Width { get; }
        int Height { get; }

        Font Font { get; set; }
        Color DefaultBackground { get; set; }
        Color DefaultForeground { get; set; }
        Color Tint { get; set; }
    }
}
