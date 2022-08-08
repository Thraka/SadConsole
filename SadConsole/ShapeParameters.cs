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
    public ColoredGlyph? FillGlyph { get; set; }

    public int[]? BoxBorderStyle { get; set; }
    public ColoredGlyph[]? BoxBorderStyleGlyphs { get; set; }
    public ColoredGlyph? BorderGlyph { get; set; }

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
    public ShapeParameters(bool hasBorder, ColoredGlyph? borderGlyph, bool ignoreBorderForeground, bool ignoreBorderBackground, bool ignoreBorderGlyph, bool ignoreBorderMirror,
                           bool hasFill, ColoredGlyph? fillGlyph, bool ignoreFillForeground, bool ignoreFillBackground, bool ignoreFillGlyph, bool ignoreFillMirror,
                           int[]? boxBorderStyle, ColoredGlyph[]? boxBorderStyleGlyphs)
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

    public static ShapeParameters CreateBorder(ColoredGlyph borderStyle,
                                               bool ignoreForeground = false, bool ignoreBackground = false, bool ignoreGlyph = false, bool ignoreMirror = false) =>
        new ShapeParameters(true, borderStyle, ignoreForeground, ignoreBackground, ignoreGlyph, ignoreMirror, false, null, false, false, false, false, null, null);

    public static ShapeParameters CreateFilled(ColoredGlyph borderStyle, ColoredGlyph fillStyle,
                                               bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderGlyph = false, bool ignoreBorderMirror = false,
                                               bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new ShapeParameters(borderStyle != null, borderStyle, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderGlyph, ignoreBorderMirror, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, null, null);

    public static ShapeParameters CreateStyledBox(int[] borderStyle, ColoredGlyph borderColors,
                                                  bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = false) =>
        new ShapeParameters(true, borderColors, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, false, null, false, false, false, false, borderStyle, null);

    public static ShapeParameters CreateStyledBoxFilled(int[] borderStyle, ColoredGlyph borderColors, ColoredGlyph fillStyle,
                                                        bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = false,
                                                        bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new ShapeParameters(true, borderColors, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, borderStyle, null);

    public static ShapeParameters CreateStyledBoxExplicit(ColoredGlyph[] borderGlyphs,
                                                          bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderMirror = true) =>
        new ShapeParameters(true, null, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderMirror, false, false, null, false, false, false, false, null, borderGlyphs);

    public static ShapeParameters CreateStyledBoxExplicitFilled(ColoredGlyph[] borderGlyphs, ColoredGlyph fillStyle,
                                                                bool ignoreBorderForeground = false, bool ignoreBorderBackground = false, bool ignoreBorderGlyph = false, bool ignoreBorderMirror = true,
                                                                bool ignoreFillForeground = false, bool ignoreFillBackground = false, bool ignoreFillGlyph = false, bool ignoreFillMirror = false) =>
        new ShapeParameters(true, null, ignoreBorderForeground, ignoreBorderBackground, ignoreBorderGlyph, ignoreBorderMirror, true, fillStyle, ignoreFillForeground, ignoreFillBackground, ignoreFillGlyph, ignoreFillMirror, null, borderGlyphs);
}
