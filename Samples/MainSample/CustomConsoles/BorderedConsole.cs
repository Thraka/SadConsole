using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace FeatureDemo.CustomConsoles
{
    public class BorderComponent : IComponent
    {
        private Console _borderConsole;
        private readonly ColoredGlyph _borderCellStyle;
        private readonly int[] _borderGlyphs;

        public uint SortOrder => 0;

        public bool IsUpdate => false;

        public bool IsRender => false;

        public bool IsMouse => false;

        public bool IsKeyboard => false;

        public BorderComponent(int[] connectedLineStyle, Color foreground, Color background)
        {
            if (!ICellSurface.ValidateLineStyle(connectedLineStyle))
            {
                throw new ArgumentException("The connected line array is invalid.", nameof(connectedLineStyle));
            }

            _borderGlyphs = connectedLineStyle;
            _borderCellStyle = new ColoredGlyph(foreground, background);
        }

        public BorderComponent(int glyph, Color foreground, Color background)
        {
            _borderGlyphs = new int[] { glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph, glyph };
            _borderCellStyle = new ColoredGlyph(foreground, background);
        }

        public void UpdateSize(Console console)
        {
            _borderConsole.Resize(console.Width + 2, console.Height + 2, console.Width + 2, console.Height + 2, true);
            _borderConsole.DrawBox(new Rectangle(0, 0, _borderConsole.Width, _borderConsole.Height), _borderCellStyle, null, _borderGlyphs);
        }

        public void OnAdded(IScreenObject screenObject)
        {
            if (screenObject is Console console)
            {
                _borderConsole = new Console(console.Width + 2, console.Height + 2);
                _borderConsole.Font = console.Font;
                _borderConsole.DrawBox(new Rectangle(0, 0, _borderConsole.Width, _borderConsole.Height), _borderCellStyle, null, _borderGlyphs);
                _borderConsole.Position = new Point(-1, -1);
                console.Children.Add(_borderConsole);
            }
            else
                throw new Exception("Can only be added to a console");
        }

        public void OnRemoved(IScreenObject console)
        {
            if (_borderConsole.Parent != null)
            {
                _borderConsole.Parent = null;
            }

            _borderConsole = null;
        }

        public void Render(IScreenObject console, TimeSpan delta) => throw new NotImplementedException();

        public void ProcessKeyboard(IScreenObject console, Keyboard info, out bool handled) => throw new NotImplementedException();

        public void ProcessMouse(IScreenObject console, MouseScreenObjectState state, out bool handled) => throw new NotImplementedException();

        public void Update(IScreenObject console, TimeSpan delta) => throw new NotImplementedException();
    }

    internal class BorderedConsole : Console
    {
        public BorderedConsole()
            : base(80, 25)
        {
            IsVisible = false;

            this.Print(1, 1, "Example of using a component to draw a border around consoles");

            var console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(176, Color.Red, Color.Black));
            console.Position = new Point(2, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(177, Color.Red, Color.Black));
            console.Position = new Point(17, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(178, Color.Red, Color.Black));
            console.Position = new Point(32, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(219, Color.Red, Color.Black));
            console.Position = new Point(47, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Thin line");
            console.SadComponents.Add(new BorderComponent(ICellSurface.ConnectedLineThin, Color.Green, Color.Black));
            console.Position = new Point(17, 12);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Thick line");
            console.SadComponents.Add(new BorderComponent(ICellSurface.ConnectedLineThick, Color.Orange, Color.Black));
            console.Position = new Point(2, 12);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Extd. line");
            console.SadComponents.Add(new BorderComponent(console.Font.IsSadExtended ? ICellSurface.ConnectedLineThinExtended : ICellSurface.ConnectedLineThin, Color.Purple, Color.Black));
            console.Position = new Point(32, 12);
            Children.Add(console);
        }

    }
}
