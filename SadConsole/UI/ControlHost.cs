using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.UI;

/// <summary>
/// Adds the ability for a host to contain and display controls from <see cref="SadConsole.UI.Controls"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("UI host")]
public class ControlHost : Components.IComponent, IEnumerable<ControlBase>, IList<ControlBase>, IContainer
{
    /// <summary>
    /// Indicates priority to other components.
    /// </summary>
    public uint SortOrder { get; set; }

    uint Components.IComponent.SortOrder => SortOrder;

    bool Components.IComponent.IsUpdate => true;

    bool Components.IComponent.IsRender => false;

    bool Components.IComponent.IsMouse => true;

    bool Components.IComponent.IsKeyboard => true;

    /// <summary>
    /// The collection of controls.
    /// </summary>
    [DataMember]
    protected List<ControlBase> ControlsList = new List<ControlBase>();

    private ControlBase? _focusedControl;
    private bool _wasFocusedBeforeCapture;
    private bool _exclusiveBeforeCapture;
    private Colors? _themeColors;
    private ControlBase? _controlWithMouse;
    private Renderers.IRenderStep? _controlsRenderStep;
    private Rectangle _parentView;

    #region Properties

    /// <summary>
    /// Gets a control by index.
    /// </summary>
    /// <param name="index">The index of the control.</param>
    /// <returns>A control.</returns>
    public ControlBase this[int index] =>
        ControlsList[index];

    /// <summary>
    /// The parent object hosting the controls.
    /// </summary>
    public IScreenSurface? ParentConsole { get; private set; }

    /// <summary>
    /// Gets or sets the colors to use with drawing the console and controls.
    /// </summary>
    public Colors? ThemeColors
    {
        get => _themeColors;
        set
        {
            _themeColors = value;
            IsDirty = true;

            if (ParentConsole != null)
                ParentConsole.IsDirty = true;

            int count = ControlsList.Count;
            for (int i = 0; i < count; i++)
                ControlsList[i].IsDirty = true;
        }
    }

    /// <summary>
    /// Indicates that the control host needs to be redrawn.
    /// </summary>
    public bool IsDirty { get; set; }

    /// <summary>
    /// Gets the control currently capturing mouse events.
    /// </summary>
    public ControlBase? CapturedControl { get; private set; }

    /// <summary>
    /// Gets or sets the control that has keyboard focus.
    /// </summary>
    public ControlBase? FocusedControl
    {
        get => _focusedControl;
        set
        {
            if (!DisableControlFocusing)
            {
                if (FocusedControlChanging(value, _focusedControl))
                {
                    ControlBase? oldControl = _focusedControl;
                    _focusedControl = value;

                    FocusedControlChanged(_focusedControl, oldControl);
                }
            }
        }
    }

    /// <summary>
    /// When <see langword="true"/>, the component will clear the console it's attached to with the theme color.
    /// </summary>
    [DataMember]
    public bool ClearOnAdded { get; set; } = true;

    /// <summary>
    /// When <see langword="true"/>, the component will disable the cursor of the console it's attached to.
    /// </summary>
    [DataMember]
    public bool DisableCursorOnAdded { get; set; } = true;

    /// <summary>
    /// When true, allows the tab command to move to the next console (when there is a parent) instead of cycling back to the first control on this console.
    /// </summary>
    [DataMember]
    public bool CanTabToNextConsole { get; set; }

    /// <summary>
    /// Sets reference to the console to tab to when the <see cref="CanTabToNextConsole"/> property is true. Set this to null to allow the engine to determine the next console.
    /// </summary>
    public IScreenSurface? NextTabConsole { get; set; }

    /// <summary>
    /// Sets reference to the console to tab to when the <see cref="CanTabToNextConsole"/> property is true. Set this to null to allow the engine to determine the next console.
    /// </summary>
    public IScreenSurface? PreviousTabConsole { get; set; }

    /// <summary>
    /// When set to true, child controls are not alerted to focused and non-focused states.
    /// </summary>
    [DataMember]
    public bool DisableControlFocusing { get; set; }

