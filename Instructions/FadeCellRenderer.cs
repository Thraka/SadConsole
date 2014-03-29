namespace SadConsole.Instructions
{
    using Microsoft.Xna.Framework;
    using SadConsole.Effects;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Animates the change to the tint of a <see cref="SadConsole.Consoles.CellsRenderer"/>.
    /// </summary>
    [DataContract]
    public class FadeCellRenderer : InstructionBase<Consoles.CellsRenderer>
    {
        /// <summary>
        /// The color to fade the tint to.
        /// </summary>
        [DataMember]
        public ColorGradient Colors { get; set; }

        [DataMember]
        public DoubleAnimation FadeAnimationSettings { get; set; }

        public FadeCellRenderer(Consoles.CellsRenderer renderer, ColorGradient colors, TimeSpan duration)
            : base(renderer)
        {
            Colors = colors;
            FadeAnimationSettings = new DoubleAnimation() { StartingValue = 0d, EndingValue = 1d, Duration = duration };
        }


        public override void Run()
        {
            if (Colors == null)
                throw new System.NullReferenceException("The Colors property is null. It must be set to an instance before this instruction can run.");

            if (!FadeAnimationSettings.IsStarted)
                FadeAnimationSettings.Start();

            if (FadeAnimationSettings.IsFinished)
                this.IsFinished = true;

            base.Target.Tint = Colors.Lerp((float)FadeAnimationSettings.CurrentValue);

            base.Run();
        }
        
        public override void Reset()
        {
            FadeAnimationSettings.Reset();

            base.Reset();
        }
    }
}
