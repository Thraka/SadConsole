using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SadConsole.Coroutine;

/// <summary>
/// A reference to a currently running coroutine.
/// This is returned by <see cref="CoroutineHandler.Start(System.Collections.Generic.IEnumerator{Coroutine.Wait},string,int)"/>.
/// </summary>
public class ActiveCoroutine : IComparable<ActiveCoroutine>
{

    private readonly IEnumerator<Wait> _enumerator;
    private readonly Stopwatch _stopwatch;
    private Wait _current;

    internal Event Event => _current.Event;

    internal bool IsWaitingForEvent => Event != null;

    /// <summary>
    /// This property stores whether or not this active coroutine is finished.
    /// A coroutine is finished if all of its waits have passed, or if it <see cref="WasCanceled"/>.
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    /// This property stores whether or not this active coroutine was cancelled using <see cref="Cancel"/>.
    /// </summary>
    public bool WasCanceled { get; private set; }

    /// <summary>
    /// The total amount of time that <see cref="MoveNext"/> took.
    /// This is the amount of time that this active coroutine took for the entirety of its "steps", or yield statements.
    /// </summary>
    public TimeSpan TotalMoveNextTime { get; private set; }

    /// <summary>
    /// The total amount of times that <see cref="MoveNext"/> was invoked.
    /// This is the amount of "steps" in your coroutine, or the amount of yield statements.
    /// </summary>
    public int MoveNextCount { get; private set; }

    /// <summary>
    /// The amount of time that the last <see cref="MoveNext"/> took.
    /// This is the amount of time that this active coroutine took for the last "step", or yield statement.
    /// </summary>
    public TimeSpan LastMoveNextTime { get; private set; }

    /// <summary>
    /// An event that gets fired when this active coroutine finishes or gets cancelled.
    /// When this event is called, <see cref="IsFinished"/> is always true.
    /// </summary>
    public event FinishCallback? OnFinished;

    /// <summary>
    /// The name of this coroutine.
    /// When not specified on startup of this coroutine, the name defaults to an empty string.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The priority of this coroutine. The higher the priority, the earlier it is advanced compared to other coroutines that advance around the same time.
    /// When not specified at startup of this coroutine, the priority defaults to 0.
    /// </summary>
    public readonly int Priority;

    internal ActiveCoroutine(IEnumerator<Wait> enumerator, string name, int priority, Stopwatch stopwatch)
    {
        _enumerator = enumerator;
        Name = name;
        Priority = priority;
        _stopwatch = stopwatch;
    }

    /// <summary>
    /// Cancels this coroutine, causing all subsequent <see cref="Wait"/>s and any code in between to be skipped.
    /// </summary>
    /// <returns>Whether the cancellation was successful, or this coroutine was already cancelled or finished</returns>
    public bool Cancel()
    {
        if (IsFinished || WasCanceled)
            return false;

        WasCanceled = true;
        IsFinished = true;
        OnFinished?.Invoke(this);
        return true;
    }

    internal bool Tick(double deltaSeconds)
    {
        if (!WasCanceled && _current.Tick(deltaSeconds))
            MoveNext();

        return IsFinished;
    }

    internal bool OnEvent(Event evt)
    {
        if (!WasCanceled && object.Equals(_current.Event, evt))
            MoveNext();

        return IsFinished;
    }

    internal bool MoveNext()
    {
        _stopwatch.Restart();
        bool result = _enumerator.MoveNext();
        _stopwatch.Stop();
        LastMoveNextTime = _stopwatch.Elapsed;
        TotalMoveNextTime += _stopwatch.Elapsed;
        MoveNextCount++;

        if (!result)
        {
            IsFinished = true;
            OnFinished?.Invoke(this);
            return false;
        }
        _current = _enumerator.Current;
        return true;
    }

    /// <summary>
    /// A delegate method used by <see cref="ActiveCoroutine.OnFinished"/>.
    /// </summary>
    /// <param name="coroutine">The coroutine that finished</param>
    public delegate void FinishCallback(ActiveCoroutine coroutine);

    /// <inheritdoc />
    public int CompareTo(ActiveCoroutine? other)
    {
        return other.Priority.CompareTo(Priority);
    }

}
