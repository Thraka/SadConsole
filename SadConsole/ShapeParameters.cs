using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Defines the parameters for generating a shape with a border and fill glyphs.
/// </summary>
public class ShapeParameters
{
    /// <summary>
    /// When true, to indicate the shape contains a border.
    /// </summary>
    public bool HasBorder { get; set; }

    /// <summary>
    /// When true, indicates this shape is filled.
    /// </summary>
    public bool HasFill { get; set; }

    /// <summary>
    /// When true, ignores the fill foreground color.
    /// </summary>
    public bool IgnoreFillForeground { get; set; }

    /// <summary>
    /// When true, ignores the fill background color.
    /// </summary>
    public bool IgnoreFillBackground { get; set; }

    /// <summary>
    /// When true, ignores the fill glyph.
    /// </summary>
    public bool IgnoreFillGlyph { get; set; }

    /// <summary>
    /// When true, ignores the fill mirror.
    /// </summary>
    public bool IgnoreFillMirror { get; set; }

    /// <summary>
    /// When true, ignores the border foreground color.
    /// </summary>
    public bool IgnoreBorderForeground { get; set; }

    /// <summary>
    /// When true, ignores the border background color.
    /// </summary>
    public bool IgnoreBorderBackground { get; set; }

    /// <summary>
    /// When true, ignores the border glyph.
    /// </summary>
    public bool IgnoreBorderGlyph { get; set; }

    /// <summary>
    /// When true, ignores the border mirror.
    /// </summary>
    public bool IgnoreBorderMirror { get; set; }

    /// <summary>
    /// The fill appearance.
    /// </summary>
    public ColoredGlyphBase? FillGlyph { get; set; }

    /// <summary>
    /// The connected lines used for the border.
    /// </summary>
    public int[]? BoxBorderStyle { get; set; }

    /// <summary>
    /// The appearances used for each part of the connected line.
    /// </summary>
    public ColoredGlyphBase[]? BoxBorderStyleGlyphs { get; set; }

    /// <summary>
    /// A single glyph used for drawing the border.
    /// </summary>
    public ColoredGlyphBase? BorderGlyph { get; set; }

    /// <summary>
    /// A set of parameters that defines how a shape should be drawn.
    /// </summary>
    /// <param name="hasBorder">When true, indicates the shape has a border.</param>
    /// <param name="borderGlyph">When not null, uses a single glyph to draw the border.</param>
    /// <param name="ignoreBorderForeground">When true, ignores the border foreground color.</param>
    /// <param name="ignoreBorderBackground">When true, ignores the border background color.</param>
    /// <param name="ignoreBorderGlyph">When true, ignores the border glyph.</param>
    /// <param name="ignoreBorderMirror">When true, ignores the border mirror.</param>
    /// <param name="hasFill">When true, indicates this shape is filled.</param>
    /// <param name="fillGlyph">When not null, this is the appearance of the fill.</param>
    /// <param name="ignoreFillForeground">When true, ignores the fill foreground color.</param>
    /// <param name="ignoreFillBackground">When true, ignores the fill background color.</param>
    /// <param name="ignoreFillGlyph">When true, ignores the fill glyph.</param>
    /// <param name="ignoreFillMirror">When true, ignores the fill mirror.</param>
    /// <param name="boxBorderStyle">When not null, the connected lines used for the border.</param>
    /// <param name="boxBorderStyleGlyphs">When not null, the appearances used for each part of the connected line.</param>
    public ShapeParameters(bool hasBorder, ColoredGlyphBase? borderGlyph, bool ignoreBorderForeground, bool ignoreBorderBackground, bool ignoreBorderGlyph, bool ignoreBorderMirror,
                           bool hasFill, ColoredGlyphBase? fillGlyph, bool ignoreFillForeground, bool ignoreFillBackground, bool ignoreFillGlyph, bool ignoreFillMirror,
                           int[]? boxBorderStyle, ColoredGlyphBase[]? boxBorderStyleGlyphs)
    {
        HasBorder = hasBorder;
        BorderGlyph = borderGlyph;
        IgnoreBorderForeground = ignoreBorderForeground;
        IgnoreBorderBackground = ignoreBorderBackground;
        IgnoreBorderGlyph = ignoreBorderGlyph;
        IgnoreBorderMirror = ignoreBorderMirror;
        HasFill = hasFill;
        FillGlyph = fillGlyph;
        IgnoreFillForeground = ignoreFillForeground;
        IgnoreFillBackground = ignoreFillBackground;
        IgnoreFillGlyph = ignoreFillGlyph;
        IgnoreFillMirror = ignoreFillMirror;
        BoxBorderStyle = boxBorderStyle;
        BoxBorderStyleGlyphs = boxBorderStyleGlyphs;
    }

