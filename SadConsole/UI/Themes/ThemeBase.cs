using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// The base class for a theme.
/// </summary>
[DataContract]
public abstract class ThemeBase
{
    /// <summary>
    /// The colors last used when <see cref="RefreshTheme(Colors, ControlBase)"/> was called.
    /// </summary>
    protected Colors? _colorsLastUsed;

    /// <summary>
    /// Default theme state of the whole control.
    /// </summary>
    [DataMember]
    public ThemeStates ControlThemeState { get; set; } = new ThemeStates();

    /// <summary>
    /// Draws the control state to the control.
    /// </summary>
    /// <param name="control">The control to draw.</param>
    /// <param name="time">The time since the last update frame call.</param>
    public abstract void UpdateAndDraw(ControlBase control, System.TimeSpan time);

    /// <summary>
    /// Called when the theme is attached to a control.
    /// </summary>
    /// <param name="control">The control that will use this theme instance.</param>
    public virtual void Attached(ControlBase control)
    {
        control.Surface = new CellSurface(control.Width, control.Height)
        {
            DefaultBackground = SadRogue.Primitives.Color.Transparent
        };
        control.Surface.Clear();
    }

    /// <summary>
    /// Creates a new theme instance based on the current instance.
    /// </summary>
    /// <returns>A new theme instance.</returns>
    public abstract ThemeBase Clone();


    /// <summary>
    /// Reloads the theme values based on the colors provided.
    /// </summary>
    /// <param name="colors">The colors to create the theme with.</param>
    /// <param name="control">The control being drawn with the theme.</param>
    [MemberNotNull("_colorsLastUsed")]
    public virtual void RefreshTheme(Colors colors, ControlBase control)
    {
        _colorsLastUsed = colors ?? control.FindThemeColors();

        ControlThemeState.RefreshTheme(_colorsLastUsed);
    }

    /// <summary>
    /// Compares two colors and if they match, returns a color that is lighter or darker based on if <see cref="Colors.IsLightTheme"/>.
    /// </summary>
    /// <param name="inColor">The base color.</param>
    /// <param name="compareColor">The color to compare with.</param>
    /// <returns>A new color.</returns>
    protected Color GetOffColor(Color inColor, Color compareColor)
    {
        if (inColor == compareColor)
            inColor = _colorsLastUsed!.IsLightTheme ? compareColor.GetDark() : compareColor.GetBright();

        return NormalizeBlack(inColor);
    }

    /// <summary>
    /// Normalizes a dark color to at least R:25 G:25 B:25 A:255.
    /// </summary>
    /// <param name="inColor">The color to check.</param>
    /// <returns>A new color.</returns>
    protected static Color NormalizeBlack(Color inColor)
    {
        if (inColor.R < 25
            && inColor.G < 25
            && inColor.B < 25)
            return new Color(25, 25, 25, 255);
        else
            return inColor;
    }
}
