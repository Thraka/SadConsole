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
    /// <param name="monogameCtorCallback">A method.</param>
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
    /// <param name="monogameInitCallback">A method.</param>
    /// <returns>The configuration object.</returns>
    public static Builder WithMonoGameInit(this Builder configBuilder, Action<MonoGameGame> monogameInitCallback)
    {
        MonoGameCallbackConfig config = configBuilder.GetOrCreateConfig<MonoGameCallbackConfig>();

        config.MonoGameInitCallback = monogameInitCallback;

        return configBuilder;
    }

    /// <summary>
    /// When called, tells the game host not to create the monogame game instance at <see cref="Game.MonoGameInstance"/>.
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

public class MonoGameCallbackConfig : IConfigurator
{
    public Action<MonoGameGame>? MonoGameCtorCallback { get; set; }
    public Action<MonoGameGame>? MonoGameInitCallback { get; set; }

    public bool SkipMonoGameGameCreation { get; set; }

    public void Run(BuilderBase config, GameHost game)
    {
    }
}
#endif
