using System;
using SadConsole.Input;

namespace SadConsole.Components
{
    /// <summary>
    /// A component that can be added to a <see cref="IScreenObject"/>.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="Update(IScreenObject, TimeSpan)"/> method.
        /// </summary>
        bool IsUpdate { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="Render(IScreenObject, TimeSpan)"/> method.
        /// </summary>
        bool IsRender { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessMouse(IScreenObject, MouseScreenObjectState, out bool)"/> method.
        /// </summary>
        bool IsMouse { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessKeyboard(IScreenObject, Keyboard, out bool)"/> method.
        /// </summary>
        bool IsKeyboard { get; }

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">The time that has elapsed from the last call to this component.</param>
        void Update(IScreenObject host, TimeSpan delta);

        /// <summary>
        /// Called by a host on the render frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">The time that has elapsed from the last call to this component.</param>
        void Render(IScreenObject host, TimeSpan delta);

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="host">The host console.</param>
        /// <param name="state">The mouse state.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled);

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="host">The host that added this component.</param>
        /// <param name="keyboard">The keyboard state.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled);

        /// <summary>
        /// Called when the component is added to a host.
        /// </summary>
        /// <param name="host">The host that added the component.</param>
        void OnAdded(IScreenObject host);

        /// <summary>
        /// Called when the component is removed from the host.
        /// </summary>
        /// <param name="host">The host that removed the component.</param>
        void OnRemoved(IScreenObject host);
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.Update(IScreenObject, TimeSpan)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class UpdateComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

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

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => true;

        bool IComponent.IsRender => false;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.Render(IScreenObject host, TimeSpan delta) { }

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
            handled = false;

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            handled = false;

        void IComponent.Update(IScreenObject host, TimeSpan delta) =>
            Update(host, delta);
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.Render(IScreenObject, TimeSpan)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class RenderComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">The time that has elapsed from the last call to this component.</param>
        public abstract void Render(IScreenObject host, TimeSpan delta);

        /// <inheritdoc />
        public virtual void OnAdded(IScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(IScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsRender => true;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.Render(IScreenObject host, TimeSpan delta) =>
            Render(host, delta);

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            handled = false;

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
            handled = false;

        void IComponent.Update(IScreenObject host, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.ProcessMouse(IScreenObject, MouseScreenObjectState, out bool)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class MouseConsoleComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="state">The state of the mouse in relation to the console.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(IScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(IScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsRender => false;

        bool IComponent.IsMouse => true;

        bool IComponent.IsKeyboard => false;

        void IComponent.Render(IScreenObject host, TimeSpan delta) { }

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard info, out bool handled) =>
            handled = false;

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            ProcessMouse(host, state, out handled);

        void IComponent.Update(IScreenObject host, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.ProcessKeyboard(IScreenObject, Keyboard, out bool)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class KeyboardConsoleComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="keyboard">The state of the keyboard.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(IScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(IScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsRender => false;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => true;

        void IComponent.Render(IScreenObject host, TimeSpan delta) { }

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
            ProcessKeyboard(host, keyboard, out handled);

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            handled = false;

        void IComponent.Update(IScreenObject host, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.ProcessMouse(IScreenObject, MouseScreenObjectState, out bool)"/> and <see cref="IComponent.ProcessKeyboard(IScreenObject, Keyboard, out bool)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class InputConsoleComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="keyboard">The state of the keyboard.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled);

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="state">The state of the mouse in relation to the console.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(IScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(IScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsRender => false;

        bool IComponent.IsMouse => true;

        bool IComponent.IsKeyboard => true;

        void IComponent.Render(IScreenObject host, TimeSpan delta) { }

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
            ProcessKeyboard(host, keyboard, out handled);

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState mouse, out bool handled) =>
            ProcessMouse(host, mouse, out handled);

        void IComponent.Update(IScreenObject host, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.Update(IScreenObject, TimeSpan)"/> and <see cref="IComponent.Render(IScreenObject, TimeSpan)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class LogicComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">The time that has elapsed from the last call to this component.</param>
        public abstract void Render(IScreenObject host, TimeSpan delta);

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">The time that has elapsed from the last call to this component.</param>
        public abstract void Update(IScreenObject host, TimeSpan delta);

        /// <inheritdoc />
        public virtual void OnAdded(IScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(IScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => true;

        bool IComponent.IsRender => true;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.Render(IScreenObject host, TimeSpan delta) =>
            Render(host, delta);

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
            handled = false;

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            handled = false;

        void IComponent.Update(IScreenObject host, TimeSpan delta) =>
            Update(host, delta);
    }

    /// <summary>
    /// A base class that implements all of <see cref="IComponent"/>.
    /// </summary>
    public abstract class ComponentBase : IComponent
    {
        /// <inheritdoc />
        public int SortOrder { get; set; }

        /// <inheritdoc />
        public bool IsUpdate => true;

        /// <inheritdoc />
        public bool IsRender => true;

        /// <inheritdoc />
        public bool IsMouse => true;

        /// <inheritdoc />
        public bool IsKeyboard => true;

        /// <inheritdoc />
        public abstract void Render(IScreenObject host, TimeSpan delta);

        /// <inheritdoc />
        public abstract void Update(IScreenObject host, TimeSpan delta);

        /// <inheritdoc />
        public abstract void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled);

        /// <inheritdoc />
        public abstract void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnRemoved(IScreenObject host) { }


        /// <inheritdoc />
        public virtual void OnAdded(IScreenObject host) { }
    }
}
