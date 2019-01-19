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
    public class ControlsConsole: ScrollingConsole, IEnumerable<ControlBase>
    {
        /// <summary>
        /// Keyboard processor shared by all Controls Consoles.
        /// </summary>
        public static Keyboard KeyboardState = new Keyboard();

        /// <summary>
        /// The collection of controls.
        /// </summary>
        [DataMember]
        protected List<ControlBase> ControlsList;

        [DataMember]
        private ControlBase _focusedControl;
        private bool _wasFocusedBeforeCapture;
        private bool _exclusiveBeforeCapture;

        private Library _theme; 

        /// <summary>
        /// When set to false, uses the static <see cref="ControlsConsole.KeyboardState"/> keyboard instead of <see cref="Global.KeyboardState"/>
        /// </summary>
        protected bool UseGlobalKeyboardInput = false;

        #region Properties

        /// <summary>
        /// Gets or sets the theme of the window.
        /// </summary>
        public Library Theme
        {
            get => _theme ?? Library.Default;
            set
            {
                _theme = value ?? throw new ArgumentNullException(nameof(Theme), "Theme cannot be set to null.");

                foreach (var control in Controls)
                    control.RefreshParentTheme();

                IsDirty = true;
                Invalidate();
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
                        var oldControl = _focusedControl;
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
            ControlsList = new List<ControlBase>();
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
                ControlsList.Add(control);

            control.Parent = this;
            control.TabIndex = ControlsList.Count - 1;
            
            control.IsDirtyChanged += ControlOnIsDirtyChanged;

            IsDirty = true;

            ReOrderControls();
        }

        private void ControlOnIsDirtyChanged(object sender, EventArgs e)
        {
            IsDirty = true;
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
                    var index = ControlsList.IndexOf(control);
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
            if (_focusedControl == null)
            {
                if (ControlsList.Count == 0) return;

                foreach (var control in ControlsList)
                {
                    if (control.TabStop)
                    {
                        FocusedControl = control;
                        break;
                    }
                }

                // Still couldn't find one, try moving previous console if we can
                if (FocusedControl == null)
                    TryTabNextConsole();
            }
            else
            {
                var index = ControlsList.IndexOf(_focusedControl);

                if (index == ControlsList.Count - 1 && !TryTabNextConsole())
                    FocusedControl = ControlsList[0];
                else
                    FocusedControl = ControlsList[index + 1];
            }
        }

        /// <summary>
        /// Gives focus to the previous control in the tab order.
        /// </summary>
        public void TabPreviousControl()
        {
            if (_focusedControl == null)
            {
                if (ControlsList.Count != 0)
                {
                    for (var i = ControlsList.Count - 1; i > 0; i--)
                    {
                        if (ControlsList[i].TabStop)
                        {
                            FocusedControl = ControlsList[i];
                            break;
                        }
                    }

                    // Still couldn't find one, try moving previous console if we can
                    if (FocusedControl == null)
                        TryTabPreviousConsole();
                }
            }
            else
            {
                var index = ControlsList.IndexOf(_focusedControl);

                if (index == 0 && !TryTabPreviousConsole())
                    FocusedControl = ControlsList[ControlsList.Count - 1];
                else
                    FocusedControl = ControlsList[index - 1];
            }
        }

        /// <summary>
        /// Tries to tab to the console that comes before this one in the <see cref="Console.Parent"/> collection of <see cref="Console.Children"/>. Sets focus to the target console if found.
        /// </summary>
        /// <returns><see langword="true"/> if the tab was successful; otherwise, <see langword="false"/>.</returns>
        protected bool TryTabPreviousConsole()
        {
            if (!CanTabToNextConsole || Parent == null) return false;

            ControlsConsole newConsole;
            var consoles = Parent.Children.OfType<ControlsConsole>().ToList();

            // If no consoles found, get out
            if (consoles.Count == 0)
                return false;

            // If a previous console has not be explicitly set, find the previous console.
            if (PreviousTabConsole == null || !consoles.Contains(PreviousTabConsole))
            {
                var parentIndex = consoles.IndexOf(this);
                if (parentIndex == 0)
                    parentIndex = consoles.Count - 1;
                else
                    parentIndex -= 1;

                // Get the new focused console
                newConsole = consoles[parentIndex];
            }
            else
                newConsole = PreviousTabConsole;

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
            if (!CanTabToNextConsole || Parent == null) return false;

            ControlsConsole newConsole;
            var consoles = Parent.Children.OfType<ControlsConsole>().ToList();

            // If no consoles found, get out
            if (consoles.Count == 0)
                return false;

            // If a previous console has not be explicitly set, find the previous console.
            if (NextTabConsole == null || !consoles.Contains(NextTabConsole))
            {
                var parentIndex = consoles.IndexOf(this);
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

            foreach (var control in ControlsList)
                control.Parent = null;

            ControlsList.Clear();
        }

        /// <summary>
        /// Checks if the specified control exists in this console.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <returns>True when the control exists in this console; otherwise false.</returns>
        public bool Contains(ControlBase control)
        {
            return ControlsList.Contains(control);
        }

        /// <summary>
        /// When overridden, allows you to prevent a control from taking focus from another control.
        /// </summary>
        /// <param name="newControl">The control requesting focus.</param>
        /// <param name="oldControl">The control that has focus.</param>
        /// <returns>True when the focus change is allowed; otherwise false.</returns>
        protected virtual bool FocusedControlChanging(ControlBase newControl, ControlBase oldControl)
        {
            return true;
        }

        /// <summary>
        /// This method is called when a control gains focus. Unless overridden, this method calls the DetermineAppearance method both the <paramref name="newControl"/> and <paramref name="oldControl"/> parameters.
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
        public void ReOrderControls()
        {
            ControlsList.Sort((x, y) =>
            {
                if (x.TabIndex == y.TabIndex)
                    return 0;
                if (x.TabIndex < y.TabIndex)
                    return -1;
                return 1;
            });
        }

        /// <summary>
        /// Signals that the console should be considered dirty and reapplies the <see cref="Theme"/>.
        /// </summary>
        public virtual void Invalidate()
        {
            Theme.ControlsConsoleTheme.Refresh(Theme.Colors);
            Theme.ControlsConsoleTheme.Draw(this, this);

            IsDirty = true;

            foreach (var control in ControlsList)
                control.IsDirty = true;
        }

        /// <summary>
        /// Calls the Update method of the base class and then Update on each control.
        /// </summary>
        public override void Update(TimeSpan time)
        {
            if (IsPaused) return;

            base.Update(time);

            foreach (var control in ControlsList)
                control.Update(time);
        }

        /// <inheritdoc />
        public override void Draw(TimeSpan update)
        {
            if (!IsDirty)
                foreach (var control in ControlsList)
                {
                    if (control.IsDirty)
                    {
                        IsDirty = true;
                        break;
                    }
                }

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

            foreach (var component in ComponentsKeyboard)
            {
                component.ProcessKeyboard(this, info, out bool isHandled);

                if (isHandled) return true;
            }

            if (UseKeyboard)
            {
                if (
                    ((info.IsKeyDown(Keys.LeftShift)  ||
                    info.IsKeyDown(Keys.RightShift)) || 
                        
                    info.IsKeyReleased(Keys.LeftShift)  ||
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
                    return FocusedControl.ProcessKeyboard(info);
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
            if (base.ProcessMouse(state) || IsExclusiveMouse)
            {
                if (CapturedControl != null)
                    CapturedControl.ProcessMouse(state);

                else
                {
                    foreach (var control in ControlsList)
                    {
                        if (control.IsVisible && control.ProcessMouse(state))
                            break;
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

            foreach (var control in ControlsList)
                control.LostMouse(state);

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
                Global.FocusedConsoles.Pop(this);

            IsExclusiveMouse = _exclusiveBeforeCapture;
            CapturedControl = null;
        }

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        public new IEnumerator<ControlBase> GetEnumerator()
        {
            return ControlsList.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ControlsList.GetEnumerator();
        }

        /// <inheritdoc />
        protected override void OnFocused()
        {
            base.OnFocused();

            FocusedControl?.DetermineState();
        }

        /// <inheritdoc />
        protected override void OnFocusLost()
        {
            base.OnFocusLost();

            FocusedControl?.DetermineState();
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            Cursor.IsVisible = false;

            foreach (var control in ControlsList)
            {
                control.Parent = this;
            }

        }
    }
}
