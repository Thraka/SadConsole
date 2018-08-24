using Microsoft.Xna.Framework;

using Console = SadConsole.Console;
using SadConsole;
using System;
using System.Linq;

namespace StarterProject.CustomConsoles
{
    class ShapesConsole: Console
    {
        public ShapesConsole()
            : base(80, 23)
        {
            UseKeyboard = false;

            DrawLine(new Point(2, 2), new Point(Width - 4, 2), Color.Yellow, glyph: '=');
            DrawBox(new Rectangle(2, 4, 6, 6), new Cell(Color.Yellow, Color.Black, '='));
            DrawBox(new Rectangle(9, 4, 6, 6), new Cell(Color.Yellow, Color.Black, '='), connectedLineStyle: ConnectedLineThin);
            DrawBox(new Rectangle(16, 4, 6, 6), new Cell(Color.Yellow, Color.Black, '='), connectedLineStyle: ConnectedLineThick);
            DrawBox(new Rectangle(23, 4, 6, 6), new Cell(Color.Black, Color.Yellow, '='), new Cell(Color.Black, Color.Yellow, 0), connectedLineStyle: ConnectedLineThick);

            DrawCircle(new Rectangle(2, 12, 16, 10), new Cell(Color.White, Color.Black, 176));
            DrawCircle(new Rectangle(19, 12, 16, 10), new Cell(Color.White, Color.Black, 176), new Cell(Color.Green, Color.Black, 178));

            IsVisible = false;
        }
    }
}
