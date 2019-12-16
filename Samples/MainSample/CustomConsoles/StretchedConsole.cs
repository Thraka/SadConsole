using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class StretchedConsole : Console
    {
        public StretchedConsole()
            : base(40, 11)
        {
            // The cell size of a console is usually the size of the font.
            // Since we're going to create a stretched console at the expense of total cells
            // 40x12 cells instead of the 80x25 cells other consoles have, we're going to
            // stretch the size of the cells based on the font * 2. This will fill our 
            // area.
            UseKeyboard = false;
            FontSize = Font.GetFontSize(Font.Sizes.Two);

            ColoredString colorString = "Text using a colored string gradient".CreateGradient(Color.DarkGreen, Color.LightGreen);
            this.Print(2, 1, colorString);

            this.Print(2, 3, "Same font as others, just doubled");

            Cursor.UseStringParser = true;
            Cursor.Position = new Point(0, 5);
            Cursor.Print("This is an [c:r f:blue]example[c:u] of how the [c:r f:green]system[c:u] is aware of how to line wrap by word and make this print pretty-like.");

            IsVisible = false;
        }
    }
}
