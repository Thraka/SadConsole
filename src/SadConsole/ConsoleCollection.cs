#if XNA
#endif

namespace SadConsole
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages the parent and children relationship for <see cref="Console"/>.
    /// </summary>
    public class ConsoleCollection : IEnumerable<Console>, System.Collections.IEnumerable
    {
        protected List<Console> screens;
        protected Console owningScreen;

        public int Count => screens.Count;

        /// <summary>
        /// When true, the collection cannot be modified.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a child object for this collection.
        /// </summary>
        /// <param name="index">The index of the child object.</param>
        /// <returns></returns>
        public Console this[int index]
        {
            get => screens[index];
            set
            {
                if (IsLocked)
                {
                    throw new Exception("The collection is locked and cannot be modified.");
                }

                if (screens[index] == value)
                {
                    return;
                }

                Console oldConsole = screens[index];
                screens[index] = value;
                RemoveScreenParent(oldConsole);
                SetScreenParent(value);
            }
        }

        /// <summary>
        /// Creates a new screen object collection and parents it to the <paramref name="owner"/> object.
        /// </summary>
        /// <param name="owner">The owning object of this collection.</param>
        public ConsoleCollection(Console owner)
        {
            screens = new List<Console>();
            owningScreen = owner;
        }

        /// <summary>
        /// Removes all consoles.
        /// </summary>
        public void Clear()
        {
            if (IsLocked)
            {
                throw new Exception("The collection is locked and cannot be modified.");
            }

            screens.Clear();
        }

        /// <summary>
        /// Returns true if this console list contains the specified <paramref name="screen"/>.
        /// </summary>
        /// <param name="screen">The console to search for.</param>
        /// <returns></returns>
        public bool Contains(Console screen) => screens.Contains(screen);

        /// <summary>
        /// When true, indicates that the <paramref name="screen"/> is at the top of the collection stack.
        /// </summary>
        /// <param name="screen">The screen object to check.</param>
        /// <returns>True when the object is on top.</returns>
        public bool IsTop(Console screen)
        {
            if (screens.Contains(screen))
            {
                return screens.IndexOf(screen) == screens.Count - 1;
            }

            return false;
        }

        /// <summary>
        /// Adds a new child object to this collection.
        /// </summary>
        /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
        /// <param name="screen">The child object.</param>
        public void Add(Console screen)
        {
            if (IsLocked)
            {
                throw new Exception("The collection is locked and cannot be modified.");
            }

            if (!screens.Contains(screen))
            {
                screens.Add(screen);
            }

            SetScreenParent(screen);
        }

        /// <summary>
        /// Inserts a child object at the specified <paramref name="index"/>.
        /// </summary>
        /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
        /// <param name="index">The 0-based index to insert the object at.</param>
        /// <param name="screen">The child object.</param>
        public void Insert(int index, Console screen)
        {
            if (IsLocked)
            {
                throw new Exception("The collection is locked and cannot be modified.");
            }

            if (!screens.Contains(screen))
            {
                screens.Insert(index, screen);
            }

            SetScreenParent(screen);
        }

        /// <summary>
        /// Removes a new child object from this collection.
        /// </summary>
        /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
        /// <param name="screen">The child object.</param>
        public void Remove(Console screen)
        {
            if (IsLocked)
            {
                throw new Exception("The collection is locked and cannot be modified.");
            }

            if (screens.Contains(screen))
            {
                screens.Remove(screen);
            }

            RemoveScreenParent(screen);
        }

        /// <summary>
        /// Moves the specified <paramref name="screen"/>  to the top of the collection.
        /// </summary>
        /// <param name="screen">The child object.</param>
        public void MoveToTop(Console screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Add(screen);
            }
        }

        /// <summary>
        /// Moves the specified <paramref name="screen"/>  to the bottom of the collection.
        /// </summary>
        /// <param name="screen">The child object.</param>
        public void MoveToBottom(Console screen)
        {
            if (screens.Contains(screen))
            {
                screens.Remove(screen);
                screens.Insert(0, screen);
            }
        }

        /// <summary>
        /// Gets the 0-based index of the <paramref name="screen"/>.
        /// </summary>
        /// <param name="screen">The child object.</param>
        public int IndexOf(Console screen) => screens.IndexOf(screen);

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

        private void SetScreenParent(Console screen) => screen.Parent = owningScreen;

        private void RemoveScreenParent(Console screen) => screen.Parent = null;

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => screens.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<Console> GetEnumerator() => screens.GetEnumerator();
    }
}
