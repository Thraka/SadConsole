namespace SadConsole.Renderers;

/// <summary>
/// Enqueues the renderer's composed output texture for final drawing.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Output")]
public class OutputSurfaceRenderStep : IRenderStep
{
    /// <inheritdoc />
    public string Name => Constants.RenderStepNames.Output;

    /// <inheritdoc />
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Output;

    /// <inheritdoc />
    public void SetData(object data) { }

    /// <inheritdoc />
    public void Reset() { }

    /// <inheritdoc />
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) => false;

    /// <inheritdoc />
    public void Composing(IRenderer renderer, IScreenSurface screenObject) { }

    /// <inheritdoc />
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
        var skiaRenderer = (ScreenSurfaceRenderer)renderer;
        GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(
            skiaRenderer.BackingTexture,
            screenObject.AbsoluteArea.Position,
            skiaRenderer.FinalDrawColor));
    }

    /// <inheritdoc />
    public void Dispose() => Reset();
}
