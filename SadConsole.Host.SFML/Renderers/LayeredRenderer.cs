namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="UI.Window"/> object by adding a <see cref="WindowRenderStep"/> to the <see cref="ScreenSurfaceRenderer.Steps"/> collection.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Window")]
public sealed class LayeredRenderer : ScreenSurfaceRenderer
{
    ///  <inheritdoc/>
    protected override void AddDefaultSteps()
    {
        Steps.Add(new LayeredSurfaceRenderStep());
        Steps.Add(new OutputSurfaceRenderStep());
        Steps.Add(new TintSurfaceRenderStep());
        Steps.Sort(RenderStepComparer.Instance);
    }
}
