#nullable enable

using System;
using MonoColor = Microsoft.Xna.Framework.Color;
using System.Collections.Generic;
using SadRogue.Primitives;
using Microsoft.Xna.Framework.Graphics;
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
    public static void Edit_EnableEditing(this IFont font)
    {
        var oldTexture = (SadConsole.Host.GameTexture)font.Image;

        // Failed conversion, we need to enable editing by converting to a RenderTarget2D
        if (!(oldTexture.Texture is RenderTarget2D))
        {
            RenderTarget2D newTexture = new RenderTarget2D(Host.Global.GraphicsDevice, font.Image.Width, font.Image.Height,
                                                                          false,
                                                                          SadConsole.Host.Global.GraphicsDevice.DisplayMode.Format,
                                                                          Microsoft.Xna.Framework.Graphics.DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            newTexture.SetData<MonoColor>(oldTexture.GetPixelsMonoColor());

            font.Image = new Host.GameTexture(newTexture, true);
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
        if (count <= 0) throw new ArgumentOutOfRangeException("You must pass a positive number of rows to add.", nameof(count));

        var oldTexture = (SadConsole.Host.GameTexture)font.Image;
        var newTexture = new Microsoft.Xna.Framework.Graphics.RenderTarget2D(SadConsole.Host.Global.GraphicsDevice,
                                                                          oldTexture.Width,
                                                                          oldTexture.Height + (count * font.GlyphHeight) + ((count + 1) * font.GlyphPadding),
                                                                          false,
                                                                          SadConsole.Host.Global.GraphicsDevice.DisplayMode.Format,
                                                                          Microsoft.Xna.Framework.Graphics.DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

        // render old texture on the new texture
        SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(newTexture);
        SadConsole.Host.Global.GraphicsDevice.Clear(MonoColor.Transparent);
        SadConsole.Host.Global.SharedSpriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate,
                                                           SadConsole.Host.Settings.MonoGameSurfaceBlendState,
                                                           Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicClamp,
                                                           Microsoft.Xna.Framework.Graphics.DepthStencilState.None,
                                                           Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone);

        SadConsole.Host.Global.SharedSpriteBatch.Draw(oldTexture.Texture, new Microsoft.Xna.Framework.Vector2(0, 0), MonoColor.White);

        SadConsole.Host.Global.SharedSpriteBatch.End();
        SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(null);

        oldTexture.Dispose();
        
        // pack old texture into the font
        font.Image = new Host.GameTexture(newTexture, true);
        font.Columns = (int)System.Math.Floor((double)font.Image.Width / (font.GlyphWidth + font.GlyphPadding));
        font.Rows = (int)System.Math.Floor((double)font.Image.Height / (font.GlyphHeight + font.GlyphPadding));
        
        // Add new glyph rects
        for (int i = (font.Rows * font.Columns) - (count * font.Columns); i < font.Rows * font.Columns; i++)
        {
            int cx = i % font.Columns;
            int cy = i / font.Columns;

            if (font.GlyphPadding != 0)
            {
                font.GlyphRectangles.Add(i, new SadRogue.Primitives.Rectangle((cx * font.GlyphWidth) + ((cx + 1) * font.GlyphPadding),
                                                 (cy * font.GlyphHeight) + ((cy + 1) * font.GlyphPadding),
                                                 font.GlyphWidth, font.GlyphHeight));
            }
            else
            {
                font.GlyphRectangles.Add(i, new SadRogue.Primitives.Rectangle(cx * font.GlyphWidth, cy * font.GlyphHeight, font.GlyphWidth, font.GlyphHeight));
            }
        }
    }

    /// <summary>
    /// Copies a glyph from one font index to another. Converts the backing texture to a render target. Is hardware accelerated.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndexFrom">The source glyph index.</param>
    /// <param name="glyphIndexTo">The target glyph index.</param>
    public static void Edit_CopyGlyph_Texture(this IFont font, int glyphIndexFrom, int glyphIndexTo)
    {
        if (glyphIndexFrom == glyphIndexTo) return;

        Edit_EnableEditing(font);

        var target = (RenderTarget2D)((Host.GameTexture)font.Image).Texture;

        SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(target);
        SadConsole.Host.Global.SharedSpriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate,
                                                           SadConsole.Host.Settings.MonoGameSurfaceBlendState,
                                                           Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicClamp,
                                                           Microsoft.Xna.Framework.Graphics.DepthStencilState.None,
                                                           Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone);

        SadConsole.Host.Global.SharedSpriteBatch.Draw(((Host.GameTexture)font.Image).Texture, font.GetGlyphSourceRectangle(glyphIndexTo).ToMonoRectangle(), font.GetGlyphSourceRectangle(glyphIndexFrom).ToMonoRectangle(), MonoColor.White);

        SadConsole.Host.Global.SharedSpriteBatch.End();
        SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(null);
    }

    /// <summary>
    /// Erases a glyph in the font by index.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndex">The glyph index to erase.</param>
    /// <param name="doSetPixels">When <see langword="true"/>, pushes the updated pixel buffer, <paramref name="cachedFontTexturePixels"/>, to the font texture.</param>
    /// <param name="cachedFontTexturePixels">A cached array of all the font's texture pixels.</param>
    public static void Edit_EraseGlyph_Pixel(this IFont font, int glyphIndex, bool doSetPixels, ref Color[] cachedFontTexturePixels) =>
        Edit_SetGlyph_Pixel(font, glyphIndex, new Color[font.GlyphWidth * font.GlyphHeight], doSetPixels, ref cachedFontTexturePixels);

    /// <summary>
    /// Sets the pixels of a font glyph by index.
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndex">The index of the glyph to update.</param>
    /// <param name="pixels">The pixels to set on the glyph.</param>
    /// <param name="doSetPixels">When <see langword="true"/>, pushes the updated pixel buffer, <paramref name="cachedFontTexturePixels"/>, to the font texture.</param>
    /// <param name="cachedFontTexturePixels">A cached array of all the font's texture pixels.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="pixels"/> count doesn't match the size of a font glyph.</exception>
    public static void Edit_SetGlyph_Pixel(this IFont font, int glyphIndex, Color[] pixels, bool doSetPixels, ref Color[] cachedFontTexturePixels)
    {
        if (pixels.Length != font.GlyphWidth * font.GlyphHeight) throw new ArgumentOutOfRangeException($"Amount of pixels must match font glyph width * height: {font.GlyphWidth * font.GlyphHeight}.", nameof(pixels));

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
    /// 
    /// </summary>
    /// <param name="font">The font being edited.</param>
    /// <param name="glyphIndex">The index of the glyph to get.</param>
    /// <param name="cachedFontTexturePixels">A cached array of all the font's texture pixels.</param>
    /// <returns>The pixels of the glyph.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Color[] Edit_GetGlyph_Pixel(this IFont font, int glyphIndex, ref Color[] cachedFontTexturePixels)
    {
        if (glyphIndex < 0 || glyphIndex > font.TotalGlyphs - 1) throw new ArgumentOutOfRangeException("Glyph index is out of range.", nameof(glyphIndex));

        var rect = font.GetGlyphSourceRectangle(glyphIndex);
        var newPixels = new Color[rect.Width * rect.Height];

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
