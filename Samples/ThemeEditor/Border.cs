using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole;
using SadRogue.Primitives;

namespace ThemeEditor
{
    class Border: ScreenSurface
    {
        private Border(IScreenSurface contents, string title): base(contents.Surface.Width + 3, contents.Surface.Height + 3)
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

        private Border(Window contents): base(contents.Surface.Width, contents.Surface.Height)
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

        public static void AddToSurface(IScreenSurface contents, string title) =>
            new Border(contents, title);

        public static void AddToWindow(Window contents) =>
            new Border(contents);

    }
}
