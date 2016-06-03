namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using CustomConsole = SadConsole.Consoles.CustomConsole;
    using Microsoft.Xna.Framework;
    using System.Linq;

    class StretchedConsole: CustomConsole
    {
        public StretchedConsole()
            : base(40, 12)
        {
            // The cell size of a console is usually the size of the font.
            // Since we're going to create a stretched console at the expense of total cells
            // 40x12 cells instead of the 80x25 cells other consoles have, we're going to
            // stretch the size of the cells based on the font * 2. This will fill our 
            // area.
            _textSurface.Font = Engine.Fonts.Values.First().GetFont(2);

            ColoredString colorString = "Text using a colored string gradient that wraps around".CreateGradient(Color.DarkGreen, Color.LightGreen, null);
            _textSurface.Print(2, 3, colorString);

            IsVisible = false;
        }
    }
}
