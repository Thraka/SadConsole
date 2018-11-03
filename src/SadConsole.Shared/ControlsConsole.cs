using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Keys = Microsoft.Xna.Framework.Input.Keys;

using SadConsole.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SadConsole.Renderers;
using SadConsole.Surfaces;
using SadConsole.Themes;

namespace SadConsole
{
    /// <summary>
    /// A basic console that can contain controls.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("Console (Controls)")]
    public class ControlsConsole: Console, IEnumerable<ControlBase>
    {
        /// <summary>
        /// Keyboard processor shared by all Controls Consoles.
        /// </summary>
        public static Input.Keyboard KeyboardState = new Input.Keyboard();

        [DataMember]
        protected List<ControlBase> _controls;

        [DataMember]
        private ControlBase _focusedControl;

        private ControlBase _capturedControl;

        private bool wasFocusedBeforeCapture;
        private bool exclusiveBeforeCapture;

        private SadConsole.Themes.ControlsConsoleTheme _theme;

        /// <summary>
        /// When set to false, uses the static <see cref="ControlsConsole.KeyboardState"/> keyboard instead of <see cref="Global.KeyboardState"/>
        /// </summary>
        protected bool UseGlobalKeyboardInput = false;

        #region Properties

        /// <summary>
        /// Gets or sets the theme of the window.
        /// </summary>
        public SadConsole.Themes.ControlsConsoleTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                IsDirty = true;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets a read-only collection of the controls this console contains.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<ControlBase> Controls => _controls.AsReadOnly();

        /// <summary>
        /// Gets the control currently capturing mouse events.
        /// </summary>
        public ControlBase CapturedControl => _capturedControl;

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

        #region Constructors
        /// <summary>
        ///  Creates a new instance of the controls console with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        /// <param name="device">The graphics device to render this console on.</param>
        public ControlsConsole(int width, int height)
            : base(width, height)
        {
            _controls = new List<ControlBase>();
            Cursor.IsVisible = false;
            AutoCursorOnFocus = false;
            UseKeyboard = true;
            UseMouse = true;
            AutoCursorOnFocus = false;
            DisableControlFocusing = false;
            Renderer = new Renderers.ControlsConsole();
            _theme = (ControlsConsoleTheme)Library.Default.ControlsConsoleTheme.Clone();
            Invalidate();
        }
        #endregion

        /// <summary>
        /// Adds an existing control to this console.
        /// </summary>
        /// <param name="control">The control to add.</param>
        public void Add(ControlBase control)
        {
            if (!_controls.Contains(control))
                _controls.Add(control);

            control.Parent = this;
            control.TabIndex = _controls.Count - 1;

            if (_controls.Count == 1)
                FocusedControl = control;

            control.IsDirtyChanged += ControlOnIsDirtyChanged;

            IsDirty = true;

            ReOrderControls();
        }

        private void ControlOnIsDirtyChanged(object sender, EventArgs e)
        {
            this.IsDirty = true;
        }


        /// <summary>
        /// Removes a control from this console.
        /// </summary>
        /// <param name="control">The control to remove.</param>
        public void Remove(ControlBase control)
        {
            if (_controls.Contains(control))
            {
                control.TabIndex = -1;
                control.Parent = null;

                if (FocusedControl == control)
                {
                    var index = _controls.IndexOf(control);
                    _controls.Remove(control);

                    if (_controls.Count == 0)
                        FocusedControl = null;
                    else if (index > _controls.Count - 1)
                        FocusedControl = _controls[_controls.Count - 1];
                    else
                        FocusedControl = _controls[index];
                }
                else
                    _controls.Remove(control);

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
                if (_controls.Count != 0)
                {
                    for (var i = 0; i < _controls.Count; i++)
                    {
                        if (_controls[i].TabStop)
                        {
                            FocusedControl = _controls[i];
                            break;
                        }
                    }

                    // Still couldn't find one, try moving previous console if we can
                    if (FocusedControl == null)
                        TryTabNextConsole();
                }
            }
            else
            {
                var index = _controls.IndexOf(_focusedControl);

                if (index == _controls.Count - 1 && !TryTabNextConsole())
                    FocusedControl = _controls[0];
                else
                    FocusedControl = _controls[index + 1];
            }
        }

