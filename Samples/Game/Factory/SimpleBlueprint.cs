namespace SadConsole.Factory
{
    /// <summary>
    /// A simple <see cref="IBlueprint{TBlueprintConfig, TProduced}"/> that can be used when no configuration object is necessary to create the object.
    /// Implements <see cref="IBlueprint{BlueprintConfig, TProduced}"/>.
    /// </summary>
    /// <typeparam name="TProduced">The type of object to create.</typeparam>
    public abstract class SimpleBlueprint<TProduced> : IBlueprint<BlueprintConfig, TProduced>
    {
        /// <summary>
        /// A unique identifier of this factory definition.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Creates a SimpleBlueprint with the given blueprint id.
        /// </summary>
        /// <param name="id">ID for the blueprint.</param>
        public SimpleBlueprint(string id) => Id = id;

        /// <summary>
        /// Calls <see cref="Create()"/>.
        /// </summary>
        /// <param name="config">Unused.</param>
        /// <returns>The created object.</returns>
        public TProduced Create(BlueprintConfig config) => Create();

        /// <summary>
        /// Creates a <typeparamref name="TProduced"/> object.
        /// </summary>
        /// <returns>The created object.</returns>
        public abstract TProduced Create();
    }
}
