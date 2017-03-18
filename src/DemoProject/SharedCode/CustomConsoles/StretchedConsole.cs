using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using Console = SadConsole.Console;
using SadConsole;
using System;
using System.Linq;

namespace StarterProject.CustomConsoles
{
    class StretchedConsole: Console, IConsoleMetadata
    {
        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Font Zoom", Summary = "Console where font has been zoomed x2" };
            }
        }

        public StretchedConsole()
            : base(40, 11)
        {
            // The cell size of a console is usually the size of the font.
            // Since we're going to create a stretched console at the expense of total cells
            // 40x12 cells instead of the 80x25 cells other consoles have, we're going to
            // stretch the size of the cells based on the font * 2. This will fill our 
            // area.
            UseKeyboard = false;
            textSurface.Font = Global.Fonts.Values.First().GetFont(SadConsole.Font.FontSizes.Two);

            ColoredString colorString = "Text using a colored string gradient".CreateGradient(ColorHelper.DarkGreen, ColorHelper.LightGreen);
            Print(2, 1, colorString);

            Print(2, 3, "Same font as others, just doubled");

            virtualCursor.UseStringParser = true;
            virtualCursor.Position = new Point(0, 5);
            virtualCursor.Print("This is an [c:r f:blue]example[c:u] of how the [c:r f:green]system[c:u] is aware of how to line wrap by word and make this print pretty-like.");

            IsVisible = false;
        }
    }
}
