using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole;
using SadRogue.Primitives;

namespace SadConsole.UI
{
    /// <summary>
    /// Creates a 3D border around a surface.
    /// </summary>
    public class Border: ScreenSurface
    {
        /// <summary>
        /// Creates a border and adds it as a child object to <paramref name="contents"/>.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="title">Optional title to display on the border.</param>
        public Border(IScreenSurface contents, string title): base(contents.Surface.Width + 3, contents.Surface.Height + 3)
        {
            Position = (-1, -1);
            Font = contents.Font;
            FontSize = contents.FontSize;

            Surface.DefaultForeground = Color.AnsiWhite;
            Surface.Clear();

            Surface.DrawBox((0, 0, Surface.Width - 1, Surface.Height - 1), new ColoredGlyph(Color.AnsiWhite, contents.Surface.DefaultBackground), new ColoredGlyph(Color.White, Color.Transparent), ICellSurface.ConnectedLineThin);
            Surface.DrawLine((Surface.Width - 1, 2), (Surface.Width - 1, Surface.Height - 2), 219, Color.AnsiWhite);
            Surface.DrawLine((1, Surface.Height - 1), (Surface.Width - 1, Surface.Height - 1), 223, Color.AnsiWhite);
            Surface.SetGlyph(Surface.Width - 1, 1, 220);

            if (!string.IsNullOrEmpty(title))
                Surface.Print(((Surface.Width - 1) / 2) - ((title.Length + 2) / 2), 0, title.Align(HorizontalAlignment.Center, title.Length + 2, ' '), contents.Surface.DefaultBackground, Color.AnsiWhite);

            IsEnabled = false;
            UseMouse = false;
            UseKeyboard = false;

            contents.Children.Add(this);
        }

        /// <summary>
        /// Creates a border and adds it as a child object to the window.
        /// </summary>
        /// <param name="contents">The window the border will be around.</param>
        public Border(Window contents): base(contents.Surface.Width, contents.Surface.Height)
        {
            Position = (1, 1);
            Font = contents.Font;
            FontSize = contents.FontSize;

            Surface.DefaultForeground = Color.AnsiWhite;
            Surface.Clear();

            Surface.DrawLine((Surface.Width - 1, 1), (Surface.Width - 1, Surface.Height - 2), 219, Color.AnsiWhite); //█
            Surface.DrawLine((0, Surface.Height - 1), (Surface.Width - 1, Surface.Height - 1), 223, Color.AnsiWhite); //▀
            Surface.SetGlyph(Surface.Width - 1, 0, 220); //▄

            IsEnabled = false;
            UseMouse = false;
            UseKeyboard = false;

            contents.Children.Add(this);
        }

        /// <summary>
        /// Helper method to add a border to a surface.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="title">Optional title to display on the border.</param>
        public static void AddToSurface(IScreenSurface contents, string title) =>
            new Border(contents, title);

        /// <summary>
        /// Helper method to add a border to a window.
        /// </summary>
        /// <param name="contents">The window the border will be around.</param>
        public static void AddToWindow(Window contents) =>
            new Border(contents);

    }
}
