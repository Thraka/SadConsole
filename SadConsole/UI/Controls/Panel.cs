using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// A control container that processes the mouse on each child. <see cref="ControlBase.TabStop"/> defaults to <see langword="false"/>.
    /// </summary>
    [DataContract]
    public class Panel : ControlBase, IContainer
    {
        /// <summary>
        /// The controls this panel contains.
        /// </summary>
        protected List<ControlBase> Controls { get; set; } = new List<ControlBase>();

        /// <inheritdoc/>
        public ControlHost Host => Parent?.Host;

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

        /// <inheritdoc/>
        public void Add(ControlBase control)
        {
            Controls.Add(control);
            if (control.Parent != this)
                control.Parent = this;

            control.IsDirtyChanged += Control_IsDirtyChanged;
        }

        private void Control_IsDirtyChanged(object sender, EventArgs e)
        {
            if (((ControlBase)sender).IsDirty) IsDirty = true;
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

        /// <inheritdoc/>
        public bool Contains(ControlBase control) =>
            Controls.Contains(control);

        /// <inheritdoc/>
        public override void Update(TimeSpan time)
        {
            base.Update(time);

            foreach (ControlBase control in Controls.ToArray())
            {
                if (control.IsDirty)
                    IsDirty = true;

                control.Update(time);

                if (control.IsDirty)
                    IsDirty = true;
            }
        }

        /// <inheritdoc/>
        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            if (IsEnabled && UseMouse)
            {
                bool processResult = base.ProcessMouse(state);

                var controls = new List<ControlBase>(Controls);
                controls.Reverse();

                int count = controls.Count;
                for (int i = 0; i < count; i++)
                {
                    ControlBase control = controls[i];
                    var state2 = new ControlMouseState(control, state);

                    if (state2.IsMouseOver && state.Mouse.RightClicked)
                        System.Diagnostics.Debugger.Break();

                    if (control.ProcessMouse(state))
                        return true;
                }

                return processResult;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override void OnMouseExit(ControlMouseState state)
        {
            base.OnMouseExit(state);

            var controls = new List<ControlBase>(Controls);

            foreach (var control in controls)
                control.LostMouse(state.OriginalMouseState);
        }

        /// <inheritdoc/>
        public override bool ProcessKeyboard(Keyboard state)
        {
            if (IsEnabled && UseKeyboard)
            {
                bool processResult = base.ProcessKeyboard(state);

                var controls = new List<ControlBase>(Controls);
                controls.Reverse();

                foreach (var control in controls)
                    if (control.ProcessKeyboard(state))
                        return true;

                return processResult;
            }

            return false;
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
}
