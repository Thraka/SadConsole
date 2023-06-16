using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// A button control that is drawn with a shadow.
/// </summary>
[DataContract]
public class Button3d : ButtonBase
{
    /// <summary>
    /// Creates an instance of the button control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control (default is 1).</param>
    public Button3d(int width, int height)
        : base(width, height)
    {
        
    }

    ///<inheritdoc/>
    protected override ICellSurface CreateControlSurface()
    {
        Surface = new CellSurface(Width + 2, Height + 1)
        {
            DefaultBackground = Color.Transparent
        };
        Surface.Clear();
        return Surface;
    }

    ///<inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        ThemeState.Normal.Background = colors.GetOffColor(ThemeState.Normal.Background, colors.ControlHostBackground);
        ThemeState.MouseOver.Background = colors.GetOffColor(ThemeState.MouseOver.Background, colors.ControlHostBackground);
        ThemeState.MouseDown.Background = colors.GetOffColor(ThemeState.MouseDown.Background, colors.ControlHostBackground);
        ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, colors.ControlHostBackground);
    }

    ///<inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Colors colors = FindThemeColors();

        // Refresh any theme state colors
        RefreshThemeStateColors(colors);

        ColoredGlyph appearance = ThemeState.GetStateAppearance(State);
        ColoredGlyph shade = new ColoredGlyph(colors.ControlForegroundNormal, colors.ControlBackgroundNormal, 176);

        // Start drawing the control
        int middle = Height != 1 ? Height / 2 : 0;

        Rectangle shadowBounds = new Rectangle(0, 0, Width, Height).WithPosition((2, 1));

        Surface.Clear();

        if (appearance.Matches(ThemeState.MouseDown))
        {
            middle += 1;

            // Redraw the control
            Surface.Fill(shadowBounds,
                appearance.Foreground,
                appearance.Background,
                appearance.Glyph, null);

            Surface.Print(shadowBounds.X, middle, Text.Align(TextAlignment, Width));
            MouseArea = new Rectangle(0, 0, Width + 2, Height + 1);
        }
        else
        {
            // Redraw the control
            Surface.Fill(new Rectangle(0, 0, Width, Height),
                appearance.Foreground,
                appearance.Background,
                appearance.Glyph, null);

            Surface.Print(0, middle, Text.Align(TextAlignment, Width));

            // Bottom line
            Surface.DrawLine(new Point(shadowBounds.X, shadowBounds.MaxExtentY),
                new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), shade.Glyph, shade.Foreground, shade.Background);

            // Side line 1
            Surface.DrawLine(new Point(shadowBounds.MaxExtentX - 1, shadowBounds.Y),
                new Point(shadowBounds.MaxExtentX - 1, shadowBounds.MaxExtentY), shade.Glyph, shade.Foreground, shade.Background);

            // Side line 2
            Surface.DrawLine(new Point(shadowBounds.MaxExtentX, shadowBounds.Y),
                new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), shade.Glyph, shade.Foreground, shade.Background);

            MouseArea = new Rectangle(0, 0, Width, Height);
        }

        IsDirty = false;
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        DetermineState();
        IsDirty = true;
    }
}
