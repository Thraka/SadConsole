#nullable enable

namespace SadConsole.Configuration;

public static partial class ExtensionsHost
{
    /// <summary>
    /// Unlimited FPS when rendering (normally limited to 60fps). Must be set before the game is created.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder UseUnlimitedFPS(this Builder configBuilder)
    {
        FpsConfig config = configBuilder.GetOrCreateConfig<FpsConfig>();

        config.UnlimitedFPS = true;

        return configBuilder;
    }

#if MONOGAME
    /// <summary>
    /// Adds a MonoGame game component that renders the frames per-second of the game.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder ShowMonoGameFPS(this Builder configBuilder)
    {
        FpsConfig config = configBuilder.GetOrCreateConfig<FpsConfig>();

        config.ShowFPSVisual = true;

        return configBuilder;
    }
#endif
}

/// <summary>
/// Configuration for FPS settings in SadConsole.
/// </summary>
public class FpsConfig : IConfigurator
{
    /// <summary>
    /// Gets or sets a value indicating whether unlimited FPS is enabled.
    /// </summary>
    public bool UnlimitedFPS { get; set; } = false;

#if MONOGAME
    /// <summary>
    /// Gets or sets a value indicating whether to show the FPS visual in MonoGame.
    /// </summary>
    public bool ShowFPSVisual { get; set; }
#endif

    /// <summary>
    /// Runs the FPS configuration.
    /// </summary>
    /// <param name="configBuilder">The configuration builder.</param>
    /// <param name="game">The game host.</param>
    public void Run(BuilderBase configBuilder, GameHost game)
    {
    }
}
