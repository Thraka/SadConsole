using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// Manages the parent and children relationship for <see cref="IScreenObject"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("IScreen Collection")]
    public class ScreenObjectCollection: IEnumerable<IScreenObject>, System.Collections.IEnumerable
    {
        protected List<IScreenObject> screens;
        protected WeakReference<IScreenObject> owningScreen;

        public int Count { get { return screens.Count; } }

        public IScreenObject this[int index]
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

        public ScreenObjectCollection(IScreenObject owner)
        {
            screens = new List<IScreenObject>();
            owningScreen = new WeakReference<IScreenObject>(owner);
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
        public bool Contains(IScreenObject screen)
        {
            return screens.Contains(screen);
        }

        public bool IsTop(IScreenObject screen)
        {
            if (screens.Contains(screen))
                return screens.IndexOf(screen) == screens.Count - 1;
            else
                return false;
        }
        
        public void Add(IScreenObject screen)
        {
            if (!screens.Contains(screen))
                screens.Add(screen);

            SetScreenParent(screen);
        }

        public void Insert(int index, IScreenObject screen)
        {
            if (!screens.Contains(screen))
                screens.Insert(index, screen);

            SetScreenParent(screen);
        }

        public void Remove(IScreenObject screen)
        {
            if (screens.Contains(screen))
                screens.Remove(screen);

            RemoveConsolesParent(screen);
        }

        public void MoveToTop(IScreenObject screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Add(screen);
            }
        }

        public void MoveToBottom(IScreenObject screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Insert(0, screen);
            }
        }

        public int IndexOf(IScreenObject screen)
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

        private bool SetScreenParent(IScreenObject screen)
        {
            if (owningScreen.TryGetTarget(out IScreenObject owner) && screen != owner )
            {
                screen.Parent = owner;
                return true;
            }

            return false;
        }

        private bool RemoveConsolesParent(IScreenObject screen)
        {
            if (owningScreen.TryGetTarget(out IScreenObject owner) && screen == owner )
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

        public IEnumerator<IScreenObject> GetEnumerator()
        {
            return screens.GetEnumerator();
        }

    }
}