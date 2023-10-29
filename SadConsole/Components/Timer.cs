using System;

namespace SadConsole.Components;

/// <summary>
/// A simple timer with callback.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Timer")]
public class Timer : Components.UpdateComponent
{
    /// <summary>
    /// Called when the timer elapses.
    /// </summary>
    public event EventHandler? TimerElapsed;

    /// <summary>
    /// Called when the timer restarts.
    /// </summary>
    public event EventHandler? TimerRestart;

    /// <summary>
    /// Called when the timer starts.
    /// </summary>
    public event EventHandler? TimerStart;

    /// <summary>
    /// Called when the timer stops.
    /// </summary>
    public event EventHandler? TimerStop;

    /// <summary>
    /// If true, the timer will restart when the time has elapsed.
    /// </summary>
    public bool Repeat { get; set; } = true;

    /// <summary>
    /// How many milliseconds to cause the timer to trigger.
    /// </summary>
    public TimeSpan TimerAmount { get; set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that the timer is running; otherwise <see langword="false"/>.
    /// </summary>
    public bool IsRunning { get; protected set; }

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
    public override void Update(IScreenObject console, TimeSpan delta)
    {
        if (IsRunning)
        {
            _countedTime += delta;

            if (_countedTime >= TimerAmount)
            {
                TimerElapsed?.Invoke(this, EventArgs.Empty);

                if (IsRunning && Repeat)
                    Restart();
                else if (IsRunning)
                    Stop();
            }
        }
    }

    /// <summary>
    /// Restarts the timer; raises the <see cref="TimerRestart"/> event.
    /// </summary>
    public void Restart()
    {
        if (IsRunning)
        {
            _countedTime = TimeSpan.Zero;
            IsRunning = true;
            TimerRestart?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Starts the timer; raises the <see cref="TimerStart"/> event.
    /// </summary>
    public void Start()
    {
        if (!IsRunning)
        {
            _countedTime = TimeSpan.Zero;
            IsRunning = true;
            TimerStart?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Starts the timer; raises the <see cref="TimerStop"/> event.
    /// </summary>
    public void Stop()
    {
        if (IsRunning)
        {
            _countedTime = TimeSpan.Zero;
            IsRunning = false;
            TimerStop?.Invoke(this, EventArgs.Empty);
        }
    }
}




