namespace SadConsole.EasingFunctions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    [DataContract]
    public abstract class EasingBase
    {
        [DataMember]
        public EasingMode Mode { get; set; }

        public EasingBase() { Mode = EasingMode.None; }

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
