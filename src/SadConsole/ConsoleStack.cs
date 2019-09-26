#if XNA
#endif

namespace SadConsole
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A stack of consoles. The top-most of the stack is considered active and represented by the <see cref="Console"/> property.
    /// </summary>
    public class ConsoleStack
    {
        private Console activeConsole;

        /// <summary>
        /// Gets the current active console.
        /// </summary>
        public Console Console => activeConsole;

        /// <summary>
        /// The stack of consoles for input processing.
        /// </summary>
        private readonly List<Console> consoles;

        internal ConsoleStack()
        {
            consoles = new List<Console>();
            activeConsole = null;
        }

        /// <summary>
        /// Clears all consoles from the active stack along with the current active console.
        /// </summary>
        public void Clear()
        {
            consoles.Clear();

            if (activeConsole != null)
            {
                activeConsole.OnFocusLost();
            }

            activeConsole = null;
        }

        /// <summary>
        /// Adds another console to active stack, setting it as the active (top most in the stack) console.
        /// </summary>
        /// <param name="console"></param>
        public void Push(Console console)
        {
            if (console != activeConsole && console != null)
            {
                if (consoles.Contains(console))
                {
                    consoles.Remove(console);
                }

                if (activeConsole != null)
                {
                    activeConsole.OnFocusLost();
                }

                consoles.Add(console);
                activeConsole = console;
                activeConsole.OnFocused();
            }
        }

        /// <summary>
        /// Replaces the top console (active console) with the provided instance. Sets <see cref="Console"/> to this instance.
        /// </summary>
        /// <param name="console">The console to make active.</param>
        public void Set(Console console)
        {
            if (activeConsole == console)
            {
                return;
            }

            if (consoles.Count != 0)
            {
                consoles.Remove(consoles.Last());
            }

            Push(console);
        }

        /// <summary>
        /// Removes the console from the active stack. If the instance is the current active console, the active console is set to the last console in the previous console.
        /// </summary>
        /// <param name="console">The console to remove.</param>
        public void Pop(Console console)
        {
            if (console == activeConsole)
            {
                activeConsole.OnFocusLost();
                consoles.Remove(console);

                if (consoles.Count != 0)
                {
                    activeConsole = consoles.Last();
                    activeConsole.OnFocused();
                }
                else
                {
                    activeConsole = null;
                }
            }
            else
            {
                consoles.Remove(console);
            }
        }

        /// <summary>
        /// Removes the top console from the stack.
        /// </summary>
        public void Pop()
        {
            if (consoles.Count != 0)
            {
                Pop(consoles.Last());
            }
        }

        public static bool operator !=(ConsoleStack left, Console right) => left.activeConsole != right;

        public static bool operator ==(ConsoleStack left, Console right) => left.activeConsole == right;
    }
}
