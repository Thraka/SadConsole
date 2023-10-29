#nullable enable

using System;

namespace SadConsole.Configuration;

#if SFML
public static partial class Extensions
{
    /// <summary>
    /// Sets the target window SadConsole renders to instead of building its own window.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="window">The target window to render the game on.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetTargetWindow(this Builder configBuilder, SFML.Graphics.RenderWindow window)
    {
        InternalStartupData config = configBuilder.GetOrCreateConfig<InternalStartupData>();

        config.TargetWindow = window;

        return configBuilder;
    }
}
#endif

public class InternalStartupData : IConfigurator
{
    public int ScreenCellsX { get; set; } = 80;
    public int ScreenCellsY { get; set; } = 25;

    public bool? FocusStartingScreen { get; set; } = null;

#if SFML
    public SFML.Graphics.RenderWindow? TargetWindow { get; set; }
#endif

    public void Run(Builder config, Game game)
    {
    }
}
