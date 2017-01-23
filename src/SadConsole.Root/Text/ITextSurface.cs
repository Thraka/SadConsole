using Microsoft.Xna.Framework;

namespace SadConsole.Text
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
}
