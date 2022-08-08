using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// The theme of a checkbox control.
/// </summary>
[DataContract]
public class CheckBoxTheme : ThemeBase
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
    /// The theme state used with the icon of the checkbox.
    /// </summary>
    public ThemeStates IconThemeState { get; protected set; }

    /// <summary>
    /// Creates a new checkbox theme with an optional bracket and check icon.
    /// </summary>
    /// <param name="leftBracketGlyph">The left bracket of the checkbox icon. Defaults to '['.</param>
    /// <param name="rightBracketGlyph">The right bracket of the checkbox icon. Defaults to ']'.</param>
    /// <param name="checkedGlyph">The checkbox checked icon. Defaults to 251'√'.</param>
    /// <param name="uncheckedGlyph">The checkbox unchecked icon. Defaults to 0.</param>
    public CheckBoxTheme(int leftBracketGlyph = '[', int rightBracketGlyph = ']', int checkedGlyph = 251, int uncheckedGlyph = 0)
    {
        BracketsThemeState = new ThemeStates();
        IconThemeState = new ThemeStates();

        LeftBracketGlyph = leftBracketGlyph;
        RightBracketGlyph = rightBracketGlyph;
        CheckedIconGlyph = checkedGlyph;
        UncheckedIconGlyph = uncheckedGlyph;
    }

    /// <inheritdoc />
    public override void RefreshTheme(Colors themeColors, ControlBase control)
    {
        base.RefreshTheme(themeColors, control);

        BracketsThemeState.RefreshTheme(_colorsLastUsed);
        IconThemeState.RefreshTheme(_colorsLastUsed);

        BracketsThemeState.SetForeground(_colorsLastUsed.Lines);

        ToggleButtonBase checkbox = (ToggleButtonBase)control;

        if (checkbox.IsSelected)
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
                IconThemeState.Normal.Foreground = _colorsLastUsed.ControlForegroundSelected;
                IconThemeState.MouseOver.Foreground = _colorsLastUsed.ControlForegroundSelected;
                IconThemeState.MouseDown.Foreground = _colorsLastUsed.ControlForegroundSelected;
                IconThemeState.Focused.Foreground = _colorsLastUsed.ControlForegroundSelected;
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
                IconThemeState.Normal.Foreground = _colorsLastUsed.ControlForegroundSelected;
                IconThemeState.MouseOver.Foreground = _colorsLastUsed.ControlForegroundSelected;
                IconThemeState.MouseDown.Foreground = _colorsLastUsed.ControlForegroundSelected;
                IconThemeState.Focused.Foreground = _colorsLastUsed.ControlForegroundSelected;
            }

            IconThemeState.SetGlyph(UncheckedIconGlyph);
        }
    }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (control is not ToggleButtonBase checkbox) return;
        if (!control.IsDirty) return;

        RefreshTheme(control.FindThemeColors(), control);

        ColoredGlyph appearance = ControlThemeState.GetStateAppearance(checkbox.State);
        ColoredGlyph bracketAppearance = BracketsThemeState.GetStateAppearance(checkbox.State);
        ColoredGlyph iconAppearance = IconThemeState.GetStateAppearance(checkbox.State);

        checkbox.Surface.Fill(appearance.Foreground, appearance.Background, null);

        // If we are doing text, then print it otherwise we're just displaying the button part
        if (checkbox.Width <= 2)
            iconAppearance.CopyAppearanceTo(checkbox.Surface[0, 0]);

        if (checkbox.Width >= 3)
        {
            bracketAppearance.CopyAppearanceTo(checkbox.Surface[0, 0]);
            iconAppearance.CopyAppearanceTo(checkbox.Surface[1, 0]);
            bracketAppearance.CopyAppearanceTo(checkbox.Surface[2, 0]);

            checkbox.Surface[0, 0].Glyph = LeftBracketGlyph;
            checkbox.Surface[2, 0].Glyph = RightBracketGlyph;
        }

        if (checkbox.Width >= 5)
            checkbox.Surface.Print(4, 0, checkbox.Text.Align(checkbox.TextAlignment, checkbox.Width - 4));

        checkbox.IsDirty = false;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() =>
        new CheckBoxTheme()
        {
            ControlThemeState = ControlThemeState.Clone(),
            BracketsThemeState = BracketsThemeState.Clone(),
            IconThemeState = IconThemeState.Clone(),
            CheckedIconColor = CheckedIconColor,
            UncheckedIconColor = UncheckedIconColor,
            CheckedIconGlyph = CheckedIconGlyph,
            UncheckedIconGlyph = UncheckedIconGlyph
        };
}

