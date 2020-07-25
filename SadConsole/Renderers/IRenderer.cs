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
        /// A 0 to 255 value represening how transparent the surface is when drawn to the screen. 255 represents full visibility.
        /// </summary>
        byte Opacity { get; set; }

        /// <summary>
        /// Called when the renderer is added to a surface.
        /// </summary>
        /// <param name="surfaceObject">The surface.</param>
        void Attach(IScreenSurface surfaceObject);

        /// <summary>
        /// Called when the renderer is removed from a surface.
        /// </summary>
        /// <param name="surfaceObject">The surface.</param>
        void Detatch(IScreenSurface surfaceObject);

        /// <summary>
        /// Refreshes a cached drawing state.
        /// </summary>
        /// <param name="surfaceObject">The surface this renderer is attached to.</param>
        /// <param name="force">When <see langword="true"/>, indicates the refresh should happen even if a surface isn't dirty.</param>
        void Refresh(IScreenSurface surfaceObject, bool force = false);

        /// <summary>
        /// Creates a drawcall in the drawing pipeline.
        /// </summary>
        /// <param name="surfaceObject">The surface this renderer is attached to.</param>
        void Render(IScreenSurface surfaceObject);
    }
}
