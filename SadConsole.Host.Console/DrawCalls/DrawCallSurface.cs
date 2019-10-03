using System;
using Mindmagma.Curses;
using SadRogue.Primitives;

namespace SadConsole.DrawCalls
{
    class DrawCallSurface : SadConsole.DrawCalls.IDrawCall
    {
        private CellSurface _surface;
        private Point _position;

        public DrawCallSurface(CellSurface surface, Point position)
        {
            _surface = surface;
            _position = position;
        }

        public void Draw()
        {
            NCurses.Move(_position.Y, _position.X);
            int index = 0;
            foreach (var cell in _surface)
            {
                index++;
                if (cell.Glyph == 0)
                { }
                else
                    NCurses.AddChar(cell.Glyph);
            }
        }
    }
}
