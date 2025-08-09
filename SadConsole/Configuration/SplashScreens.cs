#nullable enable

using System;

namespace SadConsole.Configuration;

/// <summary>
/// Extensions to the <see cref="Builder"/> type.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Sets the startup splash screen to the specified object.
    /// </summary>
    /// <typeparam name="TSplashScreen">A parameterless <see cref="IScreenSurface"/> object.</typeparam>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetSplashScreen<TSplashScreen>(this Builder configBuilder) where TSplashScreen : IScreenSurface, new()
    {
        SplashScreenConfig screens = configBuilder.GetOrCreateConfig<SplashScreenConfig>();
        screens.GenerateSplashScreen = _ => new IScreenSurface[] { new TSplashScreen() };

        return configBuilder;
    }

    /// <summary>
    /// Sets the startup splash screens to the array returned by the <paramref name="creator"/> delegate.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="creator">A delegate that returns an array of surface objects.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetSplashScreen(this Builder configBuilder, Func<GameHost, ScreenSurface[]> creator)
    {
        SplashScreenConfig screens = configBuilder.GetOrCreateConfig<SplashScreenConfig>();
        screens.GenerateSplashScreen = creator;

        return configBuilder;
    }
}

/// <summary>
/// A config object that adds splash screen objects with the <see cref="GameHost.SetSplashScreens(IScreenSurface[])"/> method.
/// </summary>
public class SplashScreenConfig : IConfigurator
{
    /// <summary>
    /// A delegate that returns a set of splash screens to use.
    /// </summary>
    public Func<GameHost, IScreenSurface[]> GenerateSplashScreen { get; set; }

    public void Run(BuilderBase config, GameHost game)
    {
        game.SetSplashScreens(GenerateSplashScreen(game));
    }
}
