using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class Label : ControlBase
{
    /// <summary>
    /// When true, only uses <see cref="ThemeStates.Normal"/> for drawing.
    /// </summary>
    [DataMember]
    public bool UseNormalStateOnly { get; set; } = true;

    /// <summary>
    /// The decorator to use when the <see cref="Controls.ShowUnderline"/> is <see langword="true"/>.
    /// </summary>
    [DataMember]
    public CellDecorator DecoratorUnderline { get; set; }

    /// <summary>
    /// The decorator to use when the <see cref="Controls.ShowStrikethrough"/> is <see langword="true"/>.
    /// </summary>
    [DataMember]
    public CellDecorator DecoratorStrikethrough { get; set; }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        ColoredGlyph appearance;

        if (!UseNormalStateOnly)
            appearance = ThemeState.GetStateAppearance(State);
        else
            appearance = ThemeState.Normal;

        Surface.Fill(TextColor ?? appearance.Foreground, appearance.Background, 0);
        Surface.Print(0, 0, DisplayText.Align(Alignment, Surface.Width));
        IFont? font = AlternateFont ?? Parent?.Host?.ParentConsole?.Font;
        Color color = TextColor ?? appearance.Foreground;

        if (font != null)
        {
            if (ShowUnderline && ShowStrikethrough)
                Surface.SetDecorator(0, Surface.Width, GetStrikethrough(font, color), GetUnderline(font, color));
            else if (ShowUnderline)
                Surface.SetDecorator(0, Surface.Width, GetUnderline(font, color));
            else if (ShowStrikethrough)
                Surface.SetDecorator(0, Surface.Width, GetStrikethrough(font, color));
        }

        IsDirty = false;
    }

    /// <summary>
    /// Gets the strikethrough glyph defined by a font. If not found, returns glyph 196.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <param name="color">The color to shade the decorator.</param>
    /// <returns>The cell decorator.</returns>
    protected CellDecorator GetStrikethrough(IFont font, Color color)
    {
        if (DecoratorStrikethrough != CellDecorator.Empty)
            return DecoratorStrikethrough;

        if (font.HasGlyphDefinition("strikethrough"))
            return font.GetDecorator("strikethrough", color);

        return new CellDecorator(color, 196, Mirror.None);
    }

    /// <summary>
    /// Gets the underline glyph defined by a font. If not found, returns glyph 95.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <param name="color">The color to shade the decorator.</param>
    /// <returns>The cell decorator.</returns>
    protected CellDecorator GetUnderline(IFont font, Color color)
    {
        if (DecoratorUnderline != CellDecorator.Empty)
            return DecoratorUnderline;

        if (font.HasGlyphDefinition("underline"))
            return font.GetDecorator("underline", color);

        return new CellDecorator(color, 95, Mirror.None);
    }
}
