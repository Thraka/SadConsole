using System.Runtime.Serialization;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a button that can be toggled on/off within a group of other buttons.
/// </summary>
[DataContract]
public partial class CheckBox : ToggleButtonBase
{
    /// <summary>
    /// Creates a new checkbox control with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the control.</param>
    /// <param name="height">The height of the control.</param>
    public CheckBox(int width, int height) : base(width, height)
    {
        _textAlignment = HorizontalAlignment.Left;

        LeftBracketGlyph = '[';
        RightBracketGlyph = ']';
        CheckedIconGlyph = 251;
        UncheckedIconGlyph = 0;

        BracketsThemeState = new ThemeStates();
        IconThemeState = new ThemeStates();
    }

    /// <summary>
    /// Creates an auto sizing checkbox control with the specified text.
    /// </summary>
    public CheckBox(string text) : base()
    {
        _textAlignment = HorizontalAlignment.Left;

        LeftBracketGlyph = '[';
        RightBracketGlyph = ']';
        CheckedIconGlyph = 251;
        UncheckedIconGlyph = 0;
        Text = text;

        BracketsThemeState = new ThemeStates();
        IconThemeState = new ThemeStates();
    }

    /// <summary>
    /// Perfroms a click on the base button and also toggles the <see cref="ToggleButtonBase.IsSelected"/> property.
    /// </summary>
    protected override void OnClick()
    {
        base.OnClick();
        IsSelected = !IsSelected;
    }
}
