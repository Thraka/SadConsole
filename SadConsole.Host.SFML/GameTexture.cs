using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SadRogue.Primitives;
using SFML.Graphics;
using SFMLColor = SFML.Graphics.Color;
using Color = SadRogue.Primitives.Color;

namespace SadConsole.Host
{
    /// <summary>
    /// Wraps a <see cref="SFML.Graphics.Texture"/> object in a way that SadConsole can interact with it.
    /// </summary>
    public class GameTexture : ITexture
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

        internal GameTexture(string path)
        {
            using (Stream fontStream = new FileStream(path, FileMode.Open))
                _texture = new Texture(fontStream);

            _resourcePath = path;
            Size = Width * Height;
        }

        internal GameTexture(Stream stream)
        {
            _texture = new Texture(stream);
            Size = Width * Height;
        }
            

        /// <summary>
        /// Wraps a texture. Doesn't dispose it when this object is disposed!
        /// </summary>
        /// <param name="texture">The texture to wrap</param>
        /// <remarks>The only time the backing texture resource is disposed is when the <see cref="GameTexture"/> object is created through <see cref="T:SadConsole.GameHost.GetTexture*"/>.</remarks>
        public GameTexture(Texture texture)
        {
            _skipDispose = true;
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
        public void SetPixels(Color[] colors)
        {
            if (colors.Length != Size) throw new ArgumentOutOfRangeException("Pixels array length must match the texture size.");
            _texture.Update(System.Runtime.InteropServices.MemoryMarshal.AsBytes(colors.AsSpan()).ToArray());
        }

        /// <inheritdoc />
        public void SetPixels(ReadOnlySpan<Color> colors)
        {
            if (colors.Length != Size) throw new ArgumentOutOfRangeException("Pixels array length must match the texture size.");
            _texture.Update(System.Runtime.InteropServices.MemoryMarshal.AsBytes(colors).ToArray());
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
        /// <param name="colors">The colors to set every pixel to.</param>
        /// <exception cref="ArgumentOutOfRangeException">The length of the colors array doesn't match the size of the texture.</exception>
        public void SetPixels(SFMLColor[] colors)
        {
            if (colors.Length != Size) throw new ArgumentOutOfRangeException("Pixels array length must match the texture size.");
            _texture.Update(System.Runtime.InteropServices.MemoryMarshal.AsBytes(colors.AsSpan()).ToArray());
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

            // Calculating color based on surrounding pixels
            Color[] pixels = GetPixels();

            int fontSizeX = (int)_texture.Size.X / surfaceWidth;
            int fontSizeY = (int)_texture.Size.Y / surfaceHeight;

            global::System.Threading.Tasks.Parallel.For(0, (int)_texture.Size.Y / fontSizeY, (h) =>
            //for (int h = 0; h < imageHeight / fontSizeY; h++)
            {
                int startY = h * fontSizeY;
                //System.Threading.Tasks.Parallel.For(0, imageWidth / fontSizeX, (w) =>
                for (int w = 0; w < _texture.Size.X / fontSizeX; w++)
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

                            Color color = pixels[cY * _texture.Size.X + cX];

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
                        float sbri = newColor.GetBrightness() * 255;

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
                        float sbri = newColor.GetBrightness() * 255;

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
}
