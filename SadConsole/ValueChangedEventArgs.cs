using System;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Event arguments for an event fired when an object's properties are changed. The change can be cancelled.
/// </summary>
public class ValueChangedCancelableEventArgs<T> : ValueChangedEventArgs<T>
{
    /// <summary>
    /// Setting this property to <see langword="true"/> indicates that the change should be cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Creates a new instance of this object with the specified old and new value.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    public ValueChangedCancelableEventArgs(T oldValue, T newValue): base(oldValue, newValue) { }
}
