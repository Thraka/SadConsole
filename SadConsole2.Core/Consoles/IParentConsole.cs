
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Represents a group of consoles.
    /// </summary>
    public interface IConsoleList : IEnumerable<IConsole>, IDraw, Input.IInput
    {
        /// <summary>
        /// Gets the count of child consoles.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets a console by index.
        /// </summary>
        /// <param name="index">The index of the console.</param>
        /// <returns>The console.</returns>
        IConsole this[int index] { get; set; }

        /// <summary>
        /// Adds a console.
        /// </summary>
        /// <param name="console">The console.</param>
        void Add(IConsole console);

        /// <summary>
        /// Inserts a console at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the console.</param>
        /// <param name="console">The console to insert.</param>
        void Insert(int index, IConsole console);

        /// <summary>
        /// Removes the specified console.
        /// </summary>
        /// <param name="console">The console.</param>
        void Remove(IConsole console);

        /// <summary>
        /// Gets the index of the spcified console.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <returns></returns>
        int IndexOf(IConsole console);

        /// <summary>
        /// Moves a console to the top of the list of child consoles. Ensures it is rendered on top of all other consoles.
        /// </summary>
        /// <param name="console">The console.</param>
        void MoveToTop(IConsole console);

        /// <summary>
        /// Moves a console to the bottom of the list of child consoels. Ensures it is rendered behind all other consoles.
        /// </summary>
        /// <param name="console"></param>
        void MoveToBottom(IConsole console);

        /// <summary>
        /// Gets the next visible console positioned after the specified console.
        /// </summary>
        /// <param name="currentConsole">The reference console.</param>
        /// <returns>The next visible console.</returns>
        IConsole NextValidConsole(IConsole currentConsole);

        /// <summary>
        /// Gets the previous visible console positioned before the specified console.
        /// </summary>
        /// <param name="currentConsole">The reference console.</param>
        /// <returns>The previous visible console.</returns>
        IConsole PreviousValidConsole(IConsole currentConsole);
    }
}
