namespace SadConsole.Input;

/// <summary>
/// Event handler to preview key presses and cancel them.
/// </summary>
public class KeyboardHandledKeyEventArgs : HandledEventArgs
{
    /// <summary>
    /// The key being pressed.
    /// </summary>
    public AsciiKey Key { get; internal set; }
}