    /// <summary>
    /// Creates a shape parameters object that describes a border.
    /// </summary>
    /// <param name="borderStyle">The appearance of the border.</param>
    /// <param name="ignoreForeground">When true, ignores the foreground of the <paramref name="borderStyle"/> value.</param>
    /// <param name="ignoreBackground">When true, ignores the background of the <paramref name="borderStyle"/> value.</param>
    /// <param name="ignoreGlyph">When true, ignores the glyph of the <paramref name="borderStyle"/> value.</param>
    /// <param name="ignoreMirror">When true, ignores the mirror of the <paramref name="borderStyle"/> value.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateBorder(ColoredGlyphBase borderStyle,
                                               bool ignoreForeground = false, bool ignoreBackground = false, bool ignoreGlyph = false, bool ignoreMirror = false) =>
        new(true, borderStyle, ignoreForeground, ignoreBackground, ignoreGlyph, ignoreMirror, false, null, false, false, false, false, null, null);

    /// <summary>
    /// Creates a shape parameters object that describes a filled object with an optional border.
    /// </summary>
    /// <param name="borderStyle">When not null, creates a border with this appearance.</param>
    /// <param name="fillStyle">The fill appearance.</param>
    /// <param name="ignoreBorderForeground">When true, ignores the foreground of the <paramref name="borderStyle"/> value.</param>
    /// <param name="ignoreBorderBackground">When true, ignores the background of the <paramref name="borderStyle"/> value.</param>
    /// <param name="ignoreBorderGlyph">When true, ignores the glyph of the <paramref name="borderStyle"/> value.</param>
    /// <param name="ignoreBorderMirror">When true, ignores the mirror of the <paramref name="borderStyle"/> value.</param>
    /// <param name="ignoreFillForeground">When true, ignores the foreground of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillBackground">When true, ignores the background of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillGlyph">When true, ignores the glyph of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillMirror">When true, ignores the mirror of the <paramref name="fillStyle"/> value.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateFilled(ColoredGlyphBase? borderStyle, ColoredGlyphBase fillStyle,
                                               bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderGlyph = false, bool ignoreBorderMirror = false,
                                               bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new(borderStyle != null, borderStyle, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderGlyph, ignoreBorderMirror, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, null, null);

    /// <summary>
    /// Creates a shape parameters object that describes a border using a connected line glyph set.
    /// </summary>
    /// <param name="borderStyle">The connected line glyphs that make up the border.</param>
    /// <param name="borderColors">The appearance colors of the border.</param>
    /// <param name="ignoreBorderForeground">When true, ignores the foreground of the <paramref name="borderColors"/> value.</param>
    /// <param name="ignoreBorderBackground">When true, ignores the background of the <paramref name="borderColors"/> value.</param>
    /// <param name="ignoreBorderMirror">When true, ignores the mirror of the <paramref name="borderColors"/> value.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateStyledBox(int[] borderStyle, ColoredGlyphBase borderColors,
                                                  bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = false) =>
        new(true, borderColors, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, false, null, false, false, false, false, borderStyle, null);

    /// <summary>
    /// Creates a shape parameters object that describes a filled box using a connected line line glyph set for the border.
    /// </summary>
    /// <param name="borderStyle">The connected line glyphs that make up the border.</param>
    /// <param name="borderColors">The appearance colors of the border.</param>
    /// <param name="fillStyle">The appearance colors of the fill.</param>
    /// <param name="ignoreBorderForeground">When true, ignores the foreground of the <paramref name="borderColors"/> value.</param>
    /// <param name="ignoreBorderBackground">When true, ignores the background of the <paramref name="borderColors"/> value.</param>
    /// <param name="ignoreBorderMirror">When true, ignores the mirror of the <paramref name="borderColors"/> value.</param>
    /// <param name="ignoreFillForeground">When true, ignores the foreground of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillBackground">When true, ignores the background of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillGlyph">When true, ignores the glyph of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillMirror">When true, ignores the mirror of the <paramref name="fillStyle"/> value.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateStyledBoxFilled(int[] borderStyle, ColoredGlyphBase borderColors, ColoredGlyphBase fillStyle,
                                                        bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = false,
                                                        bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new(true, borderColors, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, borderStyle, null);

    /// <summary>
    /// Creates a shape parameters object that describes an unfilled box using a connected line line glyph set for the border.
    /// </summary>
    /// <param name="borderGlyphs">The appearances used for each part of a connected line, specifically the glyph.</param>
    /// <param name="ignoreBorderForeground">When true, ignores the foreground of the <paramref name="borderGlyphs"/> value.</param>
    /// <param name="ignoreBorderBackground">When true, ignores the background of the <paramref name="borderGlyphs"/> value.</param>
    /// <param name="ignoreBorderMirror">When true, ignores the mirror of the <paramref name="borderGlyphs"/> value.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateStyledBoxExplicit(ColoredGlyphBase[] borderGlyphs,
                                                          bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = true) =>
        new(true, null, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, false, null, false, false, false, false, null, borderGlyphs);

    /// <summary>
    /// Creates a shape parameters object that describes a filled box using a connected line line glyph set for the border.
    /// </summary>
    /// <param name="borderGlyphs">The appearances used for each part of a connected line, specifically the glyph.</param>
    /// <param name="fillStyle">The appearance colors of the fill.</param>
    /// <param name="ignoreBorderForeground">When true, ignores the foreground of the <paramref name="borderGlyphs"/> value.</param>
    /// <param name="ignoreBorderBackground">When true, ignores the background of the <paramref name="borderGlyphs"/> value.</param>
    /// <param name="ignoreBorderGlyph">When true, ignores the glyph of the <paramref name="borderGlyphs"/> value.</param>
    /// <param name="ignoreBorderMirror">When true, ignores the mirror of the <paramref name="borderGlyphs"/> value.</param>
    /// <param name="ignoreFillForeground">When true, ignores the foreground of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillBackground">When true, ignores the background of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillGlyph">When true, ignores the glyph of the <paramref name="fillStyle"/> value.</param>
    /// <param name="ignoreFillMirror">When true, ignores the mirror of the <paramref name="fillStyle"/> value.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateStyledBoxExplicitFilled(ColoredGlyphBase[] borderGlyphs, ColoredGlyphBase fillStyle,
                                                                bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderGlyph = false, bool ignoreBorderMirror = true,
                                                                bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new(true, null, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderGlyph, ignoreBorderMirror, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, null, borderGlyphs);

    /// <summary>
    /// Creates a box using the <see cref="ICellSurface.ConnectedLineThin"/> connected line style.
    /// </summary>
    /// <param name="foreground">The foreground color of the box.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateStyledBoxThin(Color foreground) =>
        new(true, new ColoredGlyph(foreground), false, true, false, false, false, null, false, false, false, false, ICellSurface.ConnectedLineThin, null);

    /// <summary>
    /// Creates a box using the <see cref="ICellSurface.ConnectedLineThick"/> connected line style.
    /// </summary>
    /// <param name="foreground">The foreground color of the box.</param>
    /// <returns>The shape parameters.</returns>
    public static ShapeParameters CreateStyledBoxThick(Color foreground) =>
        new(true, new ColoredGlyph(foreground), false, true, false, false, false, null, false, false, false, false, ICellSurface.ConnectedLineThick, null);
}
