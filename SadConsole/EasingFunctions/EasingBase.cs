using System.Runtime.Serialization;

namespace SadConsole.EasingFunctions
{
    [DataContract]
    public abstract class EasingBase
    {
        [DataMember]
        public EasingMode Mode { get; set; }

        public EasingBase() => Mode = EasingMode.None;

        public abstract double Ease(double elapsedTime, double startingValue, double endingValue, double maxDuration);
    }

    public enum EasingMode
    {
        In,
        Out,
        InOut,
        OutIn,
        None
    }
}
