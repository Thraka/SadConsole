using System.Numerics;
using SadConsole.SerializedTypes;

namespace SadConsole.Debug.MonoGame.ImGuiTypes;

public struct ColoredGlyph
{
    public Vector4 Foreground;
    public Vector4 Background;
    public int Glyph;

    public static implicit operator ColoredGlyph(SadConsole.ColoredGlyph cell) => new ColoredGlyph()
    {
        Foreground = cell.Foreground.ToVector4(),
        Background = cell.Background.ToVector4(),
        Glyph = cell.Glyph,
        //IsVisible = cell.IsVisible,
        //Mirror = cell.Mirror,
        //Decorators = cell.Decorators
    };
}
