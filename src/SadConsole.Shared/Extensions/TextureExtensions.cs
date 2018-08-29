﻿using SadConsole.Surfaces;

namespace Microsoft.Xna.Framework.Graphics
{
    public static class TextureExtensions
    {
        public static Basic ToSurface(this Texture2D image, SadConsole.Font font, bool blockMode = false)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            Color[] pixels = new Color[imageWidth * imageHeight];
            image.GetData<Color>(pixels);

            Basic surface = new Basic(imageWidth / font.Size.X, imageHeight / font.Size.Y, font);

            global::System.Threading.Tasks.Parallel.For((int) 0, (int)(imageHeight / surface.Font.Size.Y), (h) =>
            //for (int h = 0; h < imageHeight / surface.Font.Size.Y; h++)
            {
                int startY = (h * surface.Font.Size.Y);
                //System.Threading.Tasks.Parallel.For(0, imageWidth / surface.Font.Size.X, (w) =>
                for (int w = 0; w < imageWidth / surface.Font.Size.X; w++)
                {
                    int startX = (w * surface.Font.Size.X);

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;

                    for (int y = 0; y < surface.Font.Size.Y; y++)
                    {
                        for (int x = 0; x < surface.Font.Size.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            Color color = pixels[cY * imageWidth + cX];

                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                        }
                    }

                    byte sr = (byte)(allR / (surface.Font.Size.X * surface.Font.Size.Y));
                    byte sg = (byte)(allG / (surface.Font.Size.X * surface.Font.Size.Y));
                    byte sb = (byte)(allB / (surface.Font.Size.X * surface.Font.Size.Y));

                    var newColor = new Color(sr, sg, sb);

                    float sbri = newColor.GetBrightness() * 255;

                    if (blockMode)
                    {
                        if (sbri > 204)
                            surface.SetGlyph(w, h, 219, newColor); //█
                        else if (sbri > 152)
                            surface.SetGlyph(w, h, 178, newColor); //▓
                        else if (sbri > 100)
                            surface.SetGlyph(w, h, 177, newColor); //▒
                        else if (sbri > 48)
                            surface.SetGlyph(w, h, 176, newColor); //░
                    }
                    else
                    {
                        if (sbri > 230)
                            surface.SetGlyph(w, h, (int)'#', newColor);
                        else if (sbri > 207)
                            surface.SetGlyph(w, h, (int)'&', newColor);
                        else if (sbri > 184)
                            surface.SetGlyph(w, h, (int)'$', newColor);
                        else if (sbri > 161)
                            surface.SetGlyph(w, h, (int)'X', newColor);
                        else if (sbri > 138)
                            surface.SetGlyph(w, h, (int)'x', newColor);
                        else if (sbri > 115)
                            surface.SetGlyph(w, h, (int)'=', newColor);
                        else if (sbri > 92)
                            surface.SetGlyph(w, h, (int)'+', newColor);
                        else if (sbri > 69)
                            surface.SetGlyph(w, h, (int)';', newColor);
                        else if (sbri > 46)
                            surface.SetGlyph(w, h, (int)':', newColor);
                        else if (sbri > 23)
                            surface.SetGlyph(w, h, (int)'.', newColor);
                    }
                }
            }
            );

            return surface;
        }

        public static void ToSurface(this Texture2D image, Basic surface, Color[] cachedColorArray, bool blockMode = false)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            image.GetData<Color>(cachedColorArray);

            surface.Clear();
            global::System.Threading.Tasks.Parallel.For(0, imageHeight / surface.Font.Size.Y, (h) =>
            //for (int h = 0; h < imageHeight / surface.Font.Size.Y; h++)
            {
                int startY = (h * surface.Font.Size.Y);
                //System.Threading.Tasks.Parallel.For(0, imageWidth / surface.Font.Size.X, (w) =>
                for (int w = 0; w < imageWidth / surface.Font.Size.X; w++)
                {
                    int startX = (w * surface.Font.Size.X);

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;

                    for (int y = 0; y < surface.Font.Size.Y; y++)
                    {
                        for (int x = 0; x < surface.Font.Size.X; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            Color color = cachedColorArray[cY * imageWidth + cX];

                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                        }
                    }

                    byte sr = (byte)(allR / (surface.Font.Size.X * surface.Font.Size.Y));
                    byte sg = (byte)(allG / (surface.Font.Size.X * surface.Font.Size.Y));
                    byte sb = (byte)(allB / (surface.Font.Size.X * surface.Font.Size.Y));

                    var newColor = new Color(sr, sg, sb);

                    float sbri = newColor.GetBrightness() * 255;

                    if (blockMode)
                    {
                        if (sbri > 204)
                            surface.SetGlyph(w, h, 219, newColor); //█
                        else if (sbri > 152)
                            surface.SetGlyph(w, h, 178, newColor); //▓
                        else if (sbri > 100)
                            surface.SetGlyph(w, h, 177, newColor); //▒
                        else if (sbri > 48)
                            surface.SetGlyph(w, h, 176, newColor); //░
                    }
                    else
                    {
                        if (sbri > 230)
                            surface.SetGlyph(w, h, (int)'#', newColor);
                        else if (sbri > 207)
                            surface.SetGlyph(w, h, (int)'&', newColor);
                        else if (sbri > 184)
                            surface.SetGlyph(w, h, (int)'$', newColor);
                        else if (sbri > 161)
                            surface.SetGlyph(w, h, (int)'X', newColor);
                        else if (sbri > 138)
                            surface.SetGlyph(w, h, (int)'x', newColor);
                        else if (sbri > 115)
                            surface.SetGlyph(w, h, (int)'=', newColor);
                        else if (sbri > 92)
                            surface.SetGlyph(w, h, (int)'+', newColor);
                        else if (sbri > 69)
                            surface.SetGlyph(w, h, (int)';', newColor);
                        else if (sbri > 46)
                            surface.SetGlyph(w, h, (int)':', newColor);
                        else if (sbri > 23)
                            surface.SetGlyph(w, h, (int)'.', newColor);
                    }
                }
            }
            );
        }

        public static void Save(this RenderTarget2D target, string path)
        {
            using (var stream = System.IO.File.OpenWrite(path))
                target.SaveAsPng(stream, target.Bounds.Width, target.Bounds.Height);
        }
    }
}
