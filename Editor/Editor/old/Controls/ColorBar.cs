namespace SadConsoleEditor.Controls
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Console = SadConsole.Console;

    class ColorBar : SadConsole.Controls.ControlBase
    {
        // 223
        // 30


        public event EventHandler ColorChanged;

        public Color StartingColor { get { return _startingColor; } set { _startingColor = value; Compose(); } }
        public Color EndingColor { get { return _endingColor; } set { _endingColor = value; Compose(); } }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                SetClosestIndex(value);

                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        private Color SelectedColorSafe
        {
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        private int _positions;
        private int _selectedPosition;
        private Color[] _colorSteps;

        private Color _selectedColor;
        private Color _startingColor;
        private Color _endingColor;

        public ColorBar(int width): base (width, 2)
        {
            StartingColor = Color.White;
            EndingColor = Color.Black;
        }

        private void SetClosestIndex(Color color)
        {
            ColorMine.ColorSpaces.Rgb rgbColorStop = new ColorMine.ColorSpaces.Rgb() { R = color.R, G = color.G, B = color.B };
            Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[Width];

            // Create a color weight for every cell compared to the color stop
            for (int x = 0; x < Width; x++)
            {
                ColorMine.ColorSpaces.Rgb rgbColor = new ColorMine.ColorSpaces.Rgb() { R = this[x, 0].Foreground.R, G = this[x, 0].Foreground.G, B = this[x, 0].Foreground.B };
                ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

                colorWeights[x] = new Tuple<Color, double, int>(this[x, 0].Foreground, rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);

            }

            var foundColor = colorWeights.OrderBy(t => t.Item2).First();
            _selectedPosition = foundColor.Item3;
            this.IsDirty = true;
        }


        public override void Compose()
        {
            this.Fill(Color.White, Color.Black, 0, null);

            _positions = Width;
            _colorSteps = StartingColor.LerpSteps(EndingColor, Width);

            for (int x = 0; x < Width; x++)
            {
                this[x, 0].Glyph = 219;
                this[x, 0].Foreground = _colorSteps[x];
            }

            this[_selectedPosition, 1].Glyph = 30;
            this[_selectedPosition, 1].Foreground = Color.LightGray;//this[_selectedPosition, 0].Foreground;
        }

        public override void DetermineAppearance()
        {

        }

        protected override void OnMouseIn(SadConsole.Input.MouseConsoleState info)
        {
            base.OnMouseIn(info);

            if (Parent.CapturedControl == null)
            {
                if (info.Mouse.LeftButtonDown)
                {
                    var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                    _selectedPosition = location.X;
                    SelectedColorSafe = this[_selectedPosition, 0].Foreground;
                    IsDirty = true;

                    Parent.CaptureControl(this);
                }
            }
        }

        public override bool ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
            if (Parent.CapturedControl == this)
            {
                if (info.Mouse.LeftButtonDown == false)
                    Parent.ReleaseControl();
                else
                {
                    var location = this.TransformConsolePositionByControlPosition(info.CellPosition);

                    //if (info.ConsolePosition.X >= Position.X && info.ConsolePosition.X < Position.X + Width)
                    if (location.X >= 0 && location.X <= base.Width - 1 && location.Y > -4 && location.Y < Height + 3)
                    {
                        _selectedPosition = location.X;
                        SelectedColorSafe = this[_selectedPosition, 0].Foreground;
                    }

                    IsDirty = true;
                }
            }

            return base.ProcessMouse(info);
        }
    }

    class HueBar : SadConsole.Controls.ControlBase
    {
        // 223
        // 30

        public event EventHandler ColorChanged;

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                SetClosestIndex(value);

                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        private Color SelectedColorSafe
        {
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        private int _positions;
        private int _selectedPosition;

        private Color _selectedColor;

        public HueBar(int width): base(width, 2)
        {
            Compose(true);
        }

        private void SetClosestIndex(Color color)
        {
            ColorMine.ColorSpaces.Rgb rgbColorStop = new ColorMine.ColorSpaces.Rgb() { R = color.R, G = color.G, B = color.B };
            Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[Width];

            // Create a color weight for every cell compared to the color stop
            for (int x = 0; x < Width; x++)
            {
                ColorMine.ColorSpaces.Rgb rgbColor = new ColorMine.ColorSpaces.Rgb() { R = this[x, 0].Foreground.R, G = this[x, 0].Foreground.G, B = this[x, 0].Foreground.B };
                ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

                colorWeights[x] = new Tuple<Color, double, int>(this[x, 0].Foreground, rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);

            }

            var foundColor = colorWeights.OrderBy(t => t.Item2).First();
            _selectedPosition = foundColor.Item3;
            this.IsDirty = true;
        }

        public override void Compose()
        {
            if (this.IsDirty)
            {
                this.Fill(Color.White, Color.Black, 0, null);

                _positions = Width;
                ColorGradient gradient = new ColorGradient(Color.Red, Color.Yellow, Color.Green, Color.Turquoise, Color.Blue, Color.Purple, Color.Red);

                for (int x = 0; x < Width; x++)
                {
                    this[x, 0].Glyph = 219;
                    this[x, 0].Foreground = gradient.Lerp((float)x / (float)(Width - 1));
                }

                this[_selectedPosition, 1].Glyph = 30;
                this[_selectedPosition, 1].Foreground = Color.LightGray;//this[_selectedPosition, 0].Foreground;

                // Build an array of all the colors
                Color[] colors = new Color[Width];
                for (int x = 0; x < Width; x++)
                    colors[x] = this[x, 0].Foreground;

                List<int> colorIndexesFinished = new List<int>(Width);

                foreach (var stop in gradient.Stops)
                {
                    ColorMine.ColorSpaces.Rgb rgbColorStop = new ColorMine.ColorSpaces.Rgb() { R = stop.Color.R, G = stop.Color.G, B = stop.Color.B };
                    Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[Width];

                    // Create a color weight for every cell compared to the color stop
                    for (int x = 0; x < Width; x++)
                    {
                        if (!colorIndexesFinished.Contains(x))
                        {
                            ColorMine.ColorSpaces.Rgb rgbColor = new ColorMine.ColorSpaces.Rgb() { R = colors[x].R, G = colors[x].G, B = colors[x].B };
                            ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

                            colorWeights[x] = new Tuple<Color, double, int>(colors[x], rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);
                        }
                        else
                            colorWeights[x] = new Tuple<Color, double, int>(colors[x], 10000, x);
                    }

                    var foundColor = colorWeights.OrderBy(t => t.Item2).First();

                    this[foundColor.Item3, 0].Foreground = stop.Color;
                    colorIndexesFinished.Add(foundColor.Item3);
                }

                this.IsDirty = false;
            }
        }

        public override void DetermineAppearance()
        {

        }

        protected override void OnMouseIn(SadConsole.Input.MouseConsoleState info)
        {
            base.OnMouseIn(info);

            if (Parent.CapturedControl == null)
            {
                if (info.Mouse.LeftButtonDown)
                {
                    var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                    _selectedPosition = location.X;
                    SelectedColorSafe = this[_selectedPosition, 0].Foreground;
                    IsDirty = true;

                    Parent.CaptureControl(this);
                }
            }
        }

        public override bool ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
            if (Parent.CapturedControl == this)
            {
                if (info.Mouse.LeftButtonDown == false)
                    Parent.ReleaseControl();
                else
                {
                    var location = this.TransformConsolePositionByControlPosition(info.ConsolePosition);

                    //if (info.ConsolePosition.X >= Position.X && info.ConsolePosition.X < Position.X + Width)
                    if (location.X >= 0 && location.X <= Width - 1 && location.Y > -4 && location.Y < Height + 3 )
                    {
                        _selectedPosition = location.X;
                        SelectedColorSafe = this[_selectedPosition, 0].Foreground;
                    }

                    IsDirty = true;
                }
            }

            return base.ProcessMouse(info);
        }
    }
}
