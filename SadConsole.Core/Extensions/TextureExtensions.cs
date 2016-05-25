namespace Microsoft.Xna.Framework.Graphics
{
    using SadConsole;
    using SadConsole.Consoles;
    using System;

    public static class TextureExtensions
    {
        public static TextSurface ToSurface(this Texture2D image, Font font, bool blockMode = false)
        {
            TextSurface surface = new TextSurface(image.Width / font.Size.X, image.Height / font.Size.Y, font);
            Color[] pixels = new Color[image.Width * image.Height];
            image.GetData<Color>(pixels);

            System.Threading.Tasks.Parallel.For(0, image.Height / surface.Font.Size.Y, (h) =>
            //for (int h = 0; h < image.Height / surface.Font.Size.Y; h++)
            {
                int startY = (h * surface.Font.Size.Y);
                //System.Threading.Tasks.Parallel.For(0, image.Width / surface.Font.Size.X, (w) =>
                for (int w = 0; w < image.Width / surface.Font.Size.X; w++)
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

                            Color color = pixels[cY * image.Width + cX];

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
                            surface.SetCharacter(w, h, 219, newColor); //█
                        else if (sbri > 152)
                            surface.SetCharacter(w, h, 178, newColor); //▓
                        else if (sbri > 100)
                            surface.SetCharacter(w, h, 177, newColor); //▒
                        else if (sbri > 48)
                            surface.SetCharacter(w, h, 176, newColor); //░
                    }
                    else
                    {
                        if (sbri > 230)
                            surface.SetCharacter(w, h, (int)'#', newColor);
                        else if (sbri > 207)
                            surface.SetCharacter(w, h, (int)'&', newColor);
                        else if (sbri > 184)
                            surface.SetCharacter(w, h, (int)'$', newColor);
                        else if (sbri > 161)
                            surface.SetCharacter(w, h, (int)'X', newColor);
                        else if (sbri > 138)
                            surface.SetCharacter(w, h, (int)'x', newColor);
                        else if (sbri > 115)
                            surface.SetCharacter(w, h, (int)'=', newColor);
                        else if (sbri > 92)
                            surface.SetCharacter(w, h, (int)'+', newColor);
                        else if (sbri > 69)
                            surface.SetCharacter(w, h, (int)';', newColor);
                        else if (sbri > 46)
                            surface.SetCharacter(w, h, (int)':', newColor);
                        else if (sbri > 23)
                            surface.SetCharacter(w, h, (int)'.', newColor);
                    }
                }
            }
            );

            return surface;
        }

        public static void ToSurface(this Texture2D image, TextSurface surface, Color[] cachedColorArray, bool blockMode = false)
        {
            image.GetData<Color>(cachedColorArray);
            surface.Clear();
            System.Threading.Tasks.Parallel.For(0, image.Height / surface.Font.Size.Y, (h) =>
            //for (int h = 0; h < image.Height / surface.Font.Size.Y; h++)
            {
                int startY = (h * surface.Font.Size.Y);
                //System.Threading.Tasks.Parallel.For(0, image.Width / surface.Font.Size.X, (w) =>
                for (int w = 0; w < image.Width / surface.Font.Size.X; w++)
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

                            Color color = cachedColorArray[cY * image.Width + cX];

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
                            surface.SetCharacter(w, h, 219, newColor); //█
                        else if (sbri > 152)
                            surface.SetCharacter(w, h, 178, newColor); //▓
                        else if (sbri > 100)
                            surface.SetCharacter(w, h, 177, newColor); //▒
                        else if (sbri > 48)
                            surface.SetCharacter(w, h, 176, newColor); //░
                    }
                    else
                    {
                        if (sbri > 230)
                            surface.SetCharacter(w, h, (int)'#', newColor);
                        else if (sbri > 207)
                            surface.SetCharacter(w, h, (int)'&', newColor);
                        else if (sbri > 184)
                            surface.SetCharacter(w, h, (int)'$', newColor);
                        else if (sbri > 161)
                            surface.SetCharacter(w, h, (int)'X', newColor);
                        else if (sbri > 138)
                            surface.SetCharacter(w, h, (int)'x', newColor);
                        else if (sbri > 115)
                            surface.SetCharacter(w, h, (int)'=', newColor);
                        else if (sbri > 92)
                            surface.SetCharacter(w, h, (int)'+', newColor);
                        else if (sbri > 69)
                            surface.SetCharacter(w, h, (int)';', newColor);
                        else if (sbri > 46)
                            surface.SetCharacter(w, h, (int)':', newColor);
                        else if (sbri > 23)
                            surface.SetCharacter(w, h, (int)'.', newColor);
                    }
                }
            }
            );
        }
    }
}