        /// <summary>
        /// Gives focus to the previous control in the tab order.
        /// </summary>
        public void TabPreviousControl()
        {
            if (_focusedControl == null)
            {
                if (_controls.Count != 0)
                {
                    for (var i = _controls.Count - 1; i > 0; i--)
                    {
                        if (_controls[i].TabStop)
                        {
                            FocusedControl = _controls[i];
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
                var index = _controls.IndexOf(_focusedControl);

                if (index == 0 && !TryTabPreviousConsole())
                    FocusedControl = _controls[_controls.Count - 1];
                else
                    FocusedControl = _controls[index - 1];
            }
        }

        protected bool TryTabPreviousConsole()
        {
            if (CanTabToNextConsole && Parent != null)
            {
                ControlsConsole newConsole;
                var consoles = Parent.Children.OfType<ControlsConsole>().ToList();

                // If no consoles found, get out
                if (consoles.Count == 0)
                    return false;

                // If a previous console has not be explicitly set, find the previous console.
                else if (PreviousTabConsole == null || !consoles.Contains(PreviousTabConsole))
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

            return false;
        }

        protected bool TryTabNextConsole()
        {
            if (CanTabToNextConsole && Parent != null)
            {
                ControlsConsole newConsole;
                var consoles = Parent.Children.OfType<ControlsConsole>().ToList();

                // If no consoles found, get out
                if (consoles.Count == 0)
                    return false;

                // If a previous console has not be explicitly set, find the previous console.
                else if (NextTabConsole == null || !consoles.Contains(NextTabConsole))
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

            return false;
        }

        /// <summary>
        /// Removes all controls from this console.
        /// </summary>
        public void RemoveAll()
        {
            FocusedControl = null;

            foreach (var control in _controls)
                control.Parent = null;

            _controls.Clear();
        }

        /// <summary>
        /// Checks if the specified control exists in this console.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <returns>True when the control exists in this console; otherwise false.</returns>
        public bool Contains(ControlBase control)
        {
            return _controls.Contains(control);
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
            _controls.Sort((x, y) =>
            {
                if (x.TabIndex == y.TabIndex)
                    return 0;
                else if (x.TabIndex < y.TabIndex)
                    return -1;
                else
                    return 1;
            });
        }

        public virtual void Invalidate()
        {
            Theme.Draw(this, this);

            IsDirty = true;

            foreach (var control in _controls)
                control.IsDirty = true;
        }

        /// <summary>
        /// Calls the Update method of the base class and then Update on each control.
        /// </summary>
        public override void Update(System.TimeSpan time)
        {
            if (IsPaused) return;

            base.Update(time);

            foreach (var control in _controls)
                control.Update(time);

            
        }

        public override void Draw(System.TimeSpan update)
        {
            ((Renderers.ControlsConsole) Renderer).Controls = _controls;

            if (!IsDirty)
                foreach (var control in _controls)
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
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            if (!UseGlobalKeyboardInput)
            {
                KeyboardState.Update(Global.GameTimeUpdate);
                info = KeyboardState;
            }

            var handlerResult = KeyboardHandler == null ? false : KeyboardHandler(this, info);

            if (!handlerResult && this.UseKeyboard)
            {
                if (
                    ((info.IsKeyDown(Keys.LeftShift)  ||
                    info.IsKeyDown(Keys.RightShift)) || 
                        
                    info.IsKeyReleased(Keys.LeftShift)  ||
                    info.IsKeyReleased(Keys.RightShift)) 

                    &&
                    info.IsKeyReleased(Keys.Tab))
                {
                    // TODO: Handle tab by changing focused control unless existing control doesn't support tab
                    TabPreviousControl();
                    return true;
                }
                else if (info.IsKeyReleased(Keys.Tab))
                {
                    // TODO: Handle tab by changing focused control unless existing control doesn't support tab
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
        public override bool ProcessMouse(Input.MouseConsoleState state)
        {
            if (base.ProcessMouse(state) || IsExclusiveMouse)
            {
                if (_capturedControl != null)
                    _capturedControl.ProcessMouse(state);

                else
                {
                    foreach (var control in _controls)
                    {
                        if (control.IsVisible && control.ProcessMouse(state))
                            break;
                    }
                }

                return true;
            }

            return false;
        }

        protected override void OnMouseExit(Input.MouseConsoleState state)
        {
            base.OnMouseExit(state);

            foreach (var control in _controls)
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
                wasFocusedBeforeCapture = false;
            }
            else
            {
                wasFocusedBeforeCapture = true;
            }

            exclusiveBeforeCapture = IsExclusiveMouse;
            IsExclusiveMouse = true;
            _capturedControl = control;
        }

        /// <summary>
        /// Releases the control from exclusive mouse focus. Sets the ExclusiveMouse property to false and sets the CapturedControl property to null.
        /// </summary>
        public void ReleaseControl()
        {
            if (!wasFocusedBeforeCapture)
                Global.FocusedConsoles.Pop(this);

            IsExclusiveMouse = exclusiveBeforeCapture;
            _capturedControl = null;
        }

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        public IEnumerator<ControlBase> GetEnumerator()
        {
            return _controls.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _controls.GetEnumerator();
        }
        
        protected override void OnFocused()
        {
            base.OnFocused();

            FocusedControl?.DetermineState();
        }

        protected override void OnFocusLost()
        {
            base.OnFocusLost();

            FocusedControl?.DetermineState();
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            Cursor.IsVisible = false;

            foreach (var control in _controls)
            {
                control.Parent = this;
            }

        }
    }
}
