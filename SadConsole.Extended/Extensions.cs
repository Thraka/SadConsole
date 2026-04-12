using System;
using System.Text;
using SadConsole.Effects;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Extensions for the <see cref="string"/> type.
/// </summary>
public static class ExtendedLib_StringExtensions2
{
    /// <summary>
    /// Converts a string into codepage 437.
    /// </summary>
    /// <param name="text">The string to convert</param>
    /// <param name="codepage">Optional codepage to provide.</param>
    /// <returns>A transformed string.</returns>
    public static string ToAscii(this string text, int codepage = 437)
    {
        // Converts characters such as ░▒▓│┤╡ ☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼■²ⁿ√·
        byte[] stringBytes = CodePagesEncodingProvider.Instance.GetEncoding(codepage)!.GetBytes(text);
        char[] stringChars = new char[stringBytes.Length];

        for (int i = 0; i < stringBytes.Length; i++)
            stringChars[i] = (char)stringBytes[i];

        return new string(stringChars);
    }
}

/// <summary>
/// Extensions for the <see cref="IScreenSurface"/> type
/// </summary>
public static class ExtendedLib_SurfaceExtensions
{
    /// <summary>
    /// Prints text that blinks out the characters each after the specified time, using the default foreground and background colors of the surface.
    /// </summary>
    /// <param name="surfaceObject">The surface to draw the text on.</param>
    /// <param name="text">The text to print.</param>
    /// <param name="position">The position where to print the text.</param>
    /// <param name="time">The time each glyph (one after the other) takes to print then fade.</param>
    /// <param name="effect">Optional effect to use. If <see langword="null"/> is passed, uses an instant fade.</param>
    public static void PrintFadingText(this IScreenSurface surfaceObject, string text, Point position, TimeSpan time, ICellEffect? effect = null) =>
        PrintFadingText(surfaceObject,
                       text.CreateColored(surfaceObject.Surface.DefaultForeground, surfaceObject.Surface.DefaultBackground),
                       position,
                       time,
                       effect);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="surfaceObject">The surface to draw the text on.</param>
    /// <param name="text">The text to print.</param>
    /// <param name="position">The position where to print the text.</param>
    /// <param name="time">The time each glyph (one after the other) takes to print then fade.</param>
    /// <param name="effect">Optional effect to use. If <see langword="null"/> is passed, uses an instant fade.</param>
    public static void PrintFadingText(this IScreenSurface surfaceObject, ColoredString text, Point position, TimeSpan time, ICellEffect? effect = null)
    {
        // If no effect is passed, create the default (no real color fade, just blink out)
        effect ??= new Fade()
                    {
                        FadeForeground = true,
                        UseCellBackground = true,
                        FadeDuration = System.TimeSpan.Zero,
                        CloneOnAdd = true,
                        StartDelay = time
                    };

        // On the string passed, set the effect.
        text.SetEffect(effect);
        // Enable the effect on the text, default is to ignore it.
        text.IgnoreEffect = false;

        // Use an instruction set. Each instruction is run after the previous.
        Instructions.InstructionSet instructionSet = new Instructions.InstructionSet()

            .Instruct(
                new Instructions.DrawString(text)
                {
                    TotalTimeToPrint = time,
                    Position = (1, 1),
                })
            .Wait(time * 2) // delay long enough to let the effects finish before erasing
            .Code(
                (screenObject, time) =>
                {
                    // erase the area the text was written before restarting. This removes effect, color, and glyph
                    ((ScreenSurface)screenObject).Surface.Erase(1, 1, text.Length);
                    return true;
                })
            ;

        instructionSet.RemoveOnFinished = true;

        surfaceObject.SadComponents.Add(instructionSet);
    }

    /// <summary>
    /// Draws a <see cref="SlicedBorder"/> onto the surface, filling its entire area.
    /// Corners are placed as-is. Edges and center are tiled to fill the remaining space.
    /// </summary>
    /// <param name="surfaceObject">The surface to draw the border on.</param>
    /// <param name="border">The sliced border definition to draw.</param>
    public static void DrawSlicedBorder(this IScreenSurface surfaceObject, SlicedBorder border) =>
        DrawSlicedBorder(surfaceObject, border, new Rectangle(0, 0, surfaceObject.Surface.Width, surfaceObject.Surface.Height));

