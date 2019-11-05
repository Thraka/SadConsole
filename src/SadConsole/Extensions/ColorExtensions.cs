#if XNA
#endif

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Classic 16 ansi color values.
    /// </summary>
    public static class ColorAnsi
    {
        /// <summary>
        /// The black ansi color (0, 0, 0).
        /// </summary>
        public static Color Black = new Color(0, 0, 0);
        /// <summary>
        /// The Red ansi color (170, 0, 0).
        /// </summary>
        public static Color Red = new Color(170, 0, 0);
        /// <summary>
        /// The Green ansi color 0, 170, 0).
        /// </summary>
        public static Color Green = new Color(0, 170, 0);
        /// <summary>
        /// The Yellow ansi color (170, 85, 0).
        /// </summary>
        public static Color Yellow = new Color(170, 85, 0);
        /// <summary>
        /// The Blue ansi color (0, 0, 170).
        /// </summary>
        public static Color Blue = new Color(0, 0, 170);
        /// <summary>
        /// The Magenta ansi color (170, 0, 170).
        /// </summary>
        public static Color Magenta = new Color(170, 0, 170);
        /// <summary>
        /// The Cyan ansi color (0, 170, 170).
        /// </summary>
        public static Color Cyan = new Color(0, 170, 170);
        /// <summary>
        /// The White ansi color (170, 170, 170).
        /// </summary>
        public static Color White = new Color(170, 170, 170);

        /// <summary>
        /// The BlackBright ansi color (85, 85, 85).
        /// </summary>
        public static Color BlackBright = new Color(85, 85, 85);
        /// <summary>
        /// The RedBright ansi color (255, 85, 85).
        /// </summary>
        public static Color RedBright = new Color(255, 85, 85);
        /// <summary>
        /// The GreenBright ansi color (85, 255, 85).
        /// </summary>
        public static Color GreenBright = new Color(85, 255, 85);
        /// <summary>
        /// The YellowBright ansi color (255, 255, 85).
        /// </summary>
        public static Color YellowBright = new Color(255, 255, 85);
        /// <summary>
        /// The BlueBright ansi color (85, 85, 255).
        /// </summary>
        public static Color BlueBright = new Color(85, 85, 255);
        /// <summary>
        /// The MagentaBright ansi color (255, 85, 255).
        /// </summary>
        public static Color MagentaBright = new Color(255, 85, 255);
        /// <summary>
        /// The CyanBright ansi color (85, 255, 255).
        /// </summary>
        public static Color CyanBright = new Color(85, 255, 255);
        /// <summary>
        /// The WhiteBright ansi color (255, 255, 255).
        /// </summary>
        public static Color WhiteBright = new Color(255, 255, 255);
    }

    /// <summary>
    /// Various extension methods to <see cref="Color"/> class.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Custom color mappings for the <see cref="FromParser(Color, string, out bool, out bool, out bool, out bool, out bool)"/> method.
        /// </summary>
        public static Dictionary<string, Color> ColorMappings = new Dictionary<string, Color>(16) { { "ansiblack", ColorAnsi.Black },
                                                                                                    { "ansired", ColorAnsi.Red },
                                                                                                    { "ansigreen", ColorAnsi.Green },
                                                                                                    { "ansiyellow", ColorAnsi.Yellow },
                                                                                                    { "ansiblue", ColorAnsi.Blue },
                                                                                                    { "ansimagenta", ColorAnsi.Magenta },
                                                                                                    { "ansicyan", ColorAnsi.Cyan },
                                                                                                    { "ansiwhite", ColorAnsi.White },
                                                                                                    { "ansiblackbright", ColorAnsi.BlackBright },
                                                                                                    { "ansiredbright", ColorAnsi.RedBright },
                                                                                                    { "ansigreenbright", ColorAnsi.GreenBright },
                                                                                                    { "ansiyellowbright", ColorAnsi.YellowBright },
                                                                                                    { "ansibluebright", ColorAnsi.BlueBright },
                                                                                                    { "ansimagentabright", ColorAnsi.MagentaBright },
                                                                                                    { "ansicyanbright", ColorAnsi.CyanBright },
                                                                                                    { "ansiwhitebright", ColorAnsi.WhiteBright } };

        /// <summary>
        /// Wrapper for <see cref="Color.PackedValue"/>.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static uint ToInteger(this Color color) => color.PackedValue;

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
        public static void SetHSL(this Color color, float h, float s, float l)
        {
            if (s == 0f)
            {
                color.R = (byte)(l * 255);
                color.G = (byte)(l * 255);
                color.B = (byte)(l * 255);
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

                color.R = (byte)(255 * Hue_2_RGB(var_1, var_2, h + (1 / 3)));
                color.G = (byte)(255 * Hue_2_RGB(var_1, var_2, h));
                color.B = (byte)(255 * Hue_2_RGB(var_1, var_2, h - (1 / 3)));
            }
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
        /// Converts a color to the format used by <see cref="SadConsole.ParseCommandRecolor"/> command.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>A string in this format R,G,B,A so for <see cref="Color.Green"/> you would get <code>0,128,0,255</code>.</returns>
        public static string ToParser(this Color color) => $"{color.R},{color.G},{color.B},{color.A}";

        /// <summary>
        /// Gets a color in the format of <see cref="SadConsole.ParseCommandRecolor"/>.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color FromParser(this Color color, string value, out bool keepR, out bool keepG, out bool keepB, out bool keepA, out bool useDefault)
        {
            useDefault = false;
            keepR = false;
            keepG = false;
            keepB = false;
            keepA = false;

            ArgumentException exception = new ArgumentException("Cannot parse color string");
            Color returnColor = color;

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
                        returnColor.R = colorValue;
                    }
                    else
                    {
                        throw exception;
                    }

                    // Green
                    if (channels[1] == "x")
                    {
                        keepG = true;
                    }
                    else if (byte.TryParse(channels[1], out colorValue))
                    {
                        returnColor.G = colorValue;
                    }
                    else
                    {
                        throw exception;
                    }

                    // Blue
                    if (channels[2] == "x")
                    {
                        keepB = true;
                    }
                    else if (byte.TryParse(channels[2], out colorValue))
                    {
                        returnColor.B = colorValue;
                    }
                    else
                    {
                        throw exception;
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
                            returnColor.A = colorValue;
                        }
                        else
                        {
                            throw exception;
                        }
                    }
                    else
                    {
                        returnColor.A = 255;
                    }

                    return returnColor;
                }
                else
                {
                    throw exception;
                }
            }
            else if (value == "default")
            {
                useDefault = true;
                return returnColor;
            }
            else
            {
                value = value.ToLower();

                if (ColorMappings.ContainsKey(value))
                {
                    return ColorMappings[value];
                }
                else
                {
                    // Lookup color in framework

                    TypeInfo colorType = typeof(Color).GetTypeInfo();

                    foreach (PropertyInfo item in colorType.DeclaredProperties)
                    {
                        if (item.Name.ToLower() == value)
                        {
                            return (Color)item.GetValue(null);
                        }
                    }

                    foreach (FieldInfo item in colorType.DeclaredFields)
                    {
                        if (item.Name.ToLower() == value)
                        {
                            return (Color)item.GetValue(null);
                        }
                    }


                    throw exception;
                }
            }
        }
    }


