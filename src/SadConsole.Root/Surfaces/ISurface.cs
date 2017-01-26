using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// Basic information about a text surface.
    /// </summary>
    public interface ISurface
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
        /// When true, indicates this surface needs to be redrawn.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Texture to hold the last render state of the surface.
        /// </summary>
        RenderTarget2D LastRenderResult { get; set; }

        /// <summary>
        /// In pixels, how much area of the screen this surface covers.
        /// </summary>
        Rectangle AbsoluteArea { get; set; }

        /// <summary>
        /// Each screen rectangle for <see cref="ISurfaceData.Cells"/> used in rendering.
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
        /// A view of the <see cref="ISurfaceData.Cells"/> which changes which cells will be drawn.
        /// </summary>
        Rectangle RenderArea { get; set; }

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
}
