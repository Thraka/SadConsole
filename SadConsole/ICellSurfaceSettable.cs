using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Adds a method to change the backing cells of a surface.
/// </summary>
public partial interface ICellSurfaceSettable
{
    /// <summary>
    /// Remaps the cells of this surface to a view of the <paramref name="surface"/>.
    /// </summary>
    /// <param name="surface">The target surface to map cells from.</param>
    /// <param name="view">A view rectangle of the target surface.</param>
    void SetSurface(ICellSurface surface, Rectangle view = default);

    /// <summary>
    /// Changes the cells of the surface to the provided array.
    /// </summary>
    /// <param name="cells">The cells to replace in this surface.</param>
    /// <param name="width">The viewable width of the surface.</param>
    /// <param name="height">The viewable height of the surface.</param>
    /// <param name="bufferWidth">The maximum width of the surface.</param>
    /// <param name="bufferHeight">The maximum height of the surface.</param>
    void SetSurface(ColoredGlyphBase[] cells, int width, int height, int bufferWidth, int bufferHeight);
}
