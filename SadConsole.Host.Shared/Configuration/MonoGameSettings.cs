#nullable enable
#if MONOGAME

using System;

#if WPF
using MonoGameGame = MonoGame.Framework.WpfInterop.WpfGame;
#else
using MonoGameGame = Microsoft.Xna.Framework.Game;
#endif

namespace SadConsole.Configuration;

public static partial class ExtensionsHost
{
    /// <summary>
    /// Tells the game host to use the <see cref="Microsoft.Xna.Framework.TitleContainer"/> to open streams for reading.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration object.</returns>
    public static Builder UseTitleContainer(this Builder configBuilder)
    {
        MonoGameSettings config = configBuilder.GetOrCreateConfig<MonoGameSettings>();

        config.UseTitleContainer = true;

        return configBuilder;
    }
}

/// <summary>
/// Configuration for MonoGame-specific settings in SadConsole.
/// </summary>
public class MonoGameSettings : IConfigurator
{
    /// <summary>
    /// Gets or sets a value indicating whether to use TitleContainer for opening streams.
    /// </summary>
    public bool UseTitleContainer { get; set; } = false;

    /// <summary>
    /// Runs the MonoGame settings configuration.
    /// </summary>
    /// <param name="config">The configuration builder.</param>
    /// <param name="game">The game host.</param>
    public void Run(BuilderBase config, GameHost game)
    {
    }
}
#endif
