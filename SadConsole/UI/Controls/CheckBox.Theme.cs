using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class CheckBox : ToggleButtonBase
{
    /// <summary>
    /// The glyph for the left-side bracket of the icon.
    /// </summary>
    [DataMember]
    public int LeftBracketGlyph { get; set; }

    /// <summary>
    /// The glyph for the right-side bracket of the icon.
    /// </summary>
    [DataMember]
    public int RightBracketGlyph { get; set; }

    /// <summary>
    /// The glyph of the icon when checked.
    /// </summary>
    [DataMember]
    public int CheckedIconGlyph { get; set; }

    /// <summary>
    /// The glyph of the icon when unchecked.
    /// </summary>
    [DataMember]
    public int UncheckedIconGlyph { get; set; }

    /// <summary>
    /// An optional color of the <see cref="CheckedIconGlyph"/>.
    /// </summary>
    [DataMember]
    public Color? CheckedIconColor { get; set; }

    /// <summary>
    /// An optional color of the <see cref="UncheckedIconGlyph"/>.
    /// </summary>
    [DataMember]
    public Color? UncheckedIconColor { get; set; }

    /// <summary>
    /// The theme state used with the brackets.
    /// </summary>
    public ThemeStates BracketsThemeState { get; protected set; }

    /// <summary>
    /// The theme state used with the icon of the 
    /// </summary>
    public ThemeStates IconThemeState { get; protected set; }

    /// <inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        BracketsThemeState.RefreshTheme(colors);
        IconThemeState.RefreshTheme(colors);

        BracketsThemeState.SetForeground(colors.Lines);

        if (IsSelected)
        {
            if (CheckedIconColor != null)
            {
                IconThemeState.Normal.Foreground = CheckedIconColor.Value;
                IconThemeState.MouseOver.Foreground = CheckedIconColor.Value;
                IconThemeState.MouseDown.Foreground = CheckedIconColor.Value;
                IconThemeState.Focused.Foreground = CheckedIconColor.Value;
            }
            else
            {
                IconThemeState.Normal.Foreground = colors.ControlForegroundSelected;
                IconThemeState.MouseOver.Foreground = colors.ControlForegroundSelected;
                IconThemeState.MouseDown.Foreground = colors.ControlForegroundSelected;
                IconThemeState.Focused.Foreground = colors.ControlForegroundSelected;
            }

            IconThemeState.SetGlyph(CheckedIconGlyph);
        }
        else
        {
            if (UncheckedIconColor != null)
            {
                IconThemeState.Normal.Foreground = UncheckedIconColor.Value;
                IconThemeState.MouseOver.Foreground = UncheckedIconColor.Value;
                IconThemeState.MouseDown.Foreground = UncheckedIconColor.Value;
                IconThemeState.Focused.Foreground = UncheckedIconColor.Value;
            }
            else
            {
                IconThemeState.Normal.Foreground = colors.ControlForegroundSelected;
                IconThemeState.MouseOver.Foreground = colors.ControlForegroundSelected;
                IconThemeState.MouseDown.Foreground = colors.ControlForegroundSelected;
                IconThemeState.Focused.Foreground = colors.ControlForegroundSelected;
            }

            IconThemeState.SetGlyph(UncheckedIconGlyph);
        }
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        // If automatically sized, ensure that the size is accurate
        if (AutoSize && EstimateControlSurface().Size != Surface.Area.Size)
            Surface = CreateControlSurface();

        MouseArea = Surface.Area;

        // Update the theme data
        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        // Draw the control
        ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);
        ColoredGlyphBase bracketAppearance = BracketsThemeState.GetStateAppearance(State);
        ColoredGlyphBase iconAppearance = IconThemeState.GetStateAppearance(State);

        Surface.DefaultBackground = appearance.Background;
        Surface.DefaultForeground = appearance.Foreground;
        Surface.Clear();

        int width = AutoSize ? Surface.Width : Width;

        // If we are doing text, then print it otherwise we're just displaying the button part
        if (width <= 2)
            iconAppearance.CopyAppearanceTo(Surface[0, 0]);

        if (width >= 3)
        {
            bracketAppearance.CopyAppearanceTo(Surface[0, 0]);
            iconAppearance.CopyAppearanceTo(Surface[1, 0]);
            bracketAppearance.CopyAppearanceTo(Surface[2, 0]);

            Surface[0, 0].Glyph = LeftBracketGlyph;
            Surface[2, 0].Glyph = RightBracketGlyph;
        }

        if (width >= 5)
            Surface.Print(4, 0, Text.Align(TextAlignment, width - 4));


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
        new(0, 0, Text.Length + 3 + 2, 1);
}
