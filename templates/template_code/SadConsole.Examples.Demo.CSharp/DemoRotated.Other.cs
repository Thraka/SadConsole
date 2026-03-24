#if !MONOGAME
namespace SadConsole.Examples;

internal class DemoRotatedSurface : IDemo
{
    public string Title => "Rotated Surface";

    public string Description => "This demo is only supported on MonoGame";

    public string CodeFile => "DemoRotated.Other.cs";

    public IScreenSurface CreateDemoScreen() =>
        new RotatedSurface();

    public override string ToString() =>
        Title;
}


class RotatedSurface : ScreenSurface
{
    public RotatedSurface() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height / 2)
    {
        Surface.Print(2, 1, "Not supported in SFML, use a different host");
    }
}
#endif
