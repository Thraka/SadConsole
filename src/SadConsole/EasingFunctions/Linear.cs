// Thanks to Robert Penner for his great tutorials and code examples.
// Thanks to 2013 Ivan Kuckir  ( ivan@kuckir.com ) for his Java code.
// I do not claim any rights with this .cs file.
namespace SadConsole.EasingFunctions
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Linear : EasingBase
    {
        public override double Ease(double time, double startingValue, double currentValue, double duration)
        {
            switch (Mode)
            {
                case EasingMode.In:
                case EasingMode.Out:
                case EasingMode.InOut:
                case EasingMode.OutIn:

                default:
                    return currentValue * time / duration + startingValue;
            }

        }
    }
}
