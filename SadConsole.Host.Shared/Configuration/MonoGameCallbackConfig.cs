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
    /// The <paramref name="monogameCtorCallback"/> method is called by the MonoGame constructor. Some MonoGame specific settings may only be settable via the constructor.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="monogameCtorCallback">A method to call during MonoGame construction.</param>
    /// <returns>The configuration object.</returns>
    public static Builder WithMonoGameCtor(this Builder configBuilder, Action<MonoGameGame> monogameCtorCallback)
    {
        MonoGameCallbackConfig config = configBuilder.GetOrCreateConfig<MonoGameCallbackConfig>();

        config.MonoGameCtorCallback = monogameCtorCallback;

        return configBuilder;
    }

    /// <summary>
    /// Internal only. Called by the MonoGame game to finish configuring SadConsole.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="monogameInitCallback">A method to call during MonoGame initialization.</param>
    /// <returns>The configuration object.</returns>
    public static Builder WithMonoGameInit(this Builder configBuilder, Action<MonoGameGame> monogameInitCallback)
    {
        MonoGameCallbackConfig config = configBuilder.GetOrCreateConfig<MonoGameCallbackConfig>();

        config.MonoGameInitCallback = monogameInitCallback;

        return configBuilder;
    }

    /// <summary>
    /// When called, tells the game host not to create the MonoGame game instance at <see cref="Game.MonoGameInstance"/>.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration object.</returns>
    public static Builder SkipMonoGameGameCreation(this Builder configBuilder)
    {
        MonoGameCallbackConfig config = configBuilder.GetOrCreateConfig<MonoGameCallbackConfig>();

        config.SkipMonoGameGameCreation = true;

        return configBuilder;
    }
}

/// <summary>
/// Configuration for MonoGame callback settings in SadConsole.
/// </summary>
public class MonoGameCallbackConfig : IConfigurator
{
    /// <summary>
    /// Gets or sets the callback to invoke during MonoGame construction.
    /// </summary>
    public Action<MonoGameGame>? MonoGameCtorCallback { get; set; }
    /// <summary>
    /// Gets or sets the callback to invoke during MonoGame initialization.
    /// </summary>
    public Action<MonoGameGame>? MonoGameInitCallback { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to skip MonoGame game creation.
    /// </summary>
    public bool SkipMonoGameGameCreation { get; set; }

    /// <summary>
    /// Runs the MonoGame callback configuration.
    /// </summary>
    /// <param name="config">The configuration builder.</param>
    /// <param name="game">The game host.</param>
    public void Run(BuilderBase config, GameHost game)
    {
    }
}
#endif
