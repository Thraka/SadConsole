namespace SadConsole.Configuration;

/// <summary>
/// Provides extension methods for configuring and running the SadConsole game host.
/// </summary>
public static partial class ExtensionsHost
{
    /// <summary>
    /// Runs the game using the builder configuration.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    public static void Run(this Builder configBuilder)
    {
        Game.Create(configBuilder);
        Game.Instance.Run();
        Game.Instance.Dispose();
    }
}
