using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
    /// </summary>
    public partial interface ICellSurfaceResize
    {

        /// <summary>
        /// Resizes the surface to the specified width and height.
        /// </summary>
        /// <param name="width">The viewable width of the surface.</param>
        /// <param name="height">The viewable height of the surface.</param>
        /// <param name="bufferWidth">The maximum width of the surface.</param>
        /// <param name="bufferHeight">The maximum height of the surface.</param>
        /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="DefaultForeground"/>, <see cref="DefaultBackground"/> and glyph 0.</param>
        void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear);
    }
}
