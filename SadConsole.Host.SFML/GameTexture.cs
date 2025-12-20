using System;
using System.IO;
using SadRogue.Primitives;
using SFML.Graphics;
using SFMLColor = SFML.Graphics.Color;
using Color = SadRogue.Primitives.Color;
using static SadConsole.Host.GameTextureHelpers;

namespace SadConsole.Host;

/// <summary>
/// Wraps a <see cref="SFML.Graphics.Texture"/> object in a way that SadConsole can interact with it.
/// </summary>
public partial class GameTexture : ITexture
{
    private bool _skipDispose;
    private Texture _texture;
    private string _resourcePath;

    /// <inheritdoc />
    public Texture Texture => _texture;

    /// <inheritdoc />
    public string ResourcePath => _resourcePath;

    /// <inheritdoc />
    public int Height => (int)_texture.Size.Y;

    /// <inheritdoc />
    public int Width => (int)_texture.Size.X;

    /// <inheritdoc />
    public int Size { get; private set; }

    /// <summary>
    /// Creates a new game texture from a file path.
    /// </summary>
    /// <param name="path">The file path to load the texture from.</param>
    public GameTexture(string path)
    {
        using (Stream fontStream = new FileStream(path, FileMode.Open))
            _texture = new Texture(fontStream);

        _skipDispose = false;
        _resourcePath = path;
        Size = Width * Height;
    }

    /// <summary>
    /// Creates a new game texture from a stream.
    /// </summary>
    /// <param name="stream">The stream to load the texture from.</param>
    public GameTexture(Stream stream)
    {
        _skipDispose = false;
        _texture = new Texture(stream);
        Size = Width * Height;
    }

    /// <summary>
    /// Creates a new game texture with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the texture in pixels.</param>
    /// <param name="height">The height of the texture in pixels.</param>
    public GameTexture(uint width, uint height)
    {
        _skipDispose = false;
        _texture = new Texture(width, height);
        Size = (int)(width * height);
    }

    /// <summary>
    /// Creates a new game texture with the specified width, height, and pixels.
    /// </summary>
    /// <param name="width">The width of the texture in pixels.</param>
    /// <param name="height">The height of the texture in pixels.</param>
    /// <param name="pixels">The pixels to create the texture from. The array must be <paramref name="width"/> * <paramref name="height"/>.</param>
    public GameTexture(uint width, uint height, Color[] pixels)
    {
        _skipDispose = false;
        _texture = new Texture(width, height);
        Size = (int)(width * height);
        SetPixels(pixels);
    }

    /// <summary>
    /// Wraps a texture. Doesn't dispose it when this object is disposed!
    /// </summary>
    /// <param name="texture">The texture to wrap</param>
    /// <param name="handleDispose">When <see langword="true"/>, disposing this object will dispose the texture.</param>
    /// <remarks>The only time the backing texture resource is disposed is when the <see cref="GameTexture"/> object is created through <see cref="T:SadConsole.GameHost.GetTexture*"/>.</remarks>
    public GameTexture(Texture texture, bool handleDispose = false)
    {
        _skipDispose = !handleDispose;
        _texture = texture;
        Size = Width * Height;
    }

    /// <summary>
    /// Disposes the underlaying texture object and releases reference to it.
    /// </summary>
    public void Dispose()
    {
        if (!_skipDispose)
            _texture?.Dispose();

        _texture = null;
    }

