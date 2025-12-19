using System;
using System.IO;
using SadRogue.Primitives;
using MonoColor = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework;
using Color = SadRogue.Primitives.Color;
using Point = SadRogue.Primitives.Point;
using System.Runtime.InteropServices;

namespace SadConsole.Host;

/// <summary>
/// Creates a <see cref="Microsoft.Xna.Framework.Graphics.Texture2D"/>. Generally you request this from the <see cref="GameHost.GetTexture(string)"/> method.
/// </summary>
public class GameTexture : ITexture
{
    private bool _skipDispose;
    private Microsoft.Xna.Framework.Graphics.Texture2D _texture;
    private string _resourcePath;

    /// <inheritdoc />
    public Microsoft.Xna.Framework.Graphics.Texture2D Texture => _texture;

    /// <inheritdoc />
    public string ResourcePath => _resourcePath;

    /// <inheritdoc />
    public int Height => _texture.Height;

    /// <inheritdoc />
    public int Width => _texture.Width;

    /// <inheritdoc />
    public int Size { get; private set; }

    /// <summary>
    /// Loads a <see cref="Microsoft.Xna.Framework.Graphics.Texture2D"/> from a file path.
    /// </summary>
    /// <param name="path"></param>
    public GameTexture(string path)
    {
        using Stream fontStream = SadConsole.Game.Instance.OpenStream(path);
        _texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(SadConsole.Host.Global.GraphicsDevice, fontStream);
        _resourcePath = path;
        Size = _texture.Width * _texture.Height;
    }

    /// <summary>
    /// Loads a <see cref="Microsoft.Xna.Framework.Graphics.Texture2D"/> from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the texture data.</param>
    public GameTexture(Stream stream)
    {
        _texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(SadConsole.Host.Global.GraphicsDevice, stream);
        Size = _texture.Width * _texture.Height;
    }

    /// <summary>
    /// Creates a new game texture with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the texture in pixels.</param>
    /// <param name="height">The height of the texture in pixels.</param>
    public GameTexture(int width, int height)
    {
        _texture = new Microsoft.Xna.Framework.Graphics.Texture2D(Global.GraphicsDevice, width, height);
        Size = width * height;
    }

    /// <summary>
    /// Creates a new game texture with the specified width, height, and pixels.
    /// </summary>
    /// <param name="width">The width of the texture in pixels.</param>
    /// <param name="height">The height of the texture in pixels.</param>
    /// <param name="pixels">The pixels to create the texture from. The array must be <paramref name="width"/> * <paramref name="height"/>.</param>
    public GameTexture(int width, int height, Color[] pixels)
    {
        _texture = new Microsoft.Xna.Framework.Graphics.Texture2D(Global.GraphicsDevice, width, height);
        Size = width * height;
        SetPixels(pixels);
    }

