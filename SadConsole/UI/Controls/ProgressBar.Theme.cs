using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// The theme of a radio button 
/// </summary>
public partial class ProgressBar
{
    /// <summary>
    /// The theme of the unprogressed part of the bar.
    /// </summary>
    [DataMember]
    public ThemeStates ThemeStateBackground { get; protected set; }

    /// <summary>
    /// The theme of the progressed part of the bar.
    /// </summary>
    [DataMember]
    public ThemeStates ThemeStateForeground { get; protected set; }

    /// <summary>
    /// The theme of the text displayed on the bar.
    /// </summary>
    [DataMember]
    public ThemeStates DisplayTextStates { get; protected set; }

    /// <summary>
    /// When <see langword="true"/>, prints the <see cref="Label.DisplayText"/> on the control in decorators instead of replacing the portation of the bar that overlaps the text.
    /// </summary>
    [DataMember]
    public bool PrintDisplayAsDecorator { get; set; }

    /// <inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        ThemeStateBackground.RefreshTheme(colors);
        ThemeStateBackground.SetForeground(ThemeStateBackground.Normal.Foreground);
        ThemeStateBackground.SetBackground(ThemeStateBackground.Normal.Background);
        ThemeStateBackground.Disabled = new ColoredGlyph(Color.Gray, Color.Black, 176);
        ThemeStateForeground.RefreshTheme(colors);
        ThemeStateForeground.SetForeground(ThemeStateForeground.Normal.Foreground);
        ThemeStateForeground.SetBackground(ThemeStateForeground.Normal.Background);
        ThemeStateForeground.Disabled = new ColoredGlyph(Color.Gray, Color.Black, 219);
        DisplayTextStates.RefreshTheme(colors);
        DisplayTextStates.SetForeground(DisplayTextColor);
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        RefreshThemeStateColors(FindThemeColors());

        ColoredGlyph foregroundAppearance = ThemeStateForeground.GetStateAppearance(State);
        ColoredGlyph backgroundAppearance = ThemeStateBackground.GetStateAppearance(State);
        ColoredGlyph displayTextAppearance = DisplayTextStates.GetStateAppearance(State);

        Surface.Fill(backgroundAppearance.Foreground, backgroundAppearance.Background, backgroundAppearance.Glyph);

        if (IsHorizontal)
        {
            Rectangle fillRect;

            if (HorizontalAlignment == HorizontalAlignment.Left)
                fillRect = new Rectangle(0, 0, fillSize, Height);
            else
                fillRect = new Rectangle(Width - fillSize, 0, fillSize, Height);

            Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, foregroundAppearance.Glyph);

            if (DisplayTextColor.A != 0 && !string.IsNullOrEmpty(DisplayText))
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

            Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, foregroundAppearance.Glyph);
        }

        IsDirty = false;
    }
}
