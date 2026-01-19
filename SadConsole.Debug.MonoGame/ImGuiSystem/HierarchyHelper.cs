using System;
using System.Collections.Generic;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// Provides helper methods for working with hierarchical items.
/// </summary>
public static class HierarchyHelper
{
    /// <summary>
    /// Flattens a hierarchical collection into a single enumerable, respecting expand/collapse state.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="roots">The root-level items to flatten.</param>
    /// <returns>An enumerable of all visible items in tree order.</returns>
    public static IEnumerable<T> FlattenVisible<T>(IEnumerable<T> roots) where T : class, IHierarchicalItem<T>
    {
        foreach (T root in roots)
        {
            yield return root;

            if (root.CanHaveChildren && root.ShowChildrenInHierarchy && root.IsExpanded)
            {
                foreach (T child in FlattenVisible(root.Children))
                {
                    yield return child;
                }
            }
        }
    }

    /// <summary>
    /// Flattens a hierarchical collection into a single enumerable, including all items regardless of expand state.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="roots">The root-level items to flatten.</param>
    /// <returns>An enumerable of all items in tree order.</returns>
    public static IEnumerable<T> FlattenAll<T>(IEnumerable<T> roots) where T : class, IHierarchicalItem<T>
    {
        foreach (T root in roots)
        {
            yield return root;

            if (root.CanHaveChildren && root.ShowChildrenInHierarchy)
            {
                foreach (T child in FlattenAll(root.Children))
                {
                    yield return child;
                }
            }
        }
    }

    /// <summary>
    /// Finds an item in the hierarchy by reference.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="roots">The root-level items to search.</param>
    /// <param name="item">The item to find.</param>
    /// <returns>The item if found; otherwise, <see langword="null"/>.</returns>
    public static T? Find<T>(IEnumerable<T> roots, T item) where T : class, IHierarchicalItem<T>
    {
        foreach (T root in roots)
        {
            if (ReferenceEquals(root, item))
                return root;

            if (root.CanHaveChildren && root.ShowChildrenInHierarchy)
            {
                T? found = Find(root.Children, item);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the index of an item within the flattened visible list.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="roots">The root-level items.</param>
    /// <param name="item">The item to find the index of.</param>
    /// <returns>The 0-based index of the item, or -1 if not found or not visible.</returns>
    public static int GetVisibleIndex<T>(IEnumerable<T> roots, T item) where T : class, IHierarchicalItem<T>
    {
        int index = 0;
        foreach (T visible in FlattenVisible(roots))
        {
            if (ReferenceEquals(visible, item))
                return index;
            index++;
        }
        return -1;
    }

    /// <summary>
    /// Gets the item at a specific index in the flattened visible list.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="roots">The root-level items.</param>
    /// <param name="index">The 0-based index.</param>
    /// <returns>The item at the specified index, or <see langword="null"/> if index is out of range.</returns>
    public static T? GetItemAtVisibleIndex<T>(IEnumerable<T> roots, int index) where T : class, IHierarchicalItem<T>
    {
        if (index < 0)
            return null;

        int currentIndex = 0;
        foreach (T visible in FlattenVisible(roots))
        {
            if (currentIndex == index)
                return visible;
            currentIndex++;
        }
        return null;
    }

    /// <summary>
    /// Counts the total number of visible items in the hierarchy.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="roots">The root-level items.</param>
    /// <returns>The count of visible items.</returns>
    public static int CountVisible<T>(IEnumerable<T> roots) where T : class, IHierarchicalItem<T>
    {
        int count = 0;
        foreach (T _ in FlattenVisible(roots))
            count++;
        return count;
    }

    /// <summary>
    /// Gets the root ancestor of an item.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="item">The item to find the root of.</param>
    /// <returns>The root ancestor, or the item itself if it has no parent.</returns>
    public static T GetRoot<T>(T item) where T : class, IHierarchicalItem<T>
    {
        T current = item;
        while (current.Parent != null)
            current = current.Parent;
        return current;
    }

    /// <summary>
    /// Builds the ancestor chain from root to the specified item.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="item">The item to build the path for.</param>
    /// <returns>A list of ancestors from root to the item (inclusive).</returns>
    public static List<T> GetAncestorPath<T>(T item) where T : class, IHierarchicalItem<T>
    {
        List<T> path = [];
        T? current = item;
        while (current != null)
        {
            path.Insert(0, current);
            current = current.Parent;
        }
        return path;
    }

    /// <summary>
    /// Ensures all ancestors of an item are expanded so the item is visible.
    /// </summary>
    /// <typeparam name="T">The type of hierarchical item.</typeparam>
    /// <param name="item">The item to make visible.</param>
    public static void EnsureVisible<T>(T item) where T : class, IHierarchicalItem<T>
    {
        T? parent = item.Parent;
        while (parent != null)
        {
            parent.IsExpanded = true;
            parent = parent.Parent;
        }
    }
}
