using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;
using SadConsole.Renderers;

namespace SadConsole
{
    public abstract class GameHost
    {
        protected Queue<DrawCalls.IDrawCall> _drawCalls = new Queue<DrawCalls.IDrawCall>();

        public Point WindowSize { get; }

        public abstract Renderers.IRenderSurface CreateSurface(int width, int height);

    }
}