#if SILVERLIGHT

    public static class Colors
    {
        public static Color FromPackedValue(uint value)
        {
            Color returnColor = new Color();
            returnColor.PackedValue = value;
            return returnColor;
        }

        public static Color Transparent { get { return FromPackedValue(0x00000000); } }
        public static Color AliceBlue { get { return FromPackedValue(0xfffff8f0); } }
        public static Color AntiqueWhite { get { return FromPackedValue(0xffd7ebfa); } }
        public static Color Aqua { get { return FromPackedValue(0xffffff00); } }
        public static Color Aquamarine { get { return FromPackedValue(0xffd4ff7f); } }
        public static Color Azure { get { return FromPackedValue(0xfffffff0); } }
        public static Color Beige { get { return FromPackedValue(0xffdcf5f5); } }
        public static Color Bisque { get { return FromPackedValue(0xffc4e4ff); } }
        public static Color Black { get { return FromPackedValue(0xff000000); } }
        public static Color BlanchedAlmond { get { return FromPackedValue(0xffcdebff); } }
        public static Color Blue { get { return FromPackedValue(0xffff0000); } }
        public static Color BlueViolet { get { return FromPackedValue(0xffe22b8a); } }
        public static Color Brown { get { return FromPackedValue(0xff2a2aa5); } }
        public static Color BurlyWood { get { return FromPackedValue(0xff87b8de); } }
        public static Color CadetBlue { get { return FromPackedValue(0xffa09e5f); } }
        public static Color Chartreuse { get { return FromPackedValue(0xff00ff7f); } }
        public static Color Chocolate { get { return FromPackedValue(0xff1e69d2); } }
        public static Color Coral { get { return FromPackedValue(0xff507fff); } }
        public static Color CornflowerBlue { get { return FromPackedValue(0xffed9564); } }
        public static Color Cornsilk { get { return FromPackedValue(0xffdcf8ff); } }
        public static Color Crimson { get { return FromPackedValue(0xff3c14dc); } }
        public static Color Cyan { get { return FromPackedValue(0xffffff00); } }
        public static Color DarkBlue { get { return FromPackedValue(0xff8b0000); } }
        public static Color DarkCyan { get { return FromPackedValue(0xff8b8b00); } }
        public static Color DarkGoldenrod { get { return FromPackedValue(0xff0b86b8); } }
        public static Color DarkGray { get { return FromPackedValue(0xffa9a9a9); } }
        public static Color DarkGreen { get { return FromPackedValue(0xff006400); } }
        public static Color DarkKhaki { get { return FromPackedValue(0xff6bb7bd); } }
        public static Color DarkMagenta { get { return FromPackedValue(0xff8b008b); } }
        public static Color DarkOliveGreen { get { return FromPackedValue(0xff2f6b55); } }
        public static Color DarkOrange { get { return FromPackedValue(0xff008cff); } }
        public static Color DarkOrchid { get { return FromPackedValue(0xffcc3299); } }
        public static Color DarkRed { get { return FromPackedValue(0xff00008b); } }
        public static Color DarkSalmon { get { return FromPackedValue(0xff7a96e9); } }
        public static Color DarkSeaGreen { get { return FromPackedValue(0xff8bbc8f); } }
        public static Color DarkSlateBlue { get { return FromPackedValue(0xff8b3d48); } }
        public static Color DarkSlateGray { get { return FromPackedValue(0xff4f4f2f); } }
        public static Color DarkTurquoise { get { return FromPackedValue(0xffd1ce00); } }
        public static Color DarkViolet { get { return FromPackedValue(0xffd30094); } }
        public static Color DeepPink { get { return FromPackedValue(0xff9314ff); } }
        public static Color DeepSkyBlue { get { return FromPackedValue(0xffffbf00); } }
        public static Color DimGray { get { return FromPackedValue(0xff696969); } }
        public static Color DodgerBlue { get { return FromPackedValue(0xffff901e); } }
        public static Color Firebrick { get { return FromPackedValue(0xff2222b2); } }
        public static Color FloralWhite { get { return FromPackedValue(0xfff0faff); } }
        public static Color ForestGreen { get { return FromPackedValue(0xff228b22); } }
        public static Color Fuchsia { get { return FromPackedValue(0xffff00ff); } }
        public static Color Gainsboro { get { return FromPackedValue(0xffdcdcdc); } }
        public static Color GhostWhite { get { return FromPackedValue(0xfffff8f8); } }
        public static Color Gold { get { return FromPackedValue(0xff00d7ff); } }
        public static Color Goldenrod { get { return FromPackedValue(0xff20a5da); } }
        public static Color Gray { get { return FromPackedValue(0xff808080); } }
        public static Color Green { get { return FromPackedValue(0xff008000); } }
        public static Color GreenYellow { get { return FromPackedValue(0xff2fffad); } }
        public static Color Honeydew { get { return FromPackedValue(0xfff0fff0); } }
        public static Color HotPink { get { return FromPackedValue(0xffb469ff); } }
        public static Color IndianRed { get { return FromPackedValue(0xff5c5ccd); } }
        public static Color Indigo { get { return FromPackedValue(0xff82004b); } }
        public static Color Ivory { get { return FromPackedValue(0xfff0ffff); } }
        public static Color Khaki { get { return FromPackedValue(0xff8ce6f0); } }
        public static Color Lavender { get { return FromPackedValue(0xfffae6e6); } }
        public static Color LavenderBlush { get { return FromPackedValue(0xfff5f0ff); } }
        public static Color LawnGreen { get { return FromPackedValue(0xff00fc7c); } }
        public static Color LemonChiffon { get { return FromPackedValue(0xffcdfaff); } }
        public static Color LightBlue { get { return FromPackedValue(0xffe6d8ad); } }
        public static Color LightCoral { get { return FromPackedValue(0xff8080f0); } }
        public static Color LightCyan { get { return FromPackedValue(0xffffffe0); } }
        public static Color LightGoldenrodYellow { get { return FromPackedValue(0xffd2fafa); } }
        public static Color LightGreen { get { return FromPackedValue(0xff90ee90); } }
        public static Color LightGray { get { return FromPackedValue(0xffd3d3d3); } }
        public static Color LightPink { get { return FromPackedValue(0xffc1b6ff); } }
        public static Color LightSalmon { get { return FromPackedValue(0xff7aa0ff); } }
        public static Color LightSeaGreen { get { return FromPackedValue(0xffaab220); } }
        public static Color LightSkyBlue { get { return FromPackedValue(0xffface87); } }
        public static Color LightSlateGray { get { return FromPackedValue(0xff998877); } }
        public static Color LightSteelBlue { get { return FromPackedValue(0xffdec4b0); } }
        public static Color LightYellow { get { return FromPackedValue(0xffe0ffff); } }
        public static Color Lime { get { return FromPackedValue(0xff00ff00); } }
        public static Color LimeGreen { get { return FromPackedValue(0xff32cd32); } }
        public static Color Linen { get { return FromPackedValue(0xffe6f0fa); } }
        public static Color Magenta { get { return FromPackedValue(0xffff00ff); } }
        public static Color Maroon { get { return FromPackedValue(0xff000080); } }
        public static Color MediumAquamarine { get { return FromPackedValue(0xffaacd66); } }
        public static Color MediumBlue { get { return FromPackedValue(0xffcd0000); } }
        public static Color MediumOrchid { get { return FromPackedValue(0xffd355ba); } }
        public static Color MediumPurple { get { return FromPackedValue(0xffdb7093); } }
        public static Color MediumSeaGreen { get { return FromPackedValue(0xff71b33c); } }
        public static Color MediumSlateBlue { get { return FromPackedValue(0xffee687b); } }
        public static Color MediumSpringGreen { get { return FromPackedValue(0xff9afa00); } }
        public static Color MediumTurquoise { get { return FromPackedValue(0xffccd148); } }
        public static Color MediumVioletRed { get { return FromPackedValue(0xff8515c7); } }
        public static Color MidnightBlue { get { return FromPackedValue(0xff701919); } }
        public static Color MintCream { get { return FromPackedValue(0xfffafff5); } }
        public static Color MistyRose { get { return FromPackedValue(0xffe1e4ff); } }
        public static Color Moccasin { get { return FromPackedValue(0xffb5e4ff); } }
        public static Color NavajoWhite { get { return FromPackedValue(0xffaddeff); } }
        public static Color Navy { get { return FromPackedValue(0xff800000); } }
        public static Color OldLace { get { return FromPackedValue(0xffe6f5fd); } }
        public static Color Olive { get { return FromPackedValue(0xff008080); } }
        public static Color OliveDrab { get { return FromPackedValue(0xff238e6b); } }
        public static Color Orange { get { return FromPackedValue(0xff00a5ff); } }
        public static Color OrangeRed { get { return FromPackedValue(0xff0045ff); } }
        public static Color Orchid { get { return FromPackedValue(0xffd670da); } }
        public static Color PaleGoldenrod { get { return FromPackedValue(0xffaae8ee); } }
        public static Color PaleGreen { get { return FromPackedValue(0xff98fb98); } }
        public static Color PaleTurquoise { get { return FromPackedValue(0xffeeeeaf); } }
        public static Color PaleVioletRed { get { return FromPackedValue(0xff9370db); } }
        public static Color PapayaWhip { get { return FromPackedValue(0xffd5efff); } }
        public static Color PeachPuff { get { return FromPackedValue(0xffb9daff); } }
        public static Color Peru { get { return FromPackedValue(0xff3f85cd); } }
        public static Color Pink { get { return FromPackedValue(0xffcbc0ff); } }
        public static Color Plum { get { return FromPackedValue(0xffdda0dd); } }
        public static Color PowderBlue { get { return FromPackedValue(0xffe6e0b0); } }
        public static Color Purple { get { return FromPackedValue(0xff800080); } }
        public static Color Red { get { return FromPackedValue(0xff0000ff); } }
        public static Color RosyBrown { get { return FromPackedValue(0xff8f8fbc); } }
        public static Color RoyalBlue { get { return FromPackedValue(0xffe16941); } }
        public static Color SaddleBrown { get { return FromPackedValue(0xff13458b); } }
        public static Color Salmon { get { return FromPackedValue(0xff7280fa); } }
        public static Color SandyBrown { get { return FromPackedValue(0xff60a4f4); } }
        public static Color SeaGreen { get { return FromPackedValue(0xff578b2e); } }
        public static Color SeaShell { get { return FromPackedValue(0xffeef5ff); } }
        public static Color Sienna { get { return FromPackedValue(0xff2d52a0); } }
        public static Color Silver { get { return FromPackedValue(0xffc0c0c0); } }
        public static Color SkyBlue { get { return FromPackedValue(0xffebce87); } }
        public static Color SlateBlue { get { return FromPackedValue(0xffcd5a6a); } }
        public static Color SlateGray { get { return FromPackedValue(0xff908070); } }
        public static Color Snow { get { return FromPackedValue(0xfffafaff); } }
        public static Color SpringGreen { get { return FromPackedValue(0xff7fff00); } }
        public static Color SteelBlue { get { return FromPackedValue(0xffb48246); } }
        public static Color Tan { get { return FromPackedValue(0xff8cb4d2); } }
        public static Color Teal { get { return FromPackedValue(0xff808000); } }
        public static Color Thistle { get { return FromPackedValue(0xffd8bfd8); } }
        public static Color Tomato { get { return FromPackedValue(0xff4763ff); } }
        public static Color Turquoise { get { return FromPackedValue(0xffd0e040); } }
        public static Color Violet { get { return FromPackedValue(0xffee82ee); } }
        public static Color Wheat { get { return FromPackedValue(0xffb3def5); } }
        public static Color White { get { return FromPackedValue(0xffffffff); } }
        public static Color WhiteSmoke { get { return FromPackedValue(0xfff5f5f5); } }
        public static Color Yellow { get { return FromPackedValue(0xff00ffff); } }
        public static Color YellowGreen { get { return FromPackedValue(0xff32cd9a); } }

    }
#endif

}
