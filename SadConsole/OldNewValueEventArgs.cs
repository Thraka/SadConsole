using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// Provides reference to the old and new objects.
    /// </summary>
    public class NewOldValueEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The previous object.
        /// </summary>
        public readonly T OldObject;

        /// <summary>
        /// The new object.
        /// </summary>
        public readonly T NewObject;

        /// <summary>
        /// Creates a new instance of this object with the specified old and new parent.
        /// </summary>
        /// <param name="oldObject">The old parent.</param>
        /// <param name="newObject">The new parent.</param>
        public NewOldValueEventArgs(T oldObject, T newObject) =>
            (OldObject, NewObject) = (oldObject, newObject);
    }
}
