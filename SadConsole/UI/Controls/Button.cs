using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// Simple button control with a height of 1.
/// </summary>
[DataContract]
public class Button : ButtonBase
{
    /// <summary>
    /// When true, renders the "end" glyphs on the button.
    /// </summary>
    [DataMember]
    public bool ShowEnds { get; set; } = true;

    /// <summary>
    /// The theme state used with the left end of the button.Defaults to '&lt;'.
    /// </summary>
    [DataMember]
    public int LeftEndGlyph { get; set; } = '<';

    /// <summary>
    /// The theme state used with the right end of the button. Defaults to '>'.
    /// </summary>
    [DataMember]
    public int RightEndGlyph { get; set; } = '>';

    /// <summary>
    /// The theme state used with the left end of the button.
    /// </summary>
    public ThemeStates EndsThemeState { get; protected set; } = new ThemeStates();

    /// <summary>
    /// Creates an instance of the button control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control (default is 1).</param>
    public Button(int width, int height = 1)
        : base(width, height)
    {
        
    }

    /// <summary>
    /// Creates an auto sizing button with the specified text.
    /// </summary>
    /// <param name="text">The text to display on the button.</param>
    public Button(string text) : base() =>
        Text = text;

    ///<inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        EndsThemeState.RefreshTheme(colors);
        EndsThemeState.Normal.Foreground = colors.Lines;
    }

    ///<inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        // If automatically sized, ensure that the size is accurate
        if (AutoSize && EstimateControlSurface().Size != Surface.Area.Size)
            Surface = CreateControlSurface();

        MouseArea = Surface.Area;

        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);
        ColoredGlyphBase endGlyphAppearance = EndsThemeState.GetStateAppearance(State);

        int middle = (Height != 1 ? Height / 2 : 0);

        // Redraw the control
        Surface.Fill(
            appearance.Foreground,
            appearance.Background,
            appearance.Glyph, null);

        int width = AutoSize ? Surface.Width : Width;
        
        if (ShowEnds && width >= 3)
        {
            Surface.Print(1, middle, Text.Align(TextAlignment, width - 2));
            Surface.SetCellAppearance(0, middle, endGlyphAppearance);
            Surface.SetCellAppearance(width - 1, middle, endGlyphAppearance);
            Surface[width - 1, middle].Glyph = RightEndGlyph;
            Surface[0, middle].Glyph = LeftEndGlyph;
        }
        else
            Surface.Print(0, middle, Text.Align(TextAlignment, width));

        IsDirty = false;
    }

    /// <summary>
    /// Resizes the control surface based on <see cref="ButtonBase.AutoSize"/> or the <see cref="ControlBase.Width"/> and <see cref="ControlBase.Height"/> properties.
    /// </summary>
    /// <returns>The control's surface.</returns>
    protected override ICellSurface CreateControlSurface()
    {
        if (!AutoSize) return base.CreateControlSurface();

        // Create an automatically sized control
        Rectangle area = EstimateControlSurface();

        var surface = new CellSurface(area.Width, area.Height)
        {
            DefaultBackground = SadRogue.Primitives.Color.Transparent
        };
        surface.Clear();
        return surface;
    }

    private Rectangle EstimateControlSurface() =>
        new(0, 0, Text.Length + 2 + (ShowEnds ? 2 : 0), 1);
}
