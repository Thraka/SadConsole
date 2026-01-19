using System;
using SadConsole;
using SadRogue.Primitives;

namespace SadConsole.Host;

static class GameTextureHelpers
{
    /// <summary>
    /// Minimum brightness threshold for edge detection - below this is considered "dark/empty".
    /// </summary>
    const float EdgeDetectionBrightnessThreshold = 15.0f;

    /// <summary>
    /// Processes the foreground glyph for a cell based on the selected style.
    /// </summary>
    public static void ProcessForegroundCell(ICellSurface surface, int w, int h, Color color, float brightness,
        TextureConvertForegroundStyle style, Color[] pixels, int fontSizeX, int fontSizeY, int surfaceWidth, int surfaceHeight, int textureWidth, int textureHeight, Color colorKey)
    {
        switch (style)
        {
            case TextureConvertForegroundStyle.Block:
                ProcessBlockStyle(surface, w, h, color, brightness);
                break;
            case TextureConvertForegroundStyle.EdgeShapes:
                ProcessEdgeShapesStyle(surface, w, h, color, brightness, pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight, textureWidth, textureHeight, colorKey);
                break;
            case TextureConvertForegroundStyle.AsciiSymbol:
            default:
                ProcessAsciiSymbolStyle(surface, w, h, color, brightness);
                break;
        }
    }

    /// <summary>
    /// Calculates the average color and brightness for a cell region of the texture, excluding colorKey pixels.
    /// </summary>
    /// <param name="pixels">The pixel array.</param>
    /// <param name="startX">Starting X coordinate in the texture.</param>
    /// <param name="startY">Starting Y coordinate in the texture.</param>
    /// <param name="fontSizeX">Width of the cell region.</param>
    /// <param name="fontSizeY">Height of the cell region.</param>
    /// <param name="textureWidth">Width of the texture.</param>
    /// <param name="colorKey">Color to treat as transparent/excluded.</param>
    /// <returns>A tuple containing the calculated color, brightness, and whether the cell has any valid content.</returns>
    public static (Color color, float brightness, bool hasContent) CalculateCellColor(Color[] pixels, int startX, int startY, int fontSizeX, int fontSizeY, int textureWidth, Color colorKey)
    {
        double allR = 0;
        double allG = 0;
        double allB = 0;
        int validPixelCount = 0;
        int totalPixelCount = fontSizeX * fontSizeY;

        for (int y = 0; y < fontSizeY; y++)
        {
            for (int x = 0; x < fontSizeX; x++)
            {
                int cY = y + startY;
                int cX = x + startX;

                Color color = pixels[cY * textureWidth + cX];

                // Skip transparent pixels and colorKey pixels
                if (color.A == 0 || color == colorKey)
                    continue;

                // Weight color contribution by alpha
                double alpha = color.A / 255.0;
                allR += color.R * alpha;
                allG += color.G * alpha;
                allB += color.B * alpha;
                validPixelCount++;
            }
        }

        // If no valid pixels, return empty result
        if (validPixelCount == 0)
            return (Color.Transparent, 0f, false);

        // Divide by valid pixel count for accurate color, but scale brightness by coverage
        byte sr = (byte)Math.Clamp(Math.Round(allR / validPixelCount), 0, 255);
        byte sg = (byte)Math.Clamp(Math.Round(allG / validPixelCount), 0, 255);
        byte sb = (byte)Math.Clamp(Math.Round(allB / validPixelCount), 0, 255);

        var newColor = new SadRogue.Primitives.Color(sr, sg, sb);

        // Calculate perceptual luminance using ITU-R BT.601 weights with gamma correction
        // Scale by coverage ratio so partially transparent cells appear dimmer
        float coverage = (float)validPixelCount / totalPixelCount;
        float linearLuminance = (0.299f * sr + 0.587f * sg + 0.114f * sb) / 255f;
        float perceivedBrightness = MathF.Pow(linearLuminance, 0.45f) * 255f * coverage;

        return (newColor, perceivedBrightness, true);
    }

