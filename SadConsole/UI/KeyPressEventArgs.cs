using System;

namespace SadConsole.UI;

/// <summary>
/// Event arguments to indicate that a key is being pressed on a control that allows keyboard key cancelling.
/// </summary>
public class KeyPressEventArgs : EventArgs
{
    /// <summary>
    /// The key being pressed by the textbox.
    /// </summary>
    public readonly Input.AsciiKey Key;

    /// <summary>
    /// When set to <see langword="true"/>, causes the textbox to cancel the key press.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Creates a new event args object.
    /// </summary>
    /// <param name="key">The key being pressed.</param>
    public KeyPressEventArgs(Input.AsciiKey key) =>
        Key = key;
}
