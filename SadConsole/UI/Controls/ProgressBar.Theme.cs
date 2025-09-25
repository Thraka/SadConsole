using System;
using System.Reflection;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// The theme of a radio button 
/// </summary>
public partial class ProgressBar
{
    Color? _displayTextColor = null;
    Color? _barColor = null;

    /// <summary>
    /// The theme of the progressed part of the bar.
    /// </summary>
    [DataMember]
    public ThemeStates ThemeStateBar { get; protected set; } = new ThemeStates();

    /// <summary>
    /// The theme of the text displayed on the bar.
    /// </summary>
    [DataMember]
    public ThemeStates DisplayTextStates { get; protected set; } = new ThemeStates();

    /// <summary>
    /// The glyph to use when drawing the unfilled part of the bar.
    /// </summary>
    [DataMember]
    public int BackgroundGlyph { get; set; } = 0;

    /// <summary>
    /// The glyph to use when drawing the filled part of the bar.
    /// </summary>
    [DataMember]
    public int BarGlyph { get; set; } = 219;

    /// <summary>
    /// The color to print the <see cref="DisplayText"/> string.
    /// </summary>
    [DataMember]
    public Color? DisplayTextColor
    {
        get => _displayTextColor;
        set { _displayTextColor = value; IsDirty = true; }
    }

    /// <summary>
    /// The color to print the filled part of the progress bar.
    /// </summary>
    [DataMember]
    public Color? BarColor
    {
        get => _barColor;
        set { _barColor = value; IsDirty = true; }
    }

    /// <summary>
    /// When <see langword="true"/>, prints the <see cref="Label.DisplayText"/> on the control in decorators instead of replacing the portation of the bar that overlaps the text.
    /// </summary>
    [DataMember]
    public bool PrintDisplayAsDecorator { get; set; } = true;

    /// <inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);
        DisplayTextStates.RefreshTheme(colors);
        ThemeStateBar.RefreshTheme(colors);

        if (DisplayTextColor != null)
            DisplayTextStates.Normal.Foreground = DisplayTextColor.Value;
        else
            DisplayTextStates.Normal.Foreground = DisplayTextStates.Selected.Foreground;

        if (BarColor != null)
            ThemeStateBar.Normal.Foreground = BarColor.Value;
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        var colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        ColoredGlyphBase backgroundAppearance = ThemeState.GetStateAppearanceNoMouse(State);
        ColoredGlyphBase foregroundAppearance = ThemeStateBar.GetStateAppearanceNoMouse(State);
        ColoredGlyphBase displayTextAppearance = DisplayTextStates.GetStateAppearanceNoMouse(State);

        Surface.Fill(backgroundAppearance.Foreground, backgroundAppearance.Background, BackgroundGlyph);

        if (IsHorizontal)
        {
            Rectangle fillRect;

            if (HorizontalAlignment == HorizontalAlignment.Left)
                fillRect = new Rectangle(0, 0, fillSize, Height);
            else
                fillRect = new Rectangle(Width - fillSize, 0, fillSize, Height);

            Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, BarGlyph);

            if (displayTextAppearance.Foreground.A != 0 && !string.IsNullOrEmpty(DisplayText))
            {
                string alignedString;
                if (DisplayText == "%")
                    alignedString = $"{(int)(Progress * 100)}%".Align(DisplayTextAlignment, Width);
                else
                    alignedString = DisplayText.Align(DisplayTextAlignment, Width);

                int centerRow = Surface.Height / 2;

                for (int i = 0; i < alignedString.Length; i++)
                {
                    if (alignedString[i] != ' ')
                    {
                        if (PrintDisplayAsDecorator)
                            Surface.AddDecorator(i, centerRow, 1, new CellDecorator(displayTextAppearance.Foreground, alignedString[i], Mirror.None));
                        else
                            Surface.SetGlyph(i, centerRow, alignedString[i], displayTextAppearance.Foreground);
                    }
                }
            }
        }

        else
        {
            Rectangle fillRect;

            if (VerticalAlignment == VerticalAlignment.Top)
                fillRect = new Rectangle(0, 0, Width, fillSize);
            else
                fillRect = new Rectangle(0, Height - fillSize, Width, fillSize);

            Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, BarGlyph);
        }
    }
}
