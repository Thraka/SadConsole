namespace SadConsole.Instructions
{
    using System;
    using Console = SadConsole.Console;

    /// <summary>
    /// Represents an instruction to pause for a specified duration.
    /// </summary>
    public class Wait : InstructionBase
    {
        private bool _started = false;
        private TimeSpan _lastUpdateTime = TimeSpan.Zero;

        /// <summary>
        /// The duration of the wait.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Creates a new wait timer with the specified duration.
        /// </summary>
        /// <param name="duration">How long this instruction waits until it signals <see cref="InstructionBase.IsFinished"/>.</param>
        public Wait(TimeSpan duration)
            => Duration = duration;

        /// <summary>
        /// Creates a new wait timer with a 1-second delay.
        /// </summary>
        public Wait()
            => Duration = TimeSpan.FromSeconds(1);

        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            if (!_started)
            {
                _lastUpdateTime = TimeSpan.Zero;
                _started = true;
            }
            else
            {
                _lastUpdateTime += delta;
            }

            if (_lastUpdateTime > Duration)
            {
                IsFinished = true;
            }

            base.Update(console, delta);
        }

        /// <inheritdoc />
        public override void Reset()
        {
            _started = false;

            base.Reset();
        }
    }
}
