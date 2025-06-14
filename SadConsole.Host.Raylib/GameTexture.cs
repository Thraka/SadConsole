
using System;
using System.IO;
using SadRogue.Primitives;
using Raylib_cs;
using HostColor = Raylib_cs.Color;
using Color = SadRogue.Primitives.Color;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace SadConsole.Host;

// TODO: Update with implementations for NotSupportedException

/// <summary>
/// Wraps a <see cref="Texture2D"/> object in a way that SadConsole can interact with it.
/// </summary>
public class GameTexture : ITexture
{
    private bool _skipDispose;
    private Texture2D _texture;
    private string _resourcePath;
    
    /// <inheritdoc />
    public Texture2D Texture => _texture;

    /// <inheritdoc />
    public string ResourcePath => _resourcePath;

    /// <inheritdoc />
    public int Height => _texture.Height;

    /// <inheritdoc />
    public int Width => _texture.Height;

    /// <inheritdoc />
    public int Size { get; private set; }

    public GameTexture(string path)
    {
        Raylib.LoadTexture(path);
        _skipDispose = false;
        _resourcePath = path;
        Size = Width * Height;
    }

    /// <summary>
    /// Creates a new game texture with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the texture in pixels.</param>
    /// <param name="height">The height of the texture in pixels.</param>
    //public GameTexture(int width, int height)
    //{
    //    _skipDispose = false;
    //    _texture = Raylib.LoadRenderTexture(width, height);
    //    Size = (int)(width * height);
    //}

    /// <summary>
    /// Creates a new game texture with the specified width, height, and pixels.
    /// </summary>
    /// <param name="width">The width of the texture in pixels.</param>
    /// <param name="height">The height of the texture in pixels.</param>
    /// <param name="pixels">The pixels to create the texture from. The array must be <paramref name="width"/> * <paramref name="height"/>.</param>
    //public GameTexture(int width, int height, Color[] pixels)
    //{
    //    _skipDispose = false;
    //    _texture = new Texture(width, height);
    //    Size = (int)(width * height);
    //    SetPixels(pixels);
    //}

