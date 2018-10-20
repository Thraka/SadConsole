using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole
{
    /// <summary>
    /// An object created by a factory.
    /// </summary>
    public interface IFactoryObject
    {
        /// <summary>
        /// The identifier of the <see cref="IFactoryBlueprint{TProduced}"/> that created this object.
        /// </summary>
        string DefinitionId { get; }
    }
}
