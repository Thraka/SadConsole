namespace SadConsole
{
    // Moved from https://github.com/jongallant/WorldGeneratorFinal/blob/master/Assets/Scripts/MathHelper.cs

    using System;

    public static class MathHelper
    {
        public static float Clamp(float v, float l, float h)
        {
            if (v < l)
            {
                v = l;
            }

            if (v > h)
            {
                v = h;
            }

            return v;
        }

        public static double Clamp(double v, double l, double h)
        {
            if (v < l)
            {
                v = l;
            }

            if (v > h)
            {
                v = h;
            }

            return v;
        }

        public static float Lerp(float t, float a, float b) => t + (a - t) * b;

        public static double Lerp(double t, double a, double b) => t + (a - t) * b;

        public static double QuinticBlend(double t) => t * t * t * (t * (t * 6 - 15) + 10);

        public static double Bias(double b, double t) => Math.Pow(t, Math.Log(b) / Math.Log(0.5));

        public static double Gain(double g, double t)
        {
            if (t < 0.5)
            {
                return Bias(1.0 - g, 2.0 * t) / 2.0;
            }
            else
            {
                return 1.0 - Bias(1.0 - g, 2.0 - 2.0 * t) / 2.0;
            }
        }

        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        /// <summary>
        /// Wraps a value around the min and max.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The minimum value before it transforms into the maximum.</param>
        /// <param name="max">The maximum value before it transforms into the minimum.</param>
        /// <returns>A new value if it falls outside the min/max range otherwise, the same value.</returns>
        public static float Wrap(float value, float min, float max)
        {
            if (value < min)
            {
                value = max - (min - value) % (max - min);
            }
            else
            {
                value = min + (value - min) % (max - min);
            }

            return value;
        }
    }
}
