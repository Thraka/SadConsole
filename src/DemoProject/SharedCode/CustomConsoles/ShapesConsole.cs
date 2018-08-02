using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

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
            DrawBox(new Rectangle(2, 4, 6, 6), Color.Yellow, glyph: '=');
            DrawBox(new Rectangle(9, 4, 6, 6), Color.Yellow, connectedLineStyle: ConnectedLineThin);
            DrawBox(new Rectangle(16, 4, 6, 6), Color.Yellow, connectedLineStyle: ConnectedLineThick);
            DrawBox(new Rectangle(23, 4, 6, 6), Color.Black, Color.Yellow, connectedLineStyle: ConnectedLineThick, fill: true);

            IsVisible = false;
        }
    }
}
