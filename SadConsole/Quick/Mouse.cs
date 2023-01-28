using System;
using System.Linq;
using SadConsole.Input;

namespace SadConsole.Quick;

/// <summary>
/// Adds mouse-related extension methods for <see cref="IScreenObject"/>.
/// </summary>
public static class Mouse
{
    /// <summary>
    /// Adds a mouse handler to a <see cref="IScreenObject"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    /// <param name="handler">The handler callback.</param>
    public static IScreenObject WithMouse(this IScreenObject screenObject, Func<IScreenObject, MouseScreenObjectState, bool> handler)
    {
        foreach (MouseHook hook in screenObject.GetSadComponents<MouseHook>())
        {
            if (hook.Callback == handler)
                return screenObject;
        }

        screenObject.SadComponents.Add(new MouseHook(handler));

        return screenObject;
    }

    /// <summary>
    /// Removes all of the mouse hooks added with <see cref="WithMouse(IScreenObject, Func{IScreenObject, MouseScreenObjectState, bool})"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    public static IScreenObject RemoveMouseHooks(this IScreenObject screenObject)
    {
        foreach (MouseHook hook in screenObject.GetSadComponents<MouseHook>().ToArray())
            screenObject.SadComponents.Remove(hook);

        return screenObject;
    }

    /// <summary>
    /// Removes the specified handler that was added with <see cref="WithMouse(IScreenObject, Func{IScreenObject, MouseScreenObjectState, bool})"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    /// <param name="handler">The handler callback.</param>
    public static IScreenObject RemoveMouseHook(this IScreenObject screenObject, Func<IScreenObject, MouseScreenObjectState, bool> handler)
    {
        MouseHook? existingHook = null;

        foreach (MouseHook hook in screenObject.GetSadComponents<MouseHook>())
        {
            if (hook.Callback == handler)
            {
                existingHook = hook;
                break;
            }
        }

        if (existingHook != null)
            screenObject.SadComponents.Remove(existingHook);

        return screenObject;
    }

    private class MouseHook : Components.MouseConsoleComponent
    {
        public Func<IScreenObject, MouseScreenObjectState, bool> Callback;

        public MouseHook(Func<IScreenObject, MouseScreenObjectState, bool> mouseCallback) =>
            Callback = mouseCallback;

        public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            handled = Callback(host, state);
    }
}
