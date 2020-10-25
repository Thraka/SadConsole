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
        /// Called when the renderer starts to refresh the textures.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="screen">The current screen being renderered.</param>
        /// <returns><see langword="true"/> when the step wants to force a render pass. <see langword="false"/> to let the renderer decide if a render pass will happen.</returns>
        bool RefreshPreStart();

        /// <summary>
        /// Called before the refresh ends and the renderer is going to clean up its resources.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="screen">The current screen being renderered.</param>
        void RefreshEnding();

        /// <summary>
        /// Called after the renderer has completed redrawing to the surface and closed the redraw objects.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="screen">The current screen being renderered.</param>
        void RefreshEnd();

        /// <summary>
        /// Called before the render pipeline creates draw calls.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="screen"></param>
        void RenderStart();

        /// <summary>
        /// Called after the draw calls have been created but before any tint is applied to finished texture.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="screen"></param>
        void RenderBeforeTint();

        /// <summary>
        /// Called after the draw calls have been created and the tint has ended. Rendering is considered complete.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="screen"></param>
        void RenderEnd();
    }
}
