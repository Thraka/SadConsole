using System.Collections.ObjectModel;
using SadConsole.Ansi;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.Readers;
using SadConsole.Renderers;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.Entities;
using static SadConsole.Examples.RootScreen;

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

internal class Playground : Console
{
    public Playground() : base(80, 25)
    {
    }

    protected override void OnMouseLeftClicked(MouseScreenObjectState state)
    {
        Console surface = new(80, 25, 200, 200);
        surface.FillWithRandomGarbage(255);
        surface.UseMouse = false;
        Children.Add(surface);


    }
}
