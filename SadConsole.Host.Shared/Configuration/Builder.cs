#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace SadConsole.Configuration;

/// <summary>
/// Composes a game host object.
/// </summary>
public sealed class Builder
{
    /// <summary>
    /// A collection of <see cref="IConfigurator"/> objects.
    /// </summary>
    public List<IConfigurator> Configs { get; } = new List<IConfigurator>();

    /// <summary>
    /// Adds or gets the specified config object from the <see cref="Configs"/> collection.
    /// </summary>
    /// <typeparam name="TConfig">The type of config object.</typeparam>
    /// <returns>A new instance of <typeparamref name="TConfig"/> if it's not found in the <see cref="Configs"/> collection. If found in the collection, that instance is returned.</returns>
    public TConfig GetOrCreateConfig<TConfig>() where TConfig : IConfigurator, new()
    {
        TConfig? config = Configs.OfType<TConfig>().FirstOrDefault();

        if (config == null)
        {
            config = new TConfig();
            Configs.Add(config);
        }

        return config;
    }

    /// <summary>
    /// Runs each config object in the <see cref="Configs"/> collection with the specified game instance.
    /// </summary>
    /// <param name="game">The game being created.</param>
    public void Run(Game game) =>
        Configs.ForEach(config => config.Run(this, game));

    /// <summary>
    /// Creates the <see cref="Game"/> host object with this configuration object.
    /// </summary>
    public void Run() =>
        Game.Create(this);
}