    /// <inheritdoc />
    public void SetPixel(Point position, Color color)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= _texture.Size.X || position.Y >= _texture.Size.Y)
            throw new IndexOutOfRangeException("Pixel position is out of range.");

        _texture.Update(new[] { color.R, color.G, color.B, color.A }, 1, 1, (uint)position.X, (uint)position.Y);
    }


    /// <inheritdoc />
    public void SetPixel(int index, Color color) =>
        SetPixel(Point.FromIndex(index, (int)_texture.Size.X), color);

    /// <summary>
    /// Sets the color of a pixel in the texture.
    /// </summary>
    /// <param name="position">The position to set the color.</param>
    /// <param name="color">The color to set.</param>
    /// <exception cref="IndexOutOfRangeException">The pixel position is outside the bounds of the texture.</exception>
    public void SetPixel(Point position, SFMLColor color)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= _texture.Size.X || position.Y >= _texture.Size.Y)
            throw new IndexOutOfRangeException("Pixel position is out of range.");

        _texture.Update(new[] { color.R, color.G, color.B, color.A }, 1, 1, (uint)position.X, (uint)position.Y);
    }

    /// <summary>
    /// Sets the color of a pixel in the texture by index position.
    /// </summary>
    /// <param name="index">The position to set the color. Row-major ordered.</param>
    /// <param name="color">The color to set.</param>
    public void SetPixel(int index, SFMLColor color) =>
        SetPixel(Point.FromIndex(index, (int)_texture.Size.X), color);

    /// <summary>
    /// Gets an array of colors. Row-major ordered.
    /// </summary>
    public Color[] GetPixels()
    {
        using Image image = _texture.CopyToImage();
        byte[] pixels = image.Pixels;
        Span<byte> byteSpan = pixels.AsSpan();
        return System.Runtime.InteropServices.MemoryMarshal.Cast<byte, Color>(byteSpan).ToArray();
    }

    /// <summary>
    /// Gets an array of colors. Row-major ordered.
    /// </summary>
    public SFMLColor[] GetPixelsSFMLColor()
    {
        using Image image = _texture.CopyToImage();
        byte[] pixels = image.Pixels;
        Span<byte> byteSpan = pixels.AsSpan();
        return System.Runtime.InteropServices.MemoryMarshal.Cast<byte, SFMLColor>(byteSpan).ToArray();
    }

    /// <inheritdoc />
    public void SetPixels(Color[] pixels)
    {
        if (pixels.Length != Size) throw new ArgumentOutOfRangeException(nameof(pixels), "Pixels array length must match the texture size.");
        _texture.Update(System.Runtime.InteropServices.MemoryMarshal.AsBytes(pixels.AsSpan()).ToArray());
    }

    /// <inheritdoc />
    public void SetPixels(ReadOnlySpan<Color> pixels)
    {
        if (pixels.Length != Size) throw new ArgumentOutOfRangeException("Pixels array length must match the texture size.");
        _texture.Update(System.Runtime.InteropServices.MemoryMarshal.AsBytes(pixels).ToArray());
    }

    /// <summary>
    /// Updates the texture with the image data.
    /// </summary>
    /// <param name="image">The image to copy to the texture.</param>
    public void SetPixels(Image image) =>
        _texture.Update(image);

    /// <summary>
    /// Sets all pixels in the texture to the provided colors.
    /// </summary>
    /// <param name="pixels">The colors to set every pixel to.</param>
    /// <exception cref="ArgumentOutOfRangeException">The length of the colors array doesn't match the size of the texture.</exception>
    public void SetPixels(SFMLColor[] pixels)
    {
        if (pixels.Length != Size) throw new ArgumentOutOfRangeException("Pixels array length must match the texture size.");
        _texture.Update(System.Runtime.InteropServices.MemoryMarshal.AsBytes(pixels.AsSpan()).ToArray());
    }

    /// <inheritdoc />
    public Color GetPixel(Point position)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= _texture.Size.X || position.Y >= _texture.Size.Y)
            throw new IndexOutOfRangeException("Pixel position is out of range.");

        using Image image = _texture.CopyToImage();

        return image.GetPixel((uint)position.X, (uint)position.Y).ToSadRogueColor();
    }

    /// <summary>
    /// Gets a pixel color at the specified index.
    /// </summary>
    /// <param name="index">The position in the texture.</param>
    /// <returns>The color of the position in the texture.</returns>
    /// <exception cref="IndexOutOfRangeException">The pixel position is outside the bounds of the texture.</exception>
    public SFMLColor GetPixelSFMLColor(int index) =>
        GetPixelSFMLColor(Point.FromIndex(index, (int)_texture.Size.X));

    /// <summary>
    /// Gets a pixel color at the specified position.
    /// </summary>
    /// <param name="position">The position in the texture.</param>
    /// <returns>The color of the position in the texture.</returns>
    /// <exception cref="IndexOutOfRangeException">The pixel position is outside the bounds of the texture.</exception>
    public SFMLColor GetPixelSFMLColor(Point position)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= _texture.Size.X || position.Y >= _texture.Size.Y)
            throw new IndexOutOfRangeException("Pixel position is out of range.");

        using Image image = _texture.CopyToImage();
        
        return image.GetPixel((uint)position.X, (uint)position.Y);
    }

    /// <summary>
    /// Gets a pixel in the texture by index. Row-major ordered.
    /// </summary>
    /// <param name="index">The index of the pixel.</param>
    /// <returns>The color of the pixel.</returns>
    public Color GetPixel(int index) =>
        GetPixel(Point.FromIndex(index, (int)_texture.Size.X));

    /// <inheritdoc />
    public ICellSurface ToSurface(TextureConvertMode mode, int surfaceWidth, int surfaceHeight, TextureConvertBackgroundStyle backgroundStyle = TextureConvertBackgroundStyle.Pixel, TextureConvertForegroundStyle foregroundStyle = TextureConvertForegroundStyle.Block, Color[] cachedColorArray = null, ICellSurface cachedSurface = null)
    {
        if (surfaceWidth <= 0 || surfaceHeight <= 0 || surfaceWidth > _texture.Size.X || surfaceHeight > _texture.Size.Y)
            throw new ArgumentOutOfRangeException("The size of the surface must be equal to or smaller than the texture.");

        ICellSurface surface = cachedSurface ?? new CellSurface(surfaceWidth, surfaceHeight);

        // Background mode with simple resizing.
        if (mode == TextureConvertMode.Background && backgroundStyle == TextureConvertBackgroundStyle.Pixel)
        {
            using var resizer = GetResizedTexture(surface.Width, surface.Height);

            var colors = new Color[(int)resizer.Size.X * (int)resizer.Size.Y];
            using Image image = resizer.Texture.CopyToImage();
            byte[] imagePixels = image.Pixels;

            int colorIndex = 0;
            for (int i = 0; i < imagePixels.Length; i += 4)
                surface[colorIndex++].Background = new Color(imagePixels[i], imagePixels[i + 1], imagePixels[i + 2], imagePixels[i + 3]);

            return surface;
        }

        int textureWidth = (int)_texture.Size.X;
        int textureHeight = (int)_texture.Size.Y;

        // Calculating color based on surrounding pixels
        Color[] pixels = GetPixels();

        int fontSizeX = textureWidth / surfaceWidth;
        int fontSizeY = textureHeight / surfaceHeight;

        global::System.Threading.Tasks.Parallel.For(0, textureHeight / fontSizeY, (h) =>
        {
            int startY = h * fontSizeY;
            for (int w = 0; w < textureWidth / fontSizeX; w++)
            {
                int startX = w * fontSizeX;

                // Calculate average color for this cell
                var (cellColor, cellBrightness) = CalculateCellColor(pixels, startX, startY, fontSizeX, fontSizeY, textureWidth);

                if (mode == TextureConvertMode.Background)
                {
                    surface.SetBackground(w, h, cellColor);
                }
                else if (mode == TextureConvertMode.Foreground)
                {
                    ProcessForegroundCell(surface, w, h, cellColor, cellBrightness, foregroundStyle,
                        pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight, textureWidth, textureHeight);
                }
                else if (mode == TextureConvertMode.BothFocusBackground)
                {
                    // Background is the main color, foreground is darker
                    var foreColor = cellColor.GetDarker();
                    surface.SetBackground(w, h, cellColor);
                    ProcessForegroundCell(surface, w, h, foreColor, cellBrightness, foregroundStyle,
                        pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight, textureWidth, textureHeight);
                }
                else if (mode == TextureConvertMode.BothFocusForeground)
                {
                    // Foreground is the main color, background is darker
                    var backColor = cellColor.GetDarker();
                    surface.SetBackground(w, h, backColor);
                    ProcessForegroundCell(surface, w, h, cellColor, cellBrightness, foregroundStyle,
                        pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight, textureWidth, textureHeight);
                }
            }
        });

        return surface;
    }

    private RenderTexture GetResizedTexture(int width, int height)
    {
        var resized = new RenderTexture((uint)width, (uint)height);

        var batcher = new SpriteBatch();
        batcher.Reset(resized, SadConsole.Host.Settings.SFMLSurfaceBlendMode, Transform.Identity);
        batcher.DrawQuad(new IntRect(0, 0, (int)resized.Size.X, (int)resized.Size.Y), new IntRect(0, 0, (int)_texture.Size.X, (int)_texture.Size.Y), SFMLColor.White, _texture);
        batcher.End();

        resized.Display();

        return resized;
    }
}
