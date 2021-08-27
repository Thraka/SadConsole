using System;
using System.Text;
using SadConsole.Effects;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Extensions for the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions2
    {
        /// <summary>
        /// Converts a string into codepage 437.
        /// </summary>
        /// <param name="text">The string to convert</param>
        /// <param name="codepage">Optional codepage to provide.</param>
        /// <returns>A transformed string.</returns>
        public static string ToAscii(this string text, int codepage = 437)
        {
            // Converts characters such as ░▒▓│┤╡ ☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼■²ⁿ√·
            byte[] stringBytes = CodePagesEncodingProvider.Instance.GetEncoding(437).GetBytes(text);
            char[] stringChars = new char[stringBytes.Length];

            for (int i = 0; i < stringBytes.Length; i++)
                stringChars[i] = (char)stringBytes[i];

            return new string(stringChars);
        }
    }

    public static class SurfaceExtensions
    {
        /// <summary>
        /// Prints text that blinks out the characters each after the specified time, using the default foreground and background colors of the surface.
        /// </summary>
        /// <param name="surfaceObject">The surface to draw the text on.</param>
        /// <param name="text">The text to print.</param>
        /// <param name="position">The position where to print the text.</param>
        /// <param name="time">The time each glyph (one after the other) takes to print then fade.</param>
        /// <param name="effect">Optional effect to use. If <see langword="null"/> is passed, uses an instant fade.</param>
        public static void PrintFadingText(this IScreenSurface surfaceObject, string text, Point position, TimeSpan time, ICellEffect effect = null) =>
            PrintFadingText(surfaceObject,
                           text.CreateColored(surfaceObject.Surface.DefaultForeground, surfaceObject.Surface.DefaultBackground),
                           position,
                           time,
                           effect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surfaceObject">The surface to draw the text on.</param>
        /// <param name="text">The text to print.</param>
        /// <param name="position">The position where to print the text.</param>
        /// <param name="time">The time each glyph (one after the other) takes to print then fade.</param>
        /// <param name="effect">Optional effect to use. If <see langword="null"/> is passed, uses an instant fade.</param>
        public static void PrintFadingText(this IScreenSurface surfaceObject, ColoredString text, Point position, TimeSpan time, ICellEffect effect = null)
        {
            // If no effect is passed, create the default (no real color fade, just blink out)
            if (effect == null)
            {
                effect = new SadConsole.Effects.Fade()
                {
                    FadeForeground = true,
                    UseCellBackground = true,
                    FadeDuration = 0,
                    CloneOnAdd = true,
                    StartDelay = time.TotalSeconds
                };
            }

            // On the string passed, set the effect.
            text.SetEffect(effect);
            // Enable the effect on the text, default is to ignore it.
            text.IgnoreEffect = false;

            // Use an instruction set. Each instruction is run after the previous.
            var instructionSet = new SadConsole.Instructions.InstructionSet()

                .Instruct(
                    new SadConsole.Instructions.DrawString(text)
                    {
                        TotalTimeToPrint = (float)time.TotalSeconds,
                        Position = (1, 1),
                    })
                .Wait(time * 2) // delay long enough to let the effects finish before erasing
                .Code(
                    (screenObject, time) =>
                    {
                        // erase the area the text was written before restarting. This removes effect, color, and glyph
                        ((ScreenSurface)screenObject).Surface.Erase(1, 1, text.Length);
                        return true;
                    })
                ;

            instructionSet.RemoveOnFinished = true;

            surfaceObject.SadComponents.Add(instructionSet);
        }
    }
}
