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
            _textSurface.Print(0, 0, "This console can be typed on. Use the arrow keys to move the cursor.");
            _textSurface.Print(0, 2, "Use the F1 key to cycle the active console.");

            IsVisible = false;
        }
    }
}
