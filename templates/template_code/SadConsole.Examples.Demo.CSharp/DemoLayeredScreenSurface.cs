using SadConsole.Components;

namespace SadConsole.Examples;

internal class DemoLayeredSurface : IDemo
{
    public string Title => "Layered Surface";

    public string Description => "This demo uses [c:r f:yellow]LayeredScreenSurface[c:u] type to present multiple layers of surface data.\nEach layer is the entire size of the screen, but only a small portion of each layer has a colored box, to demonstrate transparency.";

    public string CodeFile => "DemoLayeredScreenSurface.cs";

    public IScreenSurface CreateDemoScreen() =>
        new LayeredScreenSurfaceObject();

    public override string ToString() =>
        Title;
}

class LayeredScreenSurfaceObject : LayeredScreenSurface
{
    Instructions.DrawString typingInstruction;

    public LayeredScreenSurfaceObject() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Layers.Create().Fill(new Rectangle(0, 0, 7, 7), background: Color.Blue);
        Layers.Create().Fill(new Rectangle(4, 4, 7, 7), background: Color.Red);
        Layers.Create().Fill(new Rectangle(8, 8, 7, 7), background: Color.Purple);
        Layers.Last().Print(9, 10, "Hello");
    }
}