    /// <summary>
    /// The total number of controls in this component.
    /// </summary>
    public int Count => ControlsList.Count;

    /// <summary>
    /// Always returns <see langword="false"/>.
    /// </summary>
    public bool IsReadOnly => false;

    Point IContainer.AbsolutePosition => (0, 0);

    ControlHost IContainer.Host => this;

    ControlBase IList<ControlBase>.this[int index] { get => ((IList<ControlBase>)ControlsList)[index]; set => ((IList<ControlBase>)ControlsList)[index] = value; }
    #endregion

    void Components.IComponent.OnAdded(IScreenObject host)
    {
        if (ParentConsole != null) throw new Exception("Component has already been added to a host.");
        if (host is not IScreenSurface surface) throw new ArgumentException($"Must add this component to a type that implements {nameof(IScreenSurface)}");

        if (_controlsRenderStep != null)
        {
            surface.RenderSteps.Remove(_controlsRenderStep);
            _controlsRenderStep?.Dispose();
        }
        _controlsRenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.ControlHost);
        _controlsRenderStep.SetData(this);
        surface.RenderSteps.Add(_controlsRenderStep);
        surface.RenderSteps.Sort(RenderStepComparer.Instance);
        surface.UseKeyboard = true;
        surface.UseMouse = true;
        surface.FocusOnMouseClick = true;

        if (ClearOnAdded)
        {
            var colors = GetThemeColors();
            surface.Surface.DefaultBackground = colors.ControlHostBackground;
            surface.Surface.DefaultForeground = colors.ControlHostForeground;
            surface.Surface.Clear();
        }

        ParentConsole = surface;
        _parentView = ParentConsole.Surface.View;

        if (host is Console con && DisableCursorOnAdded)
        {
            // Configure the console.
            con.Cursor.IsVisible = false;
            con.Cursor.IsEnabled = false;
            con.AutoCursorOnFocus = false;
        }

