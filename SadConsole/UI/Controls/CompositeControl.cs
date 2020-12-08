using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SadConsole.Input;
using System.Linq;
using SadRogue.Primitives;
using System.Collections;

namespace SadConsole.UI.Controls
{
    public abstract class CompositeControl : ControlBase, IContainer
    {
        /// <summary>
        /// The controls this composite control is hosting. Use <see cref="AddControl(ControlBase)"/> and <see cref="RemoveControl(ControlBase)"/> to manage the collection.
        /// </summary>
        protected List<ControlBase> Controls { get; set; } = new List<ControlBase>();

        public CompositeControl(int width, int height) : base(width, height)
        {
            CreateChildControls();
        }

        /// <summary>
        /// Create each control and add it to <see cref="Controls"/>.
        /// </summary>
        protected virtual void CreateChildControls() { }

        protected void IsDirtyEventHandler(bool value) =>
            IsDirty = value;

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            if (IsEnabled && UseMouse)
            {
                bool processResult = base.ProcessMouse(state);

                var controls = new List<ControlBase>(Controls);
                controls.Reverse();

                foreach (var control in controls)
                    if (control.ProcessMouse(state))
                        return true;

                return processResult;
            }

            return false;
        }

        /// <summary>
        /// Adds a child control to this control. <see cref="IContainer.Add(ControlBase)"/> does't work on the <see cref="CompositeControl"/>.
        /// </summary>
        /// <param name="control">The control to add.</param>
        protected void AddControl(ControlBase control)
        {
            if (!Controls.Contains(control))
            {
                Controls.Add(control);
                if (control.Parent != this)
                    control.Parent = this;
            }
        }

        /// <summary>
        /// Removes a child control from this control. <see cref="IContainer.Remove(ControlBase)"/> does't work on the <see cref="CompositeControl"/>.
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

        public override void Update(TimeSpan time)
        {
            base.Update(time);

            var controls = Controls.ToArray();

            foreach (var control in controls)
                control.Update(time);
        }

        Point IContainer.AbsolutePosition => this.AbsolutePosition;

        ControlHost IContainer.Host => this.Parent?.Host;

        void IContainer.Add(ControlBase control) =>
            AddControl(control);

        bool IContainer.Contains(ControlBase control) =>
            Controls.Contains(control);

        IEnumerator<ControlBase> IEnumerable<ControlBase>.GetEnumerator() =>
            Controls.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Controls.GetEnumerator();

        bool IContainer.Remove(ControlBase control)
        {
            return false;
        }

    }
}
