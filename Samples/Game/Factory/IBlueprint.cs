namespace SadConsole.Factory
{
    /// <summary>
    /// Defines how to create a <typeparamref name="TProduced"/> object.
    /// </summary>
    /// <typeparam name="TBlueprintConfig">The type of the parameter to pass to the <see cref="Create(TBlueprintConfig)"/> function.</typeparam>
    /// <typeparam name="TProduced">The type of object to create.</typeparam>
    public interface IBlueprint<in TBlueprintConfig, out TProduced> where TBlueprintConfig : BlueprintConfig
    {
        /// <summary>
        /// A unique identifier of this factory definition.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Creates a <typeparamref name="TProduced"/> object.
        /// </summary>
        /// <param name="config">Configuration parameters used to create the object.</param>
        /// <returns>The created object.</returns>
        TProduced Create(TBlueprintConfig config);
    }
}
