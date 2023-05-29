namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="UI.Window"/> object by adding a <see cref="WindowRenderStep"/> to the <see cref="ScreenSurfaceRenderer.Steps"/> collection.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Window")]
public sealed class WindowRenderer : ScreenSurfaceRenderer
{
    ///  <inheritdoc/>
    protected override void AddDefaultSteps()
    {
        Steps.Add(new WindowRenderStep());
        base.AddDefaultSteps();
    }
}
