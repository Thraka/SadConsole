
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

        // Update the theme data
        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        // Draw the control
        ColoredGlyph appearance = ThemeState.GetStateAppearance(State);
        ColoredGlyph bracketAppearance = BracketsThemeState.GetStateAppearance(State);
        ColoredGlyph iconAppearance = IconThemeState.GetStateAppearance(State);

        Surface.Fill(appearance.Foreground, appearance.Background, null);

        // If we are doing text, then print it otherwise we're just displaying the button part
        if (Width <= 2)
            iconAppearance.CopyAppearanceTo(Surface[0, 0]);

        if (Width >= 3)
        {
            bracketAppearance.CopyAppearanceTo(Surface[0, 0]);
            iconAppearance.CopyAppearanceTo(Surface[1, 0]);
            bracketAppearance.CopyAppearanceTo(Surface[2, 0]);

            Surface[0, 0].Glyph = LeftBracketGlyph;
            Surface[2, 0].Glyph = RightBracketGlyph;
        }

        if (Width >= 5)
            Surface.Print(4, 0, Text.Align(TextAlignment, Width - 4));


        IsDirty = false;
    }
}
