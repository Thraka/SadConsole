// Thanks to Robert Penner for his great tutorials and code examples.
// Thanks to Ivan Kuckir  ( ivan@kuckir.com ) for his Java code.
// I do not claim any rights with this .cs file.
namespace SadConsole.EasingFunctions
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class Sine : EasingBase
    {
        public override double Ease(double time, double startingValue, double currentValue, double duration)
        {
            switch (Mode)
            {
                case EasingMode.In:
                    return SineEaseIn(time, startingValue, currentValue, duration);

                case EasingMode.Out:
                    return SineEaseOut(time, startingValue, currentValue, duration);

                case EasingMode.InOut:
                    if ((time /= duration / 2) < 1)
                        return currentValue / 2 * (Math.Sin(Math.PI * time / 2)) + startingValue;

                    return -currentValue / 2 * (Math.Cos(Math.PI * --time / 2) - 2) + startingValue;

                case EasingMode.OutIn:
                    if (time < duration / 2)
                        return SineEaseOut(time * 2, startingValue, currentValue / 2, duration);

                    return SineEaseIn((time * 2) - duration, startingValue + currentValue / 2, currentValue / 2, duration);

                default:
                    return currentValue * time / duration + startingValue;
            }

        }

        private double SineEaseOut(double time, double startingValue, double currentValue, double duration)
        {
            return currentValue * Math.Sin(time / duration * (Math.PI / 2)) + startingValue;
        }

        private double SineEaseIn(double time, double startingValue, double currentValue, double duration)
        {
            return -currentValue * Math.Cos(time / duration * (Math.PI / 2)) + currentValue + startingValue;
        }
    }
}
