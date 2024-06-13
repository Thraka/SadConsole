namespace SadConsole.Renderers;

public class CallbackRenderStep : IRenderStep
{
    private Action<ScreenSurfaceRenderer, IScreenSurface> _callback;

    /// <inheritdoc/>
    public string Name => "callback";

    ///  <inheritdoc/>
    public uint SortOrder { get; set; } = 80; // before tint step

    ///  <inheritdoc/>
    public void SetData(object data) { }

    ///  <inheritdoc/>
    public void Reset() { }

    public CallbackRenderStep(Action<ScreenSurfaceRenderer, IScreenSurface> callback) =>
        _callback = callback;

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) =>
        true; // Return true to make sure Composing will be called

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject) =>
        _callback((SadConsole.Renderers.ScreenSurfaceRenderer)renderer!, screenObject);

    /// <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject) { }

    ///  <inheritdoc/>
    public void Dispose() =>
        Reset();
}
