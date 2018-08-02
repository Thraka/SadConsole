using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// Manages the parent and children relationship for <see cref="ScreenObject"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("IScreen Collection")]
    public class ScreenObjectCollection : IEnumerable<ScreenObject>, System.Collections.IEnumerable
    {
        protected List<ScreenObject> screens;
        protected WeakReference<ScreenObject> owningScreen;

        public int Count { get { return screens.Count; } }

        public ScreenObject this[int index]
        {
            get { return screens[index]; }
            set
            {
                if (screens[index] != value)
                {
                    var oldConsole = screens[index];
                    screens[index] = value;
                    RemoveConsolesParent(oldConsole);
                    SetScreenParent(value);
                }
            }
        }

        public ScreenObjectCollection(ScreenObject owner)
        {
            screens = new List<ScreenObject>();
            owningScreen = new WeakReference<ScreenObject>(owner);
        }

        /// <summary>
        /// Removes all consoles.
        /// </summary>
        public void Clear()
        {
            screens.Clear();
        }

        /// <summary>
        /// Returns true if this console list contains the specified console.
        /// </summary>
        /// <param name="screen">The console to search for.</param>
        /// <returns></returns>
        public bool Contains(ScreenObject screen)
        {
            return screens.Contains(screen);
        }

        public bool IsTop(ScreenObject screen)
        {
            if (screens.Contains(screen))
                return screens.IndexOf(screen) == screens.Count - 1;
            else
                return false;
        }

        public void Add(ScreenObject screen)
        {
            if (!screens.Contains(screen))
                screens.Add(screen);

            SetScreenParent(screen);
        }

        public void Insert(int index, ScreenObject screen)
        {
            if (!screens.Contains(screen))
                screens.Insert(index, screen);

            SetScreenParent(screen);
        }

        public void Remove(ScreenObject screen)
        {
            if (screens.Contains(screen))
                screens.Remove(screen);

            RemoveConsolesParent(screen);
        }

        public void MoveToTop(ScreenObject screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Add(screen);
            }
        }

        public void MoveToBottom(ScreenObject screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Insert(0, screen);
            }
        }

        public int IndexOf(ScreenObject screen)
        {
            return screens.IndexOf(screen);
        }

        //public IConsole NextValidConsole(IConsole currentConsole)
        //{
        //    if (screens.Contains(currentConsole))
        //    {
        //        var index = screens.IndexOf(currentConsole);
        //        var counter = 0;
        //        do
        //        {
        //            index++;
        //            counter++;

        //            if (index == screens.Count)
        //                index = 0;

        //            if (screens[index].IsVisible)
        //            {
        //                return screens[index];
        //            }
        //        } while (counter < screens.Count);

        //    }

        //    return null;
        //}

        //public IConsole PreviousValidConsole(IConsole currentConsole)
        //{
        //    if (screens.Contains(currentConsole))
        //    {
        //        var index = screens.IndexOf(currentConsole);
        //        var counter = 0;
        //        do
        //        {
        //            index--;
        //            counter++;

        //            if (index == -1)
        //                index = screens.Count - 1;

        //            if (screens[index].IsVisible)
        //            {
        //                return screens[index];
        //            }
        //        } while (counter < screens.Count);

        //    }

        //    return null;
        //}

        private bool SetScreenParent(ScreenObject screen)
        {
            if (owningScreen.TryGetTarget(out ScreenObject owner) && screen != owner)
            {
                screen.Parent = owner;
                return true;
            }

            return false;
        }

        private bool RemoveConsolesParent(ScreenObject screen)
        {
            if (owningScreen.TryGetTarget(out ScreenObject owner) && screen == owner)
            {
                screen.Parent = null;
                return true;
            }

            return false;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return screens.GetEnumerator();
        }

        public IEnumerator<ScreenObject> GetEnumerator()
        {
            return screens.GetEnumerator();
        }

    }
}