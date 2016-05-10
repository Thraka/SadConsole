using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    public interface ITextSurfaceView
    {
        Rectangle AbsoluteArea { get; }
        Rectangle ViewArea { get; }
        Rectangle[] RenderRects { get; }
        Cell[] RenderCells { get; }

        Font Font { get; }
        Color DefaultBackground { get; }
        Color DefaultForeground { get; }
        Color Tint { get; }
    }
}
