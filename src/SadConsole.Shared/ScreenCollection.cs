using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// Manages the parent and children relationship for <see cref="IScreen"/>.
    /// </summary>
    public class ScreenCollection: IEnumerable<IScreen>, System.Collections.IEnumerable
    {
        protected List<IScreen> screens;
        protected WeakReference<IScreen> owningScreen;

        public int Count { get { return screens.Count; } }

        public IScreen this[int index]
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

        public ScreenCollection(IScreen owner)
        {
            screens = new List<IScreen>();
            owningScreen = new WeakReference<IScreen>(owner);
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
        /// <param name="console">The console to search for.</param>
        /// <returns></returns>
        public bool Contains(IScreen screen)
        {
            return screens.Contains(screen);
        }

        public bool IsTop(IScreen screen)
        {
            if (screens.Contains(screen))
                return screens.IndexOf(screen) == screens.Count - 1;
            else
                return false;
        }
        
        public void Add(IScreen screen)
        {
            if (!screens.Contains(screen))
                screens.Add(screen);

            SetScreenParent(screen);
        }

        public void Insert(int index, IScreen screen)
        {
            if (!screens.Contains(screen))
                screens.Insert(index, screen);

            SetScreenParent(screen);
        }

        public void Remove(IScreen screen)
        {
            if (screens.Contains(screen))
                screens.Remove(screen);

            RemoveConsolesParent(screen);
        }

        public void MoveToTop(IScreen screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Add(screen);
            }
        }

        public void MoveToBottom(IScreen screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Insert(0, screen);
            }
        }

        public int IndexOf(IScreen screen)
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

        private bool SetScreenParent(IScreen screen)
        {
            if (screen.Parent != this)
            {
                IScreen owner;
                if (owningScreen.TryGetTarget(out owner))
                    screen.Parent = owner;

                return true;
            }

            return false;
        }

        private bool RemoveConsolesParent(IScreen screen)
        {
            if (screen.Parent == this)
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

        public IEnumerator<IScreen> GetEnumerator()
        {
            return screens.GetEnumerator();
        }

    }
}