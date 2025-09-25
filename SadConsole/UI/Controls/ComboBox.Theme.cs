using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls;

public partial class ComboBox
{
    /// <summary>
    /// When <see langword="true"/>, uses the <see cref="PopupHorizontal"/> value from the interior of the control. When <see langword="false"/>, it's used from the outside of the control.
    /// </summary>
    [DataMember]
    public bool PopupInnerAligned { get; set; }

    /// <summary>
    /// Sets the horizontal orientation of the of the dropdown popup.
    /// </summary>
    [DataMember]
    public HorizontalAlignment PopupHorizontal { get; set; }

    /// <summary>
    /// Sets the vertical orientation of the of the dropdown popup.
    /// </summary>
    [DataMember]
    public VerticalAlignment PopupVertical { get; set; }

    /// <summary>
    /// The glyph to use on the control when it's collapsed.
    /// </summary>
    [DataMember]
    public int CollapsedButtonGlyph { get; set; }

    /// <summary>
    /// The glyph to use on the control when it's expanded.
    /// </summary>
    [DataMember]
    public int ExpandedButtonGlyph { get; set; }

    /// <inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        IconThemeState.RefreshTheme(colors);

        ThemeState.Normal.Background = colors.GetOffColor(ThemeState.Normal.Background, colors.ControlHostBackground);
        ThemeState.MouseOver.Background = colors.GetOffColor(ThemeState.MouseOver.Background, colors.ControlHostBackground);
        ThemeState.MouseDown.Background = colors.GetOffColor(ThemeState.MouseDown.Background, colors.ControlHostBackground);
        ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, colors.ControlHostBackground);

        // If the focused background color is the same as the non-focused, alter it so it stands out
        ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, ThemeState.Normal.Background);

        IconThemeState.Normal.Foreground = colors.Lines;
        IconThemeState.MouseOver.Foreground = colors.Lines;
        IconThemeState.MouseDown.Foreground = colors.Lines;
        IconThemeState.Focused.Foreground = colors.Lines;

        IconThemeState.Normal.Background = ThemeState.Normal.Background;
        IconThemeState.MouseOver.Background = ThemeState.MouseOver.Background;
        IconThemeState.MouseDown.Background = ThemeState.MouseDown.Background;
        IconThemeState.Focused.Background = ThemeState.Focused.Background;
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        // Update the theme data
        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        // Draw the control
        ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);
        ColoredGlyphBase iconAppearance = IconThemeState.GetStateAppearance(State);

        Surface.Fill(appearance.Foreground, appearance.Background, 0);
        iconAppearance.CopyAppearanceTo(Surface[Width - 1, 0]);

        Surface.Print(0, 0, Text.Align(TextAlignment, Surface.Width - 2));

        Surface[Width - 1, 0].Glyph = IsSelected ? ExpandedButtonGlyph : CollapsedButtonGlyph;
    }
}
