namespace ZZTGame.SadConsoleComponents;

class TickManager: SadConsole.Components.UpdateComponent
{
    public TimeSpan TimePerTick { get; }

    private SadConsole.Components.Timer _timer;
    public bool TickThisFrame { get; private set; }

    public TickManager(TimeSpan time)
    {
        _timer = new SadConsole.Components.Timer(time);
        _timer.TimerElapsed += _timer_TimerElapsed;
    }

    private void _timer_TimerElapsed(object sender, EventArgs e)
    {
        TickThisFrame = true;
    }

    public override void Update(IScreenObject host, TimeSpan delta)
    {
        var world = (ZZTGame.Screens.WorldPlay)host;

        TickThisFrame = false;

        if (!world.IsTickerPaused)
        {
            _timer.Update(host, delta);
        }
    }
}
