using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole;

/// <summary>
/// The old value and the value it changed to.
/// </summary>
public class ValueChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// The previous object.
    /// </summary>
    public readonly T OldValue;

    /// <summary>
    /// The new object.
    /// </summary>
    public readonly T NewValue;

    /// <summary>
    /// When <see langword="true"/>, indicates this value change can be cancelled; otherwise <see langword="false"/>.
    /// </summary>
    public readonly bool SupportsCancel;

    /// <summary>
    /// When <see langword="true"/>, indicates this value change can be flagged as handled and stop further event handlers; otherwise <see langword="false"/>.
    /// </summary>
    public readonly bool SupportsHandled;

    /// <summary>
    /// When <see cref="SupportsCancel"/> is <see langword="true"/>, setting this property to <see langword="true"/> causes the value change to be cancelled and to stop processing further event handlers.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// When <see cref="SupportsHandled"/> is <see langword="true"/>, setting this property to <see langword="true"/> flags this change as handled and to stop processing further event handlers.
    /// </summary>
    public bool IsHandled { get; set; }

    /// <summary>
    /// Creates a new instance of this object with the specified old and new value.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="supportsCancel">When <see langword="true"/>, indicates this value change can be cancelled.</param>
    /// <param name="supportsHandled">When <see langword="true"/>, indicates this value change can be flagged as handled and stop further event handlers.</param>
    public ValueChangedEventArgs(T oldValue, T newValue, bool supportsCancel = false, bool supportsHandled = false) =>
        (OldValue, NewValue, SupportsCancel, SupportsHandled) = (oldValue, newValue, supportsCancel, supportsCancel);
}
