using System;

namespace SadConsole;

/// <summary>
/// Event args that allow a handled flag to be set.
/// </summary>
public class HandledEventArgs : EventArgs
{
    /// <summary>
    /// When <see langword="true"/> indicates that the event has been handled and no more processing should continue.
    /// </summary>
    public bool IsHandled { get; set; }
}
