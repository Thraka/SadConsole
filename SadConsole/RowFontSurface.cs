using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// A surface where each row can use a different font with different glyph dimensions.
/// </summary>
[DataContract]
[JsonObject(memberSerialization: MemberSerialization.OptIn)]
[System.Diagnostics.CodeAnalysis.Experimental(Diagnostics.DiagnosticIDs.RowFontSurfaceFeatureExperimental)]
public class RowFontSurface : ScreenSurface
{
    /// <summary>
    /// Maps row index to font. If a row is not in the dictionary, it uses the default Font property.
    /// </summary>
    [DataMember(Name = "RowFonts")]
    public Dictionary<int, IFont> RowFonts { get; } = new();

    /// <summary>
    /// Maps row index to font size. If a row is not in the dictionary, it uses the default FontSize property.
    /// </summary>
    [DataMember(Name = "RowFontSizes")]
    public Dictionary<int, Point> RowFontSizes { get; } = new();

    /// <inheritdoc/>
    public override string DefaultRendererName => Renderers.Constants.RendererNames.RowFontSurface;

    /// <summary>
    /// Gets the height of the surface in pixels, accounting for variable row heights.
    /// </summary>
    public new int HeightPixels
    {
        get
        {
            int height = 0;
            for (int row = 0; row < Surface.View.Height; row++)
                height += GetRowHeight(row);
            return height;
        }
    }

    /// <summary>
    /// Creates a new row font surface with the specified width and height.
    /// </summary>
    /// <param name="width">The width in cells of the surface.</param>
    /// <param name="height">The height in cells of the surface.</param>
    public RowFontSurface(int width, int height) : base(width, height)
    {
    }

    /// <summary>
    /// Creates a new row font surface with the specified dimensions.
    /// </summary>
    /// <param name="width">The visible width of the surface in cells.</param>
    /// <param name="height">The visible height of the surface in cells.</param>
    /// <param name="bufferWidth">The total width of the surface in cells.</param>
    /// <param name="bufferHeight">The total height of the surface in cells.</param>
    public RowFontSurface(int width, int height, int bufferWidth, int bufferHeight)
        : base(width, height, bufferWidth, bufferHeight)
    {
    }

    /// <summary>
    /// Creates a new row font surface wrapping an existing surface.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <param name="font">The font to use with the surface.</param>
    /// <param name="fontSize">The font size.</param>
    [JsonConstructor]
    public RowFontSurface(ICellSurface surface, IFont? font = null, Point? fontSize = null)
        : base(surface, font, fontSize)
    {
    }

    /// <summary>
    /// Sets the font and font size for a specific row.
    /// </summary>
    /// <param name="row">The row index (0-based).</param>
    /// <param name="font">The font to use for this row.</param>
    /// <param name="fontSize">The font size to use for this row. If null, uses font.GetFontSize(IFont.Sizes.One).</param>
    public void SetRowFont(int row, IFont font, Point? fontSize = null)
    {
        if (row < 0 || row >= Surface.View.Height)
            throw new ArgumentOutOfRangeException(nameof(row));

        RowFonts[row] = font;
        RowFontSizes[row] = fontSize ?? font.GetFontSize(IFont.Sizes.One);
    }

    /// <summary>
    /// Gets the font for a specific row. Returns the default Font if no row-specific font is set.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <returns>The font for the row.</returns>
    public IFont GetRowFont(int row)
    {
        if (RowFonts.TryGetValue(row, out IFont? font))
            return font;

        return Font;
    }

    /// <summary>
    /// Gets the font size for a specific row. Returns the default FontSize if no row-specific size is set.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <returns>The font size for the row.</returns>
    public Point GetRowFontSize(int row)
    {
        if (RowFontSizes.TryGetValue(row, out Point fontSize))
            return fontSize;

        return FontSize;
    }

    /// <summary>
    /// Gets the pixel height of a specific row based on its font size.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <returns>The height in pixels.</returns>
    public int GetRowHeight(int row)
    {
        return GetRowFontSize(row).Y;
    }

    /// <summary>
    /// Converts a pixel position to a cell position, accounting for variable row heights.
    /// </summary>
    /// <param name="pixelPosition">Pixel position relative to the surface's AbsoluteArea.</param>
    /// <returns>Cell position, or (-1, -1) if outside the surface.</returns>
    public Point PixelToCell(Point pixelPosition)
    {
        int y = -1;
        int currentY = 0;
        for (int row = 0; row < Surface.View.Height; row++)
        {
            int rowHeight = GetRowHeight(row);
            if (pixelPosition.Y >= currentY && pixelPosition.Y < currentY + rowHeight)
            {
                y = row;
                break;
            }
            currentY += rowHeight;
        }

        if (y == -1) return new Point(-1, -1);

        int x = pixelPosition.X / FontSize.X;

        if (x < 0 || x >= Surface.View.Width || y < 0 || y >= Surface.View.Height)
            return new Point(-1, -1);

        return new Point(x, y);
    }

    /// <summary>
    /// Resizes the surface to the specified width and height.
    /// </summary>
    /// <param name="viewWidth">The viewable width of the surface.</param>
    /// <param name="viewHeight">The viewable height of the surface.</param>
    /// <param name="totalWidth">The maximum width of the surface.</param>
    /// <param name="totalHeight">The maximum height of the surface.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="ICellSurface.DefaultForeground"/>, <see cref="ICellSurface.DefaultBackground"/> and glyph 0.</param>
    public new void Resize(int viewWidth, int viewHeight, int totalWidth, int totalHeight, bool clear)
    {
        base.Resize(viewWidth, viewHeight, totalWidth, totalHeight, clear);
    }

    /// <inheritdoc/>
    protected override void OnFontChanged(IFont oldFont, Point oldFontSize)
    {
        base.OnFontChanged(oldFont, oldFontSize);
    }

    protected override void OnSurfaceChanged(ICellSurface oldSurface)
    {
    }

    /// <summary>
    /// Returns the value "RowFontSurface".
    /// </summary>
    /// <returns>The string "RowFontSurface".</returns>
    public override string ToString() =>
        "RowFontSurface";
}
