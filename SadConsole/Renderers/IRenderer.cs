using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a surface.
    /// </summary>
    public interface IRenderer: IDisposable
    {
        /// <summary>
        /// Called when the renderer is added to a surface.
        /// </summary>
        /// <param name="surfaceObject">The surface.</param>
        void Attach(ScreenObjectSurface surfaceObject);

        /// <summary>
        /// Called when the renderer is removed from a surface.
        /// </summary>
        /// <param name="surfaceObject">The surface.</param>
        void Detatch(ScreenObjectSurface surfaceObject);

        /// <summary>
        /// Refreshes a cached drawing state.
        /// </summary>
        /// <param name="surfaceObject">The surface this renderer is attached to.</param>
        void Refresh(ScreenObjectSurface surfaceObject);

        /// <summary>
        /// Creates a drawcall in the drawing pipeline.
        /// </summary>
        /// <param name="surfaceObject">The surface this renderer is attached to.</param>
        void Render(ScreenObjectSurface surfaceObject);
    }
}
