using SadRogue.Primitives;
using System;
using System.Linq;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Displays the color hues on a bar.
    /// </summary>
    public class HueBar : ControlBase
    {
        /// <summary>
        /// Raised when the <see cref="SelectedColor"/> value changes.
        /// </summary>
        public event EventHandler ColorChanged;

        /// <summary>
        /// The selected color.
        /// </summary>
        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                SetClosestIndex(value);

                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    ColorChanged?.Invoke(this, EventArgs.Empty);
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

                    ColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The position on the bar currently selected.
        /// </summary>
        public int SelectedPosition { get; private set; }

        /// <summary>
        /// Internal use by theme.
        /// </summary>
        public int _positions;
        private Color _selectedColor;

        /// <summary>
        /// Creates a new hue bar control.
        /// </summary>
        /// <param name="width">The width of the bar.</param>
        public HueBar(int width): base(width, 2)
        {
            CanFocus = false;
        }

        private void SetClosestIndex(Color color)
        {
            ColorMine.ColorSpaces.Rgb rgbColorStop = new ColorMine.ColorSpaces.Rgb() { R = color.R, G = color.G, B = color.B };
            Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[Width];

            // Create a color weight for every cell compared to the color stop
            for (int x = 0; x < Width; x++)
            {
                ColorMine.ColorSpaces.Rgb rgbColor = new ColorMine.ColorSpaces.Rgb() { R = Surface[x, 0].Foreground.R, G = Surface[x, 0].Foreground.G, B = Surface[x, 0].Foreground.B };
                ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

                colorWeights[x] = new Tuple<Color, double, int>(Surface[x, 0].Foreground, rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);

            }

            Tuple<Color, double, int> foundColor = colorWeights.OrderBy(t => t.Item2).First();
            SelectedPosition = foundColor.Item3;
            IsDirty = true;
        }
        
        /// <inheritdoc/>
        protected override void OnMouseIn(ControlMouseState info)
        {
            base.OnMouseIn(info);

            if (Parent.Host.CapturedControl == null)
            {
                if (info.OriginalMouseState.Mouse.LeftButtonDown)
                {
                    Point location = info.MousePosition;
                    SelectedPosition = location.X;
                    SelectedColorSafe = Surface[SelectedPosition, 0].Foreground;
                    IsDirty = true;

                    Parent.Host.CaptureControl(this);
                }
            }
        }

        /// <inheritdoc/>
        public override bool ProcessMouse(SadConsole.Input.MouseScreenObjectState info)
        {
            if (Parent.Host.CapturedControl == this)
            {
                if (info.Mouse.LeftButtonDown == false)
                    Parent.Host.ReleaseControl();
                else
                {
                    var newState = new ControlMouseState(this, info);
                    Point location = newState.MousePosition;

                    //if (info.ConsolePosition.X >= Position.X && info.ConsolePosition.X < Position.X + Width)
                    if (location.X >= 0 && location.X <= Width - 1 && location.Y > -4 && location.Y < Height + 3 )
                    {
                        SelectedPosition = location.X;
                        SelectedColorSafe = Surface[SelectedPosition, 0].Foreground;
                    }

                    IsDirty = true;
                }
            }

            return base.ProcessMouse(info);
        }
    }
}
