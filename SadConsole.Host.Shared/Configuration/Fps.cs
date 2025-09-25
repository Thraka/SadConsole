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

public class FpsConfig : IConfigurator
{
    public bool UnlimitedFPS { get; set; } = false;

#if MONOGAME
    public bool ShowFPSVisual { get; set; }
#endif

    public void Run(BuilderBase configBuilder, GameHost game)
    {
    }
}