    /// <summary>
    /// Draws a <see cref="SlicedBorder"/> onto the surface within the specified area.
    /// Corners are placed as-is. Edges and center are tiled to fill the remaining space.
    /// </summary>
    /// <param name="surfaceObject">The surface to draw the border on.</param>
    /// <param name="border">The sliced border definition to draw.</param>
    /// <param name="area">The rectangular area on the destination surface to fill with the border.</param>
    public static void DrawSlicedBorder(this IScreenSurface surfaceObject, SlicedBorder border, Rectangle area)
    {
        ICellSurface dest = surfaceObject.Surface;
        CellSurface src = border.Surface;

        int leftWidth = border.TopLeft.Width;
        int rightWidth = border.TopRight.Width;
        int topHeight = border.TopLeft.Height;
        int bottomHeight = border.BottomLeft.Height;

        // Area too small to fit even the corners
        if (area.Width < leftWidth + rightWidth || area.Height < topHeight + bottomHeight)
            return;

        int centerWidth = area.Width - leftWidth - rightWidth;
        int centerHeight = area.Height - topHeight - bottomHeight;

        // Fast path: every slice is a single cell (typical 3x3 source surface)
        if (leftWidth == 1 && rightWidth == 1 && topHeight == 1 && bottomHeight == 1
            && border.Top.Width == 1 && border.Top.Height == 1
            && border.Bottom.Width == 1 && border.Bottom.Height == 1
            && border.Left.Width == 1 && border.Left.Height == 1
            && border.Right.Width == 1 && border.Right.Height == 1
            && border.Center.Width == 1 && border.Center.Height == 1)
        {
            DrawSlicedBorderSingleCell(src, border, dest, area, centerWidth, centerHeight);
            dest.IsDirty = true;
            return;
        }

        // Corners
        CopySliceToDest(src, border.TopLeft, dest, area.X, area.Y);
        CopySliceToDest(src, border.TopRight, dest, area.X + area.Width - rightWidth, area.Y);
        CopySliceToDest(src, border.BottomLeft, dest, area.X, area.Y + area.Height - bottomHeight);
        CopySliceToDest(src, border.BottomRight, dest, area.X + area.Width - rightWidth, area.Y + area.Height - bottomHeight);

        // Top and bottom edges (tiled horizontally)
        if (centerWidth > 0)
        {
            TileSliceToDest(src, border.Top, dest, area.X + leftWidth, area.Y, centerWidth, topHeight);
            TileSliceToDest(src, border.Bottom, dest, area.X + leftWidth, area.Y + area.Height - bottomHeight, centerWidth, bottomHeight);
        }

        // Left and right edges (tiled vertically)
        if (centerHeight > 0)
        {
            TileSliceToDest(src, border.Left, dest, area.X, area.Y + topHeight, leftWidth, centerHeight);
            TileSliceToDest(src, border.Right, dest, area.X + area.Width - rightWidth, area.Y + topHeight, rightWidth, centerHeight);
        }

        // Center (tiled both directions)
        if (centerWidth > 0 && centerHeight > 0)
            TileSliceToDest(src, border.Center, dest, area.X + leftWidth, area.Y + topHeight, centerWidth, centerHeight);

        dest.IsDirty = true;
    }

    private static void DrawSlicedBorderSingleCell(CellSurface src, SlicedBorder border, ICellSurface dest, Rectangle area, int centerWidth, int centerHeight)
    {
        int right = area.X + area.Width - 1;
        int bottom = area.Y + area.Height - 1;

        ColoredGlyphBase srcTopLeft = src[border.TopLeft.X, border.TopLeft.Y];
        ColoredGlyphBase srcTopRight = src[border.TopRight.X, border.TopRight.Y];
        ColoredGlyphBase srcBottomLeft = src[border.BottomLeft.X, border.BottomLeft.Y];
        ColoredGlyphBase srcBottomRight = src[border.BottomRight.X, border.BottomRight.Y];

        // Corners
        srcTopLeft.CopyAppearanceTo(dest[area.X, area.Y]);
        srcTopRight.CopyAppearanceTo(dest[right, area.Y]);
        srcBottomLeft.CopyAppearanceTo(dest[area.X, bottom]);
        srcBottomRight.CopyAppearanceTo(dest[right, bottom]);

        // Edges only exist when there is space between corners
        if (centerWidth > 0)
        {
            ColoredGlyphBase srcTop = src[border.Top.X, border.Top.Y];
            ColoredGlyphBase srcBottom = src[border.Bottom.X, border.Bottom.Y];

            for (int x = area.X + 1; x < right; x++)
            {
                srcTop.CopyAppearanceTo(dest[x, area.Y]);
                srcBottom.CopyAppearanceTo(dest[x, bottom]);
            }
        }

        if (centerHeight > 0)
        {
            ColoredGlyphBase srcLeft = src[border.Left.X, border.Left.Y];
            ColoredGlyphBase srcRight = src[border.Right.X, border.Right.Y];

            for (int y = area.Y + 1; y < bottom; y++)
            {
                srcLeft.CopyAppearanceTo(dest[area.X, y]);
                srcRight.CopyAppearanceTo(dest[right, y]);
            }
        }

        // Center fill
        if (centerWidth > 0 && centerHeight > 0)
        {
            ColoredGlyphBase srcCenter = src[border.Center.X, border.Center.Y];

            for (int y = area.Y + 1; y < bottom; y++)
                for (int x = area.X + 1; x < right; x++)
                    srcCenter.CopyAppearanceTo(dest[x, y]);
        }
    }

    private static void CopySliceToDest(CellSurface source, Rectangle sourceArea, ICellSurface dest, int destX, int destY)
    {
        for (int y = 0; y < sourceArea.Height; y++)
        {
            for (int x = 0; x < sourceArea.Width; x++)
            {
                int dx = destX + x;
                int dy = destY + y;

                if (dx >= 0 && dx < dest.Width && dy >= 0 && dy < dest.Height)
                    source[sourceArea.X + x, sourceArea.Y + y].CopyAppearanceTo(dest[dx, dy]);
            }
        }
    }

    private static void TileSliceToDest(CellSurface source, Rectangle sourceArea, ICellSurface dest, int destX, int destY, int fillWidth, int fillHeight)
    {
        if (sourceArea.Width <= 0 || sourceArea.Height <= 0) return;

        for (int y = 0; y < fillHeight; y++)
        {
            for (int x = 0; x < fillWidth; x++)
            {
                int dx = destX + x;
                int dy = destY + y;

                if (dx >= 0 && dx < dest.Width && dy >= 0 && dy < dest.Height)
                {
                    int sx = sourceArea.X + (x % sourceArea.Width);
                    int sy = sourceArea.Y + (y % sourceArea.Height);
                    source[sx, sy].CopyAppearanceTo(dest[dx, dy]);
                }
            }
        }
    }
}
