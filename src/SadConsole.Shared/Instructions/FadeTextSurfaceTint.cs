using Microsoft.Xna.Framework;

using System;
using System.Runtime.Serialization;

namespace SadConsole.Instructions
{
    /// <summary>
    /// Animates the change to the tint of a <see cref="SadConsole.Consoles.ITextSurfaceRendered"/>.
    /// </summary>
    [DataContract]
    public class FadeTextSurfaceTint : InstructionBase<Surfaces.ISurface>
    {
        /// <summary>
        /// The color to fade the tint to.
        /// </summary>
        [DataMember]
        public ColorGradient Colors { get; set; }

        /// <summary>
        /// Animation provider.
        /// </summary>
        [DataMember]
        public DoubleAnimation FadeAnimationSettings { get; set; }

        /// <summary>
        /// Creates a new tint fade instruction.
        /// </summary>
        /// <param name="textSurface">The <see cref="Consoles.ITextSurfaceRendered.Tint"/> to fade.</param>
        /// <param name="colors">The gradient pattern to fade through.</param>
        /// <param name="duration">How long the fade takes.</param>
        public FadeTextSurfaceTint(Surfaces.ISurface textSurface, ColorGradient colors, TimeSpan duration)
            : base(textSurface)
        {
            Colors = colors;
            FadeAnimationSettings = new DoubleAnimation() { StartingValue = 0d, EndingValue = 1d, Duration = duration };
        }

        /// <summary>
        /// Runs the instruction.
        /// </summary>
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
        
        /// <summary>
        /// Starts the instruction over.
        /// </summary>
        public override void Reset()
        {
            FadeAnimationSettings.Reset();

            base.Reset();
        }
    }
}
