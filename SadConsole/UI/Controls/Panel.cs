using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole.UI.Controls;

/// <summary>
/// A control container that processes the mouse on each child. <see cref="ControlBase.TabStop"/> defaults to <see langword="false"/>.
/// </summary>
[DataContract]
public partial class Panel : CompositeControl, IList<ControlBase>
{
    /// <summary>
    /// Gets or sets a control in the collection of controls.
    /// </summary>
    /// <param name="index">The index of the control.</param>
    /// <returns>The control at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index doesn't exist in the collection.</exception>
    public new ControlBase this[int index]
    {
        get => Controls[index];
        set
        {
            if (index < 0 || index >= Controls.Count)
                throw new IndexOutOfRangeException();

            RemoveAt(index);
            Insert(index, value);
        }
    }
    
    /// <summary>
    /// Creates a new drawing surface control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control.</param>
    public Panel(int width, int height) : base(width, height)
    {
        TabStop = false;
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        TabStop = false;

        foreach (ControlBase control in Controls)
            control.Parent = this;
    }

    /// <summary>
    /// Removes all controls.
    /// </summary>
    public void Clear() =>
        Controls.Clear();

    /// <inheritdoc/>
    public void Add(ControlBase control)
    {
        if (Controls.Contains(control)) return;

        Controls.Add(control);
        if (control.Parent != this)
            control.Parent = this;

        control.IsDirtyChanged += Control_IsDirtyChanged;
    }

    /// <inheritdoc/>
    public bool Remove(ControlBase control)
    {
        if (Controls.Remove(control))
        {
            if (control.Parent == this)
                control.Parent = null;

            control.IsDirtyChanged -= Control_IsDirtyChanged;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes a control at the specified index.
    /// </summary>
    /// <param name="index">The index of the control to remove.</param>
    public void RemoveAt(int index) =>
        Remove(Controls[index]);

    /// <summary>
    /// Returns the index of the specified control.
    /// </summary>
    /// <param name="control">The control to search for.</param>
    /// <returns>The index of the control.</returns>
    public int IndexOf(ControlBase control) =>
        Controls.IndexOf(control);

    /// <summary>
    /// Inserts the control at the specified index.
    /// </summary>
    /// <param name="index">The index to insert at.</param>
    /// <param name="control">The control to insert.</param>
    public void Insert(int index, ControlBase control)
    {
        if (Controls.Contains(control)) return;

        Controls.Insert(index, control);

        if (control.Parent != this)
            control.Parent = this;

        control.IsDirtyChanged += Control_IsDirtyChanged;
    }

    /// <inheritdoc/>
    public bool Contains(ControlBase control) =>
        Controls.Contains(control);

    /// <inheritdoc/>
    public void CopyTo(ControlBase[] array, int arrayIndex) =>
        Controls.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    protected override void OnMouseExit(ControlMouseState state)
    {
        base.OnMouseExit(state);

        var controls = new List<ControlBase>(Controls);

        for (int i = 0; i < controls.Count; i++)
        {
            ControlBase control = controls[i];
            control.LostMouse(state.OriginalMouseState);
        }
    }

    /// <inheritdoc/>
    public override bool ProcessKeyboard(Keyboard state)
    {
        if (IsEnabled && UseKeyboard)
        {
            bool processResult = base.ProcessKeyboard(state);

            var controls = new List<ControlBase>(Controls);
            controls.Reverse();

            for (int i = 0; i < controls.Count; i++)
            {
                ControlBase control = controls[i];
                if (control.ProcessKeyboard(state))
                    return true;
            }

            return processResult;
        }

        return false;
    }

    /// <summary>
    /// When <see cref="ControlBase.IsDirty"/> is set to <see langword="true"/>, changes the child controls to also be dirty.
    /// </summary>
    protected override void OnIsDirtyChanged()
    {
        if (IsDirty)
            foreach (var control in Controls)
                control.IsDirty = true;

        base.OnIsDirtyChanged();
    }

    private void Control_IsDirtyChanged(object? sender, EventArgs e)
    {
        if (sender == null) return;

        if (((ControlBase)sender).IsDirty)
            IsDirty = true;
    }

    /// <summary>
    /// Gets an enumerator that iterates over the controls in this panel.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<ControlBase> GetEnumerator() =>
        Controls.GetEnumerator();

    /// <summary>
    /// Gets an enumerator that iterates over the controls in this panel.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() =>
        Controls.GetEnumerator();
}
