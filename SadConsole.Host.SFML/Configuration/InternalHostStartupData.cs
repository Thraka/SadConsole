#nullable enable

using System;

namespace SadConsole.Configuration;

public static partial class ExtensionsHost
{
    /// <summary>
    /// Sets the target window SadConsole renders to instead of building its own window.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="window">The target window to render the game on.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetTargetWindow(this Builder configBuilder, SFML.Graphics.RenderWindow window)
    {
        InternalHostStartupData config = configBuilder.GetOrCreateConfig<InternalHostStartupData>();

        config.TargetWindow = window;

        return configBuilder;
    }
}

public class InternalHostStartupData : IConfigurator
{
    public SFML.Graphics.RenderWindow? TargetWindow { get; set; }

    public void Run(BuilderBase config, GameHost game)
    {
    }
}
