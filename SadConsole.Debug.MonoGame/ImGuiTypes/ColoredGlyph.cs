using System.Numerics;

namespace SadConsole.Debug;

public struct ColoredGlyphReference
{
    public Vector4 Foreground;
    public Vector4 Background;
    public int Glyph;
    public bool IsVisible;
    public Mirror Mirror;

    public static implicit operator ColoredGlyphReference(SadConsole.ColoredGlyphBase cell) => new ColoredGlyphReference()
    {
        Foreground = cell.Foreground.ToVector4(),
        Background = cell.Background.ToVector4(),
        Glyph = cell.Glyph,
        IsVisible = cell.IsVisible,
        Mirror = cell.Mirror,
        //Decorators = cell.Decorators
    };

    public static bool operator ==(ColoredGlyphReference left, ColoredGlyphBase right)
    {
        return left.Foreground == right.Foreground.ToVector4() &&
               left.Background == right.Background.ToVector4() &&
               left.Glyph == right.Glyph &&
               left.IsVisible == right.IsVisible &&
               left.Mirror == right.Mirror;
    }

    public static bool operator !=(ColoredGlyphReference left, ColoredGlyphBase right)
    {
        return !(left == right);
    }

    public static bool operator ==(ColoredGlyphBase left, ColoredGlyphReference right)
    {
        return right == left;
    }

    public static bool operator !=(ColoredGlyphBase left, ColoredGlyphReference right)
    {
        return right != left;
    }
}
