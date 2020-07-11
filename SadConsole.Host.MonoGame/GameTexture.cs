using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SadRogue.Primitives;
using MonoColor = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework;
using Color = SadRogue.Primitives.Color;
using Point = SadRogue.Primitives.Point;

namespace SadConsole.Host
{
    /// <summary>
    /// Create's a <see cref="Microsoft.Xna.Framework.Graphics.Texture2D"/>. Generally you request this from the <see cref="GameHost.GetTexture"/> method.
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

        /// <summary>
        /// Loads a <see cref="Microsoft.Xna.Framework.Graphics.Texture2D"/> from a file path.
        /// </summary>
        /// <param name="path"></param>
        public GameTexture(string path)
        {
            using Stream fontStream = SadConsole.Game.Instance.OpenStream(path);
            _texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(SadConsole.Host.Global.GraphicsDevice, fontStream);

            _resourcePath = path;
        }

        /// <summary>
        /// Loads a <see cref="Microsoft.Xna.Framework.Graphics.Texture2D"/> from a stream.
        /// </summary>
        /// <param name="stream">The stream containing the texture data.</param>
        public GameTexture(Stream stream) =>
            _texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(SadConsole.Host.Global.GraphicsDevice, stream);

        /// <summary>
        /// Wraps an existing texture. The texture must be manually disposed. You cannot dispose the texture through this object.
        /// </summary>
        /// <param name="texture"></param>
        public GameTexture(Microsoft.Xna.Framework.Graphics.Texture2D texture)
        {
            _skipDispose = true;
            _texture = texture;
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
        public void SetPixel(Point position, Color color) =>
            SetPixel(position.ToIndex(_texture.Width), color);

        /// <inheritdoc />
        public void SetPixel(int index, Color color)
        {
            int size = _texture.Width * _texture.Height;

            if (index >= size || index < 0) throw new IndexOutOfRangeException("Pixel position is out of range.");

            var colors = new MonoColor[size];

            _texture.GetData(colors);
            colors[index] = color.ToMonoColor();
            _texture.SetData(colors);
        }

        /// <inheritdoc />
        public Color[] GetPixels()
        {
            var colors = new MonoColor[_texture.Width * _texture.Height];
            _texture.GetData(colors);
            return System.Runtime.InteropServices.MemoryMarshal.Cast<MonoColor, Color>(colors).ToArray();
        }

        /// <inheritdoc />
        public Color GetPixel(Point position) =>
            GetPixel(position.ToIndex(_texture.Width));

        /// <inheritdoc />
        public Color GetPixel(int index)
        {
            int size = _texture.Width * _texture.Height;

            if (index >= size || index < 0) throw new IndexOutOfRangeException("Pixel position is out of range.");

            var colors = new MonoColor[size];

            _texture.GetData(colors);
            return colors[index].ToSadRogueColor();
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
                using var resizer = GetResizedTexture(surface.BufferWidth, surface.BufferHeight);

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
                                                               Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend,
                                                               Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicClamp,
                                                               Microsoft.Xna.Framework.Graphics.DepthStencilState.None,
                                                               Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone);

            SadConsole.Host.Global.SharedSpriteBatch.Draw(_texture, resized.Bounds, MonoColor.White);
            SadConsole.Host.Global.SharedSpriteBatch.End();
            SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(null);

            return resized;
        }
    }
}
