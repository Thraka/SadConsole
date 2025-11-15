using System.Collections.Generic;
using System.Linq;
using System;
using SadRogue.Primitives;

namespace SadConsole;

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
        string adjustedText = new(fillCharacter, totalWidth);

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
    /// <returns>A <see cref="ColoredString"/> object instance.</returns>
    public static ColoredString CreateColored(this string value, Color? foreground = null, Color? background = null, Mirror? mirror = null, CellDecorator[]? decorators = null)
    {
        ColoredString returnValue = new(value);

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
    /// <returns>A <see cref="ColoredString"/> object instance.</returns>
    public static ColoredString CreateGradient(this string value, Color startingForeground, Color endingForeground)
    {
        ColoredString newString = new(value);

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
    /// <returns>A <see cref="ColoredString"/> object instance.</returns>
    public static ColoredString CreateGradient(this string value, Color startingForeground, Color endingForeground, Color startingBackground, Color endingBackground)
    {
        ColoredString newString = new(value);

        for (int i = 0; i < value.Length; i++)
        {
            newString[i].Foreground = Color.Lerp(startingForeground, endingForeground, i / (float)value.Length);
            newString[i].Background = Color.Lerp(startingBackground, endingBackground, i / (float)value.Length);
        }

        newString.IgnoreMirror = true;

        return newString;
    }

    /// <summary>
    /// Wraps text into lines by words, long words are also properly wrapped into multiple lines.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="maxCharsPerLine">The maximum number of characters per line of text returned.</param>
    /// <returns>Each line in the string.</returns>
    public static IEnumerable<string> WordWrap(this string text, int maxCharsPerLine)
    {
        string line = "";
        int availableLength = maxCharsPerLine;
        string[] words = text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .SelectMany(a => System.Text.RegularExpressions.Regex.Split(a, @"(?=[\n])")).ToArray(); // Regex split will also split \n but keep \n part of the parts
        foreach (string w in words)
        {
            string word = w;
            if (word == string.Empty)
                continue;

            int wordLength = word.Length;
            if (wordLength >= maxCharsPerLine)
            {
                if (availableLength > 0)
                {
                    var lineValue = word.Substring(0, availableLength);
                    if (lineValue.Contains('\n'))
                    {
                        var splitLine = lineValue.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < splitLine.Length - 1; i++)
                            yield return line += splitLine[i];
                        line = string.Empty;
                        word = splitLine[splitLine.Length - 1];
                    }
                    else
                    {
                        yield return line += word.Substring(0, availableLength);
                        line = string.Empty;
                        word = word[availableLength..];
                    }
                }
                else
                {
                    yield return line;
                    line = string.Empty;
                }
                availableLength = maxCharsPerLine;
                for (int count = 0; count < word.Length; count++)
                {
                    char ch = word.ElementAt(count);

                    if (ch == '\n')
                    {
                        yield return line;
                        line = string.Empty;
                        availableLength = maxCharsPerLine;
                        continue;
                    }

                    line += ch;
                    availableLength--;

                    if (availableLength == 0)
                    {
                        yield return line;
                        line = string.Empty;
                        availableLength = maxCharsPerLine;
                    }
                }
                if (availableLength > 0)
                {
                    line += " ";
                    availableLength--;
                }
                continue;
            }

            // Attempt to cut of early, if the word doesn't fit the line anymore
            if (word.Length > availableLength)
            {
                yield return line;
                line = string.Empty;
                availableLength = maxCharsPerLine;
            }

            foreach (var ch in word)
            {
                if (availableLength == 0)
                {
                    yield return line;
                    line = string.Empty;
                    availableLength = maxCharsPerLine;
                }

                if (ch == '\n')
                {
                    yield return line;
                    line = string.Empty;
                    availableLength = maxCharsPerLine;
                    continue;
                }

                line += ch;
                availableLength--;
            }
            if (availableLength > 0)
            {
                line += " ";
                availableLength--;
            }
        }

        if (!string.IsNullOrWhiteSpace(line))
            yield return line.TrimEnd();
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
        new(mask, toMask.Length);
}
