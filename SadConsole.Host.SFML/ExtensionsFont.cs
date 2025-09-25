/*
#nullable enable

using System;
using SFML.Graphics;
using HostColor = SFML.Graphics.Color;
using HostRectangle = SFML.Graphics.IntRect;
using SadRogue.Primitives;
using SadConsole.Host;

namespace SadConsole.FontEditing;

/// <summary>
/// Extension methods to handle editing <see cref="SadFont"/> objects.
/// </summary>
public static class ExtensionsFontHost
{

    /// <summary>
    /// Converts the font's backing texture into a render target, if it isn't one.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    public static RenderTexture Edit_EnableEditing(this IFont font)
    {
        GameTexture oldTexture = (GameTexture)font.Image;

        // Failed conversion, we need to enable editing by converting to a RenderTarget2D
        RenderTexture newTexture = new(oldTexture.Texture.Size.X, oldTexture.Texture.Size.Y);
        using Sprite sprite = new Sprite(oldTexture.Texture);
        newTexture.Draw(sprite);

        return newTexture;
    }

    /// <summary>
    /// Converts the font's backing texture from a render target to a normal texture.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    public static void Edit_DisableEditing(this IFont font)
    {
        GameTexture oldTexture = (GameTexture)font.Image;

        // Failed conversion, we need to enable editing by converting to a RenderTarget2D
        if (oldTexture.Texture is RenderTarget2D)
        {
            Texture2D newTexture = new(Global.GraphicsDevice, oldTexture.Width, oldTexture.Height, false, Global.GraphicsDevice.DisplayMode.Format);

            newTexture.SetData(oldTexture.GetPixelsMonoColor());

            font.Image = new GameTexture(newTexture, true);
            oldTexture.Dispose();
        }
    }

    /// <summary>
    /// Adds the specified number of rows to the font. Creates a new backing texture object that is a render target. Is hardware accelerated.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="count">The number of rows.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the number is zero or less.</exception>
    public static void Edit_AddRows(this SadFont font, int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "You must pass a positive number of rows to add.");

        var oldTexture = (GameTexture)font.Image;
        var newTexture = new RenderTarget2D(Global.GraphicsDevice,
                                            oldTexture.Width,
                                            oldTexture.Height + (count * font.GlyphHeight) + ((count + 1) * font.GlyphPadding),
                                            false,
                                            Global.GraphicsDevice.DisplayMode.Format,
                                            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

        // render old texture on the new texture
        Global.GraphicsDevice.SetRenderTarget(newTexture);
        Global.GraphicsDevice.Clear(HostColor.Transparent);
        Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate,
                                                           Host.Settings.MonoGameSurfaceBlendState,
                                                           SamplerState.AnisotropicClamp,
                                                           DepthStencilState.None,
                                                           RasterizerState.CullNone);

        Global.SharedSpriteBatch.Draw(oldTexture.Texture, new Microsoft.Xna.Framework.Vector2(0, 0), HostColor.White);

        Global.SharedSpriteBatch.End();
        Global.GraphicsDevice.SetRenderTarget(null);

        oldTexture.Dispose();

        // pack new texture into the font
        font.Image = new GameTexture(newTexture, true);
        font.Columns = (int)Math.Floor((double)font.Image.Width / (font.GlyphWidth + font.GlyphPadding));
        font.Rows = (int)Math.Floor((double)font.Image.Height / (font.GlyphHeight + font.GlyphPadding));

        // Add new glyph rects
        for (int i = (font.Rows * font.Columns) - (count * font.Columns); i < font.Rows * font.Columns; i++)
        {
            int cx = i % font.Columns;
            int cy = i / font.Columns;

            if (font.GlyphPadding != 0)
            {
                font.GlyphRectangles.Add(i, new Rectangle((cx * font.GlyphWidth) + ((cx + 1) * font.GlyphPadding),
                                                 (cy * font.GlyphHeight) + ((cy + 1) * font.GlyphPadding),
                                                 font.GlyphWidth, font.GlyphHeight));
            }
            else
            {
                font.GlyphRectangles.Add(i, new Rectangle(cx * font.GlyphWidth, cy * font.GlyphHeight, font.GlyphWidth, font.GlyphHeight));
            }
        }
    }

    /// <summary>
    /// Copies a glyph from one font index to another. Converts the backing texture to a render target. Is hardware accelerated.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndexFrom">The source glyph index.</param>
    /// <param name="glyphIndexTo">The target glyph index.</param>
    public static void Edit_CopyGlyph_GPU(this IFont font, int glyphIndexFrom, int glyphIndexTo) =>
        Edit_CopyGlyph_GPU(font, font.GetGlyphSourceRectangle(glyphIndexFrom).ToMonoRectangle(), font.GetGlyphSourceRectangle(glyphIndexTo).ToMonoRectangle(), HostColor.White);

    /// <summary>
    /// Copies a glyph from one font index to another. Converts the backing texture to a render target. Is hardware accelerated.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndexFrom">The source glyph index.</param>
    /// <param name="glyphIndexTo">The target glyph index.</param>
    /// <param name="blendColor">Color to apply while copying the glyph.</param>
    public static void Edit_CopyGlyph_GPU(this IFont font, int glyphIndexFrom, int glyphIndexTo, HostColor blendColor) =>
        Edit_CopyGlyph_GPU(font, font.GetGlyphSourceRectangle(glyphIndexFrom).ToMonoRectangle(), font.GetGlyphSourceRectangle(glyphIndexTo).ToMonoRectangle(), blendColor);

    /// <summary>
    /// Copies a glyph from one font index to another. Converts the backing texture to a render target. Is hardware accelerated.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphRectangleFrom">The source glyph rectangle.</param>
    /// <param name="glyphRectangleTo">The target glyph rectangle.</param>
    public static void Edit_CopyGlyph_GPU(this IFont font, HostRectangle glyphRectangleFrom, HostRectangle glyphRectangleTo) =>
        Edit_CopyGlyph_GPU(font, glyphRectangleFrom, glyphRectangleTo, HostColor.White);

    /// <summary>
    /// Copies a glyph from one font index to another. Converts the backing texture to a render target. Is hardware accelerated.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphRectangleFrom">The source glyph rectangle.</param>
    /// <param name="glyphRectangleTo">The target glyph rectangle.</param>
    /// <param name="blendColor">Color to apply while copying the glyph.</param>
    public static void Edit_CopyGlyph_GPU(this IFont font, HostRectangle glyphRectangleFrom, HostRectangle glyphRectangleTo, HostColor blendColor)
    {
        if (glyphRectangleFrom == glyphRectangleTo) return;

        var target = (RenderTarget2D)((GameTexture)font.Image).Texture;

        Global.GraphicsDevice.SetRenderTarget(target);
        Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate,
                                                           Host.Settings.MonoGameSurfaceBlendState,
                                                           SamplerState.AnisotropicClamp,
                                                           DepthStencilState.None,
                                                           RasterizerState.CullNone);

        Global.SharedSpriteBatch.Draw(((GameTexture)font.Image).Texture, glyphRectangleTo, glyphRectangleFrom, blendColor);

        Global.SharedSpriteBatch.End();
        Global.GraphicsDevice.SetRenderTarget(null);
    }

    /// <summary>
    /// Erases a glyph in the font by index.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndex">The glyph index to erase.</param>
    /// <param name="doSetPixels">When <see langword="true"/>, pushes the updated pixel buffer, <paramref name="cachedFontTexturePixels"/>, to the font texture.</param>
    /// <param name="cachedFontTexturePixels">A cached array of all the font's texture pixels.</param>
    public static void Edit_EraseGlyph_CPU(this IFont font, int glyphIndex, bool doSetPixels, ref Color[] cachedFontTexturePixels) =>
        Edit_SetGlyph_CPU(font, glyphIndex, new Color[font.GlyphWidth * font.GlyphHeight], doSetPixels, ref cachedFontTexturePixels);

    /// <summary>
    /// Sets the pixels of a font glyph by index.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndex">The index of the glyph to update.</param>
    /// <param name="pixels">The pixels to set on the glyph.</param>
    /// <param name="doSetPixels">When <see langword="true"/>, pushes the updated pixel buffer, <paramref name="cachedFontTexturePixels"/>, to the font texture.</param>
    /// <param name="cachedFontTexturePixels">A cached array of all the font's texture pixels.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="pixels"/> count doesn't match the size of a font glyph.</exception>
    public static void Edit_SetGlyph_CPU(this IFont font, int glyphIndex, Color[] pixels, bool doSetPixels, ref Color[] cachedFontTexturePixels)
    {
        if (pixels.Length != font.GlyphWidth * font.GlyphHeight)
            throw new ArgumentOutOfRangeException(nameof(pixels), $"Amount of pixels must match font glyph width * height: {font.GlyphWidth * font.GlyphHeight}.");

        cachedFontTexturePixels ??= font.Image.GetPixels();

        Rectangle rect = font.GetGlyphSourceRectangle(glyphIndex);

        int indexCounter = 0;
        for (int y = rect.MinExtentY; y != rect.MaxExtentY; y++)
        {
            for (int x = rect.MinExtentX; x != rect.MaxExtentX; x++)
            {
                cachedFontTexturePixels[y * font.Image.Width + x] = pixels[indexCounter];
                indexCounter++;
            }
        }

        if (doSetPixels)
            font.Image.SetPixels(cachedFontTexturePixels);
    }

    /// <summary>
    /// Returns a glyph's pixels packaged into a <see cref="Color"/> array.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndex">The index of the glyph to get.</param>
    /// <param name="cachedFontTexturePixels">A cached array of all the font's texture pixels.</param>
    /// <returns>The pixels of the glyph.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Color[] Edit_GetGlyph_CPU(this IFont font, int glyphIndex, ref Color[] cachedFontTexturePixels)
    {
        if (glyphIndex < 0 || glyphIndex > font.TotalGlyphs - 1) throw new ArgumentOutOfRangeException(nameof(glyphIndex), "Glyph index is out of range.");

        Rectangle rect = font.GetGlyphSourceRectangle(glyphIndex);
        Color[] newPixels = new Color[rect.Width * rect.Height];

        cachedFontTexturePixels ??= font.Image.GetPixels();

        int indexCounter = 0;
        for (int y = rect.MinExtentY; y != rect.MaxExtentY; y++)
        {
            for (int x = rect.MinExtentX; x != rect.MaxExtentX; x++)
            {
                newPixels[indexCounter] = cachedFontTexturePixels[y * font.Image.Width + x];
                indexCounter++;
            }
        }

        return newPixels;
    }

}
*/
