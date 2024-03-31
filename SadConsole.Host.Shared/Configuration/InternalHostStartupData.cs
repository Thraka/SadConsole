#nullable enable

using System;

namespace SadConsole.Configuration;

#if SFML
public static partial class ExtensionsHost
{
    /// <summary>
    /// Sets the target window SadConsole renders to instead of building its own window.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="window">The target window to render the game on.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetTargetWindow(this Builder configBuilder, SFML.Graphics.RenderWindow window)
    {
        InternalHostStartupData config = configBuilder.GetOrCreateConfig<InternalHostStartupData>();

        config.TargetWindow = window;

        return configBuilder;
    }
}
#elif WPF
public static partial class ExtensionsHost
{
    /// <summary>
    /// Sets the <see cref="Settings.Rendering.RenderWidth"/> and <see cref="Settings.Rendering.RenderHeight"/> values.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="width">The total pixel width of the control.</param>
    /// <param name="height">The total pixel height of the control.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetInitialRenderPixels(this Builder configBuilder, int width, int height)
    {
        InternalHostStartupData config = configBuilder.GetOrCreateConfig<InternalHostStartupData>();

        config.InitialRenderWidth = width;
        config.InitialRenderHeight = height;

        return configBuilder;
    }
}
#endif

public class InternalHostStartupData : IConfigurator
{
#if SFML
    public SFML.Graphics.RenderWindow? TargetWindow { get; set; }
#elif WPF
    public int InitialRenderWidth { get; set; }
    public int InitialRenderHeight { get; set; }
#endif

    public void Run(Builder config, GameHost game)
    {
    }
}
