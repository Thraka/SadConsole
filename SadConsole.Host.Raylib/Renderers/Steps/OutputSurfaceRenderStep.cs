using SadRogue.Primitives;

namespace SadConsole.Renderers;

/// <summary>
/// A render step that draws the <see cref="ScreenSurfaceRenderer._backingTexture"/> texture.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Output")]
public class OutputSurfaceRenderStep : IRenderStep
{
    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.Output;

    ///  <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Output;

    /// <summary>
    /// Not used.
    /// </summary>
    public void SetData(object data) { }

    ///  <inheritdoc/>
    public void Reset() { }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) =>
        false;

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject) { }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
        var hostRenderer = (ScreenSurfaceRenderer)renderer;
        GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(hostRenderer._backingTexture.Texture, screenObject.AbsoluteArea.Position, hostRenderer._finalDrawColor.ToHostColor()));
    }

    ///  <inheritdoc/>
    public void Dispose() =>
        Reset();
}
