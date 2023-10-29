﻿// Thanks to Robert Penner for his great tutorials and code examples.
// Thanks to Ivan Kuckir  ( ivan@kuckir.com ) for his Java code.
// I do not claim any rights with this .cs file.
using System;
using System.Runtime.Serialization;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.EasingFunctions;

[DataContract]
public class Expo : EasingBase
{
    public override double Ease(double time, double startingValue, double currentValue, double duration)
    {
        switch (Mode)
        {
            case EasingMode.In:
                return ExpoEaseIn(time, startingValue, currentValue, duration);

            case EasingMode.Out:
                return ExpoEaseOut(time, startingValue, currentValue, duration);

            case EasingMode.InOut:
                if (time == 0)
                {
                    return startingValue;
                }

                if (time == duration)
                {
                    return startingValue + currentValue;
                }

                if ((time /= duration / 2) < 1)
                {
                    return currentValue / 2 * Math.Pow(2, 10 * (time - 1)) + startingValue;
                }

                return currentValue / 2 * (-Math.Pow(2, -10 * --time) + 2) + startingValue;

            case EasingMode.OutIn:
                if (time < duration / 2)
                {
                    return ExpoEaseOut(time * 2, startingValue, currentValue / 2, duration);
                }

                return ExpoEaseIn((time * 2) - duration, startingValue + currentValue / 2, currentValue / 2, duration);

            default:
                return currentValue * time / duration + startingValue;
        }

    }

    private static double ExpoEaseOut(double time, double startingValue, double currentValue, double duration) =>
        (time == duration) ? startingValue + currentValue : currentValue * (-Math.Pow(2, -10 * time / duration) + 1) + startingValue;

    private static double ExpoEaseIn(double time, double startingValue, double currentValue, double duration) =>
        (time == 0) ? startingValue : currentValue * Math.Pow(2, 10 * (time / duration - 1)) + startingValue;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
