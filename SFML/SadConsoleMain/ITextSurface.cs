#if SFML
using Rectangle = SFML.Graphics.IntRect;
using SFML.Graphics;
#else
using Microsoft.Xna.Framework;
#endif

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

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">Index from the <see cref="Cells"/> array.</param>
        /// <returns>The cell.</returns>
        Cell this[int index] { get; }

        /// <summary>
        /// Gets a cell by coordinates
        /// </summary>
        /// <param name="x">The x coordinate in the surface.</param>
        /// <param name="y">The y coordinate in the surface.</param>
        /// <returns>The cell.</returns>
        Cell this[int x, int y] { get; }
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
