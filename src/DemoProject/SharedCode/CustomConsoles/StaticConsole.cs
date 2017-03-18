using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using System;
using SadConsole;
using Console = SadConsole.Consoles.Console;

namespace StarterProject.CustomConsoles
{
    /// <summary>
    /// Demonstrates a console that doesn't accept keyboard input, doesn't show the cursor (both defaults for a console), and just displays some custom text.
    /// </summary>
    class StaticConsole: Console
    {
        public StaticConsole()
            : base(80, 25)
        {
            // Prints a string using the cellData default foreground and background.
            Print(2, 1, "Text written with the default foreground and background");

            // Prints a string using the ColoredString class.
            Print(2, 2, new ColoredString("Text using a colored string", ColorHelper.LightBlue, Color.Transparent));

            // Creates a new ColoredString from an existing string and applies a gradient color to each character.
            ColoredString colorString = "Text using a colored string gradient".CreateGradient(ColorHelper.DarkGreen, ColorHelper.LightGreen);
            Print(2, 3, colorString);

            // Appends a new ColoredString to the existing ColoredString with a new color gradient.
            colorString += " with another gradient applied".CreateGradient(ColorHelper.DarkBlue, ColorHelper.LightBlue);
            Print(2, 4, colorString);

            // Prints a string, then changes the foreground of a single cell.
            Print(2, 5, "Text written with defaults, then the foreground changed");
            SetForeground(17, 5, ColorHelper.Yellow);

            // Prints a string, then changes the background of a single cell.
            Print(2, 6, "Text written with defaults, then the background changed");
            SetBackground(17, 6, ColorHelper.DarkGray);

            // Prints a string, then changes the foreground and background of a single cell.
            Print(2, 7, "Text written with defaults, then the foreground and background changed");
            SetBackground(17, 7, ColorHelper.White);
            SetForeground(17, 7, ColorHelper.Black);

            // Prints a string, then changes the effect of the cell to a Blink effect.
            Print(2, 8, "Text written with defaults, then the blink effect applied");
            SetEffect(17, 8, new SadConsole.Effects.Blink() { BlinkSpeed = 0.5f });

            IsVisible = false;
        }
    }
}
