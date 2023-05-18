using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Readers;

/// <summary>
/// Reads a texture to a cached surface. Used for animation.
/// </summary>
public class TextureToSurfaceReader
{
    private readonly Console _surface;
    private readonly Color[] _pixels;
    private readonly int[] _indexes;
    private readonly int _fontPixels;

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
    /// <param name="fontSize">Size of the font.</param>
    public TextureToSurfaceReader(int pixelWidth, int pixelHeight, IFont font, SadRogue.Primitives.Point fontSize)
    {
        _pixels = new Color[pixelWidth * pixelHeight];
        _indexes = new int[pixelWidth * pixelHeight];
        _surface = new Console(pixelWidth / fontSize.X, pixelHeight / fontSize.Y) { Font = font, FontSize = fontSize };
        _fontPixels = fontSize.X * fontSize.Y;

        // build the indexes
        int currentIndex = 0;
        for (int h = 0; h < _surface.Height; h++)
        {
            int startY = (h * _surface.FontSize.Y);
            //System.Threading.Tasks.Parallel.For(0, image.Width / surface.Font.Size.X, (w) =>
            for (int w = 0; w < _surface.Width; w++)
            {
                int startX = (w * _surface.FontSize.X);

                for (int y = 0; y < _surface.FontSize.Y; y++)
                {
                    for (int x = 0; x < _surface.FontSize.X; x++)
                    {
                        int cY = y + startY;
                        int cX = x + startX;

                        _indexes[currentIndex] = cY * pixelWidth + cX;
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
    public Console GetSurface(Texture2D image)
    {
        _surface.Clear();

        image.GetData<Color>(_pixels);

        System.Threading.Tasks.Parallel.For(0, _surface.Width * _surface.Height, (i) =>
        //for (int i = 0; i < surface.Width * surface.Height; i++)
        {
            int allR = 0;
            int allG = 0;
            int allB = 0;

            int min = i * _fontPixels;
            int max = min + _fontPixels;

            for (int pixel = min; pixel < max; pixel++)
            {
                Color color = _pixels[_indexes[pixel]];

                allR += color.R;
                allG += color.G;
                allB += color.B;
            }

            // print our character
            byte sr = (byte)(allR / _fontPixels);
            byte sg = (byte)(allG / _fontPixels);
            byte sb = (byte)(allB / _fontPixels);

            SadRogue.Primitives.Color newColor = new Color(sr, sg, sb).ToSadRogueColor();
            float sbri = newColor.GetHSLLightness() * 255;


            SadRogue.Primitives.Point surfacePoint = SadRogue.Primitives.Point.FromIndex(i, _surface.Width);
            if (UseBlockMode)
            {
                if (sbri > 204)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 219, newColor); //█
                else if (sbri > 152)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 178, newColor); //▓
                else if (sbri > 100)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 177, newColor); //▒
                else if (sbri > 48)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 176, newColor); //░
                else
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 0, SadRogue.Primitives.Color.Black);
            }
            else
            {
                if (sbri > 230)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, '#', newColor);
                else if (sbri > 207)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, '&', newColor);
                else if (sbri > 184)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, '$', newColor);
                else if (sbri > 161)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 'X', newColor);
                else if (sbri > 138)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 'x', newColor);
                else if (sbri > 115)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, '=', newColor);
                else if (sbri > 92)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, '+', newColor);
                else if (sbri > 69)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, ';', newColor);
                else if (sbri > 46)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, ':', newColor);
                else if (sbri > 23)
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, '.', newColor);
                else
                    _surface.SetGlyph(surfacePoint.X, surfacePoint.Y, 0, SadRogue.Primitives.Color.Black);
            }
        }
        );

        return _surface;
    }

}
