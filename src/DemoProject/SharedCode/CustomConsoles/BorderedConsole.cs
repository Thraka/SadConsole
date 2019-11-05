using System;
using Microsoft.Xna.Framework;

using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace StarterProject.CustomConsoles
{
    public class BorderComponent : IConsoleComponent
    {
        private Console _borderConsole;
        private readonly Cell _borderCellStyle;
        private readonly int[] _borderGlyphs;

        public int SortOrder => 0;

        public bool IsUpdate => false;

        public bool IsDraw => false;

        public bool IsMouse => false;

        public bool IsKeyboard => false;

        public BorderComponent(int[] connectedLineStyle, Color foreground, Color background)
        {
            if (!CellSurface.ValidateLineStyle(connectedLineStyle))
            {
                throw new ArgumentException("The connected line array is invalid.", nameof(connectedLineStyle));
            }

            _borderGlyphs = connectedLineStyle;
            _borderCellStyle = new Cell(foreground, background);
        }

        public BorderComponent(int glyph, Color foreground, Color background)
        {
            _borderGlyphs = new int[] { glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph };
            _borderCellStyle = new Cell(foreground, background);
        }

        public void UpdateSize(Console console)
        {
            _borderConsole.Resize(console.Width + 2, console.Height + 2, true);
            _borderConsole.DrawBox(new Rectangle(0, 0, _borderConsole.Width, _borderConsole.Height), _borderCellStyle, null, _borderGlyphs);
        }

        public void OnAdded(Console console)
        {
            _borderConsole = new Console(console.Width + 2, console.Height + 2, console.Font);
            _borderConsole.DrawBox(new Rectangle(0, 0, _borderConsole.Width, _borderConsole.Height), _borderCellStyle, null, _borderGlyphs);
            _borderConsole.Position = new Point(-1, -1);
            console.Children.Add(_borderConsole);
        }

        public void OnRemoved(Console console)
        {
            if (_borderConsole.Parent != null)
            {
                _borderConsole.Parent = null;
            }

            _borderConsole = null;
        }

        public void Draw(Console console, TimeSpan delta) => throw new NotImplementedException();

        public void ProcessKeyboard(Console console, Keyboard info, out bool handled) => throw new NotImplementedException();

        public void ProcessMouse(Console console, MouseConsoleState state, out bool handled) => throw new NotImplementedException();

        public void Update(Console console, TimeSpan delta) => throw new NotImplementedException();
    }

    internal class BorderedConsole : Console
    {
        public BorderedConsole()
            : base(80, 25)
        {
            IsVisible = false;

            Print(1, 1, "Example of using a component to draw a border around consoles");

            var console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.Components.Add(new BorderComponent(176, Color.Red, Color.Black));
            console.Position = new Point(2, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.Components.Add(new BorderComponent(177, Color.Red, Color.Black));
            console.Position = new Point(17, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.Components.Add(new BorderComponent(178, Color.Red, Color.Black));
            console.Position = new Point(32, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.Components.Add(new BorderComponent(219, Color.Red, Color.Black));
            console.Position = new Point(47, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Thin line");
            console.Components.Add(new BorderComponent(ConnectedLineThin, Color.Green, Color.Black));
            console.Position = new Point(17, 12);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Thick line");
            console.Components.Add(new BorderComponent(ConnectedLineThick, Color.Orange, Color.Black));
            console.Position = new Point(2, 12);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Extd. line");
            console.Components.Add(new BorderComponent(console.Font.Master.IsSadExtended ? ConnectedLineThinExtended : ConnectedLineThin, Color.Purple, Color.Black));
            console.Position = new Point(32, 12);
            Children.Add(console);
        }

    }
}
