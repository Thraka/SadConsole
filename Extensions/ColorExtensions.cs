using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Microsoft.Xna.Framework
{
    public static class ColorAnsi
    {
        public static readonly Color Black = new Color(0, 0, 0);
        public static readonly Color Red = new Color(170, 0, 0);
        public static readonly Color Green = new Color(0, 170, 0);
        public static readonly Color Yellow = new Color(170, 85, 0);
        public static readonly Color Blue = new Color(0, 0, 170);
        public static readonly Color Magenta = new Color(170, 0, 170);
        public static readonly Color Cyan = new Color(0, 170, 170);
        public static readonly Color White = new Color(170, 170, 170);

        public static readonly Color BlackBright = new Color(85, 85, 85);
        public static readonly Color RedBright = new Color(255, 85, 85);
        public static readonly Color GreenBright = new Color(85, 255, 85);
        public static readonly Color YellowBright = new Color(255, 255, 85);
        public static readonly Color BlueBright = new Color(85, 85, 255);
        public static readonly Color MagentaBright = new Color(255, 85, 255);
        public static readonly Color CyanBright = new Color(85, 255, 255);
        public static readonly Color WhiteBright = new Color(255, 255, 255);
    }

    public static class ColorEx
    {
        public static XElement ToXElement(this Color color, string nodeName)
        {
            return new XElement(nodeName,
                new XAttribute("r", color.R),
                new XAttribute("g", color.G),
                new XAttribute("b", color.B),
                new XAttribute("a", color.A));
        }

        public static Color FromXElement(XElement element)
        {
            return new Color()
            {
                R = Convert.ToByte(element.Attribute("r").Value),
                G = Convert.ToByte(element.Attribute("g").Value),
                B = Convert.ToByte(element.Attribute("b").Value),
                A = Convert.ToByte(element.Attribute("a").Value)
            };
        }

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

        public static Color GetRandomColor(this Color color, Random random)
        {
            return new Color(random.Next(255), random.Next(255), random.Next(255));
        }

        #region Color methods taken from mono source code
        public static float GetBrightness(this Color color)
        {
            byte minval = Math.Min(color.R, Math.Min(color.G, color.B));
            byte maxval = Math.Max(color.R, Math.Max(color.G, color.B));


            return (float)(maxval + minval) / 510;
        }


        public static float GetSaturation(this Color color)
        {
            byte minval = (byte)Math.Min(color.R, Math.Min(color.G, color.B));
            byte maxval = (byte)Math.Max(color.R, Math.Max(color.G, color.B));


            if (maxval == minval)
                return 0.0f;


            int sum = maxval + minval;
            if (sum > 255)
                sum = 510 - sum;


            return (float)(maxval - minval) / sum;
        }


        public static float GetHue(this Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            byte minval = (byte)Math.Min(r, Math.Min(g, b));
            byte maxval = (byte)Math.Max(r, Math.Max(g, b));


            if (maxval == minval)
                return 0.0f;


            float diff = (float)(maxval - minval);
            float rnorm = (maxval - r) / diff;
            float gnorm = (maxval - g) / diff;
            float bnorm = (maxval - b) / diff;


            float hue = 0.0f;
            if (r == maxval)
                hue = 60.0f * (6.0f + bnorm - gnorm);
            if (g == maxval)
                hue = 60.0f * (2.0f + rnorm - bnorm);
            if (b == maxval)
                hue = 60.0f * (4.0f + gnorm - rnorm);
            if (hue > 360.0f)
                hue = hue - 360.0f;


            return hue;
        }
        #endregion

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
