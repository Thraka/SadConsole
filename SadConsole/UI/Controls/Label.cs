using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// A simple surface for drawing text that can be moved and sized like a control.
/// </summary>
[DataContract]
public partial class Label : ControlBase
{
    private string _text = string.Empty;

    /// <summary>
    /// When <see langword="true"/>, shows an underline on the text.
    /// </summary>
    public bool ShowUnderline { get; set; }

    /// <summary>
    /// When <see langword="true"/>, shows a strikethrough on the text.
    /// </summary>
    public bool ShowStrikethrough { get; set; }

    /// <summary>
    /// Optional text color for the label. Otherwise the theme controls the color.
    /// </summary>
    public Color? TextColor { get; set; }

    /// <summary>
    /// Sets the horizontal alignment of the label. Defaults to <see cref="HorizontalAlignment.Left"/>.
    /// </summary>
    public HorizontalAlignment Alignment { get; set; }

    /// <summary>
    /// The text to display on the label. The label size is set in the constructor and cannot be changed.
    /// </summary>
    public string DisplayText
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            
            IsDirty = true;
        }
    }



    /*
ControlThemeState changes to ThemeState

_colorsLastUsed(when defined) should be defined in the `UpdateAndRedraw`: `Colors _colorsLastUsed = FindThemeColors();` that solves resolves the references.But you can rename it to `colors`.

if `GetOffColor` is used, this has been moved from `ThemeState` to the `Colors` class.

The order of code in `UpdateAndRedraw` is:

1. Check if `IsDirty == false` and return.
2. Get the current colors for the control `Colors _colorsLastUsed = FindThemeColors();`
3. Instead of calling `RefreshTheme(control.FindThemeColors(), control);` call `ThemeState.RefreshTheme(_colorsLastUsed);`
4. Copy any code in the theme's `RefreshTheme` method (if overridden).
5. Copy any code in the theme's `UpdateAndDraw` method.
6. If your code used the `control` parameter, just erase the code `control.` everywhere, since this method is on the control directly. Also, replace references of `ControlThemeState` with `ThemeState`.
7. Set `IsDirty = false`
8. Refactor the name `_colorsLastUsed` to `currentColors`.

    */





    /// <summary>
    /// A control to display simple one-line text.
    /// </summary>
    /// <param name="displayText">The text to display. Sets the width based on the length.</param>
    public Label(string displayText) : base(displayText.Length, 1)
    {
        _text = displayText;
        TabStop = false;
    }

    /// <summary>
    /// A control to display simple one-line text.
    /// </summary>
    /// <param name="length">The initial length of the label without any text.</param>
    public Label(int length) : base(length, 1) => TabStop = false;

    /// <summary>
    /// Resizes the label but forces a height of 1.
    /// </summary>
    /// <param name="width">The width of the label.</param>
    /// <param name="height">Not used.</param>
    public override void Resize(int width, int height) =>
        base.Resize(width, 1);

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        TabStop = false;
        DetermineState();
        IsDirty = true;
    }
}
