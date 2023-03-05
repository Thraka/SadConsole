using System.Runtime.Serialization;

namespace SadConsole.UI.Themes;

/// <summary>
/// The theme of a radio button control.
/// </summary>
[DataContract]
public class RadioButtonTheme : CheckBoxTheme
{
    /// <summary>
    /// Creates a new radio button theme with an optional bracket and check icon.
    /// </summary>
    /// <param name="leftBracketGlyph">The left bracket of the checkbox icon. Defaults to '('.</param>
    /// <param name="rightBracketGlyph">The right bracket of the checkbox icon. Defaults to ')'.</param>
    /// <param name="checkedGlyph">The checkbox checked icon. Defaults to 15'☼'.</param>
    /// <param name="uncheckedGlyph">The checkbox unchecked icon. Defaults to 0.</param>
    public RadioButtonTheme(int leftBracketGlyph, int rightBracketGlyph, int checkedGlyph, int uncheckedGlyph) : base(leftBracketGlyph, rightBracketGlyph, checkedGlyph, uncheckedGlyph)
    {

    }

    /// <summary>
    /// Creates the default theme for this control, using ( ) for brackets and * for checked.
    /// </summary>
    public RadioButtonTheme(): base('(', ')', 15, 0) { }

    /// <inheritdoc />
    public override ThemeBase Clone() => new RadioButtonTheme()
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
