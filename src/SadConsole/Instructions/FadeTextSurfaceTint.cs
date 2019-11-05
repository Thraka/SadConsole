#if XNA
using Microsoft.Xna.Framework;
#endif

using System;

namespace SadConsole.Instructions
{
    /// <summary>
    /// Animates a color change to <see cref="Console.Tint"/>.
    /// </summary>
    public class FadeTextSurfaceTint : InstructionBase
    {
        private Console _console;
        private ColorGradient _colors;

        /// <summary>
        /// The color to fade the tint to.
        /// </summary>
        public ColorGradient Colors
        {
            get => _colors;
            set => _colors = value ?? throw new Exception($"{nameof(Colors)} can't be set to null");
        }

        /// <summary>
        /// Animation provider.
        /// </summary>
        public DoubleAnimation FadeAnimationSettings { get; set; }

        /// <summary>
        /// Creates a new tint fade instruction that targets the specified console.
        /// </summary>
        /// <param name="console">The <see cref="Console.Tint"/> to fade.</param>
        /// <param name="colors">The gradient pattern to fade through.</param>
        /// <param name="duration">How long the fade takes.</param>
        public FadeTextSurfaceTint(Console console, ColorGradient colors, TimeSpan duration)
        {
            Colors = colors;
            FadeAnimationSettings = new DoubleAnimation() { StartingValue = 0d, EndingValue = 1d, Duration = duration };
            _console = console;
        }

        /// <summary>
        /// Creates a new tint fade instruction that uses the console passed to <see cref="IConsoleComponent.Update(Console, TimeSpan)"/>.
        /// </summary>
        /// <param name="colors">The gradient pattern to fade through.</param>
        /// <param name="duration">How long the fade takes.</param>
        public FadeTextSurfaceTint(ColorGradient colors, TimeSpan duration)
        {
            Colors = colors;
            FadeAnimationSettings = new DoubleAnimation() { StartingValue = 0d, EndingValue = 1d, Duration = duration };
        }

        /// <summary>
        /// Creates a new tint fade instruction with default settings that uses the console passed to <see cref="IConsoleComponent.Update(Console, TimeSpan)"/>.
        /// </summary>
        /// <remarks>
        /// The default settings are:
        /// 
        ///   - <see cref="Colors"/>: <see cref="Color.White"/> to <see cref="Color.Black"/>
        ///   - <see cref="FadeAnimationSettings"/>: 1 second
        /// </remarks>
        public FadeTextSurfaceTint()
        {
            Colors = new ColorGradient(Color.White, Color.Transparent);
            FadeAnimationSettings = new DoubleAnimation() { StartingValue = 0d, EndingValue = 1d, Duration = new TimeSpan(0, 0, 1) };

        }

        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            if (!FadeAnimationSettings.IsStarted)
            {
                FadeAnimationSettings.Start();

                if (_console == null)
                {
                    _console = console;
                }
            }

            if (FadeAnimationSettings.IsFinished)
            {
                IsFinished = true;
            }

            _console.Tint = Colors.Lerp((float)FadeAnimationSettings.CurrentValue);

            base.Update(console, delta);
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
