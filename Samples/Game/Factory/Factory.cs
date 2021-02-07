using System;
using System.Collections;
using System.Collections.Generic;

namespace SadConsole.Factory
{
    /// <summary>
    /// A factory that produces a type of object based on a blueprint and a set of configuration parameters.
    /// </summary>
    /// <typeparam name="TBlueprintConfig">The type of parameter passed to the <see cref="Create(string, TBlueprintConfig)"/> function each time an object is created.</typeparam>
    /// <typeparam name="TProduced">The type of object this factory creates.</typeparam>
    public class Factory<TBlueprintConfig, TProduced> : IEnumerable<IBlueprint<TBlueprintConfig, TProduced>>
        where TBlueprintConfig : BlueprintConfig
    {
        private readonly Dictionary<string, IBlueprint<TBlueprintConfig, TProduced>> _blueprints = new Dictionary<string, IBlueprint<TBlueprintConfig, TProduced>>();

        /// <summary>
        /// Adds a blueprint to the factory.
        /// </summary>
        /// <param name="blueprint">The blueprint to add.</param>
        public void Add(IBlueprint<TBlueprintConfig, TProduced> blueprint) => _blueprints[blueprint.Id] = blueprint;

        /// <summary>
        /// Creates a <typeparamref name="TProduced"/> object using the blueprint with the given factory id, and the given settings object.
        /// </summary>
        /// <param name="factoryId">The factory id of a blueprint.</param>
        /// <param name="blueprintConfig">A settings object passed to the Create function of the blueprint.</param>
        /// <returns>A new object.</returns>
        public TProduced Create(string factoryId, TBlueprintConfig blueprintConfig)
        {
            if (!_blueprints.ContainsKey(factoryId))
            {
                throw new ItemNotDefinedException(factoryId);
            }

            TProduced obj = _blueprints[factoryId].Create(blueprintConfig);
            if (obj is IFactoryObject factoryObj)
            {
                factoryObj.DefinitionId = factoryId;
            }

            return obj;
        }

        /// <summary>
        /// Checks if a blueprint exists.
        /// </summary>
        /// <param name="factoryId">The blueprint to check for.</param>
        /// <returns>Returns true when the specified <paramref name="factoryId"/> exists; otherwise false.</returns>
        public bool BlueprintExists(string factoryId) => _blueprints.ContainsKey(factoryId);

        /// <summary>
        /// Gets a blueprint by identifier.
        /// </summary>
        /// <param name="factoryId">The blueprint identifier to get.</param>
        /// <returns>The blueprint of the object.</returns>
        /// <exception cref="ItemNotDefinedException">Thrown if the factory identifier does not exist.</exception>
        public IBlueprint<TBlueprintConfig, TProduced> GetBlueprint(string factoryId)
        {
            if (!_blueprints.ContainsKey(factoryId))
            {
                throw new ItemNotDefinedException(factoryId);
            }

            return _blueprints[factoryId];
        }

        /// <summary>
        /// Gets an enumerator of all of the blueprints in the factory.
        /// </summary>
        /// <returns>An enumeration of the blueprints.</returns>
        public IEnumerator<IBlueprint<TBlueprintConfig, TProduced>> GetEnumerator() => _blueprints.Values.GetEnumerator();

        /// <summary>
        /// Gets an enumerator of all of the blueprints in the factory.
        /// </summary>
        /// <returns>An enumeration of the blueprints.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _blueprints.Values.GetEnumerator();
    }

    /// <summary>
    /// Exception thrown by <see cref="Factory{TBlueprintConfig, TProduced}"/> objects when a blueprint that doesn't exist is used.
    /// </summary>
    [Serializable]
    public class ItemNotDefinedException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="factoryId">Factory id that caused the error.</param>
        public ItemNotDefinedException(string factoryId)
            : base($"The item '{factoryId}' has not been added to this factory.")
        { }
    }

    /// <summary>
    /// A factory that produces a type of object based on a blueprint.
    /// </summary>
    /// <typeparam name="TProduced">The type of object this factory creates.</typeparam>
    public class Factory<TProduced> : Factory<BlueprintConfig, TProduced>
    {
        /// <summary>
        /// Creates a <typeparamref name="TProduced"/> object using the blueprint with the given factory id.
        /// </summary>
        /// <param name="factoryId">The factory id of a blueprint.</param>
        /// <returns>A new object.</returns>
        public TProduced Create(string factoryId) => Create(factoryId, BlueprintConfig.Empty);
    }
}
