using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SadConsole.Input;

namespace SadConsole.UI.Controls;

/// <summary>
/// Base class for controls that host and render other controls.
/// </summary>
public abstract class CompositeControl : ControlBase, IContainer
{
    /// <summary>
    /// The controls this composite control is hosting. Use <see cref="AddControl(ControlBase)"/> and <see cref="RemoveControl(ControlBase)"/> to manage the collection.
    /// </summary>
    protected List<ControlBase> Controls = new();

    /// <summary>
    /// The controls added which contain a <see cref="ControlBase.Name"/> value.
    /// </summary>
    protected Dictionary<string, ControlBase> NamedControls = new();

    /// <inheritdoc/>
    public int Count => Controls.Count;

    /// <inheritdoc/>
    protected ControlBase this[int index] => Controls[index];

    /// <inheritdoc/>
    ControlBase IContainer.this[string name] => NamedControls[name];

    /// <summary>
    /// Creates a new control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control in cells.</param>
    /// <param name="height">Height of the control in cells.</param>
    public CompositeControl(int width, int height) : base(width, height)
    {
        CreateChildControls();
    }

    /// <summary>
    /// Create each control and add it to <see cref="Controls"/>.
    /// </summary>
    protected virtual void CreateChildControls() { }

    /// <summary>
    /// A handler to forward state to <see cref="ControlBase.IsDirty"/>.
    /// </summary>
    /// <param name="value"></param>
    protected void IsDirtyEventHandler(bool value) =>
        IsDirty = value;

    /// <summary>
    /// Processes the mouse on each control hosted by this control.
    /// </summary>
    /// <param name="state">The mouse state based on the parent screen object.</param>
    /// <returns><see langword="true"/> when a control processes the mouse; otherwise <see langword="false"/>.</returns>
    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (IsEnabled && UseMouse)
        {
            var newState = new ControlMouseState(this, state);

            if (newState.IsMouseOver)
            {
                if (MouseState_IsMouseOver != true)
                {
                    MouseState_IsMouseOver = true;
                    OnMouseEnter(newState);
                }

                // Process the child controls first
                bool processResult = false;

                var controls = new List<ControlBase>(Controls);
                controls.Reverse();

                int count = controls.Count;
                for (int i = 0; i < count; i++)
                    processResult |= controls[i].ProcessMouse(state);

                if (!processResult)
                    processResult = base.ProcessMouse(state);

                // If no child control has the mouse over it, process it on this container
                if (!processResult)
                {
                    bool preventClick = MouseState_EnteredWithButtonDown;
                    OnMouseIn(newState);

                    if (!preventClick && state.Mouse.LeftClicked)
                        OnLeftMouseClicked(newState);

                    if (!preventClick && state.Mouse.RightClicked)
                        OnRightMouseClicked(newState);
                }

                return true;
            }
            else
            {
                if (MouseState_IsMouseOver)
                {
                    MouseState_IsMouseOver = false;
                    OnMouseExit(newState);
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Adds a child control to this control.
    /// </summary>
    /// <param name="control">The control to add.</param>
    protected void AddControl(ControlBase control)
    {
        if (!Controls.Contains(control))
        {
            Controls.Add(control);

            if (!string.IsNullOrEmpty(control.Name))
                NamedControls[control.Name] = control;

            if (control.Parent != this)
                control.Parent = this;
        }
    }

    /// <summary>
    /// Removes a child control from this control.
    /// </summary>
    /// <param name="control">The control to remove.</param>
    protected void RemoveControl(ControlBase control)
    {
        if (Controls.Contains(control))
        {
            Controls.Remove(control);
            if (control.Parent == this)
                control.Parent = null;
        }
    }

    /// <summary>
    /// Updates each control hosted by this control.
    /// </summary>
    /// <param name="time">The game frame time delta.</param>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        ControlBase[] controls = Controls.ToArray();

        for (int i = 0; i < controls.Length; i++)
            controls[i].UpdateAndRedraw(time);
    }

    /// <inheritdoc/>
    public bool HasNamedControl(string name) =>
        HasNamedControl(name, out _);

    /// <inheritdoc/>
    public bool HasNamedControl(string name, [NotNullWhen(true)] out ControlBase? control) =>
        NamedControls.TryGetValue(name, out control);

    ControlHost? IContainer.Host => Parent?.Host;

    #region IList
    bool ICollection<ControlBase>.IsReadOnly => true;

    void ICollection<ControlBase>.Add(ControlBase control) =>
        AddControl(control);

    IEnumerator<ControlBase> IEnumerable<ControlBase>.GetEnumerator() =>
        Controls.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        Controls.GetEnumerator();

    bool ICollection<ControlBase>.Remove(ControlBase control) =>
        throw new NotSupportedException();

    ControlBase IList<ControlBase>.this[int index]
    {
        get => Controls[index];
        set => throw new NotSupportedException();
    }

    int IList<ControlBase>.IndexOf(ControlBase item) =>
        Controls.IndexOf(item);

    void IList<ControlBase>.Insert(int index, ControlBase item) =>
        throw new NotSupportedException();

    void IList<ControlBase>.RemoveAt(int index) =>
        throw new NotSupportedException();

    void ICollection<ControlBase>.Clear() =>
        throw new NotSupportedException();

    bool ICollection<ControlBase>.Contains(ControlBase item) =>
        Controls.Contains(item);

    void ICollection<ControlBase>.CopyTo(ControlBase[] array, int arrayIndex) =>
        throw new NotSupportedException();
    #endregion
}
