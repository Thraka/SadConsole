#nullable enable

namespace SadConsole.Configuration;

/// <summary>
/// Configuration interface that's added to the <see cref="Builder"/> object.
/// </summary>
public interface IConfigurator
{
    /// <summary>
    /// Called by the <see cref="Builder"/>; Runs the specific config for this object.
    /// </summary>
    /// <param name="config">The builder configuration object.</param>
    /// <param name="game">The game object being created.</param>
    void Run(Builder config, GameHost game);
}
