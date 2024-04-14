#if FNA
namespace SadConsole.Examples;

internal class DemoShader : IDemo
{
    public string Title => "Using Shaders";

    public string Description => "This demo provides the option to apply a shader to a specific surface or all " +
                                 "of SadConsole.\r\n";

    public string CodeFile => "DemoShader.FNA.cs";

    public IScreenSurface CreateDemoScreen() =>
        new ShaderController();

    public override string ToString() =>
        Title;
}


class ShaderController : ScreenSurface
{
    public ShaderController() : base(GameSettings.ScreenDemoBounds.Height, GameSettings.ScreenDemoBounds.Height / 2)
    {
        Surface.Print(0, 0, "Not supported in FNA");
    }
}
#endif
