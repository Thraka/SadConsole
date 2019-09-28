using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    public interface IRenderer
    {
        IRenderSurface Surface { get; set; }
    }

    public interface IRenderSurface
    {
        void DrawCellSurface();

        void DrawCell(ref Cell cell, Point pixelPosition);
    }
}
