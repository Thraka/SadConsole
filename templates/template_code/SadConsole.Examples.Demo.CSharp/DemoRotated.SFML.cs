#if SFML
namespace SadConsole.Examples;

internal class DemoRotatedSurface : IDemo
{
    public string Title => "Rotated Surface";

    public string Description => "This demo is only supported on MonoGame";

    public string CodeFile => "DemoShapes.SFML.cs";

    public IScreenSurface CreateDemoScreen() =>
        new RotatedSurface();

    public override string ToString() =>
        Title;
}


class RotatedSurface : ScreenSurface
{
    public RotatedSurface() : base(GameSettings.ScreenDemoBounds.Height, GameSettings.ScreenDemoBounds.Height / 2)
    {

    }
}
#endif
