using SadRogue.Primitives;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SadConsole.UI.Controls;

/// <summary>
/// A color bar control.
/// </summary>
public class ColorBar : ControlBase
{
    /// <summary>
    /// Raised when the <see cref="SelectedColor"/> value changes.
    /// </summary>
    public event EventHandler? ColorChanged;

    /// <summary>
    /// Internal use by theme.
    /// </summary>
    public int _positions;

    /// <summary>
    /// Internal use by theme.
    /// </summary>
    public Color[] _colorSteps;

    private Color _selectedColor;
    private Color _startingColor;
    private Color _endingColor;

    /// <summary>
    /// Gets or sets the color on the left-side of the bar.
    /// </summary>
    public Color StartingColor
    {
        get => _startingColor;

        [MemberNotNull(nameof(_colorSteps))]
        set
        {
            _startingColor = value;
            _colorSteps = _startingColor.LerpSteps(_endingColor, Width);
            SetClosestIndex(value);
            IsDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the color on the right-side of the bar.
    /// </summary>
    public Color EndingColor
    {
        get => _endingColor;

        [MemberNotNull(nameof(_colorSteps))]
        set
        {
            _endingColor = value;
            _colorSteps = _startingColor.LerpSteps(_endingColor, Width);
            SetClosestIndex(value);
            IsDirty = true;
        }
    }

    /// <summary>
    /// The selected color.
    /// </summary>
    public Color SelectedColor
    {
        get => _selectedColor;
        set
        {
            SetClosestIndex(value);

            if (_selectedColor != value)
            {
                _selectedColor = value;

                _colorSteps = _startingColor.LerpSteps(_endingColor, Width);
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
    /// Creates a new color bar with the specified width.
    /// </summary>
    /// <param name="width">The width of the bar.</param>
    public ColorBar(int width): base (width, 2)
    {
        StartingColor = Color.White;
        EndingColor = Color.Black;
        CanFocus = false;
    }

    private void SetClosestIndex(Color color)
    {
        ColorMine.ColorSpaces.Rgb rgbColorStop = new() { R = color.R, G = color.G, B = color.B };
        Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[Width];

        // Create a color weight for every cell compared to the color stop
        for (int x = 0; x < Width; x++)
        {
            ColorMine.ColorSpaces.Rgb rgbColor = new() { R = _colorSteps[x].R, G = _colorSteps[x].G, B = _colorSteps[x].B };
            ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

            colorWeights[x] = new Tuple<Color, double, int>(_colorSteps[x], rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);
        }

        Tuple<Color, double, int> foundColor = colorWeights.OrderBy(t => t.Item2).First();
        SelectedPosition = foundColor.Item3;
        IsDirty = true;
    }

    /// <inheritdoc/>
    protected override void OnMouseIn(ControlMouseState info)
    {
        base.OnMouseIn(info);

        if (Parent!.Host!.CapturedControl == null)
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
        if (Parent?.Host?.CapturedControl == this)
        {
            if (info.Mouse.LeftButtonDown == false)
                Parent.Host.ReleaseControl();
            else
            {
                var newState = new ControlMouseState(this, info);
                Point location = newState.MousePosition;

                //if (info.ConsolePosition.X >= Position.X && info.ConsolePosition.X < Position.X + Width)
                if (location.X >= 0 && location.X <= base.Width - 1 && location.Y > -4 && location.Y < Height + 3)
                {
                    SelectedPosition = location.X;
                    SelectedColorSafe = Surface[SelectedPosition, 0].Foreground;
                }

                IsDirty = true;
            }
        }

        return base.ProcessMouse(info);
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Surface.Fill(Color.White, Color.Black, 0, null);

        _positions = Width;

        for (int x = 0; x < Width; x++)
        {
            Surface[x, 0].Glyph = 219;
            Surface[x, 0].Foreground = _colorSteps[x];
        }

        Surface[SelectedPosition, 1].Glyph = 30;
        Surface[SelectedPosition, 1].Foreground = Color.LightGray;//this[_selectedPosition, 0].Foreground;

        IsDirty = false;
    }
}
