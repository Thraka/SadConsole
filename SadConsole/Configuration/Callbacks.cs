#nullable enable

using System;

namespace SadConsole.Configuration;

public static partial class Extensions
{
    /// <summary>
    /// Sets an event handler for the <see cref="GameHost.Started"/> event.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="instance_Started">The event handler</param>
    /// <returns>The configuration builder.</returns>
    public static Builder OnStart(this Builder configBuilder, EventHandler<GameHost> instance_Started)
    {
        CallbackConfig config = configBuilder.GetOrCreateConfig<CallbackConfig>();

        config.event_Started = instance_Started;

        return configBuilder;
    }

    /// <summary>
    /// Sets an event handler for the <see cref="GameHost.Ending"/> event.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="instance_Ending">The event handler</param>
    /// <returns>The configuration builder.</returns>
    public static Builder OnEnd(this Builder configBuilder, EventHandler<GameHost> instance_Ending)
    {
        CallbackConfig config = configBuilder.GetOrCreateConfig<CallbackConfig>();

        config.event_Ending = instance_Ending;

        return configBuilder;
    }

    /// <summary>
    /// Sets an event handler for the <see cref="GameHost.FrameUpdate"/> event.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="instance_FrameUpdate">The event handler</param>
    /// <returns>The configuration builder.</returns>
    public static Builder AddFrameUpdateEvent(this Builder configBuilder, EventHandler<GameHost> instance_FrameUpdate)
    {
        CallbackConfig config = configBuilder.GetOrCreateConfig<CallbackConfig>();

        config.event_FrameUpdate = instance_FrameUpdate;

        return configBuilder;
    }

    /// <summary>
    /// Sets an event handler for the <see cref="GameHost.FrameRender"/> event.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="instance_FrameRender">The event handler</param>
    /// <returns>The configuration builder.</returns>
    public static Builder AddFrameRenderEvent(this Builder configBuilder, EventHandler<GameHost> instance_FrameRender)
    {
        CallbackConfig config = configBuilder.GetOrCreateConfig<CallbackConfig>();

        config.event_FrameRender = instance_FrameRender;

        return configBuilder;
    }
}

public class CallbackConfig : IConfigurator
{
    public EventHandler<GameHost>? event_Started { get; set; }
    public EventHandler<GameHost>? event_Ending { get; set; }

    public EventHandler<GameHost>? event_FrameUpdate { get; set; }
    public EventHandler<GameHost>? event_FrameRender { get; set; }

    public void Run(Builder config, GameHost game)
    {
        if (event_Started != null)
            game.Started += event_Started;

        if (event_Ending != null)
            game.Ending += event_Ending;

        if (event_FrameUpdate != null)
            game.FrameUpdate += event_FrameUpdate;

        if (event_FrameRender != null)
            game.FrameRender += event_FrameRender;
    }
}
