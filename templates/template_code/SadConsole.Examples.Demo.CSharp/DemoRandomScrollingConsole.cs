using System;
using Coroutine;
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
    private bool _initialized;
    private Task<CellSurface>? _generateSurfaceTask;
    private bool _taskCreated;

    public RandomScrollingConsole() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        // The main surface has a message to the user that it's busy
        Surface.Print(0, 0, "Generating random console data, please wait...");

        // The surface that will be generated is large, these components enable
        // mouse and keyboard interaction to move the surface view around
        SadComponents.Add(new Components.MouseDragViewPort());
        SadComponents.Add(new Components.MoveViewPortKeyboardHandler());

        // This renderer is a simplified and non-extensible renderer.
        // The default renderer allows components to inject steps into rendering,
        // which is used by the ControlsConsole or EntityManager.
        Renderer = new Renderers.OptimizedScreenSurfaceRenderer();
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (_initialized) return;

        if (!_taskCreated)
        {
            _taskCreated = true;
            _generateSurfaceTask = Task.Factory.StartNew(CreateBigSurface);
        }

        else if (_generateSurfaceTask!.IsCompleted)
        {
            // We need to set cell data to the big console data so we can use the FillWithRandom method.
            Surface = _generateSurfaceTask.Result;
            _initialized = true;
            _generateSurfaceTask = null;
        }
    }

    private CellSurface CreateBigSurface()
    {
        // This may take a second or two on slower machines.
        CellSurface surface = new(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height, 2000, 2000);
        surface.FillWithRandomGarbage(Font);
        return surface;
    }
}
