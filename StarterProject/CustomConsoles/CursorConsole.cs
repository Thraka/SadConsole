namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;

    class CursorConsole: Console
    {
        public CursorConsole()
            : base(80, 25)
        {
            // This console demonstrates the virtual cursor.
            VirtualCursor.IsVisible = true;
            VirtualCursor.Position = new Point(0, 2);
            CanUseKeyboard = true;

            // Print some intro text
            _cellData.Print(0, 0, "This console can be typed on. Use the arrow keys to move the cursor.");

            IsVisible = false;
        }
    }
}
