using System;
using SadConsole.Input;

namespace SadConsole.Components
{
    /// <summary>
    /// A base class that implements <see cref="IComponent.Update(IScreenObject, TimeSpan)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class UpdateComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public uint SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">The time that has elapsed since this method was last called.</param>
        public abstract void Update(IScreenObject host, TimeSpan delta);

        /// <inheritdoc />
        public virtual void OnAdded(IScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(IScreenObject host) { }

        uint IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => true;

        bool IComponent.IsRender => false;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.Render(IScreenObject host, TimeSpan delta) { }

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
            handled = false;

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            handled = false;
    }
}
