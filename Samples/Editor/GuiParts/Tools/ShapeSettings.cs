using System.Numerics;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts.Tools;

internal static class ShapeSettings
{
    public struct Settings
    {
        // Stolen from ShapreParameters and converted to fields
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

        public ShapeParameters ToShapeParameters() =>
            new ShapeParameters(HasBorder, BorderGlyph, IgnoreBorderForeground, IgnoreBorderBackground, IgnoreBorderGlyph, IgnoreBorderMirror,
                                HasFill, FillGlyph, IgnoreFillForeground, IgnoreFillBackground, IgnoreFillGlyph, IgnoreFillMirror,
                                BoxBorderStyle, BoxBorderStyleGlyphs);
    }
}
