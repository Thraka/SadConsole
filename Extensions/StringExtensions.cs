using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace System
{
    public static class StringEx
    {
        /// <summary>
        /// Aligns a string given a total character width and alignment style. Fills in the extra space with the space character.
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="alignment">The horizontal alignment.</param>
        /// <param name="totalWidth">The total width of the new string.</param>
        /// <returns>A new string instance.</returns>
        public static string Align(this string value, HorizontalAlignment alignment, int totalWidth)
        {
            return value.Align(alignment, totalWidth, ' ');
        }

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
                        adjustedText = adjustedText = value.Substring(0, totalWidth);
                    else
                        adjustedText = value.PadRight(totalWidth, fillCharacter);
                }

                else if (alignment == HorizontalAlignment.Right)
                {
                    if (value.Length > totalWidth)
                        adjustedText = adjustedText = value.Substring(value.Length - totalWidth);
                    else
                        adjustedText = value.PadLeft(totalWidth, fillCharacter);
                }

                else
                {
                    if (value.Length > totalWidth)
                        adjustedText = adjustedText = value.Substring((value.Length - totalWidth) / 2, totalWidth);
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
        /// Creates a <see cref="ColoredString"/> object from an existing string with the specified foreground, background, and cell effect.
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="effect">The cell effect.</param>
        /// <returns>A <see cref="ColoredString"/> object instace.</returns>
        public static ColoredString CreateColored(this string value, Color foreground, Color background, ICellEffect effect)
        {
            ColoredString newString = new ColoredString(value);
            newString.Foreground = foreground;
            newString.Background = background;
            newString.Effect = effect;
            newString.UpdateWithDefaults();
            return newString;
        }

        /// <summary>
        /// Creates a <see cref="ColoredString"/> object from an existing string with the specified foreground gradient and cell effect. 
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="startingForeground">The starting foreground color to blend.</param>
        /// <param name="endingForeground">The ending foreground color to blend.</param>
        /// <param name="effect">The cell effect.</param>
        /// <returns>A <see cref="ColoredString"/> object instace.</returns>
        public static ColoredString CreateGradient(this string value, Color startingForeground, Color endingForeground, ICellEffect effect)
        {
            ColoredString newString = new ColoredString(value);

            for (int i = 0; i < value.Length; i++)
            {
                newString[i].Foreground = Color.Lerp(startingForeground, endingForeground, (float)i / (float)value.Length);
                newString[i].Background = newString.Background;
                newString[i].Effect = newString.Effect;
            }

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
        /// <param name="effect">The cell effect.</param>
        /// <returns>A <see cref="ColoredString"/> object instace.</returns>
        public static ColoredString CreateGradient(this string value, Color startingForeground, Color endingForeground, Color startingBackground, Color endingBackground, ICellEffect effect)
        {
            ColoredString newString = new ColoredString(value);

            for (int i = 0; i < value.Length; i++)
            {
                newString[i].Foreground = Color.Lerp(startingForeground, endingForeground, (float)i / (float)value.Length);
                newString[i].Background = Color.Lerp(startingBackground, endingBackground, (float)i / (float)value.Length);
                newString[i].Effect = newString.Effect;
            }

            return newString;
        }

        /// <summary>
        /// Converts a string to a boolean when it is "0", "1", "true", or "false".
        /// </summary>
        /// <param name="item">The string to convert</param>
        /// <returns>The converted boolean value, otherwise false.</returns>
        public static bool ToBool(this string item)
        {
            int intValue;
            bool boolValue;

            if (int.TryParse(item, out intValue))
                return Convert.ToBoolean(intValue);

            if (bool.TryParse(item, out boolValue))
                return bool.Parse(item);

            return false;
        }
    }
}
