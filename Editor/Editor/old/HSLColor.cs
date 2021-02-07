using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor
{
    struct HSLColor
    {
        public HSLColor(Color color)
        {
            Hue = color.GetHue();
            Saturation = color.GetSaturation();
            Lightness = color.GetBrightness();
        }

        public float Hue;
        public float Saturation;
        public float Lightness;

        public Color ToColor(byte r, byte g, byte b)
        {
            Color color = new Color();

            color.SetHSL(Hue, Saturation, Lightness);

            return color;
        }
    }
}
