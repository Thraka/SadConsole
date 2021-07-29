using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SadConsole
{
    public static class ExtensionsSystem
    {
        /// <summary>
        /// Searches the object for a <see cref="System.Diagnostics.DebuggerDisplayAttribute"/> and returns its value. If not found, returns <see cref="object.ToString"/>.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>A string respresenting the object.</returns>
        public static string GetDebuggerDisplayValue(this object obj)
        {
            var debugger = obj.GetType()
                              .GetCustomAttributes(typeof(System.Diagnostics.DebuggerDisplayAttribute), true)
                              .FirstOrDefault()
                              as System.Diagnostics.DebuggerDisplayAttribute;

            return debugger?.Value ?? obj.ToString();
        }
    }
}
