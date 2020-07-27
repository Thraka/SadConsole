using System;
using System.Collections.Generic;
using System.Reflection;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Various extension methods to <see cref="Color"/> class.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Custom color mappings for the <see cref="FromName(string)"/> and <see cref="FromParser(Color, string, out bool, out bool, out bool, out bool, out bool)"/> methods. Key names should be lowercase.
        /// </summary>
        public static Dictionary<string, Color> ColorMappings = new Dictionary<string, Color>(162);

        /// <summary>
        /// Creates an array of colors that includes the <paramref name="color"/> and <paramref name="endingColor"/> and <paramref name="steps"/> of colors between them.
        /// </summary>
        /// <param name="color">The starting color which will be at index 0 in the array.</param>
        /// <param name="endingColor">The ending color which will be at index `steps - 1` in the array.</param>
        /// <param name="steps">The gradient steps in the array which uses <see cref="Color.Lerp(Color, Color, float)"/>.</param>
        /// <returns>An array of colors.</returns>
        public static Color[] LerpSteps(this Color color, Color endingColor, int steps)
        {
            Color[] colors = new Color[steps];

            float stopStrength = 1f / (steps - 1);

            float lerpTotal = 0f;

            colors[0] = color;
            colors[steps - 1] = endingColor;

            for (int i = 1; i < steps - 1; i++)
            {
                lerpTotal += stopStrength;

                colors[i] = Color.Lerp(color, endingColor, lerpTotal);
            }

            return colors;
        }

        /// <summary>
        /// Sets the color values based on HSL instead of RGB.
        /// </summary>
        /// <param name="color">The color to change.</param>
        /// <param name="h">The hue amount.</param>
        /// <param name="s">The saturation amount.</param>
        /// <param name="l">The luminance amount.</param>
        /// <remarks>Taken from http://www.easyrgb.com/index.php?X=MATH&amp;H=19#text19 </remarks>
        public static Color SetHSL(this Color color, float h, float s, float l)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (s == 0f)
            {
                r = (byte)(l * 255);
                g = (byte)(l * 255);
                b = (byte)(l * 255);
            }
            else
            {
                float var_2;
                float var_1;

                if (l < 0.5)
                {
                    var_2 = l * (1 + s);
                }
                else
                {
                    var_2 = (l + s) - (s * l);
                }

                var_1 = 2 * l - var_2;

                r = (byte)(255 * Hue_2_RGB(var_1, var_2, h + (1 / 3)));
                g = (byte)(255 * Hue_2_RGB(var_1, var_2, h));
                b = (byte)(255 * Hue_2_RGB(var_1, var_2, h - (1 / 3)));
            }

            return new Color(r, g, b);
        }

        private static float Hue_2_RGB(float v1, float v2, float vH)
        {
            if (vH < 0)
            {
                vH += 1;
            }

            if (vH > 1)
            {
                vH -= 1;
            }

            if ((6 * vH) < 1)
            {
                return (v1 + (v2 - v1) * 6 * vH);
            }

            if ((2 * vH) < 1)
            {
                return (v2);
            }

            if ((3 * vH) < 2)
            {
                return (v1 + (v2 - v1) * ((2 / 3) - vH) * 6);
            }

            return (v1);
        }

        /// <summary>
        /// Gets a random color.
        /// </summary>
        /// <param name="color">The color object to start with. Will be overridden.</param>
        /// <param name="random">A random object to get numbers from.</param>
        /// <returns>A new color.</returns>
        public static Color GetRandomColor(this Color color, Random random) => new Color((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));

        /// <summary>
        /// Returns a new Color using only the Red value of this color.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with only the red channel set.</returns>
        public static Color RedOnly(this Color color) => new Color(color.R, 0, 0);

        /// <summary>
        /// Returns a new Color using only the Green value of this color.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with only the green channel set.</returns>
        public static Color GreenOnly(this Color color) => new Color(0, color.G, 0);

        /// <summary>
        /// Returns a new Color using only the Blue value of this color.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with only the blue channel set.</returns>
        public static Color BlueOnly(this Color color) => new Color(0, 0, color.B);

        /// <summary>
        /// Returns a new Color using only the Alpha value of this color.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with only the alpha channel set.</returns>
        public static Color AlphaOnly(this Color color) => new Color((byte)0, (byte)0, (byte)0, color.A);

        /// <summary>
        /// Returns a new color with the red channel set to 0.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the red channel cleared.</returns>
        public static Color ClearRed(this Color color) => new Color((byte)0, color.G, color.B, color.A);

        /// <summary>
        /// Returns a new color with the green channel set to 0.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the green channel cleared.</returns>
        public static Color ClearGreen(this Color color) => new Color(color.R, (byte)0, color.B, color.A);

        /// <summary>
        /// Returns a new color with the blue channel set to 0.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the blue channel cleared.</returns>
        public static Color ClearBlue(this Color color) => new Color(color.R, color.G, (byte)0, color.A);

        /// <summary>
        /// Returns a new color with the alpha channel set to 0.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the alpha channel cleared.</returns>
        public static Color ClearAlpha(this Color color) => new Color(color.R, color.G, color.B, (byte)0);

        /// <summary>
        /// Returns a new color with the red channel set to 255.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the red channel fully set.</returns>
        public static Color FillRed(this Color color) => new Color((byte)255, color.G, color.B, color.A);

        /// <summary>
        /// Returns a new color with the green channel set to 255.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the green channel fully set.</returns>
        public static Color FillGreen(this Color color) => new Color(color.R, (byte)255, color.B, color.A);

        /// <summary>
        /// Returns a new color with the blue channel set to 255.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the blue channel fully set.</returns>
        public static Color FillBlue(this Color color) => new Color(color.R, color.G, (byte)255, color.A);

        /// <summary>
        /// Returns a new color with the alpha channel set to 255.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <returns>A color with the alpha channel fully set.</returns>
        public static Color FillAlpha(this Color color) => new Color(color.R, color.G, color.B, (byte)255);

        /// <summary>
        /// Returns a new color with the red channel set to the specified value.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <param name="value">The new value for the red channel.</param>
        /// <returns>A color with the red channel altered.</returns>
        public static Color SetRed(this Color color, byte value) => new Color(value, color.G, color.B, color.A);

        /// <summary>
        /// Returns a new color with the green channel set to the specified value.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <param name="value">The new value for the green channel.</param>
        /// <returns>A color with the green channel altered.</returns>
        public static Color SetGreen(this Color color, byte value) => new Color(color.R, value, color.B, color.A);

        /// <summary>
        /// Returns a new color with the blue channel set to the specified value.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <param name="value">The new value for the blue channel.</param>
        /// <returns>A color with the blue channel altered.</returns>
        public static Color SetBlue(this Color color, byte value) => new Color(color.R, color.G, value, color.A);

        /// <summary>
        /// Returns a new color with the alpha channel set to the specified value.
        /// </summary>
        /// <param name="color">Object instance.</param>
        /// <param name="value">The new value for the alpha channel.</param>
        /// <returns>A color with the alpha channel altered.</returns>
        public static Color SetAlpha(this Color color, byte value) => new Color(color.R, color.G, color.B, value);


        /// <summary>
        /// Gets the luma of an existing color.
        /// </summary>
        /// <param name="color">The color to calculate the luma from.</param>
        /// <returns>A value based on this code: (color.R + color.R + color.B + color.G + color.G + color.G) / 6f</returns>
        public static float GetLuma(this Color color) => (color.R + color.R + color.B + color.G + color.G + color.G) / 6f;

        /// <summary>
        /// Gets the brightness of a color.
        /// </summary>
        /// <param name="color">The color to process.</param>
        /// <returns>The brightness value.</returns>
        /// <remarks>Taken from the mono source code.</remarks>
        public static float GetBrightness(this Color color)
        {
            byte minval = Math.Min(color.R, Math.Min(color.G, color.B));
            byte maxval = Math.Max(color.R, Math.Max(color.G, color.B));

            return (float)(maxval + minval) / 510;
        }

        /// <summary>
        /// Gets the saturation of a color.
        /// </summary>
        /// <param name="color">The color to process.</param>
        /// <returns>The saturation value.</returns>
        /// <remarks>Taken from the mono source code.</remarks>
        public static float GetSaturation(this Color color)
        {
            byte minval = Math.Min(color.R, Math.Min(color.G, color.B));
            byte maxval = Math.Max(color.R, Math.Max(color.G, color.B));


            if (maxval == minval)
            {
                return 0.0f;
            }

            int sum = maxval + minval;
            if (sum > 255)
            {
                sum = 510 - sum;
            }

            return (float)(maxval - minval) / sum;
        }

        /// <summary>
        /// Gets the hue of a color.
        /// </summary>
        /// <param name="color">The color to process.</param>
        /// <returns>The hue value.</returns>
        /// <remarks>Taken from the mono source code.</remarks>
        public static float GetHue(this Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            byte minval = (byte)Math.Min(r, Math.Min(g, b));
            byte maxval = (byte)Math.Max(r, Math.Max(g, b));


            if (maxval == minval)
            {
                return 0.0f;
            }

            float diff = maxval - minval;
            float rnorm = (maxval - r) / diff;
            float gnorm = (maxval - g) / diff;
            float bnorm = (maxval - b) / diff;


            float hue = 0.0f;
            if (r == maxval)
            {
                hue = 60.0f * (6.0f + bnorm - gnorm);
            }

            if (g == maxval)
            {
                hue = 60.0f * (2.0f + rnorm - bnorm);
            }

            if (b == maxval)
            {
                hue = 60.0f * (4.0f + gnorm - rnorm);
            }

            if (hue > 360.0f)
            {
                hue = hue - 360.0f;
            }

            return hue;
        }

        /// <summary>
        /// Converts a color to the format used by <see cref="SadConsole.StringParser.ParseCommandRecolor"/> command.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>A string in this format R,G,B,A so for <see cref="Color.Green"/> you would get <code>0,128,0,255</code>.</returns>
        public static string ToParser(this Color color) => $"{color.R},{color.G},{color.B},{color.A}";

        /// <summary>
        /// Gets a color in the format of <see cref="SadConsole.StringParser.ParseCommandRecolor"/>.
        /// </summary>
        /// <param name="color">The color to use as a base.</param>
        /// <param name="value">The string parser color command.</param>
        /// <param name="keepR">Indicates that command wanted to keep the Red color channel.</param>
        /// <param name="keepG">Indicates that command wanted to keep the Green color channel.</param>
        /// <param name="keepB">Indicates that command wanted to keep the Blue color channel.</param>
        /// <param name="keepA">Indicates that command wanted to keep the Alpha color channel.</param>
        /// <param name="useDefault">Indicates that command wanted to use the default values passed.</param>
        /// <returns></returns>
        public static Color FromParser(this Color color, string value, out bool keepR, out bool keepG, out bool keepB, out bool keepA, out bool useDefault)
        {
            useDefault = false;
            keepR = false;
            keepG = false;
            keepB = false;
            keepA = false;

            string exceptionMessage = "Cannot parse color string";

            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            if (value.Contains(","))
            {
                string[] channels = value.Trim(' ').Split(',');

                if (channels.Length >= 3)
                {

                    byte colorValue;

                    // Red
                    if (channels[0] == "x")
                    {
                        keepR = true;
                    }
                    else if (byte.TryParse(channels[0], out colorValue))
                    {
                        r = colorValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }

                    // Green
                    if (channels[1] == "x")
                    {
                        keepG = true;
                    }
                    else if (byte.TryParse(channels[1], out colorValue))
                    {
                        g = colorValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }

                    // Blue
                    if (channels[2] == "x")
                    {
                        keepB = true;
                    }
                    else if (byte.TryParse(channels[2], out colorValue))
                    {
                        b = colorValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }

                    if (channels.Length == 4)
                    {
                        // Alpha
                        if (channels[3] == "x")
                        {
                            keepA = true;
                        }
                        else if (byte.TryParse(channels[3], out colorValue))
                        {
                            a = colorValue;
                        }
                        else
                        {
                            throw new ArgumentException(exceptionMessage);
                        }
                    }
                    else
                    {
                        a = 255;
                    }

                    return new Color(r, g, b, a);
                }
                else
                {
                    throw new ArgumentException(exceptionMessage);
                }
            }
            else if (value == "default")
            {
                useDefault = true;
                return new Color(r, g, b, a);
            }
            else
            {
                value = value.ToLower();

                if (ColorMappings.ContainsKey(value))
                    return ColorMappings[value];
                else
                    throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        /// Searches <see cref="ColorMappings"/> for a defined color.
        /// </summary>
        /// <param name="name">The name of a color.</param>
        /// <returns>A color.</returns>
        public static Color FromName(string name)
        {
            if (ColorMappings.ContainsKey(name))
                return ColorMappings[name];
            throw new ArgumentException($"Unable to find a color with the name {name}.");
        }

        /// <summary>
        /// Searches <see cref="ColorMappings"/> for a defined color. If color is not defined, the color specified by <paramref name="defaultColor"/> is returned.
        /// </summary>
        /// <param name="name">The name of a color.</param>
        /// <param name="defaultColor">Fallback color.</param>
        /// <returns>A color.</returns>
        public static Color FromName(string name, Color defaultColor)
        {
            if (ColorMappings.ContainsKey(name))
                return ColorMappings[name];
            return defaultColor;
        }
    }
}
