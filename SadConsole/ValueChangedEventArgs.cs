using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// The old value and the value it changed to.
    /// </summary>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The previous object.
        /// </summary>
        public readonly T OldValue;

        /// <summary>
        /// The new object.
        /// </summary>
        public readonly T NewValue;

        /// <summary>
        /// Creates a new instance of this object with the specified old and new value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ValueChangedEventArgs(T oldValue, T newValue) =>
            (OldValue, NewValue) = (oldValue, newValue);
    }
}