    /// <summary>
    /// Processes a cell using block characters (█▓▒░) based on brightness.
    /// </summary>
    static void ProcessBlockStyle(ICellSurface surface, int w, int h, Color color, float brightness)
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
    static void ProcessAsciiSymbolStyle(ICellSurface surface, int w, int h, Color color, float brightness)
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
    public static void ProcessEdgeShapesStyle(ICellSurface surface, int w, int h, Color color, float cellBrightness,
        Color[] pixels, int fontSizeX, int fontSizeY, int surfaceWidth, int surfaceHeight, int textureWidth, int textureHeight, Color colorKey)
    {
        if (cellBrightness < EdgeDetectionBrightnessThreshold)
        {
            surface.SetGlyph(w, h, ' ', color);
            return;
        }

        // Get brightness of all 8 neighbors
        var neighbors = GetNeighborBrightness(w, h, pixels, fontSizeX, fontSizeY, surfaceWidth, surfaceHeight, textureWidth, textureHeight, colorKey);

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
    static (float Top, float Bottom, float Left, float Right, float TopLeft, float TopRight, float BottomLeft, float BottomRight)
        GetNeighborBrightness(int cellW, int cellH, Color[] pixels, int fontSizeX, int fontSizeY, int surfaceWidth, int surfaceHeight, int textureWidth, int textureHeight, Color colorKey)
    {
        float GetBrightness(int cellX, int cellY)
        {
            if (cellX < 0 || cellX >= surfaceWidth || cellY < 0 || cellY >= surfaceHeight)
                return 0;

            int nStartX = cellX * fontSizeX;
            int nStartY = cellY * fontSizeY;

            double nR = 0, nG = 0, nB = 0;
            int validPixelCount = 0;
            int totalPixelCount = fontSizeX * fontSizeY;

            for (int y = 0; y < fontSizeY; y++)
            {
                for (int x = 0; x < fontSizeX; x++)
                {
                    int cY = y + nStartY;
                    int cX = x + nStartX;
                    if (cY < textureHeight && cX < textureWidth)
                    {
                        Color color = pixels[cY * textureWidth + cX];
                        
                        // Skip transparent and colorKey pixels
                        if (color.A == 0 || color == colorKey)
                            continue;

                        double alpha = color.A / 255.0;
                        nR += color.R * alpha;
                        nG += color.G * alpha;
                        nB += color.B * alpha;
                        validPixelCount++;
                    }
                }
            }

            if (validPixelCount == 0)
                return 0;

            byte nsr = (byte)Math.Clamp(Math.Round(nR / validPixelCount), 0, 255);
            byte nsg = (byte)Math.Clamp(Math.Round(nG / validPixelCount), 0, 255);
            byte nsb = (byte)Math.Clamp(Math.Round(nB / validPixelCount), 0, 255);

            float coverage = (float)validPixelCount / totalPixelCount;
            float nLinear = (0.299f * nsr + 0.587f * nsg + 0.114f * nsb) / 255f;
            return MathF.Pow(nLinear, 0.45f) * 255f * coverage;
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
    static char SelectEdgeCharacter(int darkPattern, float cellBrightness,
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

    static char SelectTopDarkChar(bool tlDark, bool trDark) =>
        (tlDark, trDark) switch
        {
            (true, true) => '_',   // Flat top edge
            (true, false) => ',',  // Transitioning down-left
            (false, true) => '.',  // Transitioning down-right
            _ => '_'               // Default flat top
        };

    static char SelectBottomDarkChar(bool blDark, bool brDark) =>
        (blDark, brDark) switch
        {
            (true, true) => '-',   // Flat bottom edge
            (true, false) => '\'', // Transitioning up-left
            (false, true) => '`',  // Transitioning up-right
            _ => '-'               // Default flat bottom
        };

    static char SelectRightDarkChar(bool trDark, bool brDark) =>
        (trDark, brDark) switch
        {
            (true, true) => ')',   // Full right curve
            (true, false) => 'b',  // Transitioning - curves at top-right
            (false, true) => 'P',  // Transitioning - curves at bottom-right
            _ => ')'               // Default right curve
        };

    static char SelectLeftDarkChar(bool tlDark, bool blDark) =>
        (tlDark, blDark) switch
        {
            (true, true) => '(',   // Full left curve
            (true, false) => 'd',  // Transitioning - curves at top-left
            (false, true) => 'q',  // Transitioning - curves at bottom-left
            _ => '('               // Default left curve
        };

    static char SelectTopRightCornerChar(bool trDark, bool tlDark, bool brDark) =>
        trDark ? ',' : (!tlDark && !brDark) ? 'J' : '/';

    static char SelectBottomRightCornerChar(bool brDark, bool trDark, bool blDark) =>
        brDark ? '\'' : (!trDark && !blDark) ? '7' : '\\';

    static char SelectBottomLeftCornerChar(bool blDark, bool brDark, bool tlDark) =>
        blDark ? '`' : (!brDark && !tlDark) ? 'r' : '/';

    static char SelectTopLeftCornerChar(bool tlDark, bool trDark, bool blDark) =>
        tlDark ? '.' : (!trDark && !blDark) ? 'L' : '\\';

    static char SelectLeftPointingEndCap(bool tlDark, bool blDark) =>
        (tlDark, blDark) switch
        {
            (true, true) => '<',
            (true, false) => 'J',
            (false, true) => '7',
            _ => '<'
        };

    static char SelectRightPointingEndCap(bool trDark, bool brDark) =>
        (trDark, brDark) switch
        {
            (true, true) => '>',
            (true, false) => 'L',
            (false, true) => 'r',
            _ => '>'
        };

    static char SelectUpPointingEndCap(bool tlDark, bool trDark) =>
        (tlDark, trDark) switch
        {
            (true, true) => '^',
            (true, false) => '/',
            (false, true) => '\\',
            _ => '^'
        };

    static char SelectDownPointingEndCap(bool blDark, bool brDark) =>
        (blDark, brDark) switch
        {
            (true, true) => 'v',
            (true, false) => '\\',
            (false, true) => '/',
            _ => 'v'
        };
}
