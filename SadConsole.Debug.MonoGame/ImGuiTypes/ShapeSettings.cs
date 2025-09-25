namespace SadConsole.ImGuiTypes;

public struct ShapeSettings
{
    // Stolen from ShapeParameters and converted to fields
    public bool HasBorder;
    public bool HasFill;
    public bool IgnoreFillForeground;
    public bool IgnoreFillBackground;
    public bool IgnoreFillGlyph;
    public bool IgnoreFillMirror;
    public bool IgnoreBorderForeground;
    public bool IgnoreBorderBackground;
    public bool IgnoreBorderGlyph;
    public bool IgnoreBorderMirror;
    public ColoredGlyphBase? FillGlyph;
    public int[]? BoxBorderStyle;
    public ColoredGlyphBase[]? BoxBorderStyleGlyphs;
    public ColoredGlyphBase? BorderGlyph;

    // flags
    public bool UseBoxBorderStyle;

    public ShapeParameters ToShapeParameters()
    {
        if (!HasBorder && HasFill)
        {
            return new ShapeParameters(true, FillGlyph, IgnoreFillForeground, IgnoreFillBackground, IgnoreFillGlyph, IgnoreFillMirror,
                HasFill, FillGlyph, IgnoreFillForeground, IgnoreFillBackground, IgnoreFillGlyph, IgnoreFillMirror,
                ICellSurface.ConnectedLineEmpty, null);
        }

        return new ShapeParameters(HasBorder, BorderGlyph, IgnoreBorderForeground, IgnoreBorderBackground, IgnoreBorderGlyph, IgnoreBorderMirror,
            HasFill, FillGlyph, IgnoreFillForeground, IgnoreFillBackground, IgnoreFillGlyph, IgnoreFillMirror,
            BoxBorderStyle, BoxBorderStyleGlyphs);
    }
}
