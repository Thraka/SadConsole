using SadConsole;
using SadConsole.Readers;

namespace SadConsole
{
    /// <summary>
    /// Various extension methods to classes that implement <see cref="ICellSurface"/> interface.
    /// </summary>
    public static class ICellSurfaceExtensions
    {
        readonly static char _spaceCharAlternative = 'a';

        /// <summary>
        /// Prints text using <see cref="TheDrawFont"/> and horizontal alignment specified. Calculates x coordinate. Truncates string to fit it in one line.
        /// </summary>
        /// <param name="cellSurface">The instance of <see cref="ICellSurface"/>.</param>
        /// <param name="y">Y coordinate of the surface.</param>
        /// <param name="text">Text to print.</param>
        /// <param name="drawFont">Instance of the <see cref="TheDrawFont"/> to use.</param>
        /// <param name="alignment"><see cref="HorizontalAlignment"/> to use.</param>
        /// <param name="padding">Amount of regular font characters used as horizontal padding on both sides of the output.</param>
        public static void PrintTheDraw(this ICellSurface cellSurface, int y, string text, TheDrawFont drawFont, HorizontalAlignment alignment, int padding = 0)
        {
            int spaceWidth = drawFont.IsCharacterSupported(' ') ? drawFont.GetCharacter(' ').Width : drawFont.GetCharacter(_spaceCharAlternative).Width,
                textLength = 0,
                printWidth = cellSurface.Width - padding * 2;
            string tempText = string.Empty;

            foreach (var item in text)
            {
                char currentChar = item;
                int charWidth = 0;

                if (drawFont.IsCharacterSupported(item))
                {
                    var charInfo = drawFont.GetCharacter(currentChar);
                    charWidth = charInfo.Width;
                }
                else
                {
                    currentChar = ' ';
                    charWidth = spaceWidth;
                }

                textLength += charWidth;

                if (textLength > printWidth)
                {
                    textLength -= charWidth;
                    break;
                }

                tempText += currentChar;
            }

            int x = alignment switch
            {
                HorizontalAlignment.Center => (printWidth - textLength) / 2,
                HorizontalAlignment.Right => printWidth - textLength,
                _ => 0
            };

            PrintTheDraw(cellSurface, x + padding, y, tempText, drawFont);
        }

        /// <summary>
        /// Prints text using TheDrawFont.
        /// </summary>
        /// <param name="cellSurface">The instance of <see cref="ICellSurface"/>.</param>
        /// <param name="x">X coordinate of the surface.</param>
        /// <param name="y">Y coordinate of the surface.</param>
        /// <param name="text">Text to print.</param>
        /// <param name="drawFont">Instance of the <see cref="TheDrawFont"/> to use.</param>
        public static void PrintTheDraw(this ICellSurface cellSurface, int x, int y, string text, TheDrawFont drawFont)
        {
            int xPos = x;
            int yPos = y;
            int tempHeight = 0;

            foreach (var item in text)
            {
                if (drawFont.IsCharacterSupported(item))
                {
                    var charInfo = drawFont.GetCharacter(item);

                    if (xPos + charInfo.Width >= cellSurface.Width)
                    {
                        yPos += tempHeight + 1;
                        xPos = x;
                    }

                    if (yPos >= cellSurface.Height)
                        break;

                    var surfaceCharacter = drawFont.GetSurface(item);

                    if (surfaceCharacter != null)
                    {
                        surfaceCharacter.Copy(cellSurface, xPos, yPos);

                        if (surfaceCharacter.Height > tempHeight)
                            tempHeight = surfaceCharacter.Height;
                    }

                    xPos += charInfo.Width;
                }
                else if (item == ' ' && drawFont.IsCharacterSupported(_spaceCharAlternative))
                {
                    xPos += drawFont.GetCharacter(_spaceCharAlternative).Width;
                }
            }
        }
    }
}
