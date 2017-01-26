// Thanks to Robert Penner for his great tutorials and code examples.
// Thanks to 2013 Ivan Kuckir  ( ivan@kuckir.com ) for his Java code.
// I do not claim any rights with this .cs file.
namespace SadConsole.EasingFunctions
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Bounce : EasingBase
    {
        public override double Ease(double time, double startingValue, double currentValue, double duration)
        {  
            switch (Mode)
            {
                case EasingMode.In:
                    return BounceEaseIn(time, startingValue, currentValue, duration);
                case EasingMode.Out:
                    return BounceEaseOut(time, startingValue, currentValue, duration);
                case EasingMode.InOut:
                    if (time < duration / 2)
                        return BounceEaseIn(time * 2, 0, currentValue, duration) * .5 + startingValue;
                    else
                        return BounceEaseOut(time * 2 - duration, 0, currentValue, duration) * .5 + currentValue * .5 + startingValue;
                case EasingMode.OutIn:
                    if (time < duration / 2)
                        return BounceEaseOut(time * 2, startingValue, currentValue / 2, duration);

                    return BounceEaseIn((time * 2) - duration, startingValue + currentValue / 2, currentValue / 2, duration);
                default:    // None
                    return BounceEaseIn(time, startingValue, currentValue, duration);
            }

        }

        private double BounceEaseOut(double time, double startingValue, double currentValue, double duration)
        {
            if ((time /= duration) < (1 / 2.75))
                return currentValue * (7.5625 * time * time) + startingValue;
            else if (time < (2 / 2.75))
                return currentValue * (7.5625 * (time -= (1.5 / 2.75)) * time + .75) + startingValue;
            else if (time < (2.5 / 2.75))
                return currentValue * (7.5625 * (time -= (2.25 / 2.75)) * time + .9375) + startingValue;
            else
                return currentValue * (7.5625 * (time -= (2.625 / 2.75)) * time + .984375) + startingValue;
        }

        private double BounceEaseIn(double time, double startingValue, double currentValue, double duration)
        {
            return currentValue - BounceEaseOut(duration - time, 0, currentValue, duration) + startingValue;
        }
    }
}
