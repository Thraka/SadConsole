﻿using System;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// The theme of the slider control.
/// </summary>
[DataContract]
public class ScrollBarTheme : ThemeBase
{
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

    /// <summary>
    /// Creates a new theme used by the <see cref="ScrollBar"/>.
    /// </summary>
    public ScrollBarTheme()
    {
        //TODO add states for ends. Bar should use base state.
        StartButtonVerticalGlyph = 30;
        EndButtonVerticalGlyph = 31;
        StartButtonHorizontalGlyph = 17;
        EndButtonHorizontalGlyph = 16;
        SliderGlyph = 219;
        BarGlyph = 176;
    }

    /// <inheritdoc />
    public override void RefreshTheme(Colors themeColors, ControlBase control)
    {
        base.RefreshTheme(themeColors, control);

        ControlThemeState.SetForeground(ControlThemeState.Normal.Foreground);
        ControlThemeState.SetBackground(ControlThemeState.Normal.Background);

        ControlThemeState.Disabled = _colorsLastUsed.Appearance_ControlDisabled.Clone();
    }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (control is not ScrollBar scrollbar)
        {
            return;
        }

        if (!scrollbar.IsDirty)
        {
            return;
        }

        RefreshTheme(control.FindThemeColors(), control);

        ColoredGlyph appearance = ControlThemeState.GetStateAppearance(scrollbar.State);
        scrollbar.Surface.Clear();

        IFont? font = scrollbar.AlternateFont ?? scrollbar.Parent?.Host?.ParentConsole?.Font;

        if (font != null)
        {
            if (scrollbar.Orientation == Orientation.Horizontal)
            {
                // Handle the arrows
                if (font.IsSadExtended && scrollbar.Height == 2)
                {
                    GlyphDefinition glyph = font.GetGlyphDefinition("ui-arrow-left+top");
                    scrollbar.Surface.SetGlyph(0, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-left+bottom");
                    scrollbar.Surface.SetGlyph(0, 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);

                    glyph = font.GetGlyphDefinition("ui-arrow-right+top");
                    scrollbar.Surface.SetGlyph(scrollbar.Width - 1, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-right+bottom");
                    scrollbar.Surface.SetGlyph(scrollbar.Width - 1, 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                }
                else
                {
                    for (int y = 0; y < scrollbar.Height; y++)
                    {
                        scrollbar.Surface.SetCellAppearance(0, y, appearance);
                        scrollbar.Surface.SetGlyph(0, y, StartButtonHorizontalGlyph);

                        scrollbar.Surface.SetCellAppearance(scrollbar.Width - 1, y, appearance);
                        scrollbar.Surface.SetGlyph(scrollbar.Width - 1, y, EndButtonHorizontalGlyph);
                    }
                }

                if (scrollbar.SliderBarSize != 0)
                {
                    for (int i = 1; i <= scrollbar.SliderBarSize; i++)
                    {
                        for (int y = 0; y < scrollbar.Height; y++)
                        {
                            scrollbar.Surface.SetCellAppearance(i, y, appearance);
                            scrollbar.Surface.SetGlyph(i, y, BarGlyph);
                        }
                    }

                    if (scrollbar.IsEnabled)
                    {
                        for (int y = 0; y < scrollbar.Height; y++)
                        {
                            scrollbar.Surface.SetCellAppearance(1 + scrollbar.CurrentSliderPosition, y, appearance);
                            scrollbar.Surface.SetGlyph(1 + scrollbar.CurrentSliderPosition, y, SliderGlyph);
                        }
                    }
                }
            }
            else
            {
                // Handle the arrows
                if (font.IsSadExtended && scrollbar.Width == 2)
                {
                    GlyphDefinition glyph = font.GetGlyphDefinition("ui-arrow-up+left");
                    scrollbar.Surface.SetGlyph(0, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-up+right");
                    scrollbar.Surface.SetGlyph(1, 0, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);

                    glyph = font.GetGlyphDefinition("ui-arrow-down+left");
                    scrollbar.Surface.SetGlyph(0, scrollbar.Height - 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                    glyph = font.GetGlyphDefinition("ui-arrow-down+right");
                    scrollbar.Surface.SetGlyph(1, scrollbar.Height - 1, glyph.Glyph, appearance.Foreground, appearance.Background, glyph.Mirror);
                }
                else
                {
                    for (int x = 0; x < scrollbar.Width; x++)
                    {
                        scrollbar.Surface.SetCellAppearance(x, 0, appearance);
                        scrollbar.Surface.SetGlyph(x, 0, StartButtonVerticalGlyph);

                        scrollbar.Surface.SetCellAppearance(x, scrollbar.Height - 1, appearance);
                        scrollbar.Surface.SetGlyph(x, scrollbar.Height - 1, EndButtonVerticalGlyph);
                    }
                }

                if (scrollbar.SliderBarSize != 0)
                {
                    for (int i = 0; i < scrollbar.SliderBarSize; i++)
                    {
                        for (int x = 0; x < scrollbar.Width; x++)
                        {
                            scrollbar.Surface.SetCellAppearance(x, i + 1, appearance);
                            scrollbar.Surface.SetGlyph(x, i + 1, BarGlyph);
                        }
                    }

                    if (scrollbar.IsEnabled)
                    {
                        for (int x = 0; x < scrollbar.Width; x++)
                        {
                            scrollbar.Surface.SetCellAppearance(x, 1 + scrollbar.CurrentSliderPosition, appearance);
                            scrollbar.Surface.SetGlyph(x, 1 + scrollbar.CurrentSliderPosition, SliderGlyph);
                        }
                    }
                }
            }

            if (scrollbar.IsSliding)
                scrollbar.MouseArea = new Rectangle(-2, -2, scrollbar.Width + 4, scrollbar.Height + 4);
            else
                scrollbar.MouseArea = new Rectangle(0, 0, scrollbar.Width, scrollbar.Height);
        }

        scrollbar.IsDirty = false;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() => new ScrollBarTheme()
    {
        ControlThemeState = ControlThemeState.Clone(),
        StartButtonVerticalGlyph = StartButtonVerticalGlyph,
        EndButtonVerticalGlyph = EndButtonVerticalGlyph,
        StartButtonHorizontalGlyph = StartButtonHorizontalGlyph,
        EndButtonHorizontalGlyph = EndButtonHorizontalGlyph,
        BarGlyph = BarGlyph,
        SliderGlyph = SliderGlyph
    };
}
