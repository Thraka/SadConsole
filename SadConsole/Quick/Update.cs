using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SadConsole.Quick;

/// <summary>
/// Adds logic extension methods for <see cref="IScreenObject"/>.
/// </summary>
public static class Update
{
    /// <summary>
    /// Adds a keyboard handler to a <see cref="IScreenObject"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    /// <param name="handler">The handler callback.</param>
    public static void WithUpdate(this IScreenObject screenObject, Action<IScreenObject, TimeSpan> handler)
    {
        foreach (UpdateHook hook in screenObject.GetSadComponents<UpdateHook>())
        {
            if (hook.Method == handler)
                return;
        }

        screenObject.SadComponents.Add(new UpdateHook(handler));
    }

    /// <summary>
    /// Removes all of the keyboard hooks added with <see cref="WithUpdate(IScreenObject, Action{IScreenObject, TimeSpan})"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    public static void RemoveUpdateHooks(this IScreenObject screenObject)
    {
        foreach (UpdateHook hook in screenObject.GetSadComponents<UpdateHook>().ToArray())
            screenObject.SadComponents.Remove(hook);
    }

    /// <summary>
    /// Removes the specified handler that was added with <see cref="WithUpdate(IScreenObject, Action{IScreenObject, TimeSpan})"/>.
    /// </summary>
    /// <param name="screenObject">The object to use.</param>
    /// <param name="handler">The handler callback.</param>
    public static void RemoveUpdateHook(this IScreenObject screenObject, Action<IScreenObject, TimeSpan> handler)
    {
        UpdateHook existingHook = null;

        foreach (UpdateHook hook in screenObject.GetSadComponents<UpdateHook>())
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

    private class UpdateHook : Components.UpdateComponent
    {
        public Action<IScreenObject, TimeSpan> Method;

        public UpdateHook(Action<IScreenObject, TimeSpan> updateMethod) =>
            Method = updateMethod;

        public override void Update(IScreenObject host, TimeSpan delta) =>
            Method(host, delta);
    }
}
