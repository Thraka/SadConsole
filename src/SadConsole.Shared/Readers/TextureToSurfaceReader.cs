using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SadConsole.Surfaces;

namespace SadConsole.Readers
{
    /// <summary>
    /// Reads a texture to a cached surface. Used for animation.
    /// </summary>
    public class TextureToSurfaceReader
    {
        int width;
        int height;
        BasicSurface surface;
        Color[] pixels;
        int[] indexes;
        int fontPixels;
        SurfaceEditor editor;

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
            surface = new BasicSurface(pixelWidth / font.Size.X, pixelHeight / font.Size.Y, font);
            fontPixels = font.Size.X * font.Size.Y;
            editor = new SurfaceEditor(surface);

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
        public BasicSurface GetSurface(Texture2D image)
        {
            editor.Clear();

            image.GetData<Color>(pixels);

            System.Threading.Tasks.Parallel.For(0, surface.Width * surface.Height, (i) =>
            //for (int i = 0; i < surface.Width * surface.Height; i++)
            {
                int allR = 0;
                int allG = 0;
                int allB = 0;

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
                float sbri = newColor.GetBrightness() * 255;


                Point surfacePoint = surface.GetPointFromIndex(i);
                if (UseBlockMode)
                {
                    if (sbri > 204)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, 219, newColor); //█
                    else if (sbri > 152)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, 178, newColor); //▓
                    else if (sbri > 100)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, 177, newColor); //▒
                    else if (sbri > 48)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, 176, newColor); //░
                    else
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, 0, Color.Black);
                }
                else
                {
                    if (sbri > 230)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'#', newColor);
                    else if (sbri > 207)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'&', newColor);
                    else if (sbri > 184)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'$', newColor);
                    else if (sbri > 161)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'X', newColor);
                    else if (sbri > 138)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'x', newColor);
                    else if (sbri > 115)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'=', newColor);
                    else if (sbri > 92)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'+', newColor);
                    else if (sbri > 69)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)';', newColor);
                    else if (sbri > 46)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)':', newColor);
                    else if (sbri > 23)
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, (int)'.', newColor);
                    else
                        editor.SetGlyph(surfacePoint.X, surfacePoint.Y, 0, Color.Black);
                }
            }
            );
            
            return surface;
        }

    }
}
