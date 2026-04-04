using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SadConsole.Coroutine;

/// <summary>
/// An object of this class can be used to start, tick and otherwise manage active <see cref="ActiveCoroutine"/>s as well as their <see cref="Event"/>s.
/// Note that a static implementation of this can be found in <see cref="CoroutineHandler"/>.
/// </summary>
public class CoroutineHandlerInstance
{

    private readonly List<ActiveCoroutine> _tickingCoroutines = [];
    private readonly Dictionary<Event, List<ActiveCoroutine>> _eventCoroutines = [];
    private readonly HashSet<(Event, ActiveCoroutine)> _eventCoroutinesToRemove = [];
    private readonly HashSet<ActiveCoroutine> _outstandingEventCoroutines = [];
    private readonly HashSet<ActiveCoroutine> _outstandingTickingCoroutines = [];
    private readonly Stopwatch _stopwatch = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// The amount of <see cref="ActiveCoroutine"/> instances that are currently waiting for a tick (waiting for time to pass)
    /// </summary>
    public int TickingCount
    {
        get
        {
            lock (_lockObject)
                return _tickingCoroutines.Count;
        }
    }
    /// <summary>
    /// The amount of <see cref="ActiveCoroutine"/> instances that are currently waiting for an <see cref="Event"/>
    /// </summary>
    public int EventCount
    {
        get
        {
            lock (_lockObject)
                return _eventCoroutines.Sum(c => c.Value.Count);
        }
    }

    /// <summary>
    /// Starts the given coroutine, returning a <see cref="ActiveCoroutine"/> object for management.
    /// Note that this calls <see cref="IEnumerable{T}.GetEnumerator"/> to get the enumerator.
    /// </summary>
    /// <param name="coroutine">The coroutine to start</param>
    /// <param name="name">The <see cref="ActiveCoroutine.Name"/> that this coroutine should have. Defaults to an empty string.</param>
    /// <param name="priority">The <see cref="ActiveCoroutine.Priority"/> that this coroutine should have. The higher the priority, the earlier it is advanced. Defaults to 0.</param>
    /// <returns>An active coroutine object representing this coroutine</returns>
    public ActiveCoroutine Start(IEnumerable<Wait> coroutine, string name = "", int priority = 0) =>
        Start(coroutine.GetEnumerator(), name, priority);

    /// <summary>
    /// Starts the given coroutine, returning a <see cref="ActiveCoroutine"/> object for management.
    /// </summary>
    /// <param name="coroutine">The coroutine to start</param>
    /// <param name="name">The <see cref="ActiveCoroutine.Name"/> that this coroutine should have. Defaults to an empty string.</param>
    /// <param name="priority">The <see cref="ActiveCoroutine.Priority"/> that this coroutine should have. The higher the priority, the earlier it is advanced compared to other coroutines that advance around the same time. Defaults to 0.</param>
    /// <returns>An active coroutine object representing this coroutine</returns>
    public ActiveCoroutine Start(IEnumerator<Wait> coroutine, string name = "", int priority = 0)
    {
        var inst = new ActiveCoroutine(coroutine, name, priority, _stopwatch);
        if (inst.MoveNext())
        {
            lock (_lockObject)
                GetOutstandingCoroutines(inst.IsWaitingForEvent).Add(inst);
        }
        return inst;
    }

    /// <summary>
    /// Causes the given action to be invoked after the given <see cref="Wait"/>.
    /// This is equivalent to a coroutine that waits for the given wait and then executes the given <see cref="Action"/>.
    /// </summary>
    /// <param name="wait">The wait to wait for</param>
    /// <param name="action">The action to execute after waiting</param>
    /// <param name="name">The <see cref="ActiveCoroutine.Name"/> that the underlying coroutine should have. Defaults to an empty string.</param>
    /// <param name="priority">The <see cref="ActiveCoroutine.Priority"/> that the underlying coroutine should have. The higher the priority, the earlier it is advanced compared to other coroutines that advance around the same time. Defaults to 0.</param>
    /// <returns>An active coroutine object representing this coroutine</returns>
    public ActiveCoroutine InvokeLater(Wait wait, Action action, string name = "", int priority = 0) =>
        Start(CoroutineHandlerInstance.InvokeLaterImpl(wait, action), name, priority);

