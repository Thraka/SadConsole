namespace SadConsole.Configuration;

/// <summary>
/// Composes a game host object.
/// </summary>
public sealed class Builder: BuilderBase
{
    /// <summary>
    /// Creates and returns a new instance of the <see cref="Builder"/> class.
    /// </summary>
    /// <returns>A new <see cref="Builder"/> instance.</returns>
    public static Builder GetBuilder() =>
        new();
}
