using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public static class IScreenListExtension
    {
        public static void MoveToTop(this List<IScreen> screenParent, IScreen screen)
        {
            if (screenParent.Contains(screen))
            {
                screenParent.Remove(screen);
                screenParent.Add(screen);
            }
        }

        public static void MoveToBottom(this List<IScreen> _consoles, IScreen console)
        {
            if (_consoles.Contains(console))
            {
                _consoles.Remove(console);
                _consoles.Insert(0, console);
            }
        }

        public static IScreen NextVisibleScreen(this List<IScreen> _consoles, IScreen currentConsole)
        {
            if (_consoles.Contains(currentConsole))
            {
                var index = _consoles.IndexOf(currentConsole);
                var counter = 0;
                do
                {
                    index++;
                    counter++;

                    if (index == _consoles.Count)
                        index = 0;

                    if (_consoles[index].IsVisible)
                    {
                        return _consoles[index];
                    }

                } while (counter < _consoles.Count);

            }

            return null;
        }


        public static IScreen PreviousVisibleScreen(this List<IScreen> _consoles, IScreen currentConsole)
        {
            if (_consoles.Contains(currentConsole))
            {
                var index = _consoles.IndexOf(currentConsole);
                var counter = 0;
                do
                {
                    index--;
                    counter++;

                    if (index == -1)
                        index = _consoles.Count - 1;

                        if (_consoles[index].IsVisible)
                        {
                            return _consoles[index];
                        }

                } while (counter < _consoles.Count);

            }

            return null;
        }

        public static bool SetParent(this IScreen console, IScreen parent)
        {
            if (console.Parent != parent)
            {
                console.Parent = parent;
                return true;
            }

            return false;
        }

        public static void RemoveParent(this IScreen console)
        {
            console.Parent = null;
        }
    }
}
