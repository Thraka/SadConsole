namespace SadConsoleEditor.Controls
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using System;
    using System.Linq;
    using Console = SadConsole.Console;

    class ColorPicker : SadConsole.Controls.ControlBase
    {
        private Color _selectedColor;
        private Color _selectedHue;
        private Point _selectedColorPosition;

        public event EventHandler SelectedColorChanged;

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    SetClosestIndex(value);
                    _selectedColor = value;
                    if (SelectedColorChanged != null)
                        SelectedColorChanged(this, EventArgs.Empty);
                }
            }
        }

        private Color SelectedColorSafe
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    if (SelectedColorChanged != null)
                        SelectedColorChanged(this, EventArgs.Empty);
                }
            }
        }

        public Color SelectedHue
        {
            get { return _selectedHue; }
            set
            {
                if (_selectedHue != value)
                {
                    _selectedHue = value;
                    IsDirty = true;
                    Compose();
                    ResetSelectedColor();
                }
            }
        }

       
        public ColorPicker(int width, int height, Color hue): base(width, height)
        {
            SelectedHue = hue;
            Compose();

            SelectedColor = hue;
            this[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
        }

        private void ResetSelectedColor()
        {
            SelectedColorSafe = this[_selectedColorPosition.X, _selectedColorPosition.Y].Background;
        }

        private void SetClosestIndex(Color color)
        {
            ColorMine.ColorSpaces.Rgb rgbColorStop = new ColorMine.ColorSpaces.Rgb() { R = color.R, G = color.G, B = color.B };
            Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[TextSurface.Cells.Length];

            // Create a color weight for every cell compared to the color stop
            for (int x = 0; x < TextSurface.Cells.Length; x++)
            {
                ColorMine.ColorSpaces.Rgb rgbColor = new ColorMine.ColorSpaces.Rgb() { R = this[x].Background.R, G = this[x].Background.G, B = this[x].Background.B };
                ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

                colorWeights[x] = new Tuple<Color, double, int>(this[x].Background, rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);

            }

            var foundColor = colorWeights.OrderBy(t => t.Item2).First();

            this[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
            _selectedColorPosition = SadConsole.Surfaces.SadConsole.Surfaces.Basic.GetPointFromIndex(foundColor.Item3, Width);
            this[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;

            this.IsDirty = true;
        }

        public override void Compose()
        {
            if (IsDirty)
            {
                Color[] colors = Color.White.LerpSteps(Color.Black, Height);
                Color[] colorsEnd = _selectedHue.LerpSteps(Color.Black, Height);

                for (int y = 0; y < Height; y++)
                {
                    this[0, y].Background = colors[y];
                    this[Width - 1, y].Background = colorsEnd[y];

                    this[0, y].Foreground = new Color(255 - colors[y].R, 255 - colors[y].G, 255 - colors[y].B);
                    this[Width - 1, y].Foreground = new Color(255 - colorsEnd[y].R, 255 - colorsEnd[y].G, 255 - colorsEnd[y].B);



                    Color[] rowColors = colors[y].LerpSteps(colorsEnd[y], Width);

                    for (int x = 1; x < Width - 1; x++)
                    {
                        this[x, y].Background = rowColors[x];
                        this[x, y].Foreground = new Color(255 - rowColors[x].R, 255 - rowColors[x].G, 255 - rowColors[x].B);
                    }
                }

                IsDirty = false;
                OnComposed?.Invoke(this);
            }
        }

        protected override void OnMouseIn(SadConsole.Input.MouseConsoleState info)
        {
            base.OnMouseIn(info);

            if (Parent.CapturedControl == null)
            {
                if (info.Mouse.LeftButtonDown)
                {
                    var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                    this[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
                    _selectedColorPosition = location;
                    SelectedColorSafe = this[_selectedColorPosition.X, _selectedColorPosition.Y].Background;
                    this[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
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
                    if (location.X >= -6 && location.X <= Width + 5 && location.Y > -4 && location.Y < Height + 3)
                    {
                        this[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
                        _selectedColorPosition = new Point(Microsoft.Xna.Framework.MathHelper.Clamp(location.X, 0, Width - 1), Microsoft.Xna.Framework.MathHelper.Clamp(location.Y, 0, Height - 1));
                        SelectedColorSafe = this[_selectedColorPosition.X, _selectedColorPosition.Y].Background;
                        this[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
                    }

                    IsDirty = true;
                }
            }

            return base.ProcessMouse(info);
        }

        public override void DetermineAppearance()
        {
            
        }
    }
}
