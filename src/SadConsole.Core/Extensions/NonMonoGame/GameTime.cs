#if !MONOGAME
namespace SadConsole
{
    using System;

    public class GameTime
    {
        private System.Diagnostics.Stopwatch timer;

        public TimeSpan TotalGameTime { get; set; }

        public TimeSpan ElapsedGameTime { get; set; }

        public bool IsRunningSlowly { get; set; }

        public GameTime()
        {
            TotalGameTime = TimeSpan.Zero;
            ElapsedGameTime = TimeSpan.Zero;
            IsRunningSlowly = false;

            timer = new System.Diagnostics.Stopwatch();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Update()
        {
            var currentTicks = timer.Elapsed.Ticks;
            ElapsedGameTime = new TimeSpan(currentTicks);
            TotalGameTime += ElapsedGameTime;
            timer.Restart();
        }
    }
}
#endif