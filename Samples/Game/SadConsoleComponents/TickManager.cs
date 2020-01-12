using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;

namespace Game.SadConsoleComponents
{
    class TickManager: SadConsole.Components.UpdateComponent
    {
        public TimeSpan TimePerTick { get; }

        private SadConsole.Timer _timer;
        public bool TickThisFrame { get; private set; }

        public TickManager(TimeSpan time)
        {
            _timer = new Timer(time);
            _timer.TimerElapsed += _timer_TimerElapsed;
        }

        private void _timer_TimerElapsed(object sender, EventArgs e)
        {
            TickThisFrame = true;
        }

        public override void Update(IScreenObject host)
        {
            var world = (Game.Screens.WorldPlay)host;

            TickThisFrame = false;

            if (!world.IsTickerPaused)
            {
                _timer.Update(host);
            }
        }
    }
}
