using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole
{
    /// <summary>
    /// Defines how to create a <typeparamref name="TProduced"/> object.
    /// </summary>
    /// <typeparam name="TProduced">The object to create.</typeparam>
    public interface IFactoryBlueprint<out TProduced>
    {
        /// <summary>
        /// A unique identifier of this factory definition.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Creates a <typeparamref name="TProduced"/> object.
        /// </summary>
        /// <returns>The created object.</returns>
        TProduced Create();
    }
}
