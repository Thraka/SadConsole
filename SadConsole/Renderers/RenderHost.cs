using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Renderers
{
    public abstract class RenderHost<TRenderSurface>
        where TRenderSurface : IRenderSurface
    {
        public abstract TRenderSurface CreateSurface(int width, int height);

        public abstract void DrawToScreen();

        public abstract void DrawToSurface(TRenderSurface surface);
    }
}
