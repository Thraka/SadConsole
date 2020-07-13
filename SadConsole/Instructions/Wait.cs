using System;

namespace SadConsole.Instructions
{
    /// <summary>
    /// Represents an instruction to pause for a specified duration.
    /// </summary>
    public class Wait : InstructionBase
    {
        private bool _started = false;

        /// <summary>
        /// How much time has passed.
        /// </summary>
        protected TimeSpan CountedTime = TimeSpan.Zero;

        /// <summary>
        /// The duration of the wait.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Creates a new wait timer with the specified duration.
        /// </summary>
        /// <param name="duration">How long this instruction waits until it signals <see cref="InstructionBase.IsFinished"/>.</param>
        public Wait(TimeSpan duration)
        {
            Duration = duration;
        }

        /// <summary>
        /// Creates a new wait timer with a 1-second delay.
        /// </summary>
        public Wait()
        {
            Duration = TimeSpan.FromSeconds(1);
        }

        /// <inheritdoc />
        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            if (!IsFinished)
            {
                if (!_started)
                {
                    CountedTime = TimeSpan.Zero;
                    _started = true;
                }
                else
                    CountedTime += delta;

                if (CountedTime > Duration)
                    IsFinished = true;

                base.Update(componentHost, delta);
            }
        }

        /// <inheritdoc />
        public override void Reset()
        {
            _started = false;

            base.Reset();
        }
    }
}
