using System;
using SadConsole;
using SadRogue.Primitives;

namespace SadConsole.Examples;

internal class DemoRandomScrolling : IDemo
{
    public string Title => "Large scrollable surface";

    public string Description => "A large surface with random data that can be scrolled around. Use the [c:r f:Red]arrow[c:u] keys to move the view.";

    public string CodeFile => "DemoRandomScrollingConsole.cs";

    public IScreenSurface CreateDemoScreen() =>
        new RandomScrollingConsole();

    public override string ToString() =>
        Title;
}

internal class RandomScrollingConsole : Console
{
    private ICellSurface? mainData;
    private bool initialized;
    private bool firstPauseDone;

    public RandomScrollingConsole() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Surface.Print(0, 0, "Generating random console data, please wait...");
        Renderer = new Renderers.OptimizedScreenSurfaceRenderer();
    }

    /// <inheritdoc />
    public override void Render(TimeSpan delta)
    {
        base.Render(delta);

        if (!firstPauseDone)
            firstPauseDone = true;

        else if (!initialized)
        {
            // Generate the content
            mainData = new CellSurface(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height, 2000, 2000);
            SadComponents.Add(new Components.MoveViewPortKeyboardHandler());
            mainData.FillWithRandomGarbage(Font);
            Surface = mainData;

            // We need to set cell data to the big console data so we can use the FillWithRandom method.
            initialized = true;
        }
    }
}
