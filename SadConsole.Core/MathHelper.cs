using System;

// Moved from https://github.com/jongallant/WorldGeneratorFinal/blob/master/Assets/Scripts/MathHelper.cs

namespace SadConsole
{

    public static class MathHelper
    {
        public static float Clamp(float v, float l, float h)
        {
            if (v < l) v = l;
            if (v > h) v = h;
            return v;
        }

        public static double Clamp(double v, double l, double h)
        {
            if (v < l) v = l;
            if (v > h) v = h;
            return v;
        }

        public static float Lerp(float t, float a, float b)
        {
            return t + (a - t) * b;
        }

        public static double Lerp(double t, double a, double b)
        {
            return t + (a - t) * b;
        }

        public static double QuinticBlend(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public static double Bias(double b, double t)
        {
            return Math.Pow(t, Math.Log(b) / Math.Log(0.5));
        }

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

    }
}