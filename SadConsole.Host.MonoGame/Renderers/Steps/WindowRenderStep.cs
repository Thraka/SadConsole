using SadRogue.Primitives;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.Renderers;

/// <summary>
/// Renders the dimmed background screen when a window is modal.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Window")]
public class WindowRenderStep : IRenderStep
{
    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.Window;

    ///  <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Window;

    /// <summary>
    /// Not used.
    /// </summary>
    public void SetData(object data) { }

    ///  <inheritdoc/>
    public void Reset() { }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) =>
        false;

    /// <summary>
    /// Does nothing.
    /// </summary>
    public void Composing(IRenderer renderer, IScreenSurface screenObject) { }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
        IWindowData window = (IWindowData)screenObject;
        UI.Colors colors = window.Controls.GetThemeColors();

        if (window.IsModal && colors.ModalBackground.A != 0)
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(colors.ModalBackground.ToMonoColor(), ((Host.GameTexture)screenObject.Font.Image).Texture, new XnaRectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), screenObject.Font.SolidGlyphRectangle.ToMonoRectangle()));
    }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public void Dispose() =>
        Reset();
}
