using Microsoft.Xna.Framework;

using Console = SadConsole.Console;

namespace FeatureDemo.CustomConsoles
{
    class CursorConsole: CustomConsole
    {
        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Virtual Cursor 1", Summary = "" };
            }
        }

        public CursorConsole()
            : base(80, 23)
        {
            // This console demonstrates the virtual cursor.
            Cursor.IsVisible = true;
            Cursor.Position = new Point(0, 2);
            UseKeyboard = true;

            // Print some intro text
            Print(0, 0, "This console can be typed on. Use the arrow keys to move the cursor.");
            Print(0, 2, "Use the F1 key to cycle the active console.");

            IsVisible = false;
        }
    }
}
