// Thanks to Robert Penner for his great tutorials and code examples.
// Thanks to Ivan Kuckir  ( ivan@kuckir.com ) for his Java code.
// I do not claim any rights with this .cs file.
namespace SadConsole.EasingFunctions
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class Circle : EasingBase
    {
        public override double Ease(double time, double startingValue, double currentValue, double duration)
        {
            switch (Mode)
            {
                case EasingMode.In:
                    return CircleEaseIn(time, startingValue, currentValue, duration);

                case EasingMode.Out:
                    return CircleEaseOut(time, startingValue, currentValue, duration);

                case EasingMode.InOut:
                    if ((time /= duration / 2) < 1)
                        return -currentValue / 2 * (Math.Sqrt(1 - time * time) - 1) + startingValue;

                    return currentValue / 2 * (Math.Sqrt(1 - (time -= 2) * time) + 1) + startingValue;

                case EasingMode.OutIn:
                    if (time < duration / 2)
                        return CircleEaseOut(time * 2, startingValue, currentValue / 2, duration);

                    return CircleEaseIn((time * 2) - duration, startingValue + currentValue / 2, currentValue / 2, duration);

                default:
                    return currentValue * time / duration + startingValue;
            }

        }

        private double CircleEaseOut(double time, double startingValue, double currentValue, double duration)
        {
            return currentValue * Math.Sqrt(1 - (time = time / duration - 1) * time) + startingValue;
        }

        private double CircleEaseIn(double time, double startingValue, double currentValue, double duration)
        {
            return -currentValue * (Math.Sqrt(1 - (time /= duration) * time) - 1) + startingValue;
        }
    }
}
