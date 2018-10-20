using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole
{
    /// <summary>
    /// A factory that produces an object based on a definition.
    /// </summary>
    /// <typeparam name="TBlueprint">A settings object that will create an object defined by <typeparamref name="TProduced"/>.</typeparam>
    /// <typeparam name="TProduced">An object this factory creates.</typeparam>
    public abstract class Factory<TBlueprint, TProduced> : IEnumerable<TBlueprint>
        where TBlueprint : IFactoryBlueprint<TProduced>
        where TProduced : IFactoryObject
    {
        private readonly Dictionary<string, TBlueprint> _definitions = new Dictionary<string, TBlueprint>();

        /// <summary>
        /// Creates a <typeparamref name="TProduced"/> object based on the definition factory id.
        /// </summary>
        /// <param name="name">The factory id of a definition.</param>
        /// <returns>A new object.</returns>
        public TProduced Create(string name)
        {
            if (!_definitions.ContainsKey(name))
                throw new ItemNotDefinedException(name);

            return _definitions[name].Create();
        }

        /// <summary>
        /// Adds a definition to the factory.
        /// </summary>
        /// <param name="item">The definition.</param>
        public void Add(TBlueprint item)
        {
            if (_definitions.ContainsKey(item.Id))
                _definitions.Remove(item.Id);

            _definitions.Add(item.Id, item);
        }

        /// <summary>
        /// Checks if a definition exists.
        /// </summary>
        /// <param name="factoryId">The definition to check for.</param>
        /// <returns>Returns true when the specified <paramref name="factoryId"/> exists; otherwise false.</returns>
        public bool DefinitionExists(string factoryId)
        {
            return _definitions.ContainsKey(factoryId);
        }

        /// <summary>
        /// Gets a definition by identifier.
        /// </summary>
        /// <param name="factoryId">The definition identifier to get.</param>
        /// <returns>The definition of the object.</returns>
        /// <exception cref="ItemNotDefinedException">Thrown if the factory identifier does not exist.</exception>
        public TBlueprint GetDefintion(string factoryId)
        {
            if (!_definitions.ContainsKey(factoryId))
                throw new ItemNotDefinedException(factoryId);

            return _definitions[factoryId];
        }

        /// <summary>
        /// Gets an enumerator of all of the definitions in the factory.
        /// </summary>
        /// <returns>An enumeration of the definitions.</returns>
        public IEnumerator<TBlueprint> GetEnumerator()
        {
            return _definitions.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator of all of the definitions in the factory.
        /// </summary>
        /// <returns>An enumeration of the definitions.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _definitions.Values.GetEnumerator();
        }

        [Serializable]
        public class ItemNotDefinedException : Exception
        {
            public ItemNotDefinedException(string factoryId)
                : base($"The item '{factoryId}' has not been added to this factory")
            {
            }
        }
    }
}
