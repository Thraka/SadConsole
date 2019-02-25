namespace SadConsole
{
    using System;
    
    /// <summary>
    /// A simple timer with callback.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// Callback to trigger when the time elapses.
        /// </summary>
        public Action<Timer, double> TriggeredCallback;

        /// <summary>
        /// If true, the timer will restart when the time has elapsed.
        /// </summary>
        public bool Repeat { get; set; } = true;

        /// <summary>
        /// How many milliseconds to cause the timer to trigger.
        /// </summary>
        public double TimerAmount { get; set; }

        /// <summary>
        /// When true, the timer does not count time.
        /// </summary>
        public bool IsOff { get; set; } = false;

        private double countedTime;

        /// <summary>
        /// Creates a new timer.
        /// </summary>
        /// <param name="triggerTime">How many milliseconds to trigger the callback.</param>
        /// <param name="callback">The callback that is called when the trigger time has passed.</param>
        public Timer(double triggerTime, Action<Timer, double> callback)
        {
            TimerAmount = triggerTime;
            TriggeredCallback = callback;
        }

        /// <summary>
        /// Updates the timer with the time since the last call.
        /// </summary>
        /// <param name="timeElapsed"></param>
        public void Update(double timeElapsed)
        {
            if (!IsOff)
            {
                countedTime += timeElapsed;

                if (countedTime >= TimerAmount)
                {
                    IsOff = true;

                    TriggeredCallback?.Invoke(this, countedTime);

                    if (Repeat)
                        Restart();
                }
            }
        }

        /// <summary>
        /// Restarts the timer; sets <see cref="IsOff"/> to false.
        /// </summary>
        public void Restart()
        {
            countedTime = 0.0d;
            IsOff = false;
        }
    }
}
