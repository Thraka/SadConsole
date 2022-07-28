using System;
using System.Linq;

namespace SadConsole.Quick;

/// <summary>
/// Adds keyboard-related extension methods for <see cref="IScreenObject"/>.
/// </summary>
public static class Keyboard
{
    /// <summary>
    /// Adds a keyboard handler to a <see cref="IScreenObject"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    /// <param name="handler">The handler callback.</param>
    public static void WithKeyboard(this IScreenObject screenObject, Func<IScreenObject, Input.Keyboard, bool> handler)
    {
        foreach (KeyboardHook hook in screenObject.GetSadComponents<KeyboardHook>())
        {
            if (hook.Method == handler)
                return;
        }

        screenObject.SadComponents.Add(new KeyboardHook(handler));
    }

    /// <summary>
    /// Removes all of the keyboard hooks added with <see cref="WithKeyboard(IScreenObject, Func{IScreenObject, Input.Keyboard, bool})"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    public static void RemoveKeyboardHooks(this IScreenObject screenObject)
    {
        foreach (KeyboardHook hook in screenObject.GetSadComponents<KeyboardHook>().ToArray())
            screenObject.SadComponents.Remove(hook);
    }

    /// <summary>
    /// Removes the specified handler that was added with <see cref="WithKeyboard(IScreenObject, Func{IScreenObject, Input.Keyboard, bool})"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    /// <param name="handler">The handler callback.</param>
    public static void RemoveKeyboardHook(this IScreenObject screenObject, Func<IScreenObject, Input.Keyboard, bool> handler)
    {
        KeyboardHook existingHook = null;

        foreach (KeyboardHook hook in screenObject.GetSadComponents<KeyboardHook>())
        {
            if (hook.Method == handler)
            {
                existingHook = hook;
                break;
            }
        }

        if (existingHook != null)
            screenObject.SadComponents.Remove(existingHook);
    }

    private class KeyboardHook : Components.KeyboardConsoleComponent
    {
        public Func<IScreenObject, Input.Keyboard, bool> Method;

        public KeyboardHook(Func<IScreenObject, Input.Keyboard, bool> keyboardMethod) =>
            Method = keyboardMethod;

        public override void ProcessKeyboard(IScreenObject host, Input.Keyboard keyboard, out bool handled) =>
            handled = Method(host, keyboard);
    }
}
