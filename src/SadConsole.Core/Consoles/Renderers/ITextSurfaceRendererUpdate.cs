using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Marks the renderer as using a surface behind the scenes. Used by <see cref="CachedTextSurfaceRenderer"/> serialization.
    /// </summary>
    public interface ITextSurfaceRendererUpdate
    {
        /// <summary>
        /// Updates the renderer with surface information.
        /// </summary>
        /// <param name="source">The surface with update information.</param>
        void Update(ITextSurfaceRendered source);
    }
}
