using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Debug.MonoGame
{
    /// <summary>
    /// A collection of <see cref="ImGuiObjectBase"/> objects that can be added to a ImGui renderer.
    /// </summary>
    public class ImGuiObjectCollection : ImGuiObjectBase, IList<ImGuiObjectBase>
    {
        List<ImGuiObjectBase> _list;

        /// <summary>
        /// 
        /// </summary>
        public ImGuiObjectCollection() =>
            _list = new List<ImGuiObjectBase>();

        public override void BuildUI(ImGuiRenderer renderer)
        {
            foreach (ImGuiObjectBase item in _list)
                item.BuildUI(renderer);
        }

        public ImGuiObjectBase this[int index] { get => ((IList<ImGuiObjectBase>)_list)[index]; set => ((IList<ImGuiObjectBase>)_list)[index] = value; }

        public int Count => ((ICollection<ImGuiObjectBase>)_list).Count;

        public bool IsReadOnly => ((ICollection<ImGuiObjectBase>)_list).IsReadOnly;

        public void Add(ImGuiObjectBase item) => ((ICollection<ImGuiObjectBase>)_list).Add(item);
        public void Clear() => ((ICollection<ImGuiObjectBase>)_list).Clear();
        public bool Contains(ImGuiObjectBase item) => ((ICollection<ImGuiObjectBase>)_list).Contains(item);
        public void CopyTo(ImGuiObjectBase[] array, int arrayIndex) => ((ICollection<ImGuiObjectBase>)_list).CopyTo(array, arrayIndex);
        public IEnumerator<ImGuiObjectBase> GetEnumerator() => ((IEnumerable<ImGuiObjectBase>)_list).GetEnumerator();
        public int IndexOf(ImGuiObjectBase item) => ((IList<ImGuiObjectBase>)_list).IndexOf(item);
        public void Insert(int index, ImGuiObjectBase item) => ((IList<ImGuiObjectBase>)_list).Insert(index, item);
        public bool Remove(ImGuiObjectBase item) => ((ICollection<ImGuiObjectBase>)_list).Remove(item);
        public void RemoveAt(int index) => ((IList<ImGuiObjectBase>)_list).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
    }
}
