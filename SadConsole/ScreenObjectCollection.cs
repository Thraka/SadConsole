using System;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// Manages the parent and children relationship for <see cref="IScreenObject"/>.
    /// </summary>
    public class ScreenObjectCollection : IEnumerable<IScreenObject>, System.Collections.IEnumerable
    {
        /// <summary>
        /// Internal list of objects.
        /// </summary>
        protected List<IScreenObject> objects;

        /// <summary>
        /// The parent object.
        /// </summary>
        protected IScreenObject owningObject;

        /// <summary>
        /// Returns the total number of objects in this collection.
        /// </summary>
        public int Count => objects.Count;

        /// <summary>
        /// When true, the collection cannot be modified.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a child object for this collection.
        /// </summary>
        /// <param name="index">The index of the child object.</param>
        /// <returns>The wanted object.</returns>
        public IScreenObject this[int index]
        {
            get => objects[index];
            set
            {
                if (IsLocked)
                {
                    throw new Exception("The collection is locked and cannot be modified.");
                }

                if (objects[index] == value)
                {
                    return;
                }

                IScreenObject oldObject = objects[index];
                objects[index] = value;
                RemoveObjParent(oldObject);
                SetObjParent(value);
            }
        }

        /// <summary>
        /// Creates a new object collection and parents it to the <paramref name="owner"/> object.
        /// </summary>
        /// <param name="owner">The owning object of this collection.</param>
        public ScreenObjectCollection(IScreenObject owner)
        {
            objects = new List<IScreenObject>();
            owningObject = owner;
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

            objects.Clear();
        }

        /// <summary>
        /// Returns true if this console list contains the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The console to search for.</param>
        /// <returns></returns>
        public bool Contains(IScreenObject obj) =>
            objects.Contains(obj);

        /// <summary>
        /// When true, indicates that the <paramref name="obj"/> is at the top of the collection stack.
        /// </summary>
        /// <param name="obj">The obj object to check.</param>
        /// <returns>True when the object is on top.</returns>
        public bool IsTop(IScreenObject obj)
        {
            if (objects.Contains(obj))
            {
                return objects.IndexOf(obj) == objects.Count - 1;
            }

            return false;
        }

        /// <summary>
        /// Adds a new child object to this collection.
        /// </summary>
        /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
        /// <param name="obj">The child object.</param>
        public void Add(IScreenObject obj)
        {
            if (IsLocked)
            {
                throw new Exception("The collection is locked and cannot be modified.");
            }

            if (!objects.Contains(obj))
            {
                objects.Add(obj);
            }

            SetObjParent(obj);
        }

        /// <summary>
        /// Inserts a child object at the specified <paramref name="index"/>.
        /// </summary>
        /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
        /// <param name="index">The 0-based index to insert the object at.</param>
        /// <param name="obj">The child object.</param>
        public void Insert(int index, IScreenObject obj)
        {
            if (IsLocked)
            {
                throw new Exception("The collection is locked and cannot be modified.");
            }

            if (!objects.Contains(obj))
            {
                objects.Insert(index, obj);
            }

            SetObjParent(obj);
        }

        /// <summary>
        /// Removes a new child object from this collection.
        /// </summary>
        /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
        /// <param name="obj">The child object.</param>
        public void Remove(IScreenObject obj)
        {
            if (IsLocked)
            {
                throw new Exception("The collection is locked and cannot be modified.");
            }

            if (objects.Contains(obj))
            {
                objects.Remove(obj);
            }

            RemoveObjParent(obj);
        }

        /// <summary>
        /// Moves the specified <paramref name="obj"/>  to the top of the collection.
        /// </summary>
        /// <param name="obj">The child object.</param>
        public void MoveToTop(IScreenObject obj)
        {
            if (objects.Contains(obj))
            {
                objects.Remove(obj);
                objects.Add(obj);
            }
        }

        /// <summary>
        /// Moves the specified <paramref name="obj"/>  to the bottom of the collection.
        /// </summary>
        /// <param name="obj">The child object.</param>
        public void MoveToBottom(IScreenObject obj)
        {
            if (objects.Contains(obj))
            {
                objects.Remove(obj);
                objects.Insert(0, obj);
            }
        }

        /// <summary>
        /// Gets the 0-based index of the <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The child object.</param>
        public int IndexOf(IScreenObject obj) =>
            objects.IndexOf(obj);

        private void SetObjParent(IScreenObject obj) =>
            obj.Parent = owningObject;

        private void RemoveObjParent(IScreenObject obj) =>
            obj.Parent = null;

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
            objects.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<IScreenObject> GetEnumerator() =>
            objects.GetEnumerator();
    }
}
