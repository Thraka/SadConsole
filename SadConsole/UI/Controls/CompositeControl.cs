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
    /// Indicates whether the most recent mouse event was handled by a child control.
    /// </summary>
    protected bool MouseLastHandledByChild = false;

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
                // Process the child controls first
                bool processResult = false;

                var controls = new List<ControlBase>(Controls);
                controls.Reverse();

                int count = controls.Count;
                for (int i = 0; i < count; i++)
                    processResult |= controls[i].ProcessMouse(state);

                // No child control used the mouse, process the base control logic
                if (!processResult)
                    processResult = base.ProcessMouse(state);

                else
                {
                    // Child control has the mouse over it, so we need to clear the base control as having the mouse over it, if it did have it
                    if (MouseState_IsMouseOver)
                    {
                        MouseState_IsMouseOver = false;
                        OnMouseExit(newState);
                    }

                    MouseLastHandledByChild = true;
                }

                return processResult;
            }
            else
            {
                if (MouseState_IsMouseOver)
                {
                    MouseState_IsMouseOver = false;
                    OnMouseExit(newState);
                }

                if (MouseLastHandledByChild)
                {
                    var controls = new List<ControlBase>(Controls);
                    controls.Reverse();

                    for (int i = 0; i < controls.Count; i++)
                        controls[i].ProcessMouse(state);

                    MouseLastHandledByChild = false;
                }
            }
        }

        return false;
    }

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
        {
            controls[i].UpdateAndRedraw(time);

            IsDirty |= controls[i].IsDirty;
        }
    }

    /// <inheritdoc/>
    bool IContainer.HasNamedControl(string name) =>
        NamedControls.TryGetValue(name, out _);

    /// <inheritdoc/>
    bool IContainer.HasNamedControl(string name, [NotNullWhen(true)] out ControlBase? control) =>
        NamedControls.TryGetValue(name, out control);

    /// <inheritdoc/>
    ControlBase IContainer.GetNamedControl(string name) =>
        NamedControls[name];

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
