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

    /// <summary>
    /// Minimum brightness threshold for edge detection - below this is considered "dark/empty".
    /// </summary>
    private const float EdgeDetectionBrightnessThreshold = 15.0f;

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

                // Calculate average color for this cell
                var (cellColor, cellBrightness) = CalculateCellColor(pixels, startX, startY, fontSizeX, fontSizeY);

                if (mode == TextureConvertMode.Background)
                {
                    surface.SetBackground(w, h, cellColor);
                }
                else if (mode == TextureConvertMode.Foreground)
                {
                    ProcessForegroundCell(surface, w, h, cellColor, cellBrightness, foregroundStyle, 
                        pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight);
                }
                else if (mode == TextureConvertMode.BothFocusBackground)
                {
                    // Background is the main color, foreground is darker
                    var foreColor = cellColor.GetDarker();
                    surface.SetBackground(w, h, cellColor);
                    ProcessForegroundCell(surface, w, h, foreColor, cellBrightness, foregroundStyle,
                        pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight);
                }
                else if (mode == TextureConvertMode.BothFocusForeground)
                {
                    // Foreground is the main color, background is darker
                    var backColor = cellColor.GetDarker();
                    surface.SetBackground(w, h, backColor);
                    ProcessForegroundCell(surface, w, h, cellColor, cellBrightness, foregroundStyle,
                        pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight);
                }
            }
        });

        return surface;
    }

    /// <summary>
    /// Processes the foreground glyph for a cell based on the selected style.
    /// </summary>
    private void ProcessForegroundCell(ICellSurface surface, int w, int h, Color color, float brightness,
        TextureConvertForegroundStyle style, Color[] pixels, int fontSizeX, int fontSizeY, int surfaceWidth, int surfaceHeight)
    {
        switch (style)
        {
            case TextureConvertForegroundStyle.Block:
                ProcessBlockStyle(surface, w, h, color, brightness);
                break;
            case TextureConvertForegroundStyle.EdgeShapes:
                ProcessEdgeShapesStyle(surface, w, h, color, brightness, pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight);
                break;
            case TextureConvertForegroundStyle.AsciiSymbol:
            default:
                ProcessAsciiSymbolStyle(surface, w, h, color, brightness);
                break;
        }
    }

    /// <summary>
    /// Calculates the average color and brightness for a cell region of the texture.
    /// </summary>
    private (Color color, float brightness) CalculateCellColor(Color[] pixels, int startX, int startY, int fontSizeX, int fontSizeY)
    {
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
                double alpha = color.A / 255.0;
                allR += color.R * alpha;
                allG += color.G * alpha;
                allB += color.B * alpha;
            }
        }

        // Divide by total pixel count, not by alpha weight
        byte sr = (byte)Math.Clamp(Math.Round(allR / pixelCount), 0, 255);
        byte sg = (byte)Math.Clamp(Math.Round(allG / pixelCount), 0, 255);
        byte sb = (byte)Math.Clamp(Math.Round(allB / pixelCount), 0, 255);

        var newColor = new SadRogue.Primitives.Color(sr, sg, sb);

        // Calculate perceptual luminance using ITU-R BT.601 weights with gamma correction
        float linearLuminance = (0.299f * sr + 0.587f * sg + 0.114f * sb) / 255f;
        float perceivedBrightness = MathF.Pow(linearLuminance, 0.45f) * 255f;

        return (newColor, perceivedBrightness);
    }

    /// <summary>
    /// Processes a cell using block characters (█▓▒░) based on brightness.
    /// </summary>
    private static void ProcessBlockStyle(ICellSurface surface, int w, int h, Color color, float brightness)
    {
        // Map to glyphs based on approximate glyph coverage:
        // █ (219) = 100% coverage -> use for brightest
        // ▓ (178) = ~75% coverage
        // ▒ (177) = ~50% coverage  
        // ░ (176) = ~25% coverage -> use for darkest (no blank to avoid transparent look)
        if (brightness > 191)
            surface.SetGlyph(w, h, 219, color); //█
        else if (brightness > 127)
            surface.SetGlyph(w, h, 178, color); //▓
        else if (brightness > 63)
            surface.SetGlyph(w, h, 177, color); //▒
        else if (brightness > 0)
            surface.SetGlyph(w, h, 176, color); //░
    }

    /// <summary>
    /// Processes a cell using ASCII density characters based on brightness.
    /// </summary>
    private static void ProcessAsciiSymbolStyle(ICellSurface surface, int w, int h, Color color, float brightness)
    {
        // Map to ASCII characters by approximate visual density
        // Ordered from highest to lowest density: # @ & $ X x = + ; : , .
        char glyph = brightness switch
        {
            > 230 => '#',
            > 207 => '@',
            > 184 => '&',
            > 161 => '$',
            > 138 => 'X',
            > 115 => 'x',
            > 92 => '=',
            > 69 => '+',
            > 46 => ';',
            > 23 => ':',
            > 0 => ',',
            _ => ' '
        };

        if (glyph != ' ')
            surface.SetGlyph(w, h, glyph, color);
    }

    /// <summary>
    /// Processes a cell using edge-detecting shaped characters for curves and edges.
    /// </summary>
    private void ProcessEdgeShapesStyle(ICellSurface surface, int w, int h, Color color, float cellBrightness,
        Color[] pixels, int fontSizeX, int fontSizeY, int surfaceWidth, int surfaceHeight)
    {
        if (cellBrightness < EdgeDetectionBrightnessThreshold)
        {
            surface.SetGlyph(w, h, ' ', color);
            return;
        }

        // Get brightness of all 8 neighbors
        var neighbors = GetNeighborBrightness(w, h, pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight);

        // Determine which neighbors are dark
        bool topDark = neighbors.Top < EdgeDetectionBrightnessThreshold;
        bool bottomDark = neighbors.Bottom < EdgeDetectionBrightnessThreshold;
        bool leftDark = neighbors.Left < EdgeDetectionBrightnessThreshold;
        bool rightDark = neighbors.Right < EdgeDetectionBrightnessThreshold;
        bool tlDark = neighbors.TopLeft < EdgeDetectionBrightnessThreshold;
        bool trDark = neighbors.TopRight < EdgeDetectionBrightnessThreshold;
        bool blDark = neighbors.BottomLeft < EdgeDetectionBrightnessThreshold;
        bool brDark = neighbors.BottomRight < EdgeDetectionBrightnessThreshold;

        // Build cardinal pattern: T=1, R=2, B=4, L=8
        int darkPattern = (topDark ? 1 : 0) | (rightDark ? 2 : 0) |
                          (bottomDark ? 4 : 0) | (leftDark ? 8 : 0);

        char edgeChar = SelectEdgeCharacter(darkPattern, cellBrightness, tlDark, trDark, blDark, brDark);
        surface.SetGlyph(w, h, edgeChar, color);
    }

    /// <summary>
    /// Gets the brightness values of all 8 neighboring cells.
    /// </summary>
    private (float Top, float Bottom, float Left, float Right, float TopLeft, float TopRight, float BottomLeft, float BottomRight)
        GetNeighborBrightness(int cellW, int cellH, Color[] pixels, int fontSizeX, int fontSizeY, int surfaceWidth, int surfaceHeight)
    {
        float GetBrightness(int cellX, int cellY)
        {
            if (cellX < 0 || cellX >= surfaceWidth || cellY < 0 || cellY >= surfaceHeight)
                return 0;

            int nStartX = cellX * fontSizeX;
            int nStartY = cellY * fontSizeY;

            double nR = 0, nG = 0, nB = 0;
            int nPixelCount = fontSizeX * fontSizeY;

            for (int y = 0; y < fontSizeY; y++)
            {
                for (int x = 0; x < fontSizeX; x++)
                {
                    int cY = y + nStartY;
                    int cX = x + nStartX;
                    if (cY < _texture.Height && cX < _texture.Width)
                    {
                        Color color = pixels[cY * _texture.Width + cX];
                        double alpha = color.A / 255.0;
                        nR += color.R * alpha;
                        nG += color.G * alpha;
                        nB += color.B * alpha;
                    }
                }
            }

            byte nsr = (byte)Math.Clamp(Math.Round(nR / nPixelCount), 0, 255);
            byte nsg = (byte)Math.Clamp(Math.Round(nG / nPixelCount), 0, 255);
            byte nsb = (byte)Math.Clamp(Math.Round(nB / nPixelCount), 0, 255);

            float nLinear = (0.299f * nsr + 0.587f * nsg + 0.114f * nsb) / 255f;
            return MathF.Pow(nLinear, 0.45f) * 255f;
        }

        return (
            Top: GetBrightness(cellW, cellH - 1),
            Bottom: GetBrightness(cellW, cellH + 1),
            Left: GetBrightness(cellW - 1, cellH),
            Right: GetBrightness(cellW + 1, cellH),
            TopLeft: GetBrightness(cellW - 1, cellH - 1),
            TopRight: GetBrightness(cellW + 1, cellH - 1),
            BottomLeft: GetBrightness(cellW - 1, cellH + 1),
            BottomRight: GetBrightness(cellW + 1, cellH + 1)
        );
    }

    /// <summary>
    /// Selects the appropriate edge/shape character based on the dark neighbor pattern.
    /// </summary>
    private static char SelectEdgeCharacter(int darkPattern, float cellBrightness,
        bool tlDark, bool trDark, bool blDark, bool brDark)
    {
        return darkPattern switch
        {
            // Interior - no dark cardinal neighbors
            0b0000 => cellBrightness > 200 ? '@' :
                      cellBrightness > 150 ? '#' :
                      cellBrightness > 100 ? '=' : '+',

            // Single dark side - edge cells with transitional options
            0b0001 => SelectTopDarkChar(tlDark, trDark),      // Top dark
            0b0100 => SelectBottomDarkChar(blDark, brDark),   // Bottom dark
            0b0010 => SelectRightDarkChar(trDark, brDark),    // Right dark
            0b1000 => SelectLeftDarkChar(tlDark, blDark),     // Left dark

            // Two adjacent dark sides - corners
            0b0011 => SelectTopRightCornerChar(trDark, tlDark, brDark),
            0b0110 => SelectBottomRightCornerChar(brDark, trDark, blDark),
            0b1100 => SelectBottomLeftCornerChar(blDark, brDark, tlDark),
            0b1001 => SelectTopLeftCornerChar(tlDark, trDark, blDark),

            // Two opposite dark sides - thin lines
            0b0101 => '|',  // Top+Bottom dark
            0b1010 => '-',  // Left+Right dark

            // Three dark sides - end caps
            0b0111 => SelectLeftPointingEndCap(tlDark, blDark),
            0b1110 => SelectRightPointingEndCap(trDark, brDark),
            0b1101 => SelectUpPointingEndCap(tlDark, trDark),
            0b1011 => SelectDownPointingEndCap(blDark, brDark),

            // All sides dark - isolated
            0b1111 => 'o',

            _ => '+'
        };
    }

    // Edge character selection helpers

    private static char SelectTopDarkChar(bool tlDark, bool trDark) =>
        (tlDark, trDark) switch
        {
            (true, true) => '_',   // Flat top edge
            (true, false) => ',',  // Transitioning down-left
            (false, true) => '.',  // Transitioning down-right
            _ => '_'               // Default flat top
        };

    private static char SelectBottomDarkChar(bool blDark, bool brDark) =>
        (blDark, brDark) switch
        {
            (true, true) => '-',   // Flat bottom edge
            (true, false) => '\'', // Transitioning up-left
            (false, true) => '`',  // Transitioning up-right
            _ => '-'               // Default flat bottom
        };

    private static char SelectRightDarkChar(bool trDark, bool brDark) =>
        (trDark, brDark) switch
        {
            (true, true) => ')',   // Full right curve
            (true, false) => 'b',  // Transitioning - curves at top-right
            (false, true) => 'P',  // Transitioning - curves at bottom-right
            _ => ')'               // Default right curve
        };

    private static char SelectLeftDarkChar(bool tlDark, bool blDark) =>
        (tlDark, blDark) switch
        {
            (true, true) => '(',   // Full left curve
            (true, false) => 'd',  // Transitioning - curves at top-left
            (false, true) => 'q',  // Transitioning - curves at bottom-left
            _ => '('               // Default left curve
        };

    private static char SelectTopRightCornerChar(bool trDark, bool tlDark, bool brDark) =>
        trDark ? ',' : (!tlDark && !brDark) ? 'J' : '/';

    private static char SelectBottomRightCornerChar(bool brDark, bool trDark, bool blDark) =>
        brDark ? '\'' : (!trDark && !blDark) ? '7' : '\\';

    private static char SelectBottomLeftCornerChar(bool blDark, bool brDark, bool tlDark) =>
        blDark ? '`' : (!brDark && !tlDark) ? 'r' : '/';

    private static char SelectTopLeftCornerChar(bool tlDark, bool trDark, bool blDark) =>
        tlDark ? '.' : (!trDark && !blDark) ? 'L' : '\\';

    private static char SelectLeftPointingEndCap(bool tlDark, bool blDark) =>
        (tlDark, blDark) switch
        {
            (true, true) => '<',
            (true, false) => 'J',
            (false, true) => '7',
            _ => '<'
        };

    private static char SelectRightPointingEndCap(bool trDark, bool brDark) =>
        (trDark, brDark) switch
        {
            (true, true) => '>',
            (true, false) => 'L',
            (false, true) => 'r',
            _ => '>'
        };

    private static char SelectUpPointingEndCap(bool tlDark, bool trDark) =>
        (tlDark, trDark) switch
        {
            (true, true) => '^',
            (true, false) => '/',
            (false, true) => '\\',
            _ => '^'
        };

    private static char SelectDownPointingEndCap(bool blDark, bool brDark) =>
        (blDark, brDark) switch
        {
            (true, true) => 'v',
            (true, false) => '\\',
            (false, true) => '/',
            _ => 'v'
        };

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