    /// <summary>
    /// Wraps an existing texture.
    /// </summary>
    /// <param name="texture">The texture being wrapped by this object.</param>
    /// <param name="handleDispose">When <see langword="true"/>, disposing this object will dispose the texture.</param>
    public GameTexture(Microsoft.Xna.Framework.Graphics.Texture2D texture, bool handleDispose = false)
    {
        _skipDispose = !handleDispose;
        _texture = texture;
        Size = _texture.Width * _texture.Height;
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

    /// <summary>
    /// Gets an array of colors. Row-major ordered.
    /// </summary>
    /// <returns>The pixels. Row-major ordered.</returns>
    public Color[] GetPixels()
    {
        MonoColor[] colors = GetPixelsMonoColor();
        return System.Runtime.InteropServices.MemoryMarshal.Cast<MonoColor, Color>(colors).ToArray();
    }

    /// <summary>
    /// Gets an array of <see cref="Microsoft.Xna.Framework.Color"/> pixels.
    /// </summary>
    /// <returns>An array of pixels. Row-major ordered.</returns>
    public MonoColor[] GetPixelsMonoColor()
    {
        var colors = new MonoColor[Size];
        _texture.GetData(colors);
        return colors;
    }

    /// <summary>
    /// Sets colors in the texture from an array of pixels. Row-major ordered.
    /// </summary>
    /// <param name="pixels">The individual pixel colors to set. Row-major ordered.</param>
    public void SetPixels(Color[] pixels)
    {
        if (pixels.Length != Size) throw new ArgumentOutOfRangeException(nameof(pixels), "Pixels array length must match the texture size.");
        _texture.SetData(System.Runtime.InteropServices.MemoryMarshal.Cast<Color, MonoColor>(pixels).ToArray());
    }

    /// <summary>
    /// Sets colors in the texture from a span of pixels. Row-major ordered.
    /// </summary>
    /// <param name="pixels">The individual pixel colors to set. Row-major ordered.</param>
    public void SetPixels(ReadOnlySpan<Color> pixels)
    {
        if (pixels.Length != Size) throw new ArgumentOutOfRangeException(nameof(pixels), "Pixels array length must match the texture size.");
        _texture.SetData(System.Runtime.InteropServices.MemoryMarshal.Cast<Color, MonoColor>(pixels).ToArray());
    }

    /// <summary>
    /// Replaces texture colors with the array of <see cref="Microsoft.Xna.Framework.Color"/> pixels.
    /// </summary>
    /// <param name="pixels">Array of <see cref="Microsoft.Xna.Framework.Color"/> pixels.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetPixels(MonoColor[] pixels)
    {
        if (pixels.Length != Size) throw new ArgumentOutOfRangeException("Colors array length must match the texture size.");
        _texture.SetData(pixels);
    }

    /// <inheritdoc />
    public void SetPixel(Point position, Color color) =>
        SetPixel(position.ToIndex(_texture.Width), color.ToMonoColor());

    /// <summary>
    /// Sets a single pixel in the texture to the specified <see cref="Microsoft.Xna.Framework.Color"/> at the given position.
    /// </summary>
    /// <param name="position">Position of the pixel in the texture.</param>
    /// <param name="color">Color of the pixel.</param>
    public void SetPixel(Point position, MonoColor color) =>
        SetPixel(position.ToIndex(_texture.Width), color);

    /// <inheritdoc />
    public void SetPixel(int index, Color color) =>
        SetPixel(index, color.ToMonoColor());

    /// <summary>
    /// Sets a single pixel in the texture to the specified <see cref="Microsoft.Xna.Framework.Color"/> at the given index.
    /// </summary>
    /// <param name="index">Index of the pixel.</param>
    /// <param name="color"><see cref="Microsoft.Xna.Framework.Color"/> of the pixel.</param>
    public void SetPixel(int index, MonoColor color)
    {
        if (index >= Size || index < 0) throw new IndexOutOfRangeException("Pixel position is out of range.");
        (int x, int y) = Point.FromIndex(index, Width);
        _texture.SetData(0, new Microsoft.Xna.Framework.Rectangle(x, y, 1, 1), new MonoColor[] { color }, 0, 1);
    }

    /// <inheritdoc />
    public Color GetPixel(Point position) =>
        GetPixelMonoColor(position.ToIndex(Width)).ToSadRogueColor();

    /// <summary>
    /// Gets the <see cref="Microsoft.Xna.Framework.Color"/> at the given position in the texture.
    /// </summary>
    /// <param name="position">Position in the texture.</param>
    /// <returns><see cref="Microsoft.Xna.Framework.Color"/> of the pixel.</returns>
    public MonoColor GetPixelMonoColor(Point position) =>
        GetPixelMonoColor(position.ToIndex(Width));

    /// <summary>
    /// Gets a pixel in the texture by index. Row-major ordered.
    /// </summary>
    /// <param name="index">The index of the pixel.</param>
    /// <returns>The color of the pixel.</returns>
    public Color GetPixel(int index) =>
        GetPixelMonoColor(index).ToSadRogueColor();

    /// <summary>
    /// Gets the <see cref="Microsoft.Xna.Framework.Color"/> at the given index in the texture.
    /// </summary>
    /// <param name="index">Index of the pixel in the texture.</param>
    /// <returns><see cref="Microsoft.Xna.Framework.Color"/> of the pixel.</returns>
    public MonoColor GetPixelMonoColor(int index)
    {
        if (index >= Size || index < 0) throw new IndexOutOfRangeException("Pixel position is out of range.");
        var data = new MonoColor[1];
        (int x, int y) = Point.FromIndex(index, Width);

        _texture.GetData(0, new Microsoft.Xna.Framework.Rectangle(x, y, 1, 1), data, 0, 1);

        return data[0];
    }

    /// <inheritdoc />
    public ICellSurface ToSurface(TextureConvertMode mode, int surfaceWidth, int surfaceHeight, TextureConvertBackgroundStyle backgroundStyle = TextureConvertBackgroundStyle.Pixel, TextureConvertForegroundStyle foregroundStyle = TextureConvertForegroundStyle.Block, Color[] cachedColorArray = null, ICellSurface cachedSurface = null)
    {
        if (surfaceWidth <= 0 || surfaceHeight <= 0 || surfaceWidth > _texture.Width || surfaceHeight > _texture.Height)
            throw new ArgumentOutOfRangeException("The size of the surface must be equal to or smaller than the texture.");

        ICellSurface surface = cachedSurface ?? new CellSurface(surfaceWidth, surfaceHeight);

        // Background mode with simple resizing.
        if (mode == TextureConvertMode.Background && backgroundStyle == TextureConvertBackgroundStyle.Pixel)
        {
            using Microsoft.Xna.Framework.Graphics.RenderTarget2D resizer = GetResizedTexture(surface.Width, surface.Height);

            var colors = new MonoColor[resizer.Width * resizer.Height];
            resizer.GetData(colors);
            Color[] sadColors = System.Runtime.InteropServices.MemoryMarshal.Cast<MonoColor, Color>(colors).ToArray();

            for (int i = 0; i < colors.Length; i++)
                surface[i].Background = sadColors[i];

            return surface;
        }

        // Calculating color based on surrounding pixels
        Color[] pixels = GetPixels();

        int fontSizeX = _texture.Width / surfaceWidth;
        int fontSizeY = _texture.Height / surfaceHeight;

        global::System.Threading.Tasks.Parallel.For(0, _texture.Height / fontSizeY, (h) =>
        {
            int startY = h * fontSizeY;
            for (int w = 0; w < _texture.Width / fontSizeX; w++)
            {
                int startX = w * fontSizeX;

                double allR = 0;
                double allG = 0;
                double allB = 0;
                int pixelCount = fontSizeX * fontSizeY;

                for (int y = 0; y < fontSizeY; y++)
                {
                    for (int x = 0; x < fontSizeX; x++)
                    {
                        int cY = y + startY;
                        int cX = x + startX;

                        Color color = pixels[cY * _texture.Width + cX];

                        // Weight color contribution by alpha, but divide by total pixels
                        // This way, transparent pixels contribute black (0,0,0) to the average
                        // rather than inflating the color values
                        double alpha = color.A / 255.0;
                        allR += color.R * alpha;
                        allG += color.G * alpha;
                        allB += color.B * alpha;
                    }
                }

                // Divide by total pixel count, not by alpha weight
                // This treats transparent pixels as contributing darkness
                byte sr = (byte)Math.Clamp(Math.Round(allR / pixelCount), 0, 255);
                byte sg = (byte)Math.Clamp(Math.Round(allG / pixelCount), 0, 255);
                byte sb = (byte)Math.Clamp(Math.Round(allB / pixelCount), 0, 255);

                var newColor = new SadRogue.Primitives.Color(sr, sg, sb);

                if (mode == TextureConvertMode.Background)
                    surface.SetBackground(w, h, newColor);

                else if (foregroundStyle == TextureConvertForegroundStyle.Block)
                {
                    // Calculate perceptual luminance using ITU-R BT.601 weights
                    // and apply gamma correction (approximate sRGB gamma of 2.2)
                    // to better match perceived brightness
                    float linearLuminance = (0.299f * sr + 0.587f * sg + 0.114f * sb) / 255f;
                    float perceivedBrightness = MathF.Pow(linearLuminance, 0.45f) * 255f;

                    // Map to glyphs based on approximate glyph coverage:
                    // █ (219) = 100% coverage -> use for brightest
                    // ▓ (178) = ~75% coverage
                    // ▒ (177) = ~50% coverage  
                    // ░ (176) = ~25% coverage -> use for darkest (no blank to avoid transparent look)
                    if (perceivedBrightness > 191)
                        surface.SetGlyph(w, h, 219, newColor); //█
                    else if (perceivedBrightness > 127)
                        surface.SetGlyph(w, h, 178, newColor); //▓
                    else if (perceivedBrightness > 63)
                        surface.SetGlyph(w, h, 177, newColor); //▒
                    else if (perceivedBrightness > 0)
                        surface.SetGlyph(w, h, 176, newColor); //░
                }
                else //else if (foregroundStyle == TextureConvertForegroundStyle.AsciiSymbol)
                {
                    // Calculate perceptual luminance with gamma correction
                    float linearLuminance = (0.299f * sr + 0.587f * sg + 0.114f * sb) / 255f;
                    float perceivedBrightness = MathF.Pow(linearLuminance, 0.45f) * 255f;

                    // Map to ASCII characters by approximate visual density
                    // Ordered from highest to lowest density: # @ & $ X x = + ; : , .
                    if (perceivedBrightness > 230)
                        surface.SetGlyph(w, h, '#', newColor);
                    else if (perceivedBrightness > 207)
                        surface.SetGlyph(w, h, '@', newColor);
                    else if (perceivedBrightness > 184)
                        surface.SetGlyph(w, h, '&', newColor);
                    else if (perceivedBrightness > 161)
                        surface.SetGlyph(w, h, '$', newColor);
                    else if (perceivedBrightness > 138)
                        surface.SetGlyph(w, h, 'X', newColor);
                    else if (perceivedBrightness > 115)
                        surface.SetGlyph(w, h, 'x', newColor);
                    else if (perceivedBrightness > 92)
                        surface.SetGlyph(w, h, '=', newColor);
                    else if (perceivedBrightness > 69)
                        surface.SetGlyph(w, h, '+', newColor);
                    else if (perceivedBrightness > 46)
                        surface.SetGlyph(w, h, ';', newColor);
                    else if (perceivedBrightness > 23)
                        surface.SetGlyph(w, h, ':', newColor);
                    else if (perceivedBrightness > 0)
                        surface.SetGlyph(w, h, ',', newColor);
                }
            }
        }
        );

        return surface;
    }

    private Microsoft.Xna.Framework.Graphics.RenderTarget2D GetResizedTexture(int width, int height)
    {
        var resized = new Microsoft.Xna.Framework.Graphics.RenderTarget2D(SadConsole.Host.Global.GraphicsDevice,
                                                                          width,
                                                                          height,
                                                                          false,
                                                                          SadConsole.Host.Global.GraphicsDevice.DisplayMode.Format,
                                                                          Microsoft.Xna.Framework.Graphics.DepthFormat.None);

        SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(resized);
        SadConsole.Host.Global.GraphicsDevice.Clear(MonoColor.Transparent);
        SadConsole.Host.Global.SharedSpriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate,
                                                           SadConsole.Host.Settings.MonoGameSurfaceBlendState,
                                                           Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicClamp,
                                                           Microsoft.Xna.Framework.Graphics.DepthStencilState.None,
                                                           Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone);

        SadConsole.Host.Global.SharedSpriteBatch.Draw(_texture, resized.Bounds, MonoColor.White);
        SadConsole.Host.Global.SharedSpriteBatch.End();
        SadConsole.Host.Global.ResetGraphicsDevice();

        return resized;
    }
}
