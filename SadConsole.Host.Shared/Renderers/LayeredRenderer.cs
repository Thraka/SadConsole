namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="Components.LayeredSurface"/> object by adding a <see cref="LayeredSurfaceRenderStep"/> to the <see cref="ScreenSurfaceRenderer.Steps"/> collection. Skips drawing the normal surface.
/// </summary>
[System.Diagnostics.DebuggerDisplay("LayeredRenderer")]
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