    /// <summary>
    /// Causes the given action to be invoked after the given <see cref="Event"/>.
    /// This is equivalent to a coroutine that waits for the given wait and then executes the given <see cref="Action"/>.
    /// </summary>
    /// <param name="evt">The event to wait for</param>
    /// <param name="action">The action to execute after waiting</param>
    /// <param name="name">The <see cref="ActiveCoroutine.Name"/> that the underlying coroutine should have. Defaults to an empty string.</param>
    /// <param name="priority">The <see cref="ActiveCoroutine.Priority"/> that the underlying coroutine should have. The higher the priority, the earlier it is advanced compared to other coroutines that advance around the same time. Defaults to 0.</param>
    /// <returns>An active coroutine object representing this coroutine</returns>
    public ActiveCoroutine InvokeLater(Event evt, Action action, string name = "", int priority = 0) =>
        InvokeLater(new Wait(evt), action, name, priority);

    /// <summary>
    /// Ticks this coroutine handler, causing all time-based <see cref="Wait"/>s to be ticked.
    /// </summary>
    /// <param name="deltaSeconds">The amount of seconds that have passed since the last time this method was invoked</param>
    public void Tick(double deltaSeconds)
    {
        lock (_lockObject)
        {
            MoveOutstandingCoroutines(false);
            _tickingCoroutines.RemoveAll(c =>
            {
                if (c.Tick(deltaSeconds))
                {
                    return true;
                }
                else if (c.IsWaitingForEvent)
                {
                    _outstandingEventCoroutines.Add(c);
                    return true;
                }
                return false;
            });
        }
    }

    /// <summary>
    /// Ticks this coroutine handler, causing all time-based <see cref="Wait"/>s to be ticked.
    /// This is a convenience method that calls <see cref="Tick(double)"/>, but accepts a <see cref="TimeSpan"/> instead of an amount of seconds.
    /// </summary>
    /// <param name="delta">The time that has passed since the last time this method was invoked</param>
    public void Tick(TimeSpan delta) =>
        Tick(delta.TotalSeconds);

    /// <summary>
    /// Raises the given event, causing all event-based <see cref="Wait"/>s to be updated.
    /// </summary>
    /// <param name="evt">The event to raise</param>
    public void RaiseEvent(Event evt)
    {
        lock (_lockObject)
        {
            MoveOutstandingCoroutines(true);
            List<ActiveCoroutine> coroutines = GetEventCoroutines(evt, false);
            if (coroutines != null)
            {
                for (int i = 0; i < coroutines.Count; i++)
                {
                    ActiveCoroutine c = coroutines[i];
                    (Event Event, ActiveCoroutine c) tup = (c.Event, c);
                    if (_eventCoroutinesToRemove.Contains(tup))
                        continue;
                    if (c.OnEvent(evt))
                    {
                        _eventCoroutinesToRemove.Add(tup);
                    }
                    else if (!c.IsWaitingForEvent)
                    {
                        _eventCoroutinesToRemove.Add(tup);
                        _outstandingTickingCoroutines.Add(c);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns a list of all currently active <see cref="ActiveCoroutine"/> objects under this handler.
    /// </summary>
    /// <returns>All active coroutines</returns>
    public IEnumerable<ActiveCoroutine> GetActiveCoroutines()
    {
        lock (_lockObject)
            return _tickingCoroutines.Concat(_eventCoroutines.Values.SelectMany(c => c));
    }

    private void MoveOutstandingCoroutines(bool evt)
    {
        // RemoveWhere is twice as fast as iterating and then clearing
        if (_eventCoroutinesToRemove.Count > 0)
        {
            _eventCoroutinesToRemove.RemoveWhere(c =>
            {
                GetEventCoroutines(c.Item1, false).Remove(c.Item2);
                return true;
            });
        }
        HashSet<ActiveCoroutine> coroutines = GetOutstandingCoroutines(evt);
        if (coroutines.Count > 0)
        {
            coroutines.RemoveWhere(c =>
            {
                List<ActiveCoroutine> list = evt ? GetEventCoroutines(c.Event, true) : _tickingCoroutines;
                int position = list.BinarySearch(c);
                list.Insert(position < 0 ? ~position : position, c);
                return true;
            });
        }
    }

    private HashSet<ActiveCoroutine> GetOutstandingCoroutines(bool evt) =>
        evt ? _outstandingEventCoroutines : _outstandingTickingCoroutines;

    private List<ActiveCoroutine> GetEventCoroutines(Event evt, bool create)
    {
        if (!_eventCoroutines.TryGetValue(evt, out List<ActiveCoroutine>? ret) && create)
        {
            ret = [];
            _eventCoroutines.Add(evt, ret);
        }
        return ret;
    }

    private static IEnumerator<Wait> InvokeLaterImpl(Wait wait, Action action)
    {
        yield return wait;
        action();
    }

}
