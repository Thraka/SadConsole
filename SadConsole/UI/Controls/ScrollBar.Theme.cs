using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class ScrollBar
{
    /// <summary>
    /// When <see langword="true"/>, indicates that the start and end glyph buttons sohuld use the extended SadConsole font characters if available.
    /// </summary>
    /// <remarks>
    /// This only affects scrollbars that have a width of two when vertical, and a height of two when horizontal.
    /// </remarks>
    [DataMember]
    public bool UseExtended { get; set; }

    /// <summary>
    /// The theme part fot the start button.
    /// </summary>
    [DataMember]
    public int StartButtonVerticalGlyph;

    /// <summary>
    /// The theme part fot the start button.
    /// </summary>
    [DataMember]
    public int EndButtonVerticalGlyph;

    /// <summary>
    /// The theme part fot the start button.
    /// </summary>
    [DataMember]
    public int StartButtonHorizontalGlyph;

    /// <summary>
    /// The theme part fot the start button.
    /// </summary>
    [DataMember]
    public int EndButtonHorizontalGlyph;

    /// <summary>
    /// The theme part for the scroll bar bar where the slider is not located.
    /// </summary>
    [DataMember]
    public int BarGlyph;

    /// <summary>
    /// The theme part for the scroll bar icon.
    /// </summary>
    [DataMember]
    public int SliderGlyph;

    /// <inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        ThemeState.SetForeground(ThemeState.Normal.Foreground);
        ThemeState.SetBackground(ThemeState.Normal.Background);

        ThemeState.Disabled = colors.Appearance_ControlDisabled.Clone();
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Colors currentColors = FindThemeColors();

        RefreshThemeStateColors(currentColors);

        ColoredGlyph appearance = ThemeState.GetStateAppearance(State);

        Surface.Clear();

        IFont? font = AlternateFont ?? Parent?.Host?.ParentConsole?.Font;

        if (font != null)
        {
            if (Orientation == Orientation.Horizontal)
            {
                // Handle the arrows
                if (font.IsSadExtended && UseExtended && Height == 2)
                {
                    GlyphDefinition glyph = font.GetGlyphDefinition("ui-arrow-left+top");
                    Surface.SetGlyph(0, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-left+bottom");
                    Surface.SetGlyph(0, 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);

                    glyph = font.GetGlyphDefinition("ui-arrow-right+top");
                    Surface.SetGlyph(Width - 1, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-right+bottom");
                    Surface.SetGlyph(Width - 1, 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                }
                else
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Surface.SetCellAppearance(0, y, appearance);
                        Surface.SetGlyph(0, y, StartButtonHorizontalGlyph);

                        Surface.SetCellAppearance(Width - 1, y, appearance);
                        Surface.SetGlyph(Width - 1, y, EndButtonHorizontalGlyph);
                    }
                }

                if (SliderBarSize != 0)
                {
                    for (int i = 1; i <= SliderBarSize; i++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            Surface.SetCellAppearance(i, y, appearance);
                            Surface.SetGlyph(i, y, BarGlyph);
                        }
                    }

                    if (IsEnabled)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            Surface.SetCellAppearance(1 + CurrentSliderPosition, y, appearance);
                            Surface.SetGlyph(1 + CurrentSliderPosition, y, SliderGlyph);
                        }
                    }
                }
            }
            else
            {
                // Handle the arrows
                if (font.IsSadExtended && UseExtended && Width == 2)
                {
                    GlyphDefinition glyph = font.GetGlyphDefinition("ui-arrow-up+left");
                    Surface.SetGlyph(0, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-up+right");
                    Surface.SetGlyph(1, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);

                    glyph = font.GetGlyphDefinition("ui-arrow-down+left");
                    Surface.SetGlyph(0, Height - 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-down+right");
                    Surface.SetGlyph(1, Height - 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                }
                else
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Surface.SetCellAppearance(x, 0, appearance);
                        Surface.SetGlyph(x, 0, StartButtonVerticalGlyph);

                        Surface.SetCellAppearance(x, Height - 1, appearance);
                        Surface.SetGlyph(x, Height - 1, EndButtonVerticalGlyph);
                    }
                }

                if (SliderBarSize != 0)
                {
                    for (int i = 0; i < SliderBarSize; i++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            Surface.SetCellAppearance(x, i + 1, appearance);
                            Surface.SetGlyph(x, i + 1, BarGlyph);
                        }
                    }

                    if (IsEnabled)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            Surface.SetCellAppearance(x, 1 + CurrentSliderPosition, appearance);
                            Surface.SetGlyph(x, 1 + CurrentSliderPosition, SliderGlyph);
                        }
                    }
                }
            }

            if (IsSliding)
                MouseArea = new Rectangle(-2, -2, Width + 4, Height + 4);
            else
                MouseArea = new Rectangle(0, 0, Width, Height);
        }

        IsDirty = false;
    }
}
