#if SFML
using Point = SFML.System.Vector2i;
using Keys = SFML.Window.Keyboard.Key;
using Rectangle = SFML.Graphics.IntRect;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Keys = Microsoft.Xna.Framework.Input.Keys;
#endif

using SadConsole.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SadConsole.Consoles
{
    /// <summary>
    /// A basic console that can contain controls.
    /// </summary>
    [DataContract]
    public class ControlsConsole: Console, IEnumerable<ControlBase>
    {
        public static Input.KeyboardInfo KeyboardState = new Input.KeyboardInfo();

        [DataMember]
        private List<ControlBase> _controls;
        [DataMember]
        private ControlBase _focusedControl;
        [DataMember]
        private ControlBase _capturedControl;
        private bool _exlusiveBeforeCapture;
        private IConsole _activeBeforeCapture;

        private SadConsole.Themes.ControlsConsoleTheme _theme;

        /// <summary>
        /// When set to false, when the ControlsConsole has <see cref="ProcessKeyboard(Input.KeyboardInfo)"/> called, <see cref="KeyboardState"/> will not be updated.
        /// </summary>
        protected bool updateKeyboardState = true;

        #region Properties

        /// <summary>
        /// Gets or sets the theme of the window.
        /// </summary>
        public SadConsole.Themes.ControlsConsoleTheme Theme
        {
            get
            {
                if (_theme == null)
                    return SadConsole.Themes.Library.Default.ControlsConsoleTheme;
                else
                    return _theme;
            }
            set { _theme = value; Invalidate(); }
        }

        /// <summary>
        /// When true, mouse events over this console will be processed even if this console is not active.
        /// </summary>
        [DataMember]
        public bool ProcessMouseWithoutFocus { get; set; }

        /// <summary>
        /// Gets a read-only collection of the controls this console contains.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<ControlBase> Controls
        {
            get { return _controls.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the control currently capturing mouse events.
        /// </summary>
        public ControlBase CapturedControl
        {
            get { return _capturedControl; }
        }

        /// <summary>
        /// Gets or sets the control that has keyboard focus.
        /// </summary>
        public ControlBase FocusedControl
        {
            get { return _focusedControl; }
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
        public IConsole NextTabConsole { get; set; }

        /// <summary>
        /// Sets reference to the console to tab to when the <see cref="CanTabToNextConsole"/> property is true. Set this to null to allow the engine to determine the next console.
        /// </summary>
        public IConsole PreviousTabConsole { get; set; }

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

            VirtualCursor.IsVisible = false;
            AutoCursorOnFocus = false;
            CanUseKeyboard = true;
            CanUseMouse = true;
            MouseCanFocus = true;
            AutoCursorOnFocus = false;
            DisableControlFocusing = false;
            Renderer = new ControlsConsoleRenderer();
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

            ReOrderControls();
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
                    int index = _controls.IndexOf(control);
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
                    FocusedControl = _controls[0];
                }
            }
            else
            {
                int index = _controls.IndexOf(_focusedControl);

                if (index == _controls.Count - 1)
                {
                    // Check to see if we should move to the next console
                    if (CanTabToNextConsole && Parent != null)
                    {
                        IConsole newConsole;

                        // If a next console has not be explicitly set, find the next console.
                        if (NextTabConsole == null || !Parent.Contains(NextTabConsole))
                        {
                            var parentIndex = Parent.IndexOf(this);
                            if (parentIndex == Parent.Count - 1)
                                parentIndex = 0;
                            else
                                parentIndex += 1;

                            // Get the new focused console
                            newConsole = Parent[parentIndex];
                        }
                        else
                            newConsole = NextTabConsole;

                        // If it's a controls console, set the focused control to the first control (if available)
                        if (newConsole is ControlsConsole)
                        {
                            // Set focus to this new console
                            Engine.ActiveConsole = newConsole;

                            var controlConsole = (ControlsConsole)newConsole;
                            if (controlConsole.Controls.Count > 0)
                                ((ControlsConsole)newConsole).FocusedControl = ((ControlsConsole)newConsole).Controls[0];
                        }
                        else
                            FocusedControl = _controls[0];
                    }
                    else
                        FocusedControl = _controls[0];
                }
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
                    FocusedControl = _controls[0];
                }
            }
            else
            {
                int index = _controls.IndexOf(_focusedControl);

                if (index == 0)
                {
                    // Check to see if we should move to the next console
                    if (CanTabToNextConsole && Parent != null)
                    {
                        IConsole newConsole;

                        // If a next console has not be explicitly set, find the previous console.
                        if (PreviousTabConsole == null || !Parent.Contains(PreviousTabConsole))
                        {
                            var parentIndex = Parent.IndexOf(this);
                            if (parentIndex == 0)
                                parentIndex = Parent.Count - 1;
                            else
                                parentIndex -= 1;

                            // Get the new focused console
                            newConsole = Parent[parentIndex];
                        }
                        else
                            newConsole = PreviousTabConsole;

                        // If it's a controls console, set the focused control to the last control (if available)
                        if (newConsole is ControlsConsole)
                        {
                            // Set focus to this new console
                            Engine.ActiveConsole = newConsole;

                            var controlConsole = (ControlsConsole)newConsole;
                            if (controlConsole.Controls.Count > 0)
                                controlConsole.FocusedControl = controlConsole.Controls[controlConsole.Controls.Count - 1];
                        }
                        else
                            FocusedControl = _controls[_controls.Count - 1];
                        
                    }
                    else
                        FocusedControl = _controls[_controls.Count - 1];
                }
                else
                    FocusedControl = _controls[index - 1];
            }
        }

        /// <summary>
        /// Removes all controls from this console.
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Parent = null;
            }

            _controls.Clear();
            FocusedControl = null;
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
            if (oldControl != null)
                oldControl.FocusLost();

            if (newControl != null)
                newControl.Focused();
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
            textSurface.DefaultForeground = Theme.FillStyle.Foreground;
            textSurface.DefaultBackground = Theme.FillStyle.Background;
            Fill(textSurface.DefaultForeground, textSurface.DefaultBackground, Theme.FillStyle.GlyphIndex, null);
        }

        public override void Render()
        {
            ((ControlsConsoleRenderer)_renderer).Controls = _controls;

            base.Render();
        }

        /// <summary>
        /// Processes the keyboard for the console.
        /// </summary>
        /// <param name="info">Keyboard information sent by the engine.</param>
        public override bool ProcessKeyboard(Input.KeyboardInfo info)
        {
            info = KeyboardState;

            if (updateKeyboardState)
                info.ProcessKeys(Engine.GameTimeUpdate);

            var handlerResult = KeyboardHandler == null ? false : KeyboardHandler(this, info);

            if (!handlerResult && this.CanUseKeyboard)
            {
                bool canTab = true;

                if (FocusedControl != null)
                    canTab = FocusedControl.TabStop;

                if (canTab)
                    if (
#if SFML
                        ((info.IsKeyDown(Keys.LShift) ||
                        info.IsKeyDown(Keys.RShift)) ||

                        info.IsKeyReleased(Keys.LShift) ||
                        info.IsKeyReleased(Keys.RShift))
#elif MONOGAME
                        ((info.IsKeyDown(Keys.LeftShift)  ||
                        info.IsKeyDown(Keys.RightShift)) || 
                        
                        info.IsKeyReleased(Keys.LeftShift)  ||
                        info.IsKeyReleased(Keys.RightShift)) 
#endif
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

                if (FocusedControl != null && FocusedControl.IsEnabled && FocusedControl.CanUseKeyboard)
                    return FocusedControl.ProcessKeyboard(info);
            }

            return false;
        }

        /// <summary>
        /// Processes the mouse for the console.
        /// </summary>
        /// <param name="info">Mouse information sent by the engine.</param>
        /// <returns>True when the mouse is over this console and it is the active console; otherwise false.</returns>
        public override bool ProcessMouse(Input.MouseInfo info)
        {
            if (base.ProcessMouse(info) && info.Console == this && (Engine.ActiveConsole == this || ProcessMouseWithoutFocus))
            {
                if (_capturedControl != null)
                    _capturedControl.ProcessMouse(info);

                else
                {
                    for (int i = 0; i < _controls.Count; i++)
                    {
                        if (_controls[i].IsVisible && _controls[i].ProcessMouse(info))
                            break;
                    }
                }

                return true;
            }

            return false;
        }

        protected override void OnMouseExit(Input.MouseInfo info)
        {
            base.OnMouseExit(info);

            for (int i = 0; i < _controls.Count; i++)
            {
                if (_controls[i].IsVisible && _controls[i].ProcessMouse(info))
                    break;
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
            _activeBeforeCapture = Engine.ActiveConsole;
            Engine.ActiveConsole = this;
            _exlusiveBeforeCapture = ExclusiveFocus;
            ExclusiveFocus = true;
            _capturedControl = control;
        }

        /// <summary>
        /// Releases the control from exclusive mouse focus. Sets the ExclusiveMouse property to false and sets the CapturedControl property to null.
        /// </summary>
        public void ReleaseControl()
        {
            Engine.ActiveConsole = _activeBeforeCapture;
            ExclusiveFocus = _exlusiveBeforeCapture;
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

        /// <summary>
        /// Calls the Update method of the base class and then Update on each control.
        /// </summary>
        public override void Update()
        {
            base.Update();

            for (int i = 0; i < _controls.Count; i++)
                _controls[i].Update();
        }

        protected override void OnFocused()
        {
            base.OnFocused();

            if (FocusedControl != null)
                FocusedControl.DetermineAppearance();
        }

        protected override void OnFocusLost()
        {
            base.OnFocusLost();

            if (FocusedControl != null)
                FocusedControl.DetermineAppearance();
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            _virtualCursor.IsVisible = false;

            foreach (var control in _controls)
            {
                control.Parent = this;
            }

        }
    }
}
