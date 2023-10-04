#nullable enable

namespace SadConsole.Configuration;

public static partial class Extensions
{
    /// <summary>
    /// Sets the 
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="value">Indicates whether or not <see cref="GameHost.Screen"/> is focused.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder IsStartingScreenFocused(this Builder configBuilder, bool value)
    {
        InternalStartupData data = configBuilder.GetOrCreateConfig<InternalStartupData>();

        data.FocusStartingScreen = value;

        return configBuilder;
    }
}
