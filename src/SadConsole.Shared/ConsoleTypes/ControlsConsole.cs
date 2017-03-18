using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Keys = Microsoft.Xna.Framework.Input.Keys;

using SadConsole.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SadConsole.Renderers;

namespace SadConsole
{
    /// <summary>
    /// A basic console that can contain controls.
    /// </summary>
    [DataContract]
    public class ControlsConsole: Console, IEnumerable<ControlBase>
    {
        /// <summary>
        /// Keyboard processor shared by all Controls Consoles.
        /// </summary>
        public static Input.Keyboard KeyboardState = new Input.Keyboard();

        [DataMember]
        private List<ControlBase> controls;

        [DataMember]
        private ControlBase focusedControl;

        private ControlBase capturedControl;

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
        /// Gets a read-only collection of the controls this console contains.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<ControlBase> Controls
        {
            get { return controls.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the control currently capturing mouse events.
        /// </summary>
        public ControlBase CapturedControl
        {
            get { return capturedControl; }
        }

        /// <summary>
        /// Gets or sets the control that has keyboard focus.
        /// </summary>
        public ControlBase FocusedControl
        {
            get { return focusedControl; }
            set
            {
                if (!DisableControlFocusing)
                {
                    if (FocusedControlChanging(value, focusedControl))
                    {
                        var oldControl = focusedControl;
                        focusedControl = value;

                        FocusedControlChanged(focusedControl, oldControl);
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
            controls = new List<ControlBase>();

            VirtualCursor.IsVisible = false;
            AutoCursorOnFocus = false;
            UseKeyboard = true;
            UseMouse = true;
            AutoCursorOnFocus = false;
            DisableControlFocusing = false;
            Renderer = new ControlsConsoleRenderer();
            Invalidate();
        }
        #endregion

        /// <summary>
        /// Marks the text surface as dirty when a control changes appearance.
        /// </summary>
        /// <param name="control">The control.</param>
        protected void ControlChanged(ControlBase control) => textSurface.IsDirty = true;


        /// <summary>
        /// Adds an existing control to this console.
        /// </summary>
        /// <param name="control">The control to add.</param>
        public void Add(ControlBase control)
        {
            if (!controls.Contains(control))
                controls.Add(control);

            control.Parent = this;
            control.TabIndex = controls.Count - 1;

            if (controls.Count == 1)
                FocusedControl = control;

            control.OnComposed = ControlChanged;
            textSurface.IsDirty = true;

            ReOrderControls();
        }


        /// <summary>
        /// Removes a control from this console.
        /// </summary>
        /// <param name="control">The control to remove.</param>
        public void Remove(ControlBase control)
        {
            if (controls.Contains(control))
            {
                control.TabIndex = -1;
                control.Parent = null;

                if (FocusedControl == control)
                {
                    int index = controls.IndexOf(control);
                    controls.Remove(control);

                    if (controls.Count == 0)
                        FocusedControl = null;
                    else if (index > controls.Count - 1)
                        FocusedControl = controls[controls.Count - 1];
                    else
                        FocusedControl = controls[index];
                }
                else
                    controls.Remove(control);

                control.OnComposed = null;

                textSurface.IsDirty = true;

                ReOrderControls();
            }
        }

        /// <summary>
        /// Gives the focus to the next control in the tab order.
        /// </summary>
        public void TabNextControl()
        {
            if (focusedControl == null)
            {
                if (controls.Count != 0)
                {
                    FocusedControl = controls[0];
                }
            }
            else
            {
                int index = controls.IndexOf(focusedControl);

                if (index == controls.Count - 1)
                {
                    // Check to see if we should move to the next console
                    if (CanTabToNextConsole && Parent != null)
                    {
                        IConsole newConsole;
                        var consoles = Parent.Children.OfType<IConsole>().ToList();

                        // If a next console has not be explicitly set, find the next console.
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

                        // If it's a controls console, set the focused control to the first control (if available)
                        if (newConsole is ControlsConsole && Global.FocusedConsoles.Console == this)
                        {
                            // Set focus to this new console
                            Global.FocusedConsoles.Set(newConsole);

                            var controlConsole = (ControlsConsole)newConsole;
                            if (controlConsole.Controls.Count > 0)
                                ((ControlsConsole)newConsole).FocusedControl = ((ControlsConsole)newConsole).Controls[0];
                        }
                        else
                            FocusedControl = controls[0];
                    }
                    else
                        FocusedControl = controls[0];
                }
                else
                    FocusedControl = controls[index + 1];

                
            }
        }

        /// <summary>
        /// Gives focus to the previous control in the tab order.
        /// </summary>
        public void TabPreviousControl()
        {
            if (focusedControl == null)
            {
                if (controls.Count != 0)
                {
                    FocusedControl = controls[0];
                }
            }
            else
            {
                int index = controls.IndexOf(focusedControl);

                if (index == 0)
                {
                    // Check to see if we should move to the next console
                    if (CanTabToNextConsole && Parent != null)
                    {
                        IConsole newConsole;
                        var consoles = Parent.Children.OfType<IConsole>().ToList();

                        // If a next console has not be explicitly set, find the previous console.
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

                        // If it's a controls console, set the focused control to the last control (if available)
                        if (newConsole is ControlsConsole)
                        {
                            // Set focus to this new console
                            Global.FocusedConsoles.Set(newConsole);

                            var controlConsole = (ControlsConsole)newConsole;
                            if (controlConsole.Controls.Count > 0)
                                controlConsole.FocusedControl = controlConsole.Controls[controlConsole.Controls.Count - 1];
                        }
                        else
                            FocusedControl = controls[controls.Count - 1];
                        
                    }
                    else
                        FocusedControl = controls[controls.Count - 1];
                }
                else
                    FocusedControl = controls[index - 1];
            }
        }

        /// <summary>
        /// Removes all controls from this console.
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Parent = null;
            }

            controls.Clear();
            FocusedControl = null;
        }

        /// <summary>
        /// Checks if the specified control exists in this console.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <returns>True when the control exists in this console; otherwise false.</returns>
        public bool Contains(ControlBase control)
        {
            return controls.Contains(control);
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
            controls.Sort((x, y) =>
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
            Fill(textSurface.DefaultForeground, textSurface.DefaultBackground, Theme.FillStyle.Glyph, null);
            textSurface.IsDirty = true;
        }

        public override void Draw(System.TimeSpan update)
        {
            ((ControlsConsoleRenderer)_renderer).Controls = controls;

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
                bool canTab = true;

                if (FocusedControl != null)
                    canTab = FocusedControl.TabStop;

                if (canTab)
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
            if (base.ProcessMouse(state))
            {
                if (capturedControl != null)
                    capturedControl.ProcessMouse(state);

                else
                {
                    for (int i = 0; i < controls.Count; i++)
                    {
                        if (controls[i].IsVisible && controls[i].ProcessMouse(state))
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

            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].LostMouse(state);
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
                wasFocusedBeforeCapture = false;
            }
            else
            {
                wasFocusedBeforeCapture = true;
            }

            exclusiveBeforeCapture = IsExclusiveMouse;
            IsExclusiveMouse = true;
            capturedControl = control;
        }

        /// <summary>
        /// Releases the control from exclusive mouse focus. Sets the ExclusiveMouse property to false and sets the CapturedControl property to null.
        /// </summary>
        public void ReleaseControl()
        {
            if (!wasFocusedBeforeCapture)
                Global.FocusedConsoles.Pop(this);

            IsExclusiveMouse = exclusiveBeforeCapture;
            capturedControl = null;
        }

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        public IEnumerator<ControlBase> GetEnumerator()
        {
            return controls.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator of the controls collection.
        /// </summary>
        /// <returns>The enumerator of the controls collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return controls.GetEnumerator();
        }

        /// <summary>
        /// Calls the Update method of the base class and then Update on each control.
        /// </summary>
        public override void Update(System.TimeSpan time)
        {
            base.Update(time);

            for (int i = 0; i < controls.Count; i++)
                controls[i].Update();
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
            virtualCursor.IsVisible = false;

            foreach (var control in controls)
            {
                control.Parent = this;
            }

        }
    }
}
