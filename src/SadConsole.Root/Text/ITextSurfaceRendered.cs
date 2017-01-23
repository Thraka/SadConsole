using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Text
{
    /// <summary>
    /// A text surface with rendering information.
    /// </summary>
    public interface ITextSurfaceRendered : ITextSurface
    {
        /// <summary>
        /// When true, indicates this surface needs to be redrawn.
        /// </summary>
        bool IsDirty { get; set; }

        RenderTarget2D LastRenderResult { get; set; }

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