        surface.MouseExit += Surface_MouseExit;
        surface.Focused += Surface_Focused;
        surface.FocusLost += Surface_FocusLost;
    }

    void Components.IComponent.OnRemoved(IScreenObject host)
    {
        if (ParentConsole != null)
        {
            ParentConsole.MouseExit -= Surface_MouseExit;
            ParentConsole.Focused -= Surface_Focused;
            ParentConsole.FocusLost -= Surface_FocusLost;

            ParentConsole = null;
        }

        if (_controlsRenderStep != null)
        {
            ((IScreenSurface)host).RenderSteps.Remove(_controlsRenderStep);
            _controlsRenderStep.Dispose();
            _controlsRenderStep = null;
        }
    }

    void Components.IComponent.ProcessKeyboard(IScreenObject host, Keyboard info, out bool handled)
    {
        handled = false;

        if (!host.UseKeyboard) return;

        if (FocusedControl != null && FocusedControl.IsEnabled && FocusedControl.UseKeyboard)
            handled = FocusedControl.ProcessKeyboard(info);

        if (!handled)
        {
            if (
                (info.IsKeyDown(Keys.LeftShift) ||
                info.IsKeyDown(Keys.RightShift) ||
                info.IsKeyReleased(Keys.LeftShift) ||
                info.IsKeyReleased(Keys.RightShift)) &&
                info.IsKeyReleased(Keys.Tab))
            {
                TabPreviousControl();
                handled = true;
                return;
            }

            if (info.IsKeyReleased(Keys.Tab))
            {
                TabNextControl();
                handled = true;
                return;
            }
        }
    }

    void Components.IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
    {
        // Never handle the mouse so that the screen object can continue calling events related to mouse
        handled = false;

        if (!host.UseMouse) return;

        if ((state.ScreenObject == host && state.IsOnScreenObject) || host.IsExclusiveMouse)
        {
            if (CapturedControl != null && CapturedControl.Parent == this)
                CapturedControl.ProcessMouse(state);
            else
            {
                var controls = ControlsList.ToList();
                controls.Reverse();

                int count = controls.Count;
                for (int i = 0; i < count; i++)
                {
                    ControlBase control = controls[i];
                    if (control.IsVisible && control.Parent == this && control.ProcessMouse(state))
                    {
                        if (_controlWithMouse != null && _controlWithMouse != control)
                            _controlWithMouse.LostMouse(state);

                        _controlWithMouse = control;
                        break;
                    }
                }
            }
        }
    }

    void Components.IComponent.Update(IScreenObject host, TimeSpan delta)
    {
        if (_parentView != ParentConsole!.Surface.View)
        {
            _parentView = ParentConsole.Surface.View;
            IsDirty = true;
        }

        ControlBase[] tempControlArray = ControlsList.ToArray();
        for (int i = 0; i < tempControlArray.Length; i++)
        {
            ControlBase control = tempControlArray[i];
            if (control.IsDirty)
                IsDirty = true;

            control.Update(delta);

            if (control.IsDirty)
                IsDirty = true;
        }
    }

    void Components.IComponent.Render(IScreenObject host, TimeSpan delta) { }

    /// <summary>
    /// Gives the focus to the next control in the tab order.
    /// </summary>
    public void TabNextControl()
    {
        if (ControlsList.Count == 0)
            return;

        ControlBase? control;

        if (_focusedControl == null)
        {
            if (FindTabControlForward(0, ControlsList.Count - 1, out control))
            {
                FocusedControl = control;
                return;
            }

            TryTabNextConsole();
        }
        else
        {
            int index = ControlsList.IndexOf(_focusedControl);

            // From first control
            if (index == 0)
            {
                if (FindTabControlForward(index + 1, ControlsList.Count - 1, out control))
                {
                    FocusedControl = control;
                    return;
                }

                TryTabNextConsole();
            }

            // From last control
            else if (index == ControlsList.Count - 1)
            {
                if (!TryTabNextConsole())
                {
                    if (FindTabControlForward(0, ControlsList.Count - 1, out control))
                    {
                        FocusedControl = control;
                        return;
                    }
                }
            }

            // Middle
            else
            {
                // Middle > End
                if (FindTabControlForward(index + 1, ControlsList.Count - 1, out control))
                {
                    FocusedControl = control;
                    return;
                }

                // Next console
                if (TryTabNextConsole())
                    return;

                // Start > Middle
                if (FindTabControlForward(0, index, out control))
                {
                    FocusedControl = control;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Gives focus to the previous control in the tab order.
    /// </summary>
    public void TabPreviousControl()
    {
        if (ControlsList.Count == 0)
            return;

        ControlBase? control;

        if (_focusedControl == null)
        {
            if (FindTabControlPrevious(ControlsList.Count - 1, 0, out control))
            {
                FocusedControl = control;
                return;
            }

            TryTabPreviousConsole();
        }
        else
        {
            int index = ControlsList.IndexOf(_focusedControl);

            // From first control
            if (index == 0)
            {
                if (!TryTabPreviousConsole())
                {
                    if (FindTabControlPrevious(ControlsList.Count - 1, 0, out control))
                    {
                        FocusedControl = control;
                        return;
                    }
                }
            }

            // From last control
            else if (index == ControlsList.Count - 1)
            {
                if (FindTabControlPrevious(index - 1, 0, out control))
                {
                    FocusedControl = control;
                    return;
                }

                TryTabPreviousConsole();
            }

            // Middle
            else
            {
                // Middle -> Start
                if (FindTabControlPrevious(index - 1, 0, out control))
                {
                    FocusedControl = control;
                    return;
                }

                // Next console
                if (TryTabPreviousConsole())
                    return;

                // End -> Middle
                if (FindTabControlPrevious(ControlsList.Count - 1, index, out control))
                {
                    FocusedControl = control;
                    return;
                }
            }
        }
    }

    private bool FindTabControlForward(int startingIndex, int endingIndex, [NotNullWhen(true)] out ControlBase? foundControl)
    {
        for (int i = startingIndex; i <= endingIndex; i++)
        {
            if (ControlsList[i].TabStop && ControlsList[i].IsEnabled && ControlsList[i].CanFocus)
            {
                foundControl = ControlsList[i];
                return true;
            }
        }

        foundControl = null;
        return false;
    }

    private bool FindTabControlPrevious(int startingIndex, int endingIndex, [NotNullWhen(true)] out ControlBase? foundControl)
    {
        for (int i = startingIndex; i >= endingIndex; i--)
        {
            if (ControlsList[i].TabStop && ControlsList[i].IsEnabled && ControlsList[i].CanFocus)
            {
                foundControl = ControlsList[i];
                return true;
            }
        }

        foundControl = null;
        return false;
    }

    private bool ParentHasComponent(IScreenSurface surface) =>
        surface.HasSadComponent<ControlHost>(out _);

    /// <summary>
    /// Tries to tab to the console that comes before this one in the <see cref="IScreenObject.Parent"/> collection of <see cref="IScreenObject.Children"/>. Sets focus to the target console if found.
    /// </summary>
    /// <returns><see langword="true"/> if the tab was successful; otherwise, <see langword="false"/>.</returns>
    protected bool TryTabPreviousConsole()
    {
        if (!CanTabToNextConsole || ParentConsole?.Parent == null) return false;

        IScreenSurface newConsole;
        var consoles = ParentConsole.Parent.Children.OfType<IScreenSurface>().Where(ParentHasComponent).ToList();

        // If no consoles found, get out
        if (consoles.Count == 0)
        {
            return false;
        }

        // If a previous console has not be explicitly set, find the previous console.
        if (PreviousTabConsole == null || !consoles.Contains(PreviousTabConsole))
        {
            int parentIndex = consoles.IndexOf(ParentConsole);
            if (parentIndex == 0)
            {
                parentIndex = consoles.Count - 1;
            }
            else
            {
                parentIndex -= 1;
            }

            // Get the new focused console
            newConsole = consoles[parentIndex];
        }
        else
        {
            newConsole = PreviousTabConsole;
        }

        // Set focus to this new console
        GameHost.Instance.FocusedScreenObjects.Set(newConsole);
        FocusedControl = null;
        ControlHost? newConsoleComponent = newConsole.GetSadComponent<ControlHost>();

        if (newConsoleComponent != null)
        {
            newConsoleComponent.FocusedControl = null;
            newConsoleComponent.TabPreviousControl();
        }

        return true;
    }

    /// <summary>
    /// Tries to tab to the console that comes after this one in the <see cref="IScreenObject.Parent"/> collection of <see cref="IScreenObject.Children"/>. Sets focus to the target console if found.
    /// </summary>
    /// <returns><see langword="true"/> if the tab was successful; otherwise, <see langword="false"/>.</returns>
    protected bool TryTabNextConsole()
    {
        if (!CanTabToNextConsole || ParentConsole?.Parent == null) return false;

        IScreenSurface newConsole;
        var consoles = ParentConsole.Parent.Children.OfType<IScreenSurface>().Where(ParentHasComponent).ToList();

        // If no consoles found, get out
        if (consoles.Count == 0)
            return false;

        // If a previous console has not be explicitly set, find the previous console.
        if (NextTabConsole == null || !consoles.Contains(NextTabConsole))
        {
            int parentIndex = consoles.IndexOf(ParentConsole);
            if (parentIndex == consoles.Count - 1)
                parentIndex = 0;
            else
                parentIndex += 1;

            // Get the new focused console
            newConsole = consoles[parentIndex];
        }
        else
            newConsole = NextTabConsole;

        // Set focus to this new console
        GameHost.Instance.FocusedScreenObjects.Set(newConsole);
        FocusedControl = null;
        ControlHost? newConsoleComponent = newConsole.GetSadComponent<ControlHost>();

        if (newConsoleComponent != null)
        {
            newConsoleComponent.FocusedControl = null;
            newConsoleComponent.TabNextControl();
        }

        return true;
    }

    /// <summary>
    /// Returns the colors assigned to this console or the library default.
    /// </summary>
    /// <returns>The found colors.</returns>
    public Colors GetThemeColors() =>
        _themeColors ?? Library.Default.Colors;

    /// <summary>
    /// Gets an array containing all of the controls this host contains.
    /// </summary>
    public ControlBase[] ToArray() =>
        ControlsList.ToArray();

    /// <summary>
    /// When overridden, allows you to prevent a control from taking focus from another control.
    /// </summary>
    /// <param name="newControl">The control requesting focus.</param>
    /// <param name="oldControl">The control that has focus.</param>
    /// <returns>True when the focus change is allowed; otherwise false.</returns>
    protected virtual bool FocusedControlChanging(ControlBase? newControl, ControlBase? oldControl) =>
        newControl != null && newControl.CanFocus && newControl.IsEnabled;

    /// <summary>
    /// This method is called when a control gains focus.
    /// </summary>
    /// <param name="newControl">The control that has focus.</param>
    /// <param name="oldControl">The control that previously had focus.</param>
    protected virtual void FocusedControlChanged(ControlBase? newControl, ControlBase? oldControl)
    {
        oldControl?.FocusLost();
        newControl?.Focused();
    }

    /// <summary>
    /// Reorders the control collection based on the tab index of each control.
    /// </summary>
    public void ReOrderControls() =>
        ControlsList.Sort((x, y) =>
        {
            if (x.TabIndex == y.TabIndex) return 0;
            if (x.TabIndex < y.TabIndex) return -1;
            return 1;
        });

    /// <inheritdoc />
    private void Surface_MouseExit(object? sender, MouseScreenObjectState state)
    {
        ControlBase control;
        for (int i = 0; i < ControlsList.Count; i++)
        {
            control = ControlsList[i];
            control.LostMouse(state);
        }
    }

    private void Surface_FocusLost(object? sender, EventArgs e)
    {
        FocusedControl?.DetermineState();
    }


    private void Surface_Focused(object? sender, EventArgs e)
    {
        FocusedControl?.DetermineState();
    }

    /// <summary>
    /// Captures a control for exclusive mouse focus. Sets the ExclusiveMouse property to true.
    /// </summary>
    /// <param name="control">The control to capture</param>
    public void CaptureControl(ControlBase control)
    {
        if (CapturedControl == control || ParentConsole == null) return;

        if (GameHost.Instance.FocusedScreenObjects.ScreenObject != ParentConsole)
        {
            GameHost.Instance.FocusedScreenObjects.Push(ParentConsole);
            _wasFocusedBeforeCapture = false;
        }
        else
        {
            _wasFocusedBeforeCapture = true;
        }

        _exclusiveBeforeCapture = ParentConsole.IsExclusiveMouse;
        ParentConsole.IsExclusiveMouse = true;
        CapturedControl = control;
    }

    /// <summary>
    /// Releases the control from exclusive mouse focus. Sets the ExclusiveMouse property to false and sets the CapturedControl property to null.
    /// </summary>
    public void ReleaseControl()
    {
        if (CapturedControl == null || ParentConsole == null) return;

        if (!_wasFocusedBeforeCapture)
            GameHost.Instance.FocusedScreenObjects.Pop(ParentConsole);

        ParentConsole.IsExclusiveMouse = _exclusiveBeforeCapture;
        CapturedControl.LostMouse(new MouseScreenObjectState(ParentConsole, GameHost.Instance.Mouse));
        CapturedControl = null;
    }

    /// <summary>
    /// Forces each control to dirty so that the next frame will redraw each control.
    /// </summary>
    public void ForceRedrawAllControls()
    {
        IsDirty = true;

        int count = ControlsList.Count;
        for (int i = 0; i < count; i++)
            ControlsList[i].IsDirty = true;
    }

    /// <summary>
    /// Gets an enumerator of the controls collection.
    /// </summary>
    /// <returns>The enumerator of the controls collection.</returns>
    public IEnumerator<ControlBase> GetEnumerator() => ControlsList.GetEnumerator();

    /// <summary>
    /// Gets an enumerator of the controls collection.
    /// </summary>
    /// <returns>The enumerator of the controls collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => ControlsList.GetEnumerator();

    /// <summary>
    /// Gets the index of the specified control.
    /// </summary>
    /// <param name="item">The control.</param>
    /// <returns>The index of the control in the backing collection.</returns>
    public int IndexOf(ControlBase item) =>
        ControlsList.IndexOf(item);

    /// <summary>
    /// Inserts an item at the specified index and sets the <see cref="ControlBase.TabIndex"/> to the specified index.
    /// </summary>
    /// <remarks>Index within the backing collection is always based on <see cref="ControlBase.TabIndex"/> ranking. There may be conflicts so you're not guaranteed that the control will be available at the specified index.</remarks>
    /// <param name="index">Index to insert at.</param>
    /// <param name="item">The control to insert.</param>
    public void Insert(int index, ControlBase item)
    {
        ((IList<ControlBase>)ControlsList).Insert(index, item);

        if (!ControlsList.Contains(item))
            ControlsList.Insert(index, item);

        item.Parent = this;
        item.TabIndex = index;

        IsDirty = true;

        ReOrderControls();
    }

    /// <summary>
    /// Removes a control by index.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= ControlsList.Count) throw new IndexOutOfRangeException();
        Remove(ControlsList[index]);
    }

    /// <summary>
    /// Adds an existing control to this console.
    /// </summary>
    /// <param name="control">The control to add.</param>
    public void Add(ControlBase control)
    {
        if (!ControlsList.Contains(control))
            ControlsList.Add(control);

        if (control.Parent != this)
            control.Parent = this;

        control.TabIndex = ControlsList.Count - 1;

        IsDirty = true;

        ReOrderControls();
    }

    /// <summary>
    /// Removes all controls from this console.
    /// </summary>
    public void Clear()
    {
        if (CapturedControl != null) ReleaseControl();

        FocusedControl = null;

        var controls = ControlsList.ToArray();
        ControlsList.Clear();

        int count = controls.Length;
        for (int i = 0; i < count; i++)
            controls[i].Parent = null;

        IsDirty = true;
    }

    /// <summary>
    /// Checks if the specified control exists in this console.
    /// </summary>
    /// <param name="control">The control to check.</param>
    /// <returns>True when the control exists in this console; otherwise false.</returns>
    public bool Contains(ControlBase control) =>
        ControlsList.Contains(control);

    /// <summary>
    /// Copies the controls to a new array.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The starting index of where to copy the controls in the destination array.</param>
    public void CopyTo(ControlBase[] array, int arrayIndex) =>
        ControlsList.CopyTo(array, arrayIndex);

    /// <summary>
    /// Removes a control from this console.
    /// </summary>
    /// <param name="control">The control to remove.</param>
    /// <returns><see langword="true"/> if item was successfully removed; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if item is not found.</returns>
    public bool Remove(ControlBase control)
    {
        if (ControlsList.Contains(control))
        {
            control.TabIndex = -1;

            if (CapturedControl == control)
                ReleaseControl();

            if (FocusedControl == control)
            {
                int index = ControlsList.IndexOf(control);
                ControlsList.Remove(control);

                if (ControlsList.Count == 0)
                    FocusedControl = null;
                else if (index > ControlsList.Count - 1)
                    FocusedControl = ControlsList[ControlsList.Count - 1];
                else
                    FocusedControl = ControlsList[index];
            }
            else
                ControlsList.Remove(control);

            control.Parent = null;
            IsDirty = true;

            ReOrderControls();
            return true;
        }
        else
            return false;
    }

    bool ICollection<ControlBase>.Remove(ControlBase item)
    {
        return ((ICollection<ControlBase>)ControlsList).Remove(item);
    }
}
