#nullable enable
#if MONOGAME

using System;

namespace SadConsole.Configuration;

public static partial class ExtensionsHost
{
    /// <summary>
    /// The <paramref name="monogameCtorCallback"/> method is called by the MonoGame constructor. Some MonoGame specific settings may only be settable via the constructor.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="monogameCtorCallback">A method.</param>
    /// <returns>The configuration object.</returns>
    public static Builder WithMonoGameCtor(this Builder configBuilder, Action<Host.Game> monogameCtorCallback)
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
    internal static Builder WithMonoGameInit(this Builder configBuilder, Action<Host.Game> monogameInitCallback)
    {
        MonoGameCallbackConfig config = configBuilder.GetOrCreateConfig<MonoGameCallbackConfig>();

        config.MonoGameInitCallback = monogameInitCallback;

        return configBuilder;
    }
}

public class MonoGameCallbackConfig : IConfigurator
{
    public Action<Host.Game>? MonoGameCtorCallback { get; set; }
    public Action<Host.Game>? MonoGameInitCallback { get; set; }

    public void Run(Builder config, GameHost game)
    {
    }
}
#endif
