using System;
using System.IO;
using SadRogue.Primitives;
using SkiaSharp;
using static SadConsole.Host.GameTextureHelpers;
using Color = SadRogue.Primitives.Color;
using Point = SadRogue.Primitives.Point;

namespace SadConsole.Host;

/// <summary>
/// Stores a SadConsole texture in an OpenGL-backed Skia surface.
/// </summary>
public sealed class GameTexture : ITexture
{
    private SKSurface? _surface;
    private SKImage? _image;
    private readonly int _width;
    private readonly int _height;

    /// <summary>
    /// Gets a texture-backed snapshot of the GPU surface.
    /// </summary>
    public SKImage Image
    {
        get
        {
            ObjectDisposedException.ThrowIf(_surface is null, this);

            if (_image is null)
            {
                _image = _surface.Snapshot();
                if (!_image.IsTextureBacked)
                {
                    _image.Dispose();
                    _image = null;
                    throw new InvalidOperationException("Skia did not create a texture-backed image snapshot.");
                }
            }

            return _image;
        }
    }

    /// <inheritdoc />
    public string ResourcePath { get; }

    /// <inheritdoc />
    public int Height => _height;

    /// <inheritdoc />
    public int Width => _width;

    /// <inheritdoc />
    public int Size => Width * Height;

    /// <summary>
    /// Loads a texture from a file.
    /// </summary>
    public GameTexture(string path)
    {
        using Stream stream = File.OpenRead(path);
        (_surface, _width, _height) = Decode(stream);
        ResourcePath = path;
    }

    /// <summary>
    /// Loads a texture from a stream.
    /// </summary>
    public GameTexture(Stream stream)
    {
        (_surface, _width, _height) = Decode(stream);
        ResourcePath = string.Empty;
    }

    /// <summary>
    /// Creates an empty texture.
    /// </summary>
    public GameTexture(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        _surface = Global.RequireDevice().CreateSurface(width, height);
        _surface.Canvas.Clear(SKColors.Transparent);
        _surface.Flush();
        _width = width;
        _height = height;
        ResourcePath = string.Empty;
    }

    /// <summary>
    /// Creates a texture initialized with pixels.
    /// </summary>
    public GameTexture(int width, int height, Color[] pixels)
        : this(width, height) => SetPixels(pixels);

    /// <summary>
    /// Uploads an existing Skia bitmap to a GPU texture.
    /// </summary>
    /// <param name="bitmap">The bitmap to wrap.</param>
    /// <param name="handleDispose">Whether this wrapper owns the bitmap.</param>
    public GameTexture(SKBitmap bitmap, bool handleDispose = false)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        _width = bitmap.Width;
        _height = bitmap.Height;
        _surface = Global.RequireDevice().CreateSurface(_width, _height);

        using var paint = new SKPaint { BlendMode = SKBlendMode.Src };
        _surface.Canvas.DrawBitmap(bitmap, 0, 0, paint);
        _surface.Flush();

        if (handleDispose)
            bitmap.Dispose();

