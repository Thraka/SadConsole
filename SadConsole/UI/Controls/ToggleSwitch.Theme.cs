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
    /// The orientation of the toggle switch relative to the text.
    /// </summary>
    /// <remarks>
    /// Valid values are <see cref="HorizontalAlignment.Left"/> and <see cref="HorizontalAlignment.Right"/>.
    /// </remarks>
    public HorizontalAlignment SwitchOrientation { get; set; }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Colors currentColors = FindThemeColors();

        ThemeState.RefreshTheme(currentColors);

        if (IsSelected)
        {
            ThemeState.Normal.Foreground = OnGlyphColor;
            ThemeState.MouseOver.Foreground = OnGlyphColor;
            ThemeState.MouseDown.Foreground = OnGlyphColor;
            ThemeState.Focused.Foreground = OnGlyphColor;

            ThemeState.SetGlyph(OnGlyph);
        }
        else
        {
            ThemeState.Normal.Foreground = OffGlyphColor;
            ThemeState.MouseOver.Foreground = OffGlyphColor;
            ThemeState.MouseDown.Foreground = OffGlyphColor;
            ThemeState.Focused.Foreground = OffGlyphColor;

            ThemeState.SetGlyph(BackgroundGlyph);
        }
    }
}
