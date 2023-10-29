﻿// Thanks to Robert Penner for his great tutorials and code examples.
// Thanks to Ivan Kuckir  ( ivan@kuckir.com ) for his Java code.
// I do not claim any rights with this .cs file.
using System.Runtime.Serialization;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.EasingFunctions;

[DataContract]
public class Quad : EasingBase
{
    public override double Ease(double time, double startingValue, double currentValue, double duration)
    {
        switch (Mode)
        {
            case EasingMode.In:
                return QuadEaseIn(time, startingValue, currentValue, duration);

            case EasingMode.Out:
                return QuadEaseOut(time, startingValue, currentValue, duration);

            case EasingMode.InOut:
                if ((time /= duration / 2) < 1)
                {
                    return currentValue / 2 * time * time + startingValue;
                }

                return -currentValue / 2 * ((--time) * (time - 2) - 1) + startingValue;

            case EasingMode.OutIn:
                if (time < duration / 2)
                {
                    return QuadEaseOut(time * 2, startingValue, currentValue / 2, duration);
                }

                return QuadEaseIn((time * 2) - duration, startingValue + currentValue / 2, currentValue / 2, duration);

            default:
                return currentValue * time / duration + startingValue;
        }

    }

    private static double QuadEaseOut(double time, double startingValue, double currentValue, double duration) =>
        -currentValue * (time /= duration) * (time - 2) + startingValue;

    private static double QuadEaseIn(double time, double startingValue, double currentValue, double duration) =>
        currentValue * (time /= duration) * time + startingValue;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
