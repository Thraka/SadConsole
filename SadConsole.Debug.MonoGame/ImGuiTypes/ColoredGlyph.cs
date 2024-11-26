using System.Numerics;

namespace SadConsole.Debug;

public struct ColoredGlyph
{
    public Vector4 Foreground;
    public Vector4 Background;
    public int Glyph;

    public static implicit operator ColoredGlyph(SadConsole.ColoredGlyphBase cell) => new ColoredGlyph()
    {
        Foreground = cell.Foreground.ToVector4(),
        Background = cell.Background.ToVector4(),
        Glyph = cell.Glyph,
        //IsVisible = cell.IsVisible,
        //Mirror = cell.Mirror,
        //Decorators = cell.Decorators
    };
}
