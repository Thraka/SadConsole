#if SFML
namespace SadConsole.Examples;

internal class DemoFontManipulation : IDemo
{
    public string Title => "Font manipulation";

    public string Description => "This demo is only supported on MonoGame";

    public string CodeFile => "DemoFontManipulation.cs";

    public IScreenSurface CreateDemoScreen() =>
        new FontEditingScreen();

    public override string ToString() =>
        Title;
}

internal class FontEditingScreen : ScreenSurface
{
    public FontEditingScreen() : base(GameSettings.ScreenDemoBounds.Height, GameSettings.ScreenDemoBounds.Height / 2)
    {

    }
}
#endif
