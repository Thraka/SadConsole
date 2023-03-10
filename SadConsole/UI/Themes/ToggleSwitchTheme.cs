using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// A toggle switch theme used with a checkbox.
/// </summary>
[DataContract]
public class ToggleSwitchTheme : ThemeBase
{
    /// <summary>
    /// The on glyph of the switch.
    /// </summary>
    [DataMember]
    public int OnGlyph { get; set; }

    /// <summary>
    /// The background glyph of the switch.
    /// </summary>
    [DataMember]
    public int BackgroundGlyph { get; set; }

    /// <summary>
    /// An optional color of the <see cref="OnGlyph"/>.
    /// </summary>
    [DataMember]
    public Color OnGlyphColor { get; set; }

    /// <summary>
    /// An optional color of the <see cref="OnGlyph"/>.
    /// </summary>
    [DataMember]
    public Color OffGlyphColor { get; set; }

    /// <summary>
    /// The theme state used for the switch.
    /// </summary>
    public ThemeStates SwitchThemeState { get; protected set; }

    /// <summary>
    /// The orientation of the toggle switch relative to the text.
    /// </summary>
    /// <remarks>
    /// Valid values are <see cref="HorizontalAlignment.Left"/> and <see cref="HorizontalAlignment.Right"/>.
    /// </remarks>
    public HorizontalAlignment SwitchOrientation { get; set; }

    /// <summary>
    /// Creates a new toggle switch theme.
    /// </summary>
    /// <param name="onGlyph">The toggle switch on icon. Defaults to 251'√'.</param>
    /// <param name="offGlyph">The toggle switch off icon. Defaults to 0.</param>
    public ToggleSwitchTheme(int onGlyph = 178, int offGlyph = 177)
    {
        SwitchThemeState = new ThemeStates();

        OnGlyph = onGlyph;
        OnGlyphColor = Color.LawnGreen;
        BackgroundGlyph = offGlyph;
        OffGlyphColor = Color.DarkGreen;
        SwitchOrientation = HorizontalAlignment.Right;
    }

    /// <inheritdoc />
    public override void RefreshTheme(Colors themeColors, ControlBase control)
    {
        base.RefreshTheme(themeColors, control);

        SwitchThemeState.RefreshTheme(_colorsLastUsed);

        ToggleButtonBase checkbox = (ToggleButtonBase)control;

        if (checkbox.IsSelected)
        {
            SwitchThemeState.Normal.Foreground = OnGlyphColor;
            SwitchThemeState.MouseOver.Foreground = OnGlyphColor;
            SwitchThemeState.MouseDown.Foreground = OnGlyphColor;
            SwitchThemeState.Focused.Foreground = OnGlyphColor;

            SwitchThemeState.SetGlyph(OnGlyph);
        }
        else
        {
            SwitchThemeState.Normal.Foreground = OffGlyphColor;
            SwitchThemeState.MouseOver.Foreground = OffGlyphColor;
            SwitchThemeState.MouseDown.Foreground = OffGlyphColor;
            SwitchThemeState.Focused.Foreground = OffGlyphColor;

            SwitchThemeState.SetGlyph(BackgroundGlyph);
        }
    }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (control is not ToggleButtonBase checkbox) return;
        if (!control.IsDirty) return;

        RefreshTheme(control.FindThemeColors(), control);

        ColoredGlyph appearance = ControlThemeState.GetStateAppearanceNoMouse(checkbox.State);
        ColoredGlyph iconAppearance = SwitchThemeState.GetStateAppearance(checkbox.State);
        ColoredGlyph iconBackgroundAppearance = _colorsLastUsed.Appearance_ControlDisabled;
        iconBackgroundAppearance.Glyph = BackgroundGlyph;


        // If we are doing text, then print it otherwise we're just displaying the button part
        if (checkbox.Width == 1)
        {
            iconAppearance.CopyAppearanceTo(checkbox.Surface[0]);
        }
        else if (checkbox.Width == 2)
        {
            iconAppearance.CopyAppearanceTo(checkbox.Surface[0]);
            iconAppearance.CopyAppearanceTo(checkbox.Surface[1]);
        }
        else if (checkbox.Width == 3)
        {
            checkbox.Surface.Fill(iconBackgroundAppearance);

            iconAppearance.CopyAppearanceTo(checkbox.Surface[1]);
            iconAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? 2 : 0]);
        }
        else if (checkbox.Width <= 5)
        {
            checkbox.Surface.Fill(iconBackgroundAppearance);

            iconAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? ^1 : 0]);
            iconAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? ^2 : 1]);
        }
        else
        {
            checkbox.Surface.Fill(ControlThemeState.GetStateAppearance(ControlStates.Normal));

            if (SwitchOrientation == HorizontalAlignment.Right)
            {
                checkbox.Surface.Print(0, 0, checkbox.Text.Align(checkbox.TextAlignment, checkbox.Width - 4), appearance);
                iconAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? ^1 : ^3]);
                iconAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? ^2 : ^4]);
                iconBackgroundAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? ^3 : ^1]);
                iconBackgroundAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? ^4 : ^2]);
            }
            else
            {
                checkbox.Surface.Print(5, 0, checkbox.Text.Align(checkbox.TextAlignment, checkbox.Width - 4), appearance);
                iconAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? 2 : 0]);
                iconAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? 3 : 1]);
                iconBackgroundAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? 0 : 2]);
                iconBackgroundAppearance.CopyAppearanceTo(checkbox.Surface[checkbox.IsSelected ? 1 : 3]);
            }

            
        }

        checkbox.IsDirty = false;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() =>
        new ToggleSwitchTheme()
        {
            ControlThemeState = ControlThemeState.Clone(),
            BackgroundGlyph = BackgroundGlyph,
            OffGlyphColor = OffGlyphColor,
            OnGlyph = OnGlyph,
            OnGlyphColor = OnGlyphColor,
            SwitchOrientation = SwitchOrientation,
            SwitchThemeState =  SwitchThemeState.Clone(),
        };
}

