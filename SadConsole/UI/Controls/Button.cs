using System;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls;

/// <summary>
/// Simple button control with a height of 1.
/// </summary>
[DataContract]
public class Button : ButtonBase
{
    /// <summary>
    /// When true, renders the "end" glyphs on the button.
    /// </summary>
    [DataMember]
    public bool ShowEnds { get; set; } = true;

    /// <summary>
    /// The theme state used with the left end of the button.Defaults to '&lt;'.
    /// </summary>
    [DataMember]
    public int LeftEndGlyph { get; set; }

    /// <summary>
    /// The theme state used with the right end of the button. Defaults to '>'.
    /// </summary>
    [DataMember]
    public int RightEndGlyph { get; set; }

    /// <summary>
    /// Creates an instance of the button control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control (default is 1).</param>
    public Button(int width, int height = 1)
        : base(width, height)
    {
        
    }

    ///<inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Colors colors = FindThemeColors();

        ThemeState.RefreshTheme(colors);

        ColoredGlyph appearance = ThemeState.GetStateAppearance(State);
        ColoredGlyph endGlyphAppearance = ThemeState.GetStateAppearance(State);

        endGlyphAppearance.Foreground = colors.Lines;

        int middle = (Height != 1 ? Height / 2 : 0);

        // Redraw the control
        Surface.Fill(
            appearance.Foreground,
            appearance.Background,
            appearance.Glyph, null);

        if (ShowEnds && Width >= 3)
        {
            Surface.Print(1, middle, Text.Align(TextAlignment, Width - 2));
            Surface.SetCellAppearance(0, middle, endGlyphAppearance);
            Surface[0, middle].Glyph = LeftEndGlyph;
            Surface.SetCellAppearance(Width - 1, middle, endGlyphAppearance);
            Surface[Width - 1, middle].Glyph = RightEndGlyph;
        }
        else
            Surface.Print(0, middle, Text.Align(TextAlignment, Width));

        IsDirty = false;
    }
}
