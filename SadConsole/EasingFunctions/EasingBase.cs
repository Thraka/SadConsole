using System.Runtime.Serialization;

namespace SadConsole.EasingFunctions
{
    /// <summary>
    /// The base class for an easing function.
    /// </summary>
    [DataContract]
    public abstract class EasingBase
    {
        /// <summary>
        /// The easing mode.
        /// </summary>
        [DataMember]
        public EasingMode Mode { get; set; }

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        public EasingBase() =>
            Mode = EasingMode.None;

        /// <summary>
        /// Called to apply an easing function to the value.
        /// </summary>
        /// <param name="elapsedTime">The total time applied to the function.</param>
        /// <param name="startingValue">The starting value.</param>
        /// <param name="endingValue">The ending value.</param>
        /// <param name="maxDuration">Total time applied to easing.</param>
        /// <returns>A calculated value.</returns>
        public abstract double Ease(double elapsedTime, double startingValue, double endingValue, double maxDuration);
    }

    /// <summary>
    /// The types of easing modes.
    /// </summary>
    public enum EasingMode
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        In,
        Out,
        InOut,
        OutIn,
        None
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
