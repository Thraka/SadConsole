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
        // console sizes
        readonly Point _sizeSmall = new(12, 4),
                       _sizeLarge = new(16, 4);

        public BorderedConsole() : base(80, 25)
        {
            IsVisible = false;

            // border component section
            int seperator = 4, x = 2, y = 5;
            PrintHeading(y - 4, "a component");
            DisplayConsoleWithBorderComponent((x + _sizeSmall.X * 0 + seperator * 0, y), "Glyph line", ICellSurface.CreateLine(176), Color.Red);
            DisplayConsoleWithBorderComponent((x + _sizeSmall.X * 1 + seperator * 1, y), "Glyph line", ICellSurface.CreateLine(177), Color.Red);
            DisplayConsoleWithBorderComponent((x + _sizeSmall.X * 2 + seperator * 2, y), "Glyph line", ICellSurface.CreateLine(178), Color.Red);
            DisplayConsoleWithBorderComponent((x + _sizeSmall.X * 3 + seperator * 3, y), "Thick line", ICellSurface.ConnectedLineThick, Color.Green);
            DisplayConsoleWithBorderComponent((x + _sizeSmall.X * 4 + seperator * 4, y), "Thin line", ICellSurface.ConnectedLineThin, Color.Orange);

            // border class section
            y = 16;
            PrintHeading(y - 4, "the Border class");
            var borderParams = Border.BorderParameters.GetDefault()
                .AddTitle("White Border", Color.Black, Color.White)
                .ChangeBorderColors(Color.White, Color.Black)
                .AddShadow();
            DisplayConsoleWithBorderClass((x + _sizeLarge.X * 0 + seperator * 0, y), "Sample|Content", borderParams);

            borderParams.AddTitle("Green Border", Color.Black, Color.Green)
                .ChangeBorderColors(Color.Green, Color.Black);
            DisplayConsoleWithBorderClass((x + _sizeLarge.X * 1 + seperator * 1, y), "Lorem|Ipsum", borderParams);

            borderParams = Border.BorderParameters.GetDefault()
                .AddTitle("Thick Border", Color.Black, Color.Yellow)
                .ChangeBorderGlyph(ICellSurface.ConnectedLineThick, Color.Yellow, Color.Black)
                .AddShadow(176, Color.YellowGreen, Color.DarkGoldenrod);
            DisplayConsoleWithBorderClass((x + _sizeLarge.X * 2 + seperator * 2, y), "Sample|Content", borderParams);

            borderParams = Border.BorderParameters.GetDefault()
                .AddTitle("Glyph Border", Color.White, Color.Crimson)
                .ChangeBorderGlyph(219, Color.Crimson * 0.9f, Color.Black)
                .AddShadow(176, Color.LightBlue, Color.Brown);
            DisplayConsoleWithBorderClass((x + _sizeLarge.X * 3 + seperator * 3, y), "Lorem|Ipsum", borderParams);
        }

        void PrintHeading(int y, string name)
        {
            Surface.Print(1, y, $"Examples of using {name} to draw a border around consoles:");
        }

        void DisplayConsoleWithBorderComponent(Point position, string content, int[] borderGlyphs, Color glyphForegroundColor)
        {
            var console = CreateConsole(position, _sizeSmall, content);
            console.SadComponents.Add(new BorderComponent(borderGlyphs, glyphForegroundColor, Color.Black));
        }

        void DisplayConsoleWithBorderClass(Point position, string content, Border.BorderParameters borderParams)
        {
            var console = CreateConsole(position, _sizeLarge, content);
            Border border = new(console, borderParams);
        }

        Console CreateConsole(Point position, Point size, string content)
        {
            var console = new Console(size.X, size.Y) { Position = position };
            string[] lines = content.Split('|', 2);
            if (lines.Length > 0) console.Print(1, 1, lines[0]);
            if (lines.Length > 1) console.Print(6, 2, lines[1]);
            Children.Add(console);
            return console;
        }
    }

    /// <summary>
    /// A simple component that draws a border around a console.
    /// </summary>
    class BorderComponent : IComponent
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
