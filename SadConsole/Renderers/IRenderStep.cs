using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Renderers;

/// <summary>
/// A rendering step processed by an <see cref="IRenderer"/>.
/// </summary>
public interface IRenderStep : IDisposable
{
    /// <summary>
    /// Indicates priority related to other steps. Lowest runs first.
    /// </summary>
    uint SortOrder { get; set; }

    /// <summary>
    /// Adds supplemental data to the render step. 
    /// </summary>
    /// <param name="data">The data to add to the step.</param>
    void SetData(object data);

    /// <summary>
    /// Called when the step should reset any state or texture information.
    /// </summary>
    void Reset();

    /// <summary>
    /// Called to redraw the render step if needed.
    /// </summary>
    /// <param name="renderer">The renderer the render step is using.</param>
    /// <param name="screenObject">The surface associated with the renderer. This may be null.</param>
    /// <param name="backingTextureChanged"><see langword="true"/> to indicate the <see cref="IRenderer.Output"/> changed; otherwise <see langword="false"/>.</param>
    /// <param name="isForced"><see langword="true"/> when refresh is being forced; otherwise <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the step is going to draw something new and is requesting a <see cref="Composing"/> step; otherwise <see langword="false"/>.</returns>
    bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced);

    /// <summary>
    /// Called when the renderer needs to redraw the <see cref="IRenderer.Output"/> texture.
    /// </summary>
    /// <param name="renderer">The renderer the render step is using.</param>
    /// <param name="screenObject">The surface associated with the renderer. This may be null.</param>
    void Composing(IRenderer renderer, IScreenSurface screenObject);

    /// <summary>
    /// Called when building draw calls for the render pipeline.
    /// </summary>
    /// <param name="renderer">The renderer the render step is using.</param>
    /// <param name="screenObject">The surface associated with the renderer. This may be null.</param>
    void Render(IRenderer renderer, IScreenSurface screenObject);

    /// <summary>
    /// Called when various states in the host change.
    /// </summary>
    /// <param name="host">The host that uses this component.</param>
    public void OnHostUpdated(IScreenObject host) { }
}
