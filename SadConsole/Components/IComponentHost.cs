using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SadConsole.Components;

/// <summary>
/// Provides a collection of <see cref="IComponent"/> objects.
/// </summary>
public interface IComponentHost
{
    /// <summary>
    /// A collection of components processed by this console.
    /// </summary>
    ObservableCollection<IComponent> SadComponents { get; }

    /// <summary>
    /// Gets the first component of the specified type.
    /// </summary>
    /// <typeparam name="TComponent">The component to find.</typeparam>
    /// <returns>The component if found, otherwise null.</returns>
    TComponent? GetSadComponent<TComponent>() where TComponent : class, IComponent;

    /// <summary>
    /// Gets components of the specified types.
    /// </summary>
    /// <typeparam name="TComponent">The component to find</typeparam>
    /// <returns>The components found.</returns>
    IEnumerable<TComponent> GetSadComponents<TComponent>() where TComponent : class, IComponent;

    /// <summary>
    /// Indicates whether or not the component exists in the <see cref="SadComponents"/> collection.
    /// </summary>
    /// <typeparam name="TComponent">The component to find.</typeparam>
    /// <returns><see langword="true"/> when the component exists; otherwise <see langword="false"/>.</returns>
    bool HasSadComponent<TComponent>(out TComponent? component) where TComponent : class, IComponent;

    /// <summary>
    /// Uses the <see cref="IComponent.SortOrder"/> to compare the <paramref name="left"/> component with the <paramref name="right"/> component.
    /// </summary>
    /// <param name="left">The first component to compare.</param>
    /// <param name="right">The second component to compare.</param>
    /// <returns><code>1</code> when the <paramref name="left"/> sort order is greater than <paramref name="right"/>; <code>-1</code> when <paramref name="left"/> is less than <paramref name="right"/>; <code>0</code> when they are equal.</returns>
    public static int CompareComponent(IComponent left, IComponent right)
    {
        if (left.SortOrder > right.SortOrder)
            return 1;

        if (left.SortOrder < right.SortOrder)
            return -1;

        return 0;
    }
}
