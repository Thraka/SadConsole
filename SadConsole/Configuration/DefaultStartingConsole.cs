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

/// <summary>
/// Configures the default starting console for the game.
/// </summary>
public class StartingConsoleConfig : IConfigurator
{
    /// <summary>
    /// Creates and configures the default starting console.
    /// </summary>
    /// <param name="configBuilder">The builder configuration.</param>
    /// <param name="game">The game host instance.</param>
    public void Run(BuilderBase configBuilder, GameHost game)
    {
        game.StartingConsole = new Console(game.ScreenCellsX, game.ScreenCellsY);
        game.Screen = game.StartingConsole;
        game.Screen.IsFocused = true;
    }
}
