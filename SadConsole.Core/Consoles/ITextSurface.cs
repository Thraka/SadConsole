using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Basic information about a text surface.
    /// </summary>
    public interface ITextSurface
    {
        /// <summary>
        /// The width of the surface.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the surface.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The default background of the surface. Some operations take this into account.
        /// </summary>
        Color DefaultBackground { get; set; }

        /// <summary>
        /// The default foreground of the surface. Some operations take this into account.
        /// </summary>
        Color DefaultForeground { get; set; }

        /// <summary>
        /// Each cell of the surface.
        /// </summary>
        Cell[] Cells { get; }
    }

    /// <summary>
    /// A text surface with rendering information.
    /// </summary>
    public interface ITextSurfaceRendered: ITextSurface
    {
        /// <summary>
        /// In pixels, how much area of the screen this surface covers.
        /// </summary>
        Rectangle AbsoluteArea { get; set; }

        /// <summary>
        /// Each screen rectangle for <see cref="ITextSurface.Cells"/> used in rendering.
        /// </summary>
        Rectangle[] RenderRects { get; }

        /// <summary>
        /// The cells used for rendering if the <see cref="RenderArea"/> is not the entire text surface.
        /// </summary>
        Cell[] RenderCells { get; }

        /// <summary>
        /// Font used for rendering.
        /// </summary>
        Font Font { get; set; }

        /// <summary>
        /// An optional color tint applied after rendering. Recolors the entire surface. Use <see cref="Color.Transparent"/> to disable this.
        /// </summary>
        Color Tint { get; set; }

        /// <summary>
        /// A view of the <see cref="ITextSurface.Cells"/> which changes which cells will be drawn.
        /// </summary>
        Rectangle RenderArea { get; set; }
    }
}
