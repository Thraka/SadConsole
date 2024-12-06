using System.Collections;
using System.Collections.Generic;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// A collection of <see cref="ImGuiObjectBase"/> objects that can be added to a ImGui renderer.
/// </summary>
public class ImGuiObjectCollection : ImGuiObjectBase, IList<ImGuiObjectBase>
{
    List<ImGuiObjectBase> _list;

    /// <summary>
    /// Creates a new instance of this object.
    /// </summary>
    public ImGuiObjectCollection() =>
        _list = [];

    /// <summary>
    /// Draws the objects contained in this collection if <see cref="ImGuiObjectBase.IsVisible"/> is <see langword="true"/>.
    /// </summary>
    /// <param name="renderer">The ImGui renderer.</param>
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (!IsVisible) return;

        foreach (ImGuiObjectBase item in _list)
            item.BuildUI(renderer);
    }

    /// <inheritdoc/>
    public ImGuiObjectBase this[int index] { get => ((IList<ImGuiObjectBase>)_list)[index]; set => ((IList<ImGuiObjectBase>)_list)[index] = value; }
    /// <inheritdoc/>
    public int Count => ((ICollection<ImGuiObjectBase>)_list).Count;
    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<ImGuiObjectBase>)_list).IsReadOnly;
    /// <inheritdoc/>
    public void Add(ImGuiObjectBase item) => ((ICollection<ImGuiObjectBase>)_list).Add(item);
    /// <inheritdoc/>
    public void Clear() => ((ICollection<ImGuiObjectBase>)_list).Clear();
    /// <inheritdoc/>
    public bool Contains(ImGuiObjectBase item) => ((ICollection<ImGuiObjectBase>)_list).Contains(item);
    /// <inheritdoc/>
    public void CopyTo(ImGuiObjectBase[] array, int arrayIndex) => ((ICollection<ImGuiObjectBase>)_list).CopyTo(array, arrayIndex);
    /// <inheritdoc/>
    public IEnumerator<ImGuiObjectBase> GetEnumerator() => ((IEnumerable<ImGuiObjectBase>)_list).GetEnumerator();
    /// <inheritdoc/>
    public int IndexOf(ImGuiObjectBase item) => ((IList<ImGuiObjectBase>)_list).IndexOf(item);
    /// <inheritdoc/>
    public void Insert(int index, ImGuiObjectBase item) => ((IList<ImGuiObjectBase>)_list).Insert(index, item);
    /// <inheritdoc/>
    public bool Remove(ImGuiObjectBase item) => ((ICollection<ImGuiObjectBase>)_list).Remove(item);
    /// <inheritdoc/>
    public void RemoveAt(int index) => ((IList<ImGuiObjectBase>)_list).RemoveAt(index);
    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
}
