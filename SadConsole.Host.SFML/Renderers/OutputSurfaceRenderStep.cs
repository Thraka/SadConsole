namespace SadConsole.Renderers
{
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
            var sfmlRenderer = (ScreenSurfaceRenderer)renderer;

            if (screenObject.Tint.A != 255)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(sfmlRenderer._backingTexture.Texture, new SFML.System.Vector2i(screenObject.AbsoluteArea.Position.X, screenObject.AbsoluteArea.Position.Y), sfmlRenderer._finalDrawColor));
        }

        ///  <inheritdoc/>
        public void Dispose() =>
            Reset();
    }
}
