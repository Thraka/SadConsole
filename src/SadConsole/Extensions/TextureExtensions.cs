using SadConsole;

namespace Microsoft.Xna.Framework.Graphics
{
    public static class TextureExtensions
    {
        public static CellSurface ToSurface(this Texture2D image, SadConsole.Font font, bool blockMode = false)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            Color[] pixels = new Color[imageWidth * imageHeight];
            image.GetData<Color>(pixels);

            CellSurface surface = new CellSurface(imageWidth / font.Size.X, imageHeight / font.Size.Y);

            global::System.Threading.Tasks.Parallel.For(0, imageHeight / font.Size.Y, (h) =>
            //for (int h = 0; h < imageHeight / font.Size.Y; h++)
            {
                int startY = (h * font.Size.Y);
                //System.Threading.Tasks.Parallel.For(0, imageWidth / font.Size.X, (w) =>
                for (int w = 0; w < imageWidth / font.Size.X; w++)
                {
                    int startX = (w * font.Size.X);

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;

                    for (int y = 0; y < font.Size.Y; y++)
                    {
                        for (int x = 0; x < font.Size.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            Color color = pixels[cY * imageWidth + cX];

                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                        }
                    }

                    byte sr = (byte)(allR / (font.Size.X * font.Size.Y));
                    byte sg = (byte)(allG / (font.Size.X * font.Size.Y));
                    byte sb = (byte)(allB / (font.Size.X * font.Size.Y));

                    var newColor = new Color(sr, sg, sb);

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

        public static void ToSurface(this Texture2D image, CellSurface surface, Color[] cachedColorArray, Font font, bool blockMode = false)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            image.GetData<Color>(cachedColorArray);

            surface.Clear();
            global::System.Threading.Tasks.Parallel.For(0, imageHeight / font.Size.Y, (h) =>
            //for (int h = 0; h < imageHeight / font.Size.Y; h++)
            {
                int startY = (h * font.Size.Y);
                //System.Threading.Tasks.Parallel.For(0, imageWidth / font.Size.X, (w) =>
                for (int w = 0; w < imageWidth / font.Size.X; w++)
                {
                    int startX = (w * font.Size.X);

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;

                    for (int y = 0; y < font.Size.Y; y++)
                    {
                        for (int x = 0; x < font.Size.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            Color color = cachedColorArray[cY * imageWidth + cX];

                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                        }
                    }

                    byte sr = (byte)(allR / (font.Size.X * font.Size.Y));
                    byte sg = (byte)(allG / (font.Size.X * font.Size.Y));
                    byte sb = (byte)(allB / (font.Size.X * font.Size.Y));

                    var newColor = new Color(sr, sg, sb);

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

        public static void Save(this Texture2D target, string path)
        {
            using (System.IO.FileStream stream = System.IO.File.OpenWrite(path))
            {
                target.SaveAsPng(stream, target.Bounds.Width, target.Bounds.Height);
            }
        }
    }
}
