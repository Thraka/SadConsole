using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class ToggleSwitch
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
    /// The color of the <see cref="OnGlyph"/> when the control is <see cref="ToggleButtonBase.IsSelected"/> is true.
    /// </summary>
    [DataMember]
    public Color OnGlyphColor { get; set; }

    /// <summary>
    /// The color of the <see cref="OnGlyph"/> when the control is <see cref="ToggleButtonBase.IsSelected"/> is false.
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

    /// <inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        SwitchThemeState.RefreshTheme(colors);

        if (IsSelected)
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

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        ColoredGlyphBase appearance = ThemeState.GetStateAppearanceNoMouse(State & ~ControlStates.Focused);
        ColoredGlyphBase iconAppearance = SwitchThemeState.GetStateAppearance(State);
        ColoredGlyphBase iconBackgroundAppearance = colors.Appearance_ControlDisabled;
        iconBackgroundAppearance.Glyph = BackgroundGlyph;

        // If we are doing text, then print it otherwise we're just displaying the button part
        if (Width == 1)
        {
            iconAppearance.CopyAppearanceTo(Surface[0]);
        }
        else if (Width == 2)
        {
            iconAppearance.CopyAppearanceTo(Surface[0]);
            iconAppearance.CopyAppearanceTo(Surface[1]);
        }
        else if (Width == 3)
        {
            Surface.Fill(iconBackgroundAppearance);

            iconAppearance.CopyAppearanceTo(Surface[1]);
            iconAppearance.CopyAppearanceTo(Surface[IsSelected ? 2 : 0]);
        }
        else if (Width <= 5)
        {
            Surface.Fill(iconBackgroundAppearance);

            iconAppearance.CopyAppearanceTo(Surface[IsSelected ? ^1 : 0]);
            iconAppearance.CopyAppearanceTo(Surface[IsSelected ? ^2 : 1]);
        }
        else
        {
            Surface.Fill(ThemeState.GetStateAppearance(ControlStates.Normal));

            if (SwitchOrientation == HorizontalAlignment.Right)
            {
                Surface.Print(0, 0, Text.Align(TextAlignment, Width - 4), appearance);
                iconAppearance.CopyAppearanceTo(Surface[IsSelected ? ^1 : ^3]);
                iconAppearance.CopyAppearanceTo(Surface[IsSelected ? ^2 : ^4]);
                iconBackgroundAppearance.CopyAppearanceTo(Surface[IsSelected ? ^3 : ^1]);
                iconBackgroundAppearance.CopyAppearanceTo(Surface[IsSelected ? ^4 : ^2]);
            }
            else if (SwitchOrientation == HorizontalAlignment.Stretch)
            {
                Surface.Fill(iconBackgroundAppearance);
                iconAppearance.CopyAppearanceTo(Surface[IsSelected ? ^1 : 0]);
                iconAppearance.CopyAppearanceTo(Surface[IsSelected ? ^2 : 1]);
            }
            else
            {
                Surface.Print(Width - 4, 0, Text.Align(TextAlignment, Width - 4), appearance);
                iconAppearance.CopyAppearanceTo(Surface[IsSelected ? 2 : 0]);
                iconAppearance.CopyAppearanceTo(Surface[IsSelected ? 3 : 1]);
                iconBackgroundAppearance.CopyAppearanceTo(Surface[IsSelected ? 0 : 2]);
                iconBackgroundAppearance.CopyAppearanceTo(Surface[IsSelected ? 1 : 3]);
            }
        }

        IsDirty = false;
    }
}
