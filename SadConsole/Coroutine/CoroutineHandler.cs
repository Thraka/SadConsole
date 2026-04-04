using System;
using System.Collections.Generic;

namespace SadConsole.Coroutine;

/// <summary>
/// This class can be used for static coroutine handling of any kind.
/// Note that it uses an underlying <see cref="CoroutineHandlerInstance"/> object for management.
/// </summary>
public static class CoroutineHandler
{
    private static readonly CoroutineHandlerInstance s_instance = new();

    /// <inheritdoc cref="CoroutineHandlerInstance.TickingCount"/>
    public static int TickingCount => s_instance.TickingCount;

    /// <inheritdoc cref="CoroutineHandlerInstance.EventCount"/>
    public static int EventCount => s_instance.EventCount;

    /// <inheritdoc cref="CoroutineHandlerInstance.Start(IEnumerable{Wait},string,int)"/>
    public static ActiveCoroutine Start(IEnumerable<Wait> coroutine, string name = "", int priority = 0) =>
        s_instance.Start(coroutine, name, priority);

    /// <inheritdoc cref="CoroutineHandlerInstance.Start(IEnumerator{Wait},string,int)"/>
    public static ActiveCoroutine Start(IEnumerator<Wait> coroutine, string name = "", int priority = 0) =>
        s_instance.Start(coroutine, name, priority);

    /// <inheritdoc cref="CoroutineHandlerInstance.InvokeLater(Wait,Action,string,int)"/>
    public static ActiveCoroutine InvokeLater(Wait wait, Action action, string name = "", int priority = 0) =>
        s_instance.InvokeLater(wait, action, name, priority);

    /// <inheritdoc cref="CoroutineHandlerInstance.InvokeLater(Event,Action,string,int)"/>
    public static ActiveCoroutine InvokeLater(Event evt, Action action, string name = "", int priority = 0) =>
        s_instance.InvokeLater(evt, action, name, priority);

    /// <inheritdoc cref="CoroutineHandlerInstance.Tick(double)"/>
    public static void Tick(double deltaSeconds) =>
        s_instance.Tick(deltaSeconds);

    /// <inheritdoc cref="CoroutineHandlerInstance.Tick(TimeSpan)"/>
    public static void Tick(TimeSpan delta) =>
        s_instance.Tick(delta);

    /// <inheritdoc cref="CoroutineHandlerInstance.RaiseEvent"/>
    public static void RaiseEvent(Event evt) =>
        s_instance.RaiseEvent(evt);

    /// <inheritdoc cref="CoroutineHandlerInstance.GetActiveCoroutines"/>
    public static IEnumerable<ActiveCoroutine> GetActiveCoroutines() =>
        s_instance.GetActiveCoroutines();

}
