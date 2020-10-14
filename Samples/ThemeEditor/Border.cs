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
        private Border(IScreenSurface contents, string title): base(contents.Surface.BufferWidth + 3, contents.Surface.BufferHeight + 3)
        {
            Position = (-1, -1);
            Font = contents.Font;
            FontSize = contents.FontSize;

            Surface.DefaultForeground = Color.AnsiWhite;
            Surface.Clear();

            Surface.DrawBox((0, 0, Surface.BufferWidth - 1, Surface.BufferHeight - 1), new ColoredGlyph(Color.AnsiWhite, contents.Surface.DefaultBackground), new ColoredGlyph(Color.White, Color.Transparent), ICellSurface.ConnectedLineThin);
            Surface.DrawLine((Surface.BufferWidth - 1, 2), (Surface.BufferWidth - 1, Surface.BufferHeight - 2), 219, Color.AnsiWhite);
            Surface.DrawLine((1, Surface.BufferHeight - 1), (Surface.BufferWidth - 1, Surface.BufferHeight - 1), 223, Color.AnsiWhite);
            Surface.SetGlyph(Surface.BufferWidth - 1, 1, 220);

            if (!string.IsNullOrEmpty(title))
                Surface.Print(((Surface.BufferWidth - 1) / 2) - ((title.Length + 2) / 2), 0, title.Align(HorizontalAlignment.Center, title.Length + 2, ' '), contents.Surface.DefaultBackground, Color.AnsiWhite);

            IsEnabled = false;
            UseMouse = false;
            UseKeyboard = false;

            contents.Children.Add(this);
        }

        private Border(Window contents): base(contents.Surface.BufferWidth, contents.Surface.BufferHeight)
        {
            Position = (1, 1);
            Font = contents.Font;
            FontSize = contents.FontSize;

            Surface.DefaultForeground = Color.AnsiWhite;
            Surface.Clear();

            Surface.DrawLine((Surface.BufferWidth - 1, 1), (Surface.BufferWidth - 1, Surface.BufferHeight - 2), 219, Color.AnsiWhite); //█
            Surface.DrawLine((0, Surface.BufferHeight - 1), (Surface.BufferWidth - 1, Surface.BufferHeight - 1), 223, Color.AnsiWhite); //▀
            Surface.SetGlyph(Surface.BufferWidth - 1, 0, 220); //▄

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
