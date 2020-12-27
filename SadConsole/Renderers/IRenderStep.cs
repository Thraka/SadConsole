using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Renderers
{
    /// <summary>
    /// A rendering step processed by an <see cref="IRenderer"/>.
    /// </summary>
    public interface IRenderStep: IDisposable
    {
        // Maybe add this in the future
        ///// <summary>
        ///// <see langword="true"/> when the step uses the active sprite batch in use by the renderer; othwerise <see langword="false"/>.
        ///// </summary>
        //bool UseSharedSpriteCompose { get; }

        /// <summary>
        /// Indicates priority related to other steps. Lowest runs first.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Called when the step is added to the renderer.
        /// </summary>
        /// <param name="renderer">The renderer the render step was added to.</param>
        /// <param name="surface">The surface associated with the renderer. This may be null.</param>
        void OnAdded(IRenderer renderer, IScreenSurface surface);

        /// <summary>
        /// Called when the step is removed from the renderer.
        /// </summary>
        /// <param name="renderer">The renderer the render step was removed from.</param>
        /// <param name="surface">The surface associated with the renderer. This may be null.</param>
        void OnRemoved(IRenderer renderer, IScreenSurface surface);

        /// <summary>
        /// Called when the surface chances.
        /// </summary>
        /// <param name="renderer">The renderer the render step was removed from.</param>
        /// <param name="surface">The surface associated with the renderer. This may be null.</param>
        void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface);

        /// <summary>
        /// Called to redraw the render step if needed.
        /// </summary>
        /// <returns><see langword="true"/> when the step is going to draw something new and is requesting a <see cref="Composing"/> step; otherwise <see langword="false"/>.</returns>
        bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced);

        /// <summary>
        /// Called when the renderer needs to redraw the <see cref="IRenderer.Output"/> texture.
        /// </summary>
        void Composing();

        /// <summary>
        /// Called when building draw calls for the render pipeline.
        /// </summary>
        void Render();
    }
}
