#if XNA
using Microsoft.Xna.Framework.Input;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SadConsole.Controls;
using SadConsole.Input;
using SadConsole.Themes;
using Keyboard = SadConsole.Input.Keyboard;

namespace SadConsole
{
    /// <summary>
    /// A basic console that can contain controls.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("Console (Controls)")]
    public class ControlsConsole : ScrollingConsole, IEnumerable<ControlBase>
    {
        /// <summary>
        /// Raised when the console has been redrawn.
        /// </summary>
        public event EventHandler Invalidated;

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
        [DataMember]
        private Themes.Colors _themeColors;

        private ControlsConsoleTheme _theme;

        /// <summary>
        /// When set to false, uses the static <see cref="ControlsConsole.KeyboardState"/> keyboard instead of <see cref="Global.KeyboardState"/>
        /// </summary>
        protected bool UseGlobalKeyboardInput = false;

        #region Properties

        /// <summary>
        /// The theme for the console. Defaults to <see cref="Library.ControlsConsoleTheme"/>.
        /// </summary>
        public  Themes.ControlsConsoleTheme Theme
        {
            get => _theme ?? Library.Default.ControlsConsoleTheme;
            set
            {
                _theme = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the colors to use with the <see cref="Theme"/>.
        /// </summary>
        public Colors ThemeColors
        {
            get => _themeColors;
            set { _themeColors = value; IsDirty = true; OnThemeColorsChanged(_themeColors); }
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
        ///  Creates a new instance of the controls console with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        public ControlsConsole(int width, int height) : this(width, height, Global.FontDefault) { }

        /// <summary>
        ///  Creates a new instance of the controls console with the specified width, height, and font.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        /// <param name="font">The font used with the console.</param>
        public ControlsConsole(int width, int height, Font font) : base(width, height, font)
        {
            Cursor.IsVisible = false;
            AutoCursorOnFocus = false;
            UseKeyboard = true;
            UseMouse = true;
            AutoCursorOnFocus = false;
            DisableControlFocusing = false;
            Renderer = new Renderers.ControlsConsole { Controls = ControlsList };
            // ReSharper disable once VirtualMemberCallInConstructor
            Invalidate();
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

            control.IsDirtyChanged += ControlOnIsDirtyChanged;

            IsDirty = true;

            ReOrderControls();
        }

        private void ControlOnIsDirtyChanged(object sender, EventArgs e) =>
                // TODO: This is ineffecient and causes the listbox to hold mouse over visual status. Fix below
                //if (sender is ControlBase control && control.IsDirty)
                IsDirty = true;

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

                //control.OnComposed = null;

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
        /// Tries to tab to the console that comes before this one in the <see cref="Console.Parent"/> collection of <see cref="Console.Children"/>. Sets focus to the target console if found.
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
            Global.FocusedConsoles.Set(newConsole);
            newConsole.FocusedControl = null;
            newConsole.TabPreviousControl();

            return true;
        }

        /// <summary>
        /// Tries to tab to the console that comes after this one in the <see cref="Console.Parent"/> collection of <see cref="Console.Children"/>. Sets focus to the target console if found.
        /// </summary>
        /// <returns><see langword="true"/> if the tab was successful; otherwise, <see langword="false"/>.</returns>
        protected bool TryTabNextConsole()
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
            if (NextTabConsole == null || !consoles.Contains(NextTabConsole))
            {
                int parentIndex = consoles.IndexOf(this);
                if (parentIndex == consoles.Count - 1)
                {
                    parentIndex = 0;
                }
                else
                {
                    parentIndex += 1;
                }

                // Get the new focused console
                newConsole = consoles[parentIndex];
            }
            else
            {
                newConsole = NextTabConsole;
            }

            // Set focus to this new console
            Global.FocusedConsoles.Set(newConsole);
            newConsole.FocusedControl = null;
            newConsole.TabNextControl();

            return true;
        }

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

        /// <inheritdoc />
        protected override void OnDirtyChanged()
        {
            if (IsDirty)
            {
                foreach (ControlBase control in ControlsList)
                    control.IsDirty = true;
            }

            base.OnDirtyChanged();
        }

        /// <summary>
        /// Signals that the console should be considered dirty and draws <see cref="Theme"/>, calls the customizable <see cref="Invalidate"/> method, then rasies the <see cref="Invalidated"/> event.
        /// </summary>
        protected virtual void Invalidate()
        {
            Theme.Draw(this, this);
            OnInvalidate();
            RaiseInvalidated();
        }

        /// <summary>
        /// Called by <see cref="OnInvalidate"/> as a way to customize drawing on teh console.
        /// </summary>
        protected virtual void OnInvalidate() { }

        /// <summary>
        /// Raises the <see cref="Invalidated"/> event.
        /// </summary>
        protected void RaiseInvalidated() =>
            Invalidated?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Calls the Update method of the base class and then Update on each control.
        /// </summary>
        public override void Update(TimeSpan time)
        {
            if (IsPaused)
            {
                return;
            }

            base.Update(time);

            foreach (ControlBase control in ControlsList)
            {
                control.Update(time);
            }
        }

        /// <inheritdoc />
        public override void Draw(TimeSpan update)
        {
            if (!IsDirty)
            {
                foreach (ControlBase control in ControlsList)
                {
                    if (control.IsDirty)
                    {
                        IsDirty = true;
                        break;
                    }
                }
            }

            if (IsDirty)
                Invalidate();

            base.Draw(update);
        }

        /// <summary>
        /// Processes the keyboard for the console.
        /// </summary>
        /// <param name="info">Keyboard information sent by the engine.</param>
        public override bool ProcessKeyboard(Keyboard info)
        {
            if (!UseGlobalKeyboardInput)
            {
                KeyboardState.Update(Global.GameTimeUpdate);
                info = KeyboardState;
            }

            foreach (Components.IConsoleComponent component in ComponentsKeyboard)
            {
                component.ProcessKeyboard(this, info, out bool isHandled);

                if (isHandled)
                {
                    return true;
                }
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
        public override bool ProcessMouse(MouseConsoleState state)
        {
            if (!IsVisible)
            {
                return false;
            }

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
        protected override void OnMouseExit(MouseConsoleState state)
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
            if (Global.FocusedConsoles.Console != this)
            {
                Global.FocusedConsoles.Push(this);
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
                Global.FocusedConsoles.Pop(this);
            }

            IsExclusiveMouse = _exclusiveBeforeCapture;
            CapturedControl = null;
        }

        /// <summary>
        /// Called when the <see cref="ThemeColors"/> property changes.
        /// </summary>
        /// <param name="themeColors">The new colors.</param>
        protected virtual void OnThemeColorsChanged(Colors themeColors) { }

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
