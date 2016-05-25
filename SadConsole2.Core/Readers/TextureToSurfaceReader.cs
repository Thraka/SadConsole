using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Readers
{
    /// <summary>
    /// Reads a texture to a cached surface. Used for animation.
    /// </summary>
    public class TextureToSurfaceReader
    {
        int width;
        int height;
        TextSurface surface;
        Color[] pixels;
        int[] indexes;
        int fontPixels;

        /// <summary>
        /// Renders the cells as blocks instead of characters.
        /// </summary>
        public bool UseBlockMode { get; set; }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="pixelWidth">Width the source texture.</param>
        /// <param name="pixelHeight">Height of the source texture.</param>
        /// <param name="font">Font used for rendering.</param>
        public TextureToSurfaceReader(int pixelWidth, int pixelHeight, Font font)
        {
            this.width = pixelWidth;
            this.height = pixelHeight;
            pixels = new Color[pixelWidth * pixelHeight];
            indexes = new int[pixelWidth * pixelHeight];
            surface = new TextSurface(pixelWidth / font.Size.X, pixelHeight / font.Size.Y, font);
            fontPixels = font.Size.X * font.Size.Y;

            // build the indexes
            int currentIndex = 0;
            for (int h = 0; h < surface.Height; h++)
            {
                int startY = (h * surface.Font.Size.Y);
                //System.Threading.Tasks.Parallel.For(0, image.Width / surface.Font.Size.X, (w) =>
                for (int w = 0; w < surface.Width; w++)
                {
                    int startX = (w * surface.Font.Size.X);

                    for (int y = 0; y < surface.Font.Size.Y; y++)
                    {
                        for (int x = 0; x < surface.Font.Size.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            indexes[currentIndex] = cY * pixelWidth + cX;
                            currentIndex++;
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// Returns a surface with the specified image rendered to it as characters.
        /// </summary>
        /// <param name="image">The image to render.</param>
        /// <returns>The surface.</returns>
        public TextSurface GetSurface(Texture2D image)
        {
            //surface.Clear();
            image.GetData<Color>(pixels);

            System.Threading.Tasks.Parallel.For(0, surface.Width * surface.Height, (i) =>
            //for (int i = 0; i < surface.Width * surface.Height; i++)
            {
                float allR = 0;
                float allG = 0;
                float allB = 0;
                float sbri = 0;

                int min = i * fontPixels;
                int max = min + fontPixels;

                for (int pixel = min; pixel < max; pixel++)
                {
                    Color color = pixels[indexes[pixel]];

                    allR += color.R;
                    allG += color.G;
                    allB += color.B;
                }

                // print our character
                byte sr = (byte)(allR / fontPixels);
                byte sg = (byte)(allG / fontPixels);
                byte sb = (byte)(allB / fontPixels);

                var newColor = new Color(sr, sg, sb);
                sbri = newColor.GetBrightness() * 255;

                allR = 0;
                allG = 0;
                allB = 0;

                Point surfacePoint = surface.GetPointFromIndex(i);
                if (UseBlockMode)
                {
                    if (sbri > 204)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, 219, newColor); //█
                    else if (sbri > 152)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, 178, newColor); //▓
                    else if (sbri > 100)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, 177, newColor); //▒
                    else if (sbri > 48)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, 176, newColor); //░
                    else
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, 0, Color.Black);
                }
                else
                {
                    if (sbri > 230)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'#', newColor);
                    else if (sbri > 207)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'&', newColor);
                    else if (sbri > 184)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'$', newColor);
                    else if (sbri > 161)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'X', newColor);
                    else if (sbri > 138)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'x', newColor);
                    else if (sbri > 115)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'=', newColor);
                    else if (sbri > 92)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'+', newColor);
                    else if (sbri > 69)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)';', newColor);
                    else if (sbri > 46)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)':', newColor);
                    else if (sbri > 23)
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, (int)'.', newColor);
                    else
                        surface.SetCharacter(surfacePoint.X, surfacePoint.Y, 0, Color.Black);
                }
            }
            );
            
            return surface;
        }

    }
}
