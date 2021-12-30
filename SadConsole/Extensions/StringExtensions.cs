using SadConsole;
using SadConsole.StringParser;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Helpers for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Aligns a string given a total character width and alignment style. Fills in the extra space with the space character.
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="alignment">The horizontal alignment.</param>
        /// <param name="totalWidth">The total width of the new string.</param>
        /// <returns>A new string instance.</returns>
        public static string Align(this string value, HorizontalAlignment alignment, int totalWidth) => value.Align(alignment, totalWidth, ' ');

        /// <summary>
        /// Aligns a string given a total character width and alignment style.
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="alignment">The horizontal alignment.</param>
        /// <param name="totalWidth">The total width of the new string.</param>
        /// <param name="fillCharacter">The character to use to fill in the extra spaces after alignment.</param>
        /// <returns>A new string instance.</returns>
        public static string Align(this string value, HorizontalAlignment alignment, int totalWidth, char fillCharacter)
        {
            string adjustedText = new string(fillCharacter, totalWidth);

            if (!string.IsNullOrEmpty(value))
            {

                if (alignment == HorizontalAlignment.Left)
                {
                    if (value.Length > totalWidth)
                        adjustedText = value.Substring(0, totalWidth);
                    else
                        adjustedText = value.PadRight(totalWidth, fillCharacter);
                }

                else if (alignment == HorizontalAlignment.Right)
                {
                    if (value.Length > totalWidth)
                        adjustedText = value.Substring(value.Length - totalWidth);
                    else
                        adjustedText = value.PadLeft(totalWidth, fillCharacter);
                }

                else
                {
                    if (value.Length > totalWidth)
                        adjustedText = value.Substring((value.Length - totalWidth) / 2, totalWidth);
                    else
                    {
                        int pad = (totalWidth - value.Length) / 2;
                        adjustedText = value.PadRight(pad + value.Length, fillCharacter).PadLeft(totalWidth, fillCharacter);
                    }
                }
            }

            return adjustedText;
        }

        /// <summary>
        /// Creates a <see cref="ColoredString"/> object from an existing string with the specified foreground and background, setting the ignore properties if needed.
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="foreground">The foreground color. If null, <see cref="ColoredString.IgnoreForeground"/> will be set.</param>
        /// <param name="background">The background color. If null, <see cref="ColoredString.IgnoreBackground"/> will be set.</param>
        /// <param name="mirror">The mirror setting. If null, <see cref="ColoredString.IgnoreMirror"/> will be set.</param>
        /// <param name="decorators">The decorators setting. If null, <see cref="ColoredString.IgnoreDecorators"/> will be set.</param>
        /// <returns>A <see cref="ColoredString"/> object instace.</returns>
        public static ColoredString CreateColored(this string value, Color? foreground = null, Color? background = null, Mirror? mirror = null, CellDecorator[] decorators = null)
        {
            ColoredString returnValue = new ColoredString(value);

            // Foreground
            if (foreground.HasValue)
                returnValue.SetForeground(foreground.Value);
            else
                returnValue.IgnoreForeground = true;

            // Background
            if (background.HasValue)
                returnValue.SetBackground(background.Value);
            else
                returnValue.IgnoreBackground = true;

            // Mirror
            if (mirror.HasValue)
                returnValue.SetMirror(mirror.Value);
            else
                returnValue.IgnoreMirror = true;

            // Decorators
            if (decorators != null)
                returnValue.SetDecorators(decorators);
            else
                returnValue.IgnoreDecorators = true;

            return returnValue;
        }

        /// <summary>
        /// Creates a <see cref="ColoredString"/> object from an existing string with the specified foreground gradient and cell effect. 
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="startingForeground">The starting foreground color to blend.</param>
        /// <param name="endingForeground">The ending foreground color to blend.</param>
        /// <returns>A <see cref="ColoredString"/> object instace.</returns>
        public static ColoredString CreateGradient(this string value, Color startingForeground, Color endingForeground)
        {
            ColoredString newString = new ColoredString(value);

            for (int i = 0; i < value.Length; i++)
                newString[i].Foreground = Color.Lerp(startingForeground, endingForeground, i / (float)value.Length);

            newString.IgnoreBackground = true;
            newString.IgnoreMirror = true;

            return newString;
        }

        /// <summary>
        /// Creates a <see cref="ColoredString"/> object from an existing string with the specified foreground gradient, background gradient, and cell effect. 
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="startingForeground">The starting foreground color to blend.</param>
        /// <param name="endingForeground">The ending foreground color to blend.</param>
        /// <param name="startingBackground">The starting background color to blend.</param>
        /// <param name="endingBackground">The ending background color to blend.</param>
        /// <returns>A <see cref="ColoredString"/> object instace.</returns>
        public static ColoredString CreateGradient(this string value, Color startingForeground, Color endingForeground, Color startingBackground, Color endingBackground)
        {
            ColoredString newString = new ColoredString(value);

            for (int i = 0; i < value.Length; i++)
            {
                newString[i].Foreground = Color.Lerp(startingForeground, endingForeground, i / (float)value.Length);
                newString[i].Background = Color.Lerp(startingBackground, endingBackground, i / (float)value.Length);
            }

            newString.IgnoreMirror = true;

            return newString;
        }

        /// <summary>
        /// Converts a string to a boolean when it is "0", "1", "true", or "false".
        /// </summary>
        /// <param name="item">The string to convert</param>
        /// <returns>The converted boolean value, otherwise false.</returns>
        public static bool ToBool(this string item)
        {
            if (int.TryParse(item, out int intValue))
                return System.Convert.ToBoolean(intValue);

            if (bool.TryParse(item, out bool boolValue))
                return bool.Parse(item);

            return false;
        }

        /// <summary>
        /// Returns a string of mask characters the same length as the input string.
        /// </summary>
        /// <param name="toMask">The string to mask.</param>
        /// <param name="mask">The mask to use.</param>
        /// <returns>A string of masks.</returns>
        public static string Masked(this string toMask, char mask) =>
            new string(mask, toMask.Length);
    }
}
