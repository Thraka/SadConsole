namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using System.Linq;
    using SFML.Graphics;

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
            textSurface.Font = Engine.Fonts.Values.First().GetFont(SadConsole.Font.FontSizes.Two);

            ColoredString colorString = "Text using a colored string gradient that wraps around".CreateGradient(Color.Green, Color.Cyan);
            Print(2, 3, colorString);

            IsVisible = false;
        }
    }
}
