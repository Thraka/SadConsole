namespace SadConsole.EasingFunctions
{
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class EasingBase
    {
        [DataMember]
        public EasingMode Mode { get; set; }

        public EasingBase() => Mode = EasingMode.None;

        public abstract double Ease(double time, double startingValue, double endingValue, double duration);
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
