#nullable enable

using System;
using SadRogue.Primitives;

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
    /// Sets the initial screen size of the window, in cells.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="width">The cell count along the x-axis.</param>
    /// <param name="height">The cell count along the y-axis.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetScreenSize(this Builder configBuilder, int width, int height)
    {
        InternalStartupData data = configBuilder.GetOrCreateConfig<InternalStartupData>();

        data.ScreenCellsX = width;
        data.ScreenCellsY = height;

        return configBuilder;
    }

    /// <summary>
    /// Sets the initial screen size of the window based on the given resolution in pixels.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="resolution">The screen resolution in pixels.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetScreenSize(this Builder configBuilder, Point resolution)
    {
        InternalStartupData data = configBuilder.GetOrCreateConfig<InternalStartupData>();

        data.ScreenCellsXYByResolution = (gameHost) =>
        {
            Point defaultFontSize = gameHost.DefaultFont.GetFontSize(gameHost.DefaultFontSize);
            return new Point(resolution.X / defaultFontSize.X, resolution.Y / defaultFontSize.Y);
        };

        return configBuilder;
    }
}

public class StartingScreenConfig : IConfigurator
{
    public Func<GameHost, IScreenObject> GenerateStartingObject { get; set; }

    public void Run(Builder configBuilder, GameHost game)
    {
        game.FocusedScreenObjects.Clear();
        game.Screen = GenerateStartingObject(game);
        game.DestroyDefaultStartingConsole();

        InternalStartupData data = configBuilder.GetOrCreateConfig<InternalStartupData>();
        if (data.FocusStartingScreen.HasValue)
            game.Screen.IsFocused = data.FocusStartingScreen.Value;
    }
}
