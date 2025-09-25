namespace SadConsole.Input;

/// <summary>
/// Holds the state of keystrokes by a keyboard.
/// </summary>
public interface IKeyboardState
{
    /// <summary>
    /// Gets the current state of the Caps Lock key.
    /// </summary>
    public bool CapsLock { get; }

    /// <summary>
    /// Gets the current state of the Num Lock key.
    /// </summary>
    public bool NumLock { get; }

    /// <summary>
    /// Gets whether given key is currently being pressed.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns>true if the key is pressed; false otherwise.</returns>
    public bool IsKeyDown(Keys key);

    /// <summary>
    /// Gets whether given key is currently being not pressed.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns>true if the key is not pressed; false otherwise.</returns>
    public bool IsKeyUp(Keys key);

    /// <summary>
    /// Returns an array of values holding keys that are currently being pressed.
    /// </summary>
    /// <returns>The keys that are currently being pressed.</returns>
    public Keys[] GetPressedKeys();

    /// <summary>
    /// If applicable to the host implementation, refreshes the keyboard state.
    /// </summary>
    void Refresh();
}
