using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Renderers
{
    public class ConsoleRenderer : RenderHost<ConsoleRenderSurface>
    {
        public override ConsoleRenderSurface CreateSurface(int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void DrawToScreen()
        {
            throw new NotImplementedException();
        }

        public override void DrawToSurface(ConsoleRenderSurface surface)
        {
            throw new NotImplementedException();
        }
    }

    public class ConsoleRenderSurface : IRenderSurface
    {
        internal IntPtr _window;

        internal ConsoleRenderSurface() { }

        public void DrawCell(ref Cell cell, SadRogue.Primitives.Point pixelPosition)
        {
            throw new NotImplementedException();
        }

        public void DrawCellSurface()
        {
            throw new NotImplementedException();
        }
    }
}
