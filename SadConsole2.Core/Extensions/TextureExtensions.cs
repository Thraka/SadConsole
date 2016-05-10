namespace Microsoft.Xna.Framework.Graphics
{
    using SadConsole;
    using System;

    public static class TextureExtensions
    {
        public static void DrawImageToSurface(this Texture2D texture, CellSurface surface, Point position, bool useBackground, Func<Color, Color, Color> blendOperation = null)
        {
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);

            int startX = position.X;
            int widthCounter = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (widthCounter >= texture.Width)
                {
                    widthCounter = 0;
                    position.X = startX;
                    position.Y++;
                }

                if (surface.IsValidCell(position.X, position.Y))
                {
                    int destinationIndex = position.ToIndex(surface.Width);

                    if (useBackground)
                    {
                        if (blendOperation == null)
                            surface[destinationIndex].Background = pixels[i];
                        else
                            surface[destinationIndex].Background = blendOperation(surface[destinationIndex].Background, pixels[i]);
                    }
                    else
                    {
                        if (blendOperation == null)
                            surface[destinationIndex].Foreground = pixels[i];
                        else
                            surface[destinationIndex].Foreground = blendOperation(surface[destinationIndex].Foreground, pixels[i]);
                    }
                }

                position.X++;
                widthCounter++;
            }
        }

        public static void DrawImageToSurface(this Texture2D texture, CellSurface surface, Point position, Action<int, Cell, Color> cellProcessor)
        {
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);

            int startX = position.X;
            int widthCounter = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (widthCounter >= texture.Width)
                {
                    widthCounter = 0;
                    position.X = startX;
                    position.Y++;
                }

                if (surface.IsValidCell(position.X, position.Y))
                {
                    int destinationIndex = position.ToIndex(surface.Width);

                    cellProcessor(destinationIndex, surface[destinationIndex], pixels[i]);
                }

                position.X++;
                widthCounter++;
            }
        }
    }
}
