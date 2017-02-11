using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using SadConsole.Renderers;
using SadConsole.Input;
using System;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace SadConsole
{
    public partial class Console : SurfaceEditor, IConsole
    {
        /// <summary>
        /// Represents the active console handing keyboard input and possibly mouse input.
        /// </summary>
        public static ActiveConsoleStack ActiveConsoles = new ActiveConsoleStack();

        /// <summary>
        /// Keeps track of the active consoles used for input by SadConsole.
        /// </summary>
        public class ActiveConsoleStack
        {
            private IConsole activeConsole;

            /// <summary>
            /// Gets the current active console.
            /// </summary>
            public IConsole Console
            {
                get
                {
                    return activeConsole;
                }
            }

            /// <summary>
            /// The stack of consoles for input processing.
            /// </summary>
            List<IConsole> consoles;

            internal ActiveConsoleStack()
            {
                consoles = new List<IConsole>();
                activeConsole = null;
            }

            /// <summary>
            /// Clears all consoles from the active stack along with the current active console.
            /// </summary>
            public void Clear()
            {
                consoles.Clear();
                activeConsole = null;
            }

            /// <summary>
            /// Adds another console to active stack, setting it as the active (top most in the stack) console.
            /// </summary>
            /// <param name="console"></param>
            public void Push(IConsole console)
            {
                if (console != activeConsole && console != null)
                {
                    activeConsole = console;
                    consoles.Add(console);
                }
            }

            /// <summary>
            /// Replaces the top console (active console) with the provided instance. Sets <see cref="Console"/> to this instance.
            /// </summary>
            /// <param name="console">The console to make active.</param>
            public void Set(IConsole console)
            {
                if (activeConsole == console)
                    return;

                if (consoles.Count != 0)
                    consoles.Remove(consoles.Last());

                Push(console);
            }
            
            /// <summary>
            /// Removes the console from the active stack. If the instance is the current active console, the active console is set to the last console in the previous console.
            /// </summary>
            /// <param name="console">The console to remove.</param>
            public void Pop(IConsole console)
            {
                if (console == activeConsole)
                {
                    consoles.Remove(console);

                    if (consoles.Count != 0)
                        activeConsole = consoles.Last();
                    else
                        activeConsole = null;
                }
                else
                    consoles.Remove(console);
            }

            public static bool operator !=(ActiveConsoleStack left, IConsole right)
            {
                return left.activeConsole != right;
            }

            public static bool operator ==(ActiveConsoleStack left, IConsole right)
            {
                return left.activeConsole == right;
            }
        }
    }
}