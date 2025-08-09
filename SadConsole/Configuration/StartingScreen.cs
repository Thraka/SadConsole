﻿using System;

namespace SadConsole.Configuration;

public static partial class Extensions
{
    /// <summary>
    /// Sets the <see cref="SadConsole.GameHost.Screen"/> property to the specified type.
    /// </summary>
    /// <typeparam name="TRootObject">A parameterless <see cref="IScreenObject"/> object.</typeparam>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetStartingScreen<TRootObject>(this Builder configBuilder) where TRootObject : IScreenObject, new()
    {
        StartingScreenConfig startup = configBuilder.GetOrCreateConfig<StartingScreenConfig>();
        startup.GenerateStartingObject = _ => new TRootObject();

        return configBuilder;
    }

    /// <summary>
    /// Sets the <see cref="SadConsole.GameHost.Screen"/> property to the return value of the <paramref name="creator"/> parameter.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="creator">A method that returns an object as the starting screen.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetStartingScreen(this Builder configBuilder, Func<GameHost, IScreenObject> creator)
    {
        StartingScreenConfig startup = configBuilder.GetOrCreateConfig<StartingScreenConfig>();
        startup.GenerateStartingObject = creator;

        return configBuilder;
    }

    /// <summary>
    /// Either focuses or unfocuses the starting screen.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="value">Indicates whether or not <see cref="GameHost.Screen"/> is focused.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder IsStartingScreenFocused(this Builder configBuilder, bool value)
    {
        StartingScreenConfig data = configBuilder.GetOrCreateConfig<StartingScreenConfig>();

        data.FocusStartingScreen = value;

        return configBuilder;
    }
}

public class StartingScreenConfig : IConfigurator
{
    public Func<GameHost, IScreenObject> GenerateStartingObject { get; set; }

    public bool? FocusStartingScreen { get; set; } = null;

    public void Run(BuilderBase configBuilder, GameHost game)
    {
        game.FocusedScreenObjects.Clear();
        game.Screen = GenerateStartingObject(game);
        game.DestroyDefaultStartingConsole();

        if (FocusStartingScreen.HasValue)
            game.Screen.IsFocused = FocusStartingScreen.Value;
    }
}
