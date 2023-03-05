using System;
using System.Collections.Generic;

namespace SadConsole;

/// <summary>
/// Manages the parent and children relationship for <see cref="IScreenObject"/>.
/// </summary>
public class ScreenObjectCollection : ScreenObjectCollection<IScreenObject>
{

    /// <summary>
    /// Creates a new object collection and parents it to the <paramref name="owner"/> object.
    /// </summary>
    /// <param name="owner">The owning object of this collection.</param>
    public ScreenObjectCollection(IScreenObject owner) : base(owner) { }
}

/// <summary>
/// Manages the parent and children relationship for <see cref="IScreenObject"/>.
/// </summary>
public class ScreenObjectCollection<TScreenObject> : IReadOnlyList<TScreenObject>, IEnumerable<TScreenObject>, System.Collections.IEnumerable
    where TScreenObject : class, IScreenObject
{
    /// <summary>
    /// Raised when the items in this collection are added, removed, or repositioned.
    /// </summary>
    public event EventHandler? CollectionChanged;

    /// <summary>
    /// Internal list of objects.
    /// </summary>
    protected List<TScreenObject> objects;

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
    public TScreenObject this[int index]
    {
        get => objects[index];
        set
        {
            if (IsLocked)
                throw new Exception("The collection is locked and cannot be modified.");

            if (objects[index] == value)
                return;

            TScreenObject oldObject = objects[index];
            objects[index] = value;
            RemoveObjParent(oldObject);
            SetObjParent(value);
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Creates a new object collection and parents it to the <paramref name="owner"/> object.
    /// </summary>
    /// <param name="owner">The owning object of this collection.</param>
    public ScreenObjectCollection(IScreenObject owner)
    {
        objects = new List<TScreenObject>();
        owningObject = owner;
    }

    /// <summary>
    /// Removes all consoles.
    /// </summary>
    public void Clear()
    {
        if (IsLocked)
            throw new Exception("The collection is locked and cannot be modified.");

        objects.Clear();
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Returns true if this console list contains the specified <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The console to search for.</param>
    /// <returns></returns>
    public bool Contains(TScreenObject obj) =>
        objects.Contains(obj);

    /// <summary>
    /// When true, indicates that the <paramref name="obj"/> is at the top of the collection stack.
    /// </summary>
    /// <param name="obj">The obj object to check.</param>
    /// <returns>True when the object is on top.</returns>
    public bool IsTop(TScreenObject obj)
    {
        if (objects.Contains(obj))
            return objects.IndexOf(obj) == objects.Count - 1;

        return false;
    }

    /// <summary>
    /// Adds a new child object to this collection.
    /// </summary>
    /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
    /// <param name="obj">The child object.</param>
    public void Add(TScreenObject obj)
    {
        if (IsLocked)
            throw new Exception("The collection is locked and cannot be modified.");

        if (!objects.Contains(obj))
            objects.Add(obj);

        SetObjParent(obj);
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Inserts a child object at the specified <paramref name="index"/>.
    /// </summary>
    /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
    /// <param name="index">The 0-based index to insert the object at.</param>
    /// <param name="obj">The child object.</param>
    public void Insert(int index, TScreenObject obj)
    {
        if (IsLocked)
            throw new Exception("The collection is locked and cannot be modified.");

        if (!objects.Contains(obj))
            objects.Insert(index, obj);

        SetObjParent(obj);
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Removes a new child object from this collection.
    /// </summary>
    /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
    /// <param name="obj">The child object.</param>
    public void Remove(TScreenObject obj)
    {
        if (IsLocked)
            throw new Exception("The collection is locked and cannot be modified.");

        if (objects.Contains(obj))
            objects.Remove(obj);

        RemoveObjParent(obj);
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Sorts the collection based on <see cref="IScreenObject.SortOrder"/>.
    /// </summary>
    /// <param name="comparer">The comparer to use</param>
    /// <exception cref="Exception">Thrown when the <see cref="IsLocked"/> property is set to true.</exception>
    public void Sort(IComparer<IScreenObject> comparer)
    {
        if (IsLocked)
            throw new Exception("The collection is locked and cannot be modified.");

        objects.Sort(comparer);
    }

    /// <summary>
    /// Moves the specified <paramref name="obj"/>  to the top of the collection.
    /// </summary>
    /// <param name="obj">The child object.</param>
    public void MoveToTop(TScreenObject obj)
    {
        if (objects.Contains(obj))
        {
            objects.Remove(obj);
            objects.Add(obj);
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Moves the specified <paramref name="obj"/>  to the bottom of the collection.
    /// </summary>
    /// <param name="obj">The child object.</param>
    public void MoveToBottom(TScreenObject obj)
    {
        if (objects.Contains(obj))
        {
            objects.Remove(obj);
            objects.Insert(0, obj);
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets the 0-based index of the <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The child object.</param>
    public int IndexOf(TScreenObject obj) =>
        objects.IndexOf(obj);

    private void SetObjParent(TScreenObject obj) =>
        obj.Parent = owningObject;

    private void RemoveObjParent(TScreenObject obj) =>
        obj.Parent = null;

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        objects.GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<TScreenObject> GetEnumerator() =>
        objects.GetEnumerator();
}
