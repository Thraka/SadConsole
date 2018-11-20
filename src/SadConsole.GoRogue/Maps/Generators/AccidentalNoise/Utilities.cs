using System;
using System.Linq;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public static class Utilities
    {
        public static Double Clamp(Double value, Double low, Double high)
        {
            if (value < low) return low;
            if (value > high) return high;
            return value;
        }

        public static Int32 Clamp(Int32 value, Int32 low, Int32 high)
        {
            if (value < low) return low;
            if (value > high) return high;
            return value;
        }

        private static Double logPointFive = Math.Log(0.5);
        public static Double Bias(Double bias, Double target)
        {
            return Math.Pow(target, Math.Log(bias) / logPointFive);
        }

        public static Double Lerp(Double t, Double a, Double b)
        {
            return a + t * (b - a);
        }

        public static Double Gain(Double g, Double t)
        {
            if (t < 0.50) return Bias(1.00 - g, 2.00 * t) / 2.00;
            return 1.00 - Bias(1.00 - g, 2.00 - 2.00 * t) / 2.00;
        }

        public static Double QuinticBlend(Double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
    }
}