        ResourcePath = string.Empty;
    }

    /// <inheritdoc />
    public Color[] GetPixels()
    {
        SKSurface surface = GetSurface();
        var imageInfo = new SKImageInfo(Width, Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        byte[] data = new byte[imageInfo.BytesSize];

        unsafe
        {
            fixed (byte* pixels = data)
            {
                if (!surface.ReadPixels(imageInfo, (nint)pixels, imageInfo.RowBytes, 0, 0))
                    throw new InvalidOperationException("The GPU texture could not be read back.");
            }
        }

        var colors = new Color[Size];
        for (int index = 0, offset = 0; index < colors.Length; index++, offset += 4)
            colors[index] = new Color(data[offset], data[offset + 1], data[offset + 2], data[offset + 3]);

        return colors;
    }

    /// <inheritdoc />
    public void SetPixels(Color[] colors) => SetPixels(colors.AsSpan());

    /// <inheritdoc />
    public void SetPixels(ReadOnlySpan<Color> colors)
    {
        if (colors.Length != Size)
            throw new ArgumentOutOfRangeException(nameof(colors), "Pixels array length must match the texture size.");

        byte[] data = new byte[Size * 4];
        for (int index = 0, offset = 0; index < colors.Length; index++, offset += 4)
        {
            Color color = colors[index];
            data[offset] = color.R;
            data[offset + 1] = color.G;
            data[offset + 2] = color.B;
            data[offset + 3] = color.A;
        }

        var imageInfo = new SKImageInfo(Width, Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        using SKImage upload = SKImage.FromPixelCopy(imageInfo, data, imageInfo.RowBytes)
            ?? throw new InvalidOperationException("Skia could not create the texture upload image.");
        using var paint = new SKPaint { BlendMode = SKBlendMode.Src };

        SKCanvas canvas = BeginDraw();
        canvas.DrawImage(upload, 0, 0, paint);
        EndDraw();
    }

    /// <inheritdoc />
    public void SetPixel(Point position, Color color)
    {
        ValidatePosition(position.X, position.Y);

        using var paint = new SKPaint
        {
            Color = ToSkColor(color),
            BlendMode = SKBlendMode.Src
        };

        SKCanvas canvas = BeginDraw();
        canvas.DrawRect(position.X, position.Y, 1, 1, paint);
        EndDraw();
    }

    /// <inheritdoc />
    public void SetPixel(int index, Color color)
    {
        ValidateIndex(index);
        SetPixel(Point.FromIndex(index, Width), color);
    }

    /// <inheritdoc />
    public Color GetPixel(Point position)
    {
        ValidatePosition(position.X, position.Y);

        var imageInfo = new SKImageInfo(1, 1, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        byte[] data = new byte[4];
        unsafe
        {
            fixed (byte* pixel = data)
            {
                if (!GetSurface().ReadPixels(imageInfo, (nint)pixel, imageInfo.RowBytes, position.X, position.Y))
                    throw new InvalidOperationException("The GPU texture pixel could not be read back.");
            }
        }

        return new Color(data[0], data[1], data[2], data[3]);
    }

    /// <inheritdoc />
    public Color GetPixel(int index)
    {
        ValidateIndex(index);
        return GetPixel(Point.FromIndex(index, Width));
    }

    /// <inheritdoc />
    public ICellSurface ToSurface(TextureConvertMode mode, int surfaceWidth, int surfaceHeight,
                                  TextureConvertBackgroundStyle backgroundStyle = TextureConvertBackgroundStyle.Pixel,
                                  TextureConvertForegroundStyle foregroundStyle = TextureConvertForegroundStyle.Block,
                                  Color? colorKey = null, Color[]? cachedColorArray = null,
                                  ICellSurface? cachedSurface = null)
    {
        if (surfaceWidth <= 0 || surfaceHeight <= 0 || surfaceWidth > Width || surfaceHeight > Height)
            throw new ArgumentOutOfRangeException(nameof(surfaceWidth), "The size of the surface must be equal to or smaller than the texture.");

        ICellSurface surface = cachedSurface ?? new CellSurface(surfaceWidth, surfaceHeight);
        Color effectiveColorKey = colorKey ?? Color.Transparent;
        Color[] pixels = cachedColorArray ?? GetPixels();

        if (pixels.Length != Size)
            throw new ArgumentOutOfRangeException(nameof(cachedColorArray), "The cached color array must match the texture size.");

        if (mode == TextureConvertMode.Background && backgroundStyle == TextureConvertBackgroundStyle.Pixel)
        {
            for (int y = 0; y < surfaceHeight; y++)
            {
                int sourceY = y * Height / surfaceHeight;
                for (int x = 0; x < surfaceWidth; x++)
                {
                    Color color = pixels[(sourceY * Width) + (x * Width / surfaceWidth)];
                    if (color != effectiveColorKey && color.A != 0)
                        surface[x, y].Background = color;
                }
            }

            return surface;
        }

        int cellWidth = Width / surfaceWidth;
        int cellHeight = Height / surfaceHeight;

        global::System.Threading.Tasks.Parallel.For(0, surfaceHeight, y =>
        {
            int startY = y * cellHeight;
            for (int x = 0; x < surfaceWidth; x++)
            {
                int startX = x * cellWidth;
                (Color cellColor, float brightness, bool hasContent) = CalculateCellColor(
                    pixels, startX, startY, cellWidth, cellHeight, Width, effectiveColorKey);

                if (!hasContent)
                    continue;

                switch (mode)
                {
                    case TextureConvertMode.Background:
                        surface.SetBackground(x, y, cellColor);
                        break;
                    case TextureConvertMode.Foreground:
                        ProcessForegroundCell(surface, x, y, cellColor, brightness, foregroundStyle,
                            pixels, cellWidth, cellHeight, surfaceWidth, surfaceHeight, Width, Height, effectiveColorKey);
                        break;
                    case TextureConvertMode.BothFocusBackground:
                        surface.SetBackground(x, y, cellColor);
                        ProcessForegroundCell(surface, x, y, cellColor.GetDarker(), brightness, foregroundStyle,
                            pixels, cellWidth, cellHeight, surfaceWidth, surfaceHeight, Width, Height, effectiveColorKey);
                        break;
                    case TextureConvertMode.BothFocusForeground:
                        surface.SetBackground(x, y, cellColor.GetDarker());
                        ProcessForegroundCell(surface, x, y, cellColor, brightness, foregroundStyle,
                            pixels, cellWidth, cellHeight, surfaceWidth, surfaceHeight, Width, Height, effectiveColorKey);
                        break;
                }
            }
        });

        return surface;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _image?.Dispose();
        _image = null;
        _surface?.Dispose();
        _surface = null;
    }

    internal SKCanvas BeginDraw()
    {
        SKSurface surface = GetSurface();
        _image?.Dispose();
        _image = null;
        return surface.Canvas;
    }

    internal void EndDraw() => GetSurface().Flush();

    internal static SKColor ToSkColor(Color color) => new(color.R, color.G, color.B, color.A);

    internal static Color ToSadRogueColor(SKColor color) => new(color.Red, color.Green, color.Blue, color.Alpha);

    private static (SKSurface Surface, int Width, int Height) Decode(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using SKImage decoded = SKImage.FromEncodedData(stream)
            ?? throw new ArgumentException("The stream does not contain a supported image.", nameof(stream));
        SKSurface surface = Global.RequireDevice().CreateSurface(decoded.Width, decoded.Height);

        using var paint = new SKPaint { BlendMode = SKBlendMode.Src };
        surface.Canvas.DrawImage(decoded, 0, 0, paint);
        surface.Flush();
        return (surface, decoded.Width, decoded.Height);
    }

    private SKSurface GetSurface()
    {
        SKSurface surface = _surface ?? throw new ObjectDisposedException(nameof(GameTexture));
        if (surface.Context is not GRContext context)
            throw new InvalidOperationException("The texture surface is no longer GPU-backed.");

        Global.RequireDevice().ValidateContext(context);
        return surface;
    }

    private void ValidateIndex(int index)
    {
        if ((uint)index >= (uint)Size)
            throw new IndexOutOfRangeException("Pixel position is out of range.");
    }

    private void ValidatePosition(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
            throw new IndexOutOfRangeException("Pixel position is out of range.");
    }
}
