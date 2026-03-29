using System.Collections.Generic;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// Defines a hierarchical item that can have a parent and children, enabling tree-like structures.
/// </summary>
/// <typeparam name="T">The type of the hierarchical item, which must implement this interface.</typeparam>
public interface IHierarchicalItem<T> where T : class, IHierarchicalItem<T>
{
    /// <summary>
    /// Gets or sets the parent of this item, or <see langword="null"/> if this is a root item.
    /// </summary>
    T? Parent { get; set; }

    /// <summary>
    /// Gets the children of this item.
    /// </summary>
    IReadOnlyList<T> Children { get; }

    /// <summary>
    /// Gets a value indicating whether this item can contain children.
    /// </summary>
    bool CanHaveChildren { get; }

    /// <summary>
    /// Gets a value indicating whether children should be displayed in the hierarchy UI.
    /// When <see langword="false"/>, children exist but are hidden from the tree view.
    /// </summary>
    bool ShowChildrenInHierarchy { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this item's children are expanded in the UI.
    /// Only meaningful when <see cref="CanHaveChildren"/> and <see cref="ShowChildrenInHierarchy"/> are <see langword="true"/>.
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// Gets the depth of this item in the hierarchy (0 for root items).
    /// </summary>
    int Depth => Parent == null ? 0 : Parent.Depth + 1;
}
