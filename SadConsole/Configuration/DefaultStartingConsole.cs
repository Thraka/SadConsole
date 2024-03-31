#nullable enable

namespace SadConsole.Configuration;

public static partial class Extensions
{
    /// <summary>
    /// Sets the <see cref="GameHost.StartingConsole"/> and <see cref="GameHost.Screen"/> properties to new console when the game starts.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder UseDefaultConsole(this Builder configBuilder)
    {
        configBuilder.GetOrCreateConfig<StartingConsoleConfig>();

        return configBuilder;
    }
}

public class StartingConsoleConfig : IConfigurator
{
    public void Run(Builder configBuilder, GameHost game)
    {
        InternalStartupData data = configBuilder.GetOrCreateConfig<InternalStartupData>();

        game.StartingConsole = new Console(data.ScreenCellsX, data.ScreenCellsY);
        game.Screen = game.StartingConsole;
        game.Screen!.IsFocused = true;
    }
}
