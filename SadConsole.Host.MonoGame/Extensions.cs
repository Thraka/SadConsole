using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;

namespace SadConsole.Host.MonoGame
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a <see cref="Mirror"/> type to a MonoGame <see cref="Microsoft.Xna.Framework.Graphics.SpriteEffects"/> type.
        /// </summary>
        /// <param name="mirror">The value to convert.</param>
        /// <returns>The MonoGame equivalent.</returns>
        public static Microsoft.Xna.Framework.Graphics.SpriteEffects ToMonoGame(this Mirror mirror)
        {
            Microsoft.Xna.Framework.Graphics.SpriteEffects result = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

            if (mirror == Mirror.None)
                return result;

            if ((mirror & Mirror.Vertical) == Mirror.Vertical)
                result |= Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;

            if ((mirror & Mirror.Horizontal) == Mirror.Horizontal)
                result |= Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;

            return result;
        }
    }
}

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Extensions for the <see cref="Texture2D"/> type.
    /// </summary>
    public static class TextureExtensions
    {
        /// <summary>
        /// Converts a texture's pixels to a <see cref="ICellSurface"/>.
        /// </summary>
        /// <param name="image">The texture to process.</param>
        /// <param name="font">The font used with the cell surface.</param>
        /// <param name="fontSize">The size of the font.</param>
        /// <param name="blockMode"><see langword="true"/> to indicate the result should use block characters instead of text characters.</param>
        /// <returns></returns>
        public static ICellSurface ToSurface(this Texture2D image, IFont font, SadRogue.Primitives.Point fontSize, bool blockMode = false)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            Color[] pixels = new Color[imageWidth * imageHeight];
            image.GetData<Color>(pixels);

            ICellSurface surface = new CellSurface(imageWidth / fontSize.X, imageHeight / fontSize.Y);

            global::System.Threading.Tasks.Parallel.For(0, imageHeight / fontSize.Y, (h) =>
            //for (int h = 0; h < imageHeight / fontSize.Y; h++)
            {
                int startY = (h * fontSize.Y);
                //System.Threading.Tasks.Parallel.For(0, imageWidth / fontSize.X, (w) =>
                for (int w = 0; w < imageWidth / fontSize.X; w++)
                {
                    int startX = (w * fontSize.X);

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;

                    for (int y = 0; y < fontSize.Y; y++)
                    {
                        for (int x = 0; x < fontSize.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            Color color = pixels[cY * imageWidth + cX];

                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                        }
                    }

                    byte sr = (byte)(allR / (fontSize.X * fontSize.Y));
                    byte sg = (byte)(allG / (fontSize.X * fontSize.Y));
                    byte sb = (byte)(allB / (fontSize.X * fontSize.Y));

                    var newColor = new SadRogue.Primitives.Color(sr, sg, sb);

                    float sbri = newColor.GetBrightness() * 255;

                    if (blockMode)
                    {
                        if (sbri > 204)
                        {
                            surface.SetGlyph(w, h, 219, newColor); //█
                        }
                        else if (sbri > 152)
                        {
                            surface.SetGlyph(w, h, 178, newColor); //▓
                        }
                        else if (sbri > 100)
                        {
                            surface.SetGlyph(w, h, 177, newColor); //▒
                        }
                        else if (sbri > 48)
                        {
                            surface.SetGlyph(w, h, 176, newColor); //░
                        }
                    }
                    else
                    {
                        if (sbri > 230)
                        {
                            surface.SetGlyph(w, h, '#', newColor);
                        }
                        else if (sbri > 207)
                        {
                            surface.SetGlyph(w, h, '&', newColor);
                        }
                        else if (sbri > 184)
                        {
                            surface.SetGlyph(w, h, '$', newColor);
                        }
                        else if (sbri > 161)
                        {
                            surface.SetGlyph(w, h, 'X', newColor);
                        }
                        else if (sbri > 138)
                        {
                            surface.SetGlyph(w, h, 'x', newColor);
                        }
                        else if (sbri > 115)
                        {
                            surface.SetGlyph(w, h, '=', newColor);
                        }
                        else if (sbri > 92)
                        {
                            surface.SetGlyph(w, h, '+', newColor);
                        }
                        else if (sbri > 69)
                        {
                            surface.SetGlyph(w, h, ';', newColor);
                        }
                        else if (sbri > 46)
                        {
                            surface.SetGlyph(w, h, ':', newColor);
                        }
                        else if (sbri > 23)
                        {
                            surface.SetGlyph(w, h, '.', newColor);
                        }
                    }
                }
            }
            );

            return surface;
        }

        /// <summary>
        /// Converts a texture's pixels to the specified <see cref="ICellSurface"/>.
        /// </summary>
        /// <param name="image">The texture to process.</param>
        /// <param name="surface">The surface to draw on.</param>
        /// <param name="cachedColorArray">A buffer holding the color information from the texture.</param>
        /// <param name="font">The font used with the cell surface.</param>
        /// <param name="fontSize">The size of the font.</param>
        /// <param name="blockMode"><see langword="true"/> to indicate the result should use block characters instead of text characters.</param>
        public static void ToSurface(this Texture2D image, ICellSurface surface, Color[] cachedColorArray, IFont font, SadRogue.Primitives.Point fontSize, bool blockMode = false)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            image.GetData<Color>(cachedColorArray);

            surface.Clear();
            global::System.Threading.Tasks.Parallel.For(0, imageHeight / fontSize.Y, (h) =>
            //for (int h = 0; h < imageHeight / fontSize.Y; h++)
            {
                int startY = (h * fontSize.Y);
                //System.Threading.Tasks.Parallel.For(0, imageWidth / fontSize.X, (w) =>
                for (int w = 0; w < imageWidth / fontSize.X; w++)
                {
                    int startX = (w * fontSize.X);

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;

                    for (int y = 0; y < fontSize.Y; y++)
                    {
                        for (int x = 0; x < fontSize.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            Color color = cachedColorArray[cY * imageWidth + cX];

                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                        }
                    }

                    byte sr = (byte)(allR / (fontSize.X * fontSize.Y));
                    byte sg = (byte)(allG / (fontSize.X * fontSize.Y));
                    byte sb = (byte)(allB / (fontSize.X * fontSize.Y));

                    var newColor = new SadRogue.Primitives.Color(sr, sg, sb);

                    float sbri = newColor.GetBrightness() * 255;

                    if (blockMode)
                    {
                        if (sbri > 204)
                        {
                            surface.SetGlyph(w, h, 219, newColor); //█
                        }
                        else if (sbri > 152)
                        {
                            surface.SetGlyph(w, h, 178, newColor); //▓
                        }
                        else if (sbri > 100)
                        {
                            surface.SetGlyph(w, h, 177, newColor); //▒
                        }
                        else if (sbri > 48)
                        {
                            surface.SetGlyph(w, h, 176, newColor); //░
                        }
                    }
                    else
                    {
                        if (sbri > 230)
                        {
                            surface.SetGlyph(w, h, '#', newColor);
                        }
                        else if (sbri > 207)
                        {
                            surface.SetGlyph(w, h, '&', newColor);
                        }
                        else if (sbri > 184)
                        {
                            surface.SetGlyph(w, h, '$', newColor);
                        }
                        else if (sbri > 161)
                        {
                            surface.SetGlyph(w, h, 'X', newColor);
                        }
                        else if (sbri > 138)
                        {
                            surface.SetGlyph(w, h, 'x', newColor);
                        }
                        else if (sbri > 115)
                        {
                            surface.SetGlyph(w, h, '=', newColor);
                        }
                        else if (sbri > 92)
                        {
                            surface.SetGlyph(w, h, '+', newColor);
                        }
                        else if (sbri > 69)
                        {
                            surface.SetGlyph(w, h, ';', newColor);
                        }
                        else if (sbri > 46)
                        {
                            surface.SetGlyph(w, h, ':', newColor);
                        }
                        else if (sbri > 23)
                        {
                            surface.SetGlyph(w, h, '.', newColor);
                        }
                    }
                }
            }
            );
        }

        /// <summary>
        /// Saves a texture to a png file.
        /// </summary>
        /// <param name="target">The texture.</param>
        /// <param name="path">The path to a png file.</param>
        public static void Save(this Texture2D target, string path)
        {
            using (System.IO.FileStream stream = System.IO.File.OpenWrite(path))
            {
                target.SaveAsPng(stream, target.Bounds.Width, target.Bounds.Height);
            }
        }
    }
}
