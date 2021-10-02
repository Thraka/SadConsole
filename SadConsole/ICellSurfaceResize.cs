﻿using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Adds a method to support resizing a surface.
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
        /// <param name="clear">When <see langword="true"/>, indicates each cell should be reset to the default values.</param>
        void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear);

        /// <summary>
        /// Resizes the surface and view to the specified width and height.
        /// </summary>
        /// <param name="width">The viewable width of the surface.</param>
        /// <param name="height">The viewable height of the surface.</param>
        /// <param name="clear">When <see langword="true"/>, indicates each cell should be reset to the default values.</param>
        void Resize(int width, int height, bool clear);
    }
}
