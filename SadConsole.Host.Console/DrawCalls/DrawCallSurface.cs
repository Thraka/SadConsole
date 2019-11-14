using System;
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
            var area = _surface.GetViewRectangle();
            for (int y = 0; y < area.Height; y++)
            {
                System.Console.SetCursorPosition(_position.X, y + _position.Y);

                    var text = _surface.GetString(area.X, y + area.Y, area.Width).Replace('\0', ' ');
                    System.Console.Write(text);
            }

           // System.Console.SetCursorPosition(0, 0);

           // int index = 0;
           // foreach (var cell in _surface)
           // {
           //     index++;
           //     if (cell.Glyph == 0)
           //     { }
                
           // }
        }
    }
}
