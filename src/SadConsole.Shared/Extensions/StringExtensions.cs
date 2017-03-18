using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole;
using SadConsole.StringParser;
using System.Windows;

namespace System
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
        /// Creates a <see cref="ColoredString"/> object from an existing string with the specified foreground and background, setting the ignore properties if needed.
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="foreground">The foreground color. If null, <see cref="ColoredString.IgnoreForeground"/> will be set.</param>
        /// <param name="background">The background color. If null, <see cref="ColoredString.IgnoreBackground"/> will be set.</param>
        /// <param name="mirror">The mirror setting. If null, <see cref="ColoredString.IgnoreMirror"/> will be set.</param>
        /// <returns>A <see cref="ColoredString"/> object instace.</returns>
        public static ColoredString CreateColored(this string value, Color? foreground = null, Color? background = null, SpriteEffects? mirror = null)
        {
            var stacks = new ParseCommandStacks();

            if (foreground.HasValue)
                stacks.AddSafe(new ParseCommandRecolor() { R = foreground.Value.R, G = foreground.Value.G, B = foreground.Value.B, A = foreground.Value.A, CommandType = CommandTypes.Foreground });

            if (background.HasValue)
                stacks.AddSafe(new ParseCommandRecolor() { R = background.Value.R, G = background.Value.G, B = background.Value.B, A = background.Value.A, CommandType = CommandTypes.Background });

            if (mirror.HasValue)
                stacks.AddSafe(new ParseCommandMirror() { Mirror = mirror.Value, CommandType = CommandTypes.Mirror });

            ColoredString newString = ColoredString.Parse(value, initialBehaviors: stacks);

            if (!foreground.HasValue)
                newString.IgnoreForeground = true;

            if (!background.HasValue)
                newString.IgnoreBackground = true;

            if (!mirror.HasValue)
                newString.IgnoreMirror = true;
            
            return newString;
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
            {
                newString[i].Foreground = ColorHelper.Lerp(startingForeground, endingForeground, (float)i / (float)value.Length);
            }

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
                newString[i].Foreground = ColorHelper.Lerp(startingForeground, endingForeground, (float)i / (float)value.Length);
                newString[i].Background = ColorHelper.Lerp(startingBackground, endingBackground, (float)i / (float)value.Length);
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
