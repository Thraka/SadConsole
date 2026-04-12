using System;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Represents a 9-slice border backed by a <see cref="CellSurface"/>.
/// The surface contains the complete border artwork, and nine <see cref="Rectangle"/>
/// regions designate which areas of the surface map to each slice:
/// <code>
/// ┌───────────┬───────────┬───────────┐
/// │ TopLeft   │    Top    │ TopRight  │
/// ├───────────┼───────────┼───────────┤
/// │   Left    │  Center   │   Right   │
/// ├───────────┼───────────┼───────────┤
/// │ BotLeft   │  Bottom   │ BotRight  │
/// └───────────┴───────────┴───────────┘
/// </code>
/// Corner slices are drawn as-is. Edge slices (Top, Bottom, Left, Right) are
/// tiled to fill the required length. The Center slice is tiled to fill the
/// interior area.
/// </summary>
public class SlicedBorder
{
    /// <summary>
    /// The backing surface that contains the complete border artwork.
    /// </summary>
    public CellSurface Surface { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the top-left corner slice.
    /// </summary>
    public Rectangle TopLeft { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the top edge slice.
    /// </summary>
    public Rectangle Top { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the top-right corner slice.
    /// </summary>
    public Rectangle TopRight { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the left edge slice.
    /// </summary>
    public Rectangle Left { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the center slice.
    /// </summary>
    public Rectangle Center { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the right edge slice.
    /// </summary>
    public Rectangle Right { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the bottom-left corner slice.
    /// </summary>
    public Rectangle BottomLeft { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the bottom edge slice.
    /// </summary>
    public Rectangle Bottom { get; set; }

    /// <summary>
    /// The region of <see cref="Surface"/> that maps to the bottom-right corner slice.
    /// </summary>
    public Rectangle BottomRight { get; set; }

    /// <summary>
    /// Creates a new <see cref="SlicedBorder"/> with the specified backing surface and slice regions.
    /// </summary>
    /// <param name="surface">The surface containing the border artwork.</param>
    /// <param name="topLeft">The top-left corner region.</param>
    /// <param name="top">The top edge region.</param>
    /// <param name="topRight">The top-right corner region.</param>
    /// <param name="left">The left edge region.</param>
    /// <param name="center">The center region.</param>
    /// <param name="right">The right edge region.</param>
    /// <param name="bottomLeft">The bottom-left corner region.</param>
    /// <param name="bottom">The bottom edge region.</param>
    /// <param name="bottomRight">The bottom-right corner region.</param>
    public SlicedBorder(CellSurface surface, Rectangle topLeft, Rectangle top, Rectangle topRight,
                                              Rectangle left, Rectangle center, Rectangle right,
                                              Rectangle bottomLeft, Rectangle bottom, Rectangle bottomRight)
    {
        Surface = surface;
        TopLeft = topLeft;
        Top = top;
        TopRight = topRight;
        Left = left;
        Center = center;
        Right = right;
        BottomLeft = bottomLeft;
        Bottom = bottom;
        BottomRight = bottomRight;
    }

    /// <summary>
    /// Creates a new <see cref="SlicedBorder"/> from a surface, automatically dividing it into
    /// a 3×3 grid where each cell is one slice. The surface width and height must each be
    /// divisible into three equal parts.
    /// </summary>
    /// <param name="surface">A surface whose dimensions are evenly divisible by 3.</param>
    /// <returns>A new <see cref="SlicedBorder"/> with slices evenly distributed across the surface.</returns>
    /// <exception cref="ArgumentException">Thrown when the surface dimensions are not divisible by 3.</exception>
    public static SlicedBorder CreateFrom3x3(CellSurface surface)
    {
        if (surface.Width % 3 != 0 || surface.Height % 3 != 0)
            throw new ArgumentException("Surface width and height must each be divisible by 3.", nameof(surface));

        int sliceWidth = surface.Width / 3;
        int sliceHeight = surface.Height / 3;

        return new SlicedBorder(surface,
            new Rectangle(0, 0, sliceWidth, sliceHeight),
            new Rectangle(sliceWidth, 0, sliceWidth, sliceHeight),
            new Rectangle(sliceWidth * 2, 0, sliceWidth, sliceHeight),
            new Rectangle(0, sliceHeight, sliceWidth, sliceHeight),
            new Rectangle(sliceWidth, sliceHeight, sliceWidth, sliceHeight),
            new Rectangle(sliceWidth * 2, sliceHeight, sliceWidth, sliceHeight),
            new Rectangle(0, sliceHeight * 2, sliceWidth, sliceHeight),
            new Rectangle(sliceWidth, sliceHeight * 2, sliceWidth, sliceHeight),
            new Rectangle(sliceWidth * 2, sliceHeight * 2, sliceWidth, sliceHeight)
        );
    }

    /// <summary>
    /// Creates a new <see cref="SlicedBorder"/> from a surface using a simple 1-cell border layout.
    /// The surface must be at least 3×3. Corners are 1×1, edges are 1-cell wide/tall, and the
    /// center is the remaining interior area.
    /// </summary>
    /// <param name="surface">A surface that is at least 3×3.</param>
    /// <returns>A new <see cref="SlicedBorder"/> with a 1-cell border and the rest as center.</returns>
    /// <exception cref="ArgumentException">Thrown when the surface is smaller than 3×3.</exception>
    public static SlicedBorder CreateFromSingleCellBorder(CellSurface surface)
    {
        if (surface.Width < 3 || surface.Height < 3)
            throw new ArgumentException("Surface must be at least 3x3.", nameof(surface));

        int innerWidth = surface.Width - 2;
        int innerHeight = surface.Height - 2;

        return new SlicedBorder(surface,
            new Rectangle(0, 0, 1, 1),
            new Rectangle(1, 0, innerWidth, 1),
            new Rectangle(surface.Width - 1, 0, 1, 1),
            new Rectangle(0, 1, 1, innerHeight),
            new Rectangle(1, 1, innerWidth, innerHeight),
            new Rectangle(surface.Width - 1, 1, 1, innerHeight),
            new Rectangle(0, surface.Height - 1, 1, 1),
            new Rectangle(1, surface.Height - 1, innerWidth, 1),
            new Rectangle(surface.Width - 1, surface.Height - 1, 1, 1)
        );
    }

    /// <summary>
    /// Draws this 9-slice border onto a target surface, filling the specified area.
    /// Corner slices are drawn as-is, edge slices are tiled along their respective edges,
    /// and the center slice is tiled to fill the interior.
    /// </summary>
    /// <param name="target">The surface to draw onto.</param>
    /// <param name="area">The rectangle on the target surface that defines where the border is drawn.</param>
    public void Draw(ICellSurface target, Rectangle area)
    {
        // Corners
        TileSlice(target, TopLeft, area.X, area.Y, TopLeft.Width, TopLeft.Height);
        TileSlice(target, TopRight, area.X + area.Width - TopRight.Width, area.Y, TopRight.Width, TopRight.Height);
        TileSlice(target, BottomLeft, area.X, area.Y + area.Height - BottomLeft.Height, BottomLeft.Width, BottomLeft.Height);
        TileSlice(target, BottomRight, area.X + area.Width - BottomRight.Width, area.Y + area.Height - BottomRight.Height, BottomRight.Width, BottomRight.Height);

        // Edges
        int edgeTopWidth = area.Width - TopLeft.Width - TopRight.Width;
        int edgeBottomWidth = area.Width - BottomLeft.Width - BottomRight.Width;
        int edgeLeftHeight = area.Height - TopLeft.Height - BottomLeft.Height;
        int edgeRightHeight = area.Height - TopRight.Height - BottomRight.Height;

        if (edgeTopWidth > 0)
            TileSlice(target, Top, area.X + TopLeft.Width, area.Y, edgeTopWidth, Top.Height);

        if (edgeBottomWidth > 0)
            TileSlice(target, Bottom, area.X + BottomLeft.Width, area.Y + area.Height - Bottom.Height, edgeBottomWidth, Bottom.Height);

        if (edgeLeftHeight > 0)
            TileSlice(target, Left, area.X, area.Y + TopLeft.Height, Left.Width, edgeLeftHeight);

        if (edgeRightHeight > 0)
            TileSlice(target, Right, area.X + area.Width - Right.Width, area.Y + TopRight.Height, Right.Width, edgeRightHeight);

        // Center
        int centerWidth = area.Width - Left.Width - Right.Width;
        int centerHeight = area.Height - Top.Height - Bottom.Height;

        if (centerWidth > 0 && centerHeight > 0)
            TileSlice(target, Center, area.X + Left.Width, area.Y + Top.Height, centerWidth, centerHeight);

        target.IsDirty = true;
    }

    /// <summary>
    /// Tiles a slice region from <see cref="Surface"/> onto a rectangular area of the target surface.
    /// </summary>
    private void TileSlice(ICellSurface target, Rectangle slice, int targetX, int targetY, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            int sy = slice.Y + (y % slice.Height);
            int ty = targetY + y;

            for (int x = 0; x < width; x++)
            {
                int sx = slice.X + (x % slice.Width);
                int tx = targetX + x;

                Surface[sx, sy].CopyAppearanceTo(target[tx, ty]);
            }
        }
    }
}
