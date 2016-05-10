namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;

    class StretchedConsole: Console
    {
        public StretchedConsole()
            : base(40, 12)
        {
            // The cell size of a console is usually the size of the font.
            // Since we're going to create a stretched console at the expense of total cells
            // 40x12 cells instead of the 80x25 cells other consoles have, we're going to
            // stretch the size of the cells based on the font * 2. This will fill our 
            // area.
            CellSize = new Point(_font.CellWidth * 2, _font.CellHeight * 2);

            ColoredString colorString = "Text using a colored string gradient that wraps around".CreateGradient(Color.DarkGreen, Color.LightGreen, null);
            _cellData.Print(2, 3, colorString);

            IsVisible = false;
        }
    }
}
