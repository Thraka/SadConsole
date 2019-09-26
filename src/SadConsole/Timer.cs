using System;

namespace SadConsole
{
    /// <summary>
    /// A simple timer with callback.
    /// </summary>
    public class Timer : Components.UpdateConsoleComponent
    {
        /// <summary>
        /// Called when the timer elapses.
        /// </summary>
        public event EventHandler TimerElapsed;

        /// <summary>
        /// Called when the timer restarts.
        /// </summary>
        public event EventHandler TimerRestart;

        /// <summary>
        /// If true, the timer will restart when the time has elapsed.
        /// </summary>
        public bool Repeat { get; set; } = true;

        /// <summary>
        /// How many milliseconds to cause the timer to trigger.
        /// </summary>
        public TimeSpan TimerAmount { get; set; }

        /// <summary>
        /// When true, the timer does not count time.
        /// </summary>
        public bool IsPaused { get; set; } = false;

        private TimeSpan _countedTime;

        /// <summary>
        /// Creates a new timer.
        /// </summary>
        /// <param name="triggerTime">Duration of the timer.</param>
        public Timer(TimeSpan triggerTime) =>
            TimerAmount = triggerTime;

        /// <summary>
        /// Updates the timer with the time since the last call.
        /// </summary>
        /// <param name="console">The parent object.</param>
        /// <param name="delta">The time since the last frame update.</param>
        public override void Update(Console console, TimeSpan delta)
        {
            if (!IsPaused)
            {
                _countedTime += delta;

                if (_countedTime >= TimerAmount)
                {
                    IsPaused = true;

                    TimerElapsed?.Invoke(this, EventArgs.Empty);

                    if (Repeat)
                    {
                        Restart();
                    }
                }
            }
        }

        /// <summary>
        /// Restarts the timer; sets <see cref="IsPaused"/> to false.
        /// </summary>
        public void Restart()
        {
            _countedTime = TimeSpan.Zero;
            IsPaused = false;
            TimerRestart?.Invoke(this, EventArgs.Empty);
        }
    }
}




