using System;

namespace SadConsole.Coroutine;

/// <summary>
/// Represents either an amount of time, or an <see cref="Coroutine.Event"/> that is being waited for by an <see cref="ActiveCoroutine"/>.
/// </summary>
public struct Wait
{
    internal readonly Event? Event;
    private double _seconds;

    /// <summary>
    /// Creates a new wait that waits for the given <see cref="Coroutine.Event"/>.
    /// </summary>
    /// <param name="evt">The event to wait for</param>
    public Wait(Event evt)
    {
        Event = evt;
        _seconds = 0;
    }

    /// <summary>
    /// Creates a new wait that waits for the given amount of seconds.
    /// </summary>
    /// <param name="seconds">The amount of seconds to wait for</param>
    public Wait(double seconds)
    {
        _seconds = seconds;
        Event = null;
    }

    /// <summary>
    /// Creates a new wait that waits for the given <see cref="TimeSpan"/>.
    /// Note that the exact value may be slightly different, since waits operate in <see cref="TimeSpan.TotalSeconds"/> rather than ticks.
    /// </summary>
    /// <param name="time">The time span to wait for</param>
    public Wait(TimeSpan time) : this(time.TotalSeconds)
    {
    }

    internal bool Tick(double deltaSeconds)
    {
        _seconds -= deltaSeconds;
        return _seconds <= 0;
    }
}
