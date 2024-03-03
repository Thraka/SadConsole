using System;

namespace SadConsole.Components;

/// <summary>
/// A component used only with <see cref="GameHost.RootComponents"/>. Runs logic before the <see cref="GameHost.Screen"/> is processed.
/// </summary>
public abstract class RootComponent
{
    /// <summary>
    /// Code to run during update.
    /// </summary>
    /// <param name="delta">The time that has elapsed since the last frame.</param>
    public abstract void Run(TimeSpan delta);
}