    /// <summary>
    /// Wraps a texture. Doesn't dispose it when this object is disposed!
    /// </summary>
    /// <param name="texture">The texture to wrap</param>
    /// <param name="handleDispose">When <see langword="true"/>, disposing this object will dispose the texture.</param>
    /// <remarks>The only time the backing texture resource is disposed is when the <see cref="GameTexture"/> object is created through <see cref="T:SadConsole.GameHost.GetTexture*"/>.</remarks>
    public GameTexture(Texture2D texture, bool handleDispose = false)
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
        if (!_skipDispose && Raylib.IsTextureValid(_texture))
            Raylib.UnloadTexture(_texture);
    }

    /// <inheritdoc />
    public void SetPixel(Point position, Color color)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= _texture.Width || position.Y >= _texture.Height)
            throw new IndexOutOfRangeException("Pixel position is out of range.");

        Image img = Raylib.LoadImageFromTexture(_texture);
        unsafe
        {
            Raylib.ImageDrawPixel(&img, position.X, position.Y, color.ToHostColor());
            Raylib.UpdateTexture(_texture, img.Data);
        }
        Raylib.UnloadImage(img);
    }


    /// <inheritdoc />
    public void SetPixel(int index, Color color) =>
        SetPixel(Point.FromIndex(index, _texture.Width), color);

    /// <summary>
    /// Sets the color of a pixel in the texture.
    /// </summary>
    /// <param name="position">The position to set the color.</param>
    /// <param name="color">The color to set.</param>
    /// <exception cref="IndexOutOfRangeException">The pixel position is outside the bounds of the texture.</exception>
    public void SetPixel(Point position, HostColor color)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= _texture.Width || position.Y >= _texture.Height)
            throw new IndexOutOfRangeException("Pixel position is out of range.");
        
        Image img = Raylib.LoadImageFromTexture(_texture);
        unsafe
        {
            Raylib.ImageDrawPixel(&img, position.X, position.Y, color);
            Raylib.UpdateTexture(_texture, img.Data);
        }
        Raylib.UnloadImage(img);
    }

    /// <summary>
    /// Sets the color of a pixel in the texture by index position.
    /// </summary>
    /// <param name="index">The position to set the color. Row-major ordered.</param>
    /// <param name="color">The color to set.</param>
    public void SetPixel(int index, HostColor color) =>
        SetPixel(Point.FromIndex(index, _texture.Width), color);

    /// <summary>
    /// Gets an array of colors. Row-major ordered.
    /// </summary>
    public Color[] GetPixels() =>
        throw new NotSupportedException();

    /// <summary>
    /// Gets an array of colors. Row-major ordered.
    /// </summary>
    public HostColor[] GetPixelsHostColor() =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public void SetPixels(Color[] pixels) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public void SetPixels(ReadOnlySpan<Color> pixels) =>
        throw new NotSupportedException();

    /// <summary>
    /// Updates the texture with the image data.
    /// </summary>
    /// <param name="image">The image to copy to the texture.</param>
    public void SetPixels(Image image)
    {
        unsafe
        {
            Raylib.UpdateTexture(_texture, &image.Data);
        }
    }

    /// <summary>
    /// Sets all pixels in the texture to the provided colors.
    /// </summary>
    /// <param name="pixels">The colors to set every pixel to.</param>
    /// <exception cref="ArgumentOutOfRangeException">The length of the colors array doesn't match the size of the texture.</exception>
    public void SetPixels(HostColor[] pixels) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public Color GetPixel(Point position)
    {
        //if (position.X < 0 || position.Y < 0 || position.X >= _texture.Width || position.Y >= _texture.Height)
        //    throw new IndexOutOfRangeException("Pixel position is out of range.");

        //Image img = Raylib.LoadImageFromTexture(_texture);

        //unsafe
        //{
        //    Raylib.GetPixelColor(*(img.Data) + Point.ToIndex(position.X, position.Y, _texture.Width), _texture.Format).ToSadRogueColor
        //}

        //using Image image = _texture.CopyToImage();

        //Raylib.UnloadImage(img);

        //return image.GetPixel((uint)position.X, (uint)position.Y).ToSadRogueColor();
        throw new NotSupportedException();
    }

    /// <summary>
    /// Gets a pixel color at the specified index.
    /// </summary>
    /// <param name="index">The position in the texture.</param>
    /// <returns>The color of the position in the texture.</returns>
    /// <exception cref="IndexOutOfRangeException">The pixel position is outside the bounds of the texture.</exception>
    public HostColor GetPixelHostColor(int index) =>
        GetPixelHostColor(Point.FromIndex(index, (int)_texture.Width));

    /// <summary>
    /// Gets a pixel color at the specified position.
    /// </summary>
    /// <param name="position">The position in the texture.</param>
    /// <returns>The color of the position in the texture.</returns>
    /// <exception cref="IndexOutOfRangeException">The pixel position is outside the bounds of the texture.</exception>
    public HostColor GetPixelHostColor(Point position)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= _texture.Width || position.Y >= _texture.Height)
            throw new IndexOutOfRangeException("Pixel position is out of range.");

        //using Image image = _texture.CopyToImage();

        //return image.GetPixel((uint)position.X, (uint)position.Y);
        throw new NotSupportedException();
    }

    /// <summary>
    /// Gets a pixel in the texture by index. Row-major ordered.
    /// </summary>
    /// <param name="index">The index of the pixel.</param>
    /// <returns>The color of the pixel.</returns>
    public Color GetPixel(int index) =>
        GetPixel(Point.FromIndex(index, (int)_texture.Width));

    //TODO: Testing
    /// <inheritdoc />
    public ICellSurface ToSurface(TextureConvertMode mode, int surfaceWidth, int surfaceHeight, TextureConvertBackgroundStyle backgroundStyle = TextureConvertBackgroundStyle.Pixel, TextureConvertForegroundStyle foregroundStyle = TextureConvertForegroundStyle.Block, Color[] cachedColorArray = null, ICellSurface cachedSurface = null)
    {
        if (surfaceWidth <= 0 || surfaceHeight <= 0 || surfaceWidth > _texture.Width || surfaceHeight > _texture.Height)
            throw new ArgumentOutOfRangeException("The size of the surface must be equal to or smaller than the texture.");

        ICellSurface surface = cachedSurface ?? new CellSurface(surfaceWidth, surfaceHeight);

        // Background mode with simple resizing.
        if (mode == TextureConvertMode.Background && backgroundStyle == TextureConvertBackgroundStyle.Pixel)
        {
            RenderTexture2D resizer = GetResizedTexture(surface.Width, surface.Height);

            Color[] colors = new Color[resizer.Texture.Width * resizer.Texture.Height];

            Image img = Raylib.LoadImageFromTexture(resizer.Texture);
            int arraySize = Raylib.GetPixelDataSize(resizer.Texture.Width, resizer.Texture.Height, resizer.Texture.Format);
            byte[] imagePixels = new byte[arraySize];
             
            int colorIndex = 0;
            for (int i = 0; i < imagePixels.Length; i += 4)
                surface[colorIndex++].Background = new Color(imagePixels[i], imagePixels[i + 1], imagePixels[i + 2], imagePixels[i + 3]);

            return surface;
        }

        // Calculating color based on surrounding pixels
        Color[] pixels = GetPixels();

        int fontSizeX = (int)_texture.Width / surfaceWidth;
        int fontSizeY = (int)_texture.Height / surfaceHeight;

        global::System.Threading.Tasks.Parallel.For(0, (int)_texture.Height / fontSizeY, (h) =>
        //for (int h = 0; h < imageHeight / fontSizeY; h++)
        {
            int startY = h * fontSizeY;
            //System.Threading.Tasks.Parallel.For(0, imageWidth / fontSizeX, (w) =>
            for (int w = 0; w < _texture.Width / fontSizeX; w++)
            {
                int startX = w * fontSizeX;

                float allR = 0;
                float allG = 0;
                float allB = 0;

                for (int y = 0; y < fontSizeY; y++)
                {
                    for (int x = 0; x < fontSizeX; x++)
                    {
                        int cY = y + startY;
                        int cX = x + startX;

                        Color color = pixels[cY * _texture.Width + cX];

                        allR += color.R;
                        allG += color.G;
                        allB += color.B;
                    }
                }

                byte sr = (byte)(allR / (fontSizeX * fontSizeY));
                byte sg = (byte)(allG / (fontSizeX * fontSizeY));
                byte sb = (byte)(allB / (fontSizeX * fontSizeY));

                var newColor = new SadRogue.Primitives.Color(sr, sg, sb);

                if (mode == TextureConvertMode.Background)
                    surface.SetBackground(w, h, newColor);

                else if (foregroundStyle == TextureConvertForegroundStyle.Block)
                {
                    float sbri = newColor.GetHSLLightness() * 255;

                    if (sbri > 204)
                        surface.SetGlyph(w, h, 219, newColor); //█
                    else if (sbri > 152)
                        surface.SetGlyph(w, h, 178, newColor); //▓
                    else if (sbri > 100)
                        surface.SetGlyph(w, h, 177, newColor); //▒
                    else if (sbri > 48)
                        surface.SetGlyph(w, h, 176, newColor); //░
                }
                else //else if (foregroundStyle == TextureConvertForegroundStyle.AsciiSymbol)
                {
                    float sbri = newColor.GetHSLLightness() * 255;

                    if (sbri > 230)
                        surface.SetGlyph(w, h, '#', newColor);
                    else if (sbri > 207)
                        surface.SetGlyph(w, h, '&', newColor);
                    else if (sbri > 184)
                        surface.SetGlyph(w, h, '$', newColor);
                    else if (sbri > 161)
                        surface.SetGlyph(w, h, 'X', newColor);
                    else if (sbri > 138)
                        surface.SetGlyph(w, h, 'x', newColor);
                    else if (sbri > 115)
                        surface.SetGlyph(w, h, '=', newColor);
                    else if (sbri > 92)
                        surface.SetGlyph(w, h, '+', newColor);
                    else if (sbri > 69)
                        surface.SetGlyph(w, h, ';', newColor);
                    else if (sbri > 46)
                        surface.SetGlyph(w, h, ':', newColor);
                    else if (sbri > 23)
                        surface.SetGlyph(w, h, '.', newColor);
                }
            }
        }
        );

        return surface;
    }

    private RenderTexture2D GetResizedTexture(int width, int height)
    {
        RenderTexture2D texture = Raylib.LoadRenderTexture(width, height);

        Raylib.BeginDrawing();
        Raylib.DrawTexturePro(_texture,
                              new Raylib_cs.Rectangle(0f, 0f, new Vector2(_texture.Width, _texture.Height)),
                              new Raylib_cs.Rectangle(0f, 0f, new Vector2(width, height)),
                              Vector2.Zero, 0f, HostColor.White);
        Raylib.EndDrawing();

        return texture;
    }
}
