using System;
using System.Collections.Generic;
using SadConsole.Input;

namespace SadConsole.Host;

/// <summary>
/// A keyboard state populated by the client that embeds the Skia host.
/// </summary>
public sealed class Keyboard : IKeyboardState
{
    private Keys[] _pressedKeys = Array.Empty<Keys>();

    /// <inheritdoc />
    public bool CapsLock { get; private set; }

    /// <inheritdoc />
    public bool NumLock { get; private set; }

    /// <summary>
    /// Replaces the current keyboard state.
    /// </summary>
    public void SetState(IEnumerable<Keys> pressedKeys, bool capsLock = false, bool numLock = false)
    {
        ArgumentNullException.ThrowIfNull(pressedKeys);

        var uniqueKeys = new HashSet<Keys>(pressedKeys);
        uniqueKeys.Remove(Keys.None);
        _pressedKeys = new List<Keys>(uniqueKeys).ToArray();
        CapsLock = capsLock;
        NumLock = numLock;
    }

    /// <summary>
    /// Sets whether an individual key is pressed.
    /// </summary>
    public void SetKey(Keys key, bool isDown)
    {
        if (key == Keys.None)
            return;

        var keys = new HashSet<Keys>(_pressedKeys);
        if (isDown)
            keys.Add(key);
        else
            keys.Remove(key);

        _pressedKeys = new List<Keys>(keys).ToArray();
    }

    /// <inheritdoc />
    public Keys[] GetPressedKeys() => (Keys[])_pressedKeys.Clone();

    /// <inheritdoc />
    public bool IsKeyDown(Keys key) => Array.IndexOf(_pressedKeys, key) >= 0;

    /// <inheritdoc />
    public bool IsKeyUp(Keys key) => !IsKeyDown(key);

    /// <inheritdoc />
    public void Refresh() { }
}
