using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoPlayground : IDemo
{
    public string Title => "Playground";

    public string Description => "This demo exists as a place to mess with SadConsole features. By default, it's empty, but add code to the [c:r f:yellow:w]Playground class hosted in this code file.";

    public string CodeFile => "DemoPlayground.cs";

    public IScreenSurface CreateDemoScreen() =>
        new Playground();

    public override string ToString() =>
        Title;
}

internal class Playground : ScreenSurface
{
    public Playground() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
    }
}
