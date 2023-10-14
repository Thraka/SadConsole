using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Defines the parameters for generating a shape with a border and fill glyphs.
/// </summary>
public class ShapeParameters
{
    public bool HasBorder { get; set; }
    public bool HasFill { get; set; }
    public bool IgnoreFillForeground { get; set; }
    public bool IgnoreFillBackground { get; set; }
    public bool IgnoreFillGlyph { get; set; }
    public bool IgnoreFillMirror { get; set; }
    public bool IgnoreBorderForeground { get; set; }
    public bool IgnoreBorderBackground { get; set; }
    public bool IgnoreBorderGlyph { get; set; }
    public bool IgnoreBorderMirror { get; set; }
    public ColoredGlyphBase? FillGlyph { get; set; }

    public int[]? BoxBorderStyle { get; set; }
    public ColoredGlyphBase[]? BoxBorderStyleGlyphs { get; set; }
    public ColoredGlyphBase? BorderGlyph { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hasBorder"></param>
    /// <param name="borderGlyph"></param>
    /// <param name="ignoreBorderForeground"></param>
    /// <param name="ignoreBorderBackground"></param>
    /// <param name="ignoreBorderGlyph"></param>
    /// <param name="ignoreBorderMirror"></param>
    /// <param name="hasFill"></param>
    /// <param name="fillGlyph"></param>
    /// <param name="ignoreFillForeground"></param>
    /// <param name="ignoreFillBackground"></param>
    /// <param name="ignoreFillGlyph"></param>
    /// <param name="ignoreFillMirror"></param>
    /// <param name="boxBorderStyle"></param>
    /// <param name="boxBorderStyleGlyphs"></param>
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

    public static ShapeParameters CreateBorder(ColoredGlyphBase borderStyle,
                                               bool ignoreForeground = false, bool ignoreBackground = false, bool ignoreGlyph = false, bool ignoreMirror = false) =>
        new ShapeParameters(true, borderStyle, ignoreForeground, ignoreBackground, ignoreGlyph, ignoreMirror, false, null, false, false, false, false, null, null);

    public static ShapeParameters CreateFilled(ColoredGlyphBase borderStyle, ColoredGlyphBase fillStyle,
                                               bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderGlyph = false, bool ignoreBorderMirror = false,
                                               bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new ShapeParameters(borderStyle != null, borderStyle, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderGlyph, ignoreBorderMirror, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, null, null);

    public static ShapeParameters CreateStyledBox(int[] borderStyle, ColoredGlyphBase borderColors,
                                                  bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = false) =>
        new ShapeParameters(true, borderColors, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, false, null, false, false, false, false, borderStyle, null);

    public static ShapeParameters CreateStyledBoxFilled(int[] borderStyle, ColoredGlyphBase borderColors, ColoredGlyphBase fillStyle,
                                                        bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = false,
                                                        bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new ShapeParameters(true, borderColors, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, borderStyle, null);

    public static ShapeParameters CreateStyledBoxExplicit(ColoredGlyphBase[] borderGlyphs,
                                                          bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = true) =>
        new ShapeParameters(true, null, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, false, null, false, false, false, false, null, borderGlyphs);

    public static ShapeParameters CreateStyledBoxExplicitFilled(ColoredGlyphBase[] borderGlyphs, ColoredGlyphBase fillStyle,
                                                                bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderGlyph = false, bool ignoreBorderMirror = true,
                                                                bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new ShapeParameters(true, null, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderGlyph, ignoreBorderMirror, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, null, borderGlyphs);

    public static ShapeParameters CreateStyledBoxThin(Color foreground) =>
        new ShapeParameters(true, new ColoredGlyph(foreground), false, true, false, false, false, null, false, false, false, false, ICellSurface.ConnectedLineThin, null);

    public static ShapeParameters CreateStyledBoxThick(Color foreground) =>
        new ShapeParameters(true, new ColoredGlyph(foreground), false, true, false, false, false, null, false, false, false, false, ICellSurface.ConnectedLineThick, null);
}
