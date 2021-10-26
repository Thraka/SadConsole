using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace FeatureDemo.CustomConsoles
{
    internal class BorderedConsole : Console
    {
        Point _size = new(12, 4);

        public BorderedConsole() : base(80, 25)
        {
            int y = 5;
            IsVisible = false;

            PrintTitle(y - 4, "a component");
            DisplayConsoleWithBorderComponent(02, y, "Glyph line", ICellSurface.CreateLine(176), Color.Red);
            DisplayConsoleWithBorderComponent(17, y, "Glyph line", ICellSurface.CreateLine(177), Color.Red);
            DisplayConsoleWithBorderComponent(32, y, "Glyph line", ICellSurface.CreateLine(178), Color.Red);
            DisplayConsoleWithBorderComponent(47, y, "Thick line", ICellSurface.ConnectedLineThick, Color.Green);
            DisplayConsoleWithBorderComponent(62, y, "Thin line", ICellSurface.ConnectedLineThin, Color.Orange);

            y = 16;
            PrintTitle(y - 4, "the Border class");
            DisplayConsoleWithBorderClass(02, y, "Border Class");

            var borderParams = Border.BorderParameters.GetDefault()
                .AddTitle("Green Border", Color.Black, Color.Green)
                .ChangeBorderColors(Color.Green, Color.Black)
                .AddShadow();
            DisplayConsoleWithBorderClass(19, y, borderParams);

            borderParams = Border.BorderParameters.GetDefault()
                .AddTitle("Thick Border", Color.Black, Color.Yellow)
                .ChangeBorderGlyph(ICellSurface.ConnectedLineThick, Color.Yellow, Color.Black)
                .AddShadow(176, Color.YellowGreen, Color.DarkGoldenrod);
            DisplayConsoleWithBorderClass(36, y, borderParams);

            borderParams = Border.BorderParameters.GetDefault()
                .AddTitle("Glyph Border", Color.White, Color.Crimson)
                .ChangeBorderGlyph(219, Color.Crimson * 0.9f, Color.Black)
                .AddShadow(176, Color.LightBlue, Color.Brown);
            DisplayConsoleWithBorderClass(53, y, borderParams);
        }

        void PrintTitle(int y, string name)
        {
            Surface.Print(1, y, $"Examples of using {name} to draw a border around consoles:");
        }

        void DisplayConsoleWithBorderComponent(int x, int y, string desc, int[] lines, Color fCol)
        {
            var console = CreateConsole(x, y, desc);
            console.SadComponents.Add(new BorderComponent(lines, fCol, Color.Black));
        }

        void DisplayConsoleWithBorderClass(int x, int y, Border.BorderParameters borderParams)
        {
            var console = CreateConsole(x, y, "Content", "Here");
            Border border = new(console, borderParams);
        }

        void DisplayConsoleWithBorderClass(int x, int y, string desc)
        {
            var console = CreateConsole(x, y, "Content", "Here");
            console.Print(1, 1, "Content");
            Border border = new(console, desc);
        }

        Console CreateConsole(int x, int y, string line1, string line2 = "")
        {
            var console = new Console(_size.X, _size.Y) { Position = (x, y) };
            console.Print(1, 1, line1);
            console.Print(6, 2, line2);
            Children.Add(console);
            return console;
        }
    }

    /// <summary>
    /// A simple component that draws a border around a console.
    /// </summary>
    public class BorderComponent : IComponent
    {
        readonly ShapeParameters _shapeParams;
        Rectangle _borderRectangle;
        Console _borderConsole;

        public uint SortOrder => 0;

        public bool IsUpdate => false;

        public bool IsRender => false;

        public bool IsMouse => false;

        public bool IsKeyboard => false;

        public BorderComponent(int glyph, Color foreground, Color background) : this(ICellSurface.CreateLine(glyph), foreground, background) { }

        public BorderComponent(int[] connectedLineStyle, Color foreground, Color background)
        {
            if (!ICellSurface.ValidateLineStyle(connectedLineStyle))
            {
                throw new ArgumentException("The connected line array is invalid.", nameof(connectedLineStyle));
            }

            var borderCellStyle = new ColoredGlyph(foreground, background);
            _shapeParams = ShapeParameters.CreateStyledBox(connectedLineStyle, borderCellStyle);
        }

        public void UpdateSize(Console console)
        {
            _borderConsole.Resize(console.Width + 2, console.Height + 2, console.Width + 2, console.Height + 2, true);
            _borderRectangle = new Rectangle(0, 0, _borderConsole.Width, _borderConsole.Height);
            _borderConsole.DrawBox(_borderRectangle, _shapeParams);
        }

        public void OnAdded(IScreenObject screenObject)
        {
            if (screenObject is Console console)
            {
                _borderConsole = new(console.Width + 2, console.Height + 2)
                {
                    Font = console.Font,
                    Position = new Point(-1, -1)
                };
                _borderRectangle = new Rectangle(0, 0, _borderConsole.Width, _borderConsole.Height);
                _borderConsole.DrawBox(_borderRectangle, _shapeParams);
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

}
