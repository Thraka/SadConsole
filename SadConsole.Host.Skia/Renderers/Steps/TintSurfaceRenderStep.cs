namespace SadConsole.Renderers;

/// <summary>
/// Enqueues a tint overlay over the rendered surface.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Tint")]
public class TintSurfaceRenderStep : IRenderStep
{
    /// <inheritdoc />
    public string Name => Constants.RenderStepNames.Tint;

    /// <inheritdoc />
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Tint;

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
        if (screenObject.Tint.A != 0)
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screenObject.Tint, screenObject.AbsoluteArea));
    }

    /// <inheritdoc />
    public void Dispose() => Reset();
}
