﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// Wraps a collection of objects for ImGui controls, like listboxes.
/// </summary>
/// <typeparam name="T">The type of object wrapped.</typeparam>
public class ImGuiList<T> where T : class, ITitle
{
    private string[] _localNames;

    /// <summary>
    /// Each item's title.
    /// </summary>
    /// <remarks>Refreshed when this property is accessed.</remarks>
    public string[] Names
    {
        get
        {
            // Refresh the array with latest titles
            for (int index = 0; index < Objects.Count; index++)
                _localNames[index] = Objects[index].Title;

            return _localNames;
        }
    }

    /// <summary>
    /// The objects wrapped by this list.
    /// </summary>
    public ObservableCollection<T> Objects { get; }

    /// <summary>
    /// The number of items this list is wrapping.
    /// </summary>
    public int Count => Objects.Count;

    /// <summary>
    /// The selected index. Controlled by ImGui objects.
    /// </summary>
    public int SelectedItemIndex = -1;

    /// <summary>
    /// The selected item or <see langword="null"/>.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Thrown when the property is set to a value that doesn't exist in the collection.</exception>
    public T? SelectedItem
    {
        get => SelectedItemIndex == -1 ? null : Objects[SelectedItemIndex];
        set
        {
            if (value == null)
                SelectedItemIndex = -1;

            else if (Objects.Contains(value))
                SelectedItemIndex = Objects.IndexOf(value);

            else
                throw new System.InvalidOperationException("List doesn't contain the item.");
        }
    }

    /// <summary>
    /// Creates a new list, wrapping the provided items.
    /// </summary>
    /// <param name="items">The items to wrap.</param>
    public ImGuiList(params T[] items)
    {
        Objects = [..items];
        Objects.CollectionChanged += Objects_CollectionChanged;
        Objects_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Indicates that an item is selected.
    /// </summary>
    /// <returns>Checks if <see cref="SelectedItemIndex"/> doesn't equal -1.</returns>
    [MemberNotNullWhen(true, nameof(SelectedItem))]
    public bool IsItemSelected() =>
        SelectedItemIndex != -1;

    [MemberNotNull(nameof(_localNames))]
    private void Objects_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        _localNames = Objects.Count == 0 ? [] : new string[Objects.Count];
}
