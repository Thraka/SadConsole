using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Represents a texture provided by a game host.
    /// </summary>
    public interface ITexture : IDisposable
    {
        /// <summary>
        /// The file path to the texture.
        /// </summary>
        string ResourcePath { get; }

        /// <summary>
        /// The height of the texture.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The width of the texture.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Size of the texture (Width * Height).
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets an array of colors. Row-major ordered.
        /// </summary>
        Color[] GetPixels();

        /// <summary>
        /// Sets colors in the texture;
        /// </summary>
        /// <param name="colors"></param>
        void SetPixels(Color[] colors);

        /// <summary>
        /// Sets a specific pixel in the texture to a color by x,y coordinate.
        /// </summary>
        /// <param name="position">The position of the pixel to set.</param>
        /// <param name="color">The color to set.</param>
        void SetPixel(Point position, Color color);

        /// <summary>
        /// Sets a specific pixel in the texture to a color by index. Row-major ordered.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        void SetPixel(int index, Color color);

        /// <summary>
        /// Gets a pixel in the texture by x,y coordinate.
        /// </summary>
        /// <param name="position">The x,y coordinate of the pixel.</param>
        /// <returns>The color of the pixel.</returns>
        Color GetPixel(Point position);

        /// <summary>
        /// Gets a pixel in the texture by index. Row-major ordered.
        /// </summary>
        /// <param name="index">The index of the pixel.</param>
        /// <returns>The color of the pixel.</returns>
        Color GetPixel(int index);

        /// <summary>
        /// Converts the texture into a cell surface based on the specified mode.
        /// </summary>
        /// <param name="mode">The mode used when converting the texture to a surface.</param>
        /// <param name="surfaceWidth">How many cells wide the returned surface is.</param>
        /// <param name="surfaceHeight">How many cells high the returned surface is.</param>
        /// <param name="backgroundStyle">The style to use when <paramref name="mode"/> is <see cref="TextureConvertMode.Background"/>.</param>
        /// <param name="foregroundStyle">The style to use when <paramref name="mode"/> is <see cref="TextureConvertMode.Foreground"/>.</param>
        /// <param name="cachedColorArray">When provided, this array is used for color data. It must match the texture's expected <see cref="GetPixels"/> bounds. Used with <paramref name="cachedColorArray"/>.</param>
        /// <param name="cachedSurface">The cell surface to use instead of creating a new one. Used with <paramref name="cachedColorArray"/>.</param>
        /// <returns>A new surface.</returns>
        /// <remarks></remarks>
        ICellSurface ToSurface(TextureConvertMode mode, int surfaceWidth, int surfaceHeight,
                               TextureConvertBackgroundStyle backgroundStyle = TextureConvertBackgroundStyle.Pixel,
                               TextureConvertForegroundStyle foregroundStyle = TextureConvertForegroundStyle.Block,
                               Color[] cachedColorArray = null,
                               ICellSurface cachedSurface = null);
    }

    /// <summary>
    /// The conversion mode from <see cref="ITexture"/> to <see cref="ICellSurface"/>.
    /// </summary>
    public enum TextureConvertMode
    {
        /// <summary>
        /// Fills the background of each cell with the pixel color.
        /// </summary>
        Background,
        /// <summary>
        /// Fills the foreground of each cell with the pixel color.
        /// </summary>
        Foreground
    }

    /// <summary>
    /// The style applied when <see cref="TextureConvertMode.Foreground"/> is set.
    /// </summary>
    public enum TextureConvertForegroundStyle
    {
        /// <summary>
        /// Fills the surface with block ascii that represents the brightness of the pixel. Foreground is set to the pixel color.
        /// </summary>
        Block,
        /// <summary>
        /// Fills the surface with ascii symbols that represents the brightness of the pixel. Foreground is set to the pixel color.
        /// </summary>
        AsciiSymbol,
        /* TODO
        /// <summary>
        /// Fills the surface with block ascii letters and symbols that represents the brightness of the pixel. Foreground is set to the pixel color.
        /// </summary>
        AsciiLetter
        */
    }

    /// <summary>
    /// The style applied when <see cref="TextureConvertMode.Background"/> is set.
    /// </summary>
    public enum TextureConvertBackgroundStyle
    {
        /// <summary>
        /// Simply resizes the image and maps the cell to the pixel without any other color conversion.
        /// </summary>
        Pixel,
        /// <summary>
        /// Calculates the cell color based on the surrounding colors of the image.
        /// </summary>
        Smooth,
    }
}
