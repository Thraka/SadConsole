using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadConsole.UI.Themes;
using SadConsole.UI.Controls;
using Keyboard = SadConsole.Input.Keyboard;

namespace SadConsole.UI
{
    /// <summary>
    /// A basic console that can contain controls.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("Console (Controls)")]
    public class ControlsConsole : Console, IEnumerable<ControlBase>
    {
        /// <summary>
        /// Raised when the console has been redrawn.
        /// </summary>
        public event EventHandler<HandledEventArgs> Invalidated;

        /// <summary>
        /// Keyboard processor shared by all Controls Consoles.
        /// </summary>
        public static Keyboard KeyboardState = new Keyboard();

        /// <summary>
        /// The collection of controls.
        /// </summary>
        [DataMember]
        protected List<ControlBase> ControlsList = new List<ControlBase>();

        [DataMember]
        private ControlBase _focusedControl;
        private bool _wasFocusedBeforeCapture;
        private bool _exclusiveBeforeCapture;
        private Themes.Colors _themeColors;

        #region Properties

        /// <summary>
        /// Gets or sets the colors to use with drawing the console and controls.
        /// </summary>
        public Colors ThemeColors
        {
            get => _themeColors;
            set
            {
                _themeColors = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets a read-only collection of the controls this console contains.
        /// </summary>
        public ReadOnlyCollection<ControlBase> Controls => ControlsList.AsReadOnly();

        /// <summary>
        /// Gets the control currently capturing mouse events.
        /// </summary>
        public ControlBase CapturedControl { get; private set; }

        /// <summary>
        /// Gets or sets the control that has keyboard focus.
        /// </summary>
        public ControlBase FocusedControl
        {
            get => _focusedControl;
            set
            {
                if (!DisableControlFocusing)
                {
                    if (FocusedControlChanging(value, _focusedControl))
                    {
                        ControlBase oldControl = _focusedControl;
                        _focusedControl = value;

                        FocusedControlChanged(_focusedControl, oldControl);
                    }
                }
            }
        }

        /// <summary>
        /// When true, allows the tab command to move to the next console (when there is a parent) instead of cycling back to the first control on this console.
        /// </summary>
        [DataMember]
        public bool CanTabToNextConsole { get; set; }

        /// <summary>
        /// Sets reference to the console to tab to when the <see cref="CanTabToNextConsole"/> property is true. Set this to null to allow the engine to determine the next console.
        /// </summary>
        public ControlsConsole NextTabConsole { get; set; }

        /// <summary>
        /// Sets reference to the console to tab to when the <see cref="CanTabToNextConsole"/> property is true. Set this to null to allow the engine to determine the next console.
        /// </summary>
        public ControlsConsole PreviousTabConsole { get; set; }

        /// <summary>
        /// When set to true, child controls are not alerted to (non-)focused states.
        /// </summary>
        public bool DisableControlFocusing { get; set; }
        #endregion

        /// <summary>
        /// Creates a new console.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        public ControlsConsole(int width, int height) : this(width, height, width, height, null) { }

        /// <summary>
        /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        /// <param name="initialCells">The initial cells to seed the surface.</param>
        public ControlsConsole(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells) { }

        /// <summary>
        /// Creates a new console with the specified width and height, with <see cref="SadRogue.Primitives.Color.Transparent"/> for the background and <see cref="SadRogue.Primitives.Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The visible width of the console in cells.</param>
        /// <param name="height">The visible height of the console in cells.</param>
        /// <param name="bufferWidth">The total width of the console in cells.</param>
        /// <param name="bufferHeight">The total height of the console in cells.</param>
        public ControlsConsole(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null) { }

        /// <summary>
        /// Creates a new console using the specified surface's cells.
        /// </summary>
        /// <param name="surface">The surface.</param>
        public ControlsConsole(ICellSurface surface) : this(surface.View.Width, surface.View.Height, surface.BufferWidth, surface.BufferHeight, surface.Cells) { }

        /// <summary>
        /// Creates a console with the specified width and height, with <see cref="SadRogue.Primitives.Color.Transparent"/> for the background and <see cref="SadRogue.Primitives.Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the console in cells.</param>
        /// <param name="height">The height of the console in cells.</param>
        /// <param name="bufferWidth">The total width of the console in cells.</param>
        /// <param name="bufferHeight">The total height of the console in cells.</param>
        /// <param name="initialCells">The cells to seed the console with. If <see langword="null"/>, creates the cells for you.</param>
        public ControlsConsole(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells)
        {
            Cursor.IsVisible = false;
            IsCursorDisabled = true;
            AutoCursorOnFocus = false;
            UseKeyboard = true;
            UseMouse = true;
            AutoCursorOnFocus = false;
            DisableControlFocusing = false;
        }

        /// <summary>
        /// Adds an existing control to this console.
        /// </summary>
        /// <param name="control">The control to add.</param>
        public void Add(ControlBase control)
        {
            if (!ControlsList.Contains(control))
            {
                ControlsList.Add(control);
            }

            control.Parent = this;
            control.TabIndex = ControlsList.Count - 1;

            IsDirty = true;

            ReOrderControls();
        }

        /// <summary>
        /// Removes a control from this console.
        /// </summary>
        /// <param name="control">The control to remove.</param>
        public void Remove(ControlBase control)
        {
            if (ControlsList.Contains(control))
            {
                control.TabIndex = -1;
                control.Parent = null;

                if (FocusedControl == control)
                {
                    int index = ControlsList.IndexOf(control);
                    ControlsList.Remove(control);

                    if (ControlsList.Count == 0)
                    {
                        FocusedControl = null;
                    }
                    else if (index > ControlsList.Count - 1)
                    {
                        FocusedControl = ControlsList[ControlsList.Count - 1];
                    }
                    else
                    {
                        FocusedControl = ControlsList[index];
                    }
                }
                else
                {
                    ControlsList.Remove(control);
                }

                IsDirty = true;

                ReOrderControls();
            }
        }

        /// <summary>
        /// Gives the focus to the next control in the tab order.
        /// </summary>
        public void TabNextControl()
        {
            if (ControlsList.Count == 0)
                return;

            ControlBase control;

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

            ControlBase control;

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


        private bool FindTabControlForward(int startingIndex, int endingIndex, out ControlBase foundControl)
        {
            for (int i = startingIndex; i <= endingIndex; i++)
            {
                if (ControlsList[i].TabStop)
                {
                    foundControl = ControlsList[i];
                    return true;
                }
            }

            foundControl = null;
            return false;
        }

        private bool FindTabControlPrevious(int startingIndex, int endingIndex, out ControlBase foundControl)
        {
            for (int i = startingIndex; i >= endingIndex; i--)
            {
                if (ControlsList[i].TabStop)
                {
                    foundControl = ControlsList[i];
                    return true;
                }
            }

            foundControl = null;
            return false;
        }

        /// <summary>
        /// Tries to tab to the console that comes before this one in the <see cref="IScreenObject.Parent"/> collection of <see cref="IScreenObject.Children"/>. Sets focus to the target console if found.
        /// </summary>
        /// <returns><see langword="true"/> if the tab was successful; otherwise, <see langword="false"/>.</returns>
        protected bool TryTabPreviousConsole()
        {
            if (!CanTabToNextConsole || Parent == null)
            {
                return false;
            }

            ControlsConsole newConsole;
            var consoles = Parent.Children.OfType<ControlsConsole>().ToList();

            // If no consoles found, get out
            if (consoles.Count == 0)
            {
                return false;
            }

            // If a previous console has not be explicitly set, find the previous console.
            if (PreviousTabConsole == null || !consoles.Contains(PreviousTabConsole))
            {
                int parentIndex = consoles.IndexOf(this);
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
            Global.FocusedScreenObjects.Set(newConsole);
            FocusedControl = null;
            newConsole.FocusedControl = null;
            newConsole.TabPreviousControl();

            return true;
        }

        /// <summary>
        /// Tries to tab to the console that comes after this one in the <see cref="IScreenObject.Parent"/> collection of <see cref="IScreenObject.Children"/>. Sets focus to the target console if found.
        /// </summary>
        /// <returns><see langword="true"/> if the tab was successful; otherwise, <see langword="false"/>.</returns>
        protected bool TryTabNextConsole()
        {
            if (!CanTabToNextConsole || Parent == null)
                return false;

            ControlsConsole newConsole;
            var consoles = Parent.Children.OfType<ControlsConsole>().ToList();

            // If no consoles found, get out
            if (consoles.Count == 0)
                return false;

            // If a previous console has not be explicitly set, find the previous console.
            if (NextTabConsole == null || !consoles.Contains(NextTabConsole))
            {
                int parentIndex = consoles.IndexOf(this);
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
            Global.FocusedScreenObjects.Set(newConsole);
            FocusedControl = null;
            newConsole.FocusedControl = null;
            newConsole.TabNextControl();

            return true;
        }

        /// <summary>
        /// Returns the colors assigned to this console or the library default.
        /// </summary>
        /// <returns>The found colors.</returns>
        public Colors GetThemeColors() =>
            _themeColors ?? Library.Default.Colors;

        /// <summary>
        /// Removes all controls from this console.
        /// </summary>
        public void RemoveAll()
        {
            FocusedControl = null;

            foreach (ControlBase control in ControlsList)
            {
                control.Parent = null;
            }

            ControlsList.Clear();
            IsDirty = true;
        }

        /// <summary>
        /// Checks if the specified control exists in this console.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <returns>True when the control exists in this console; otherwise false.</returns>
        public bool Contains(ControlBase control) => ControlsList.Contains(control);

        /// <summary>
        /// When overridden, allows you to prevent a control from taking focus from another control.
        /// </summary>
        /// <param name="newControl">The control requesting focus.</param>
        /// <param name="oldControl">The control that has focus.</param>
        /// <returns>True when the focus change is allowed; otherwise false.</returns>
        protected virtual bool FocusedControlChanging(ControlBase newControl, ControlBase oldControl) => newControl?.CanFocus ?? true;

        /// <summary>
        /// This method is called when a control gains focus.
        /// </summary>
        /// <param name="newControl">The control that has focus.</param>
        /// <param name="oldControl">The control that previously had focus.</param>
        protected virtual void FocusedControlChanged(ControlBase newControl, ControlBase oldControl)
        {
            oldControl?.FocusLost();
            newControl?.Focused();
        }

        /// <summary>
        /// Reorders the control collection based on the tab index of each control.
        /// </summary>
        public void ReOrderControls() => ControlsList.Sort((x, y) =>
                                       {
                                           if (x.TabIndex == y.TabIndex)
                                           {
                                               return 0;
                                           }

                                           if (x.TabIndex < y.TabIndex)
                                           {
                                               return -1;
                                           }

                                           return 1;
                                       });

        /// <summary>
        /// Called when the console is redrawn. Clears the console and allows custom drawing prior to control drawing.
        /// </summary>
        protected virtual void OnInvalidated()
        {
            if (RaiseInvalidated()) return;

            var colors = GetThemeColors();

            Surface.DefaultForeground = colors.ControlHostFore;
            Surface.DefaultBackground = colors.ControlHostBack;
            Surface.Clear();
        }

        /// <summary>
        /// Riases the <see cref="Invalidated"/> event.
        /// </summary>
        protected bool RaiseInvalidated()
        {
            var args = new HandledEventArgs();
            Invalidated?.Invoke(this, args);
            return args.IsHandled;
        }

        /// <summary>
        /// Calls the Update method of the base class and then Update on each control.
        /// </summary>
        /// <param name="delta">The time that has elapsed since this method was last called.</param>
        public override void Update(TimeSpan delta)
        {
            if (!IsEnabled) return;

            base.Update(delta);

            foreach (ControlBase control in ControlsList.ToArray())
            {
                if (control.IsDirty)
                    IsDirty = true;

                control.Update(delta);
            }

            if (IsDirty)
                OnInvalidated();
        }

        /// <inheritdoc />
        public override void Draw(TimeSpan delta)
        {
            if (!IsVisible) return;

            base.Draw(delta);
        }

        /// <summary>
        /// Processes the keyboard for the console.
        /// </summary>
        /// <param name="info">Keyboard information sent by the engine.</param>
        public override bool ProcessKeyboard(Keyboard info)
        {
            if (!UseKeyboard) return false;

            foreach (Components.IComponent component in ComponentsKeyboard.ToArray())
            {
                component.ProcessKeyboard(this, info, out bool isHandled);

                if (isHandled)
                    return true;
            }

            if (UseKeyboard)
            {
                if (
                    ((info.IsKeyDown(Keys.LeftShift) ||
                    info.IsKeyDown(Keys.RightShift)) ||

                    info.IsKeyReleased(Keys.LeftShift) ||
                    info.IsKeyReleased(Keys.RightShift))

                    &&
                    info.IsKeyReleased(Keys.Tab))
                {
                    TabPreviousControl();
                    return true;
                }

                if (info.IsKeyReleased(Keys.Tab))
                {
                    TabNextControl();
                    return false;
                }

                if (FocusedControl != null && FocusedControl.IsEnabled && FocusedControl.UseKeyboard)
                {
                    return FocusedControl.ProcessKeyboard(info);
                }
            }

            return false;
        }

        /// <summary>
        /// Processes the mouse for the console.
        /// </summary>
        /// <param name="state">Mouse information sent by the engine.</param>
        /// <returns>True when the mouse is over this console and it is the active console; otherwise false.</returns>
        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            if (!IsVisible) return false;

            if (base.ProcessMouse(state) || IsExclusiveMouse)
            {
                if (CapturedControl != null)
                {
                    CapturedControl.ProcessMouse(state);
                }
                else
                {
                    var controls = ControlsList.ToList();
                    foreach (ControlBase control in controls)
                    {
                        if (control.IsVisible && control.ProcessMouse(state))
                        {
                            break;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override void OnMouseExit(MouseScreenObjectState state)
        {
            base.OnMouseExit(state);

            foreach (ControlBase control in ControlsList)
            {
                control.LostMouse(state);
            }

            //if (_focusedControl != null)
            //_focusedControl.ProcessMouse(info);
        }

        /// <summary>
        /// Captures a control for exclusive mouse focus. Sets the ExclusiveMouse property to true.
        /// </summary>
        /// <param name="control">The control to capture</param>
        public void CaptureControl(ControlBase control)
        {
            if (Global.FocusedScreenObjects.ScreenObject != this)
            {
                Global.FocusedScreenObjects.Push(this);
                _wasFocusedBeforeCapture = false;
            }
            else
            {
                _wasFocusedBeforeCapture = true;
            }

            _exclusiveBeforeCapture = IsExclusiveMouse;
            IsExclusiveMouse = true;
            CapturedControl = control;
        }

        /// <summary>
        /// Releases the control from exclusive mouse focus. Sets the ExclusiveMouse property to false and sets the CapturedControl property to null.
        /// </summary>
        public void ReleaseControl()
        {
            if (!_wasFocusedBeforeCapture)
            {
                Global.FocusedScreenObjects.Pop(this);
            }

            IsExclusiveMouse = _exclusiveBeforeCapture;
            CapturedControl = null;
        }

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        public new IEnumerator<ControlBase> GetEnumerator() => ControlsList.GetEnumerator();

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ControlsList.GetEnumerator();

        /// <inheritdoc />
        public override void OnFocused()
        {
            base.OnFocused();

            FocusedControl?.DetermineState();
        }

        /// <inheritdoc />
        public override void OnFocusLost()
        {
            base.OnFocusLost();

            FocusedControl?.DetermineState();
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            Cursor.IsVisible = false;

            foreach (ControlBase control in ControlsList)
            {
                control.Parent = this;
            }

        }
    }
}
