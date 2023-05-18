using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole.Renderers;

/// <summary>
/// Renders a tint on top of where the output texture is drawn.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Tint")]
public class TintSurfaceRenderStep : IRenderStep
{
    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.Tint;

    ///  <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Tint;

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

    /// <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
        if (screenObject.Tint.A != 0)
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screenObject.Tint.ToMonoColor(), ((SadConsole.Host.GameTexture)screenObject.Font.Image).Texture, screenObject.AbsoluteArea.ToMonoRectangle(), screenObject.Font.SolidGlyphRectangle.ToMonoRectangle()));
    }

    ///  <inheritdoc/>
    public void Dispose() =>
        Reset();
}
