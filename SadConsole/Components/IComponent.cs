using System;
//using SadConsole.Input;

namespace SadConsole.Components
{
    /// <summary>
    /// A component that can be added to a <see cref="ScreenObject"/>.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="Update(ScreenObject, TimeSpan)"/> method.
        /// </summary>
        bool IsUpdate { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="Draw(ScreenObject, TimeSpan)"/> method.
        /// </summary>
        bool IsDraw { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessMouse(ScreenObject, MouseConsoleState, out bool)"/> method.
        /// </summary>
        bool IsMouse { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessKeyboard(ScreenObject, Keyboard, out bool)"/> method.
        /// </summary>
        bool IsKeyboard { get; }

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        void Update(ScreenObject host);

        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        void Draw(ScreenObject host);

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="host">The host console.</param>
        /// <param name="state">The mouse state.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        //void ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled);

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="host">The host console.</param>
        /// <param name="info">The keyboard state.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        //void ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled);

        /// <summary>
        /// Called when the component is added to a host.
        /// </summary>
        /// <param name="host">The host that added the component.</param>
        void OnAdded(ScreenObject host);

        /// <summary>
        /// Called when the component is removed from the host.
        /// </summary>
        /// <param name="host">The host that removed the component.</param>
        void OnRemoved(ScreenObject host);
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.Update(SadConsole.Console, TimeSpan)"/> of <see cref="IComponent"/>.
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
        /// <param name="delta">Time since the last call.</param>
        public abstract void Update(ScreenObject host);

        /// <inheritdoc />
        public virtual void OnAdded(ScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(ScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => true;

        bool IComponent.IsDraw => false;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.Draw(ScreenObject host) { }

        //void IComponent.ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled) => handled = false;

        //void IComponent.ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled) => handled = false;

        void IComponent.Update(ScreenObject host) =>
            Update(host);
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.Draw(Console, TimeSpan)"/> of <see cref="IComponent"/>.
    /// </summary>
    public abstract class DrawComponent : IComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Draw(ScreenObject host);

        /// <inheritdoc />
        public virtual void OnAdded(ScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(ScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsDraw => true;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.Draw(ScreenObject host) =>
            Draw(host);

        //void IComponent.ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled) => handled = false;

        //void IComponent.ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled) => handled = false;

        void IComponent.Update(ScreenObject host) { }
    }

    /*
    
    /// <summary>
    /// A base class that implements <see cref="IComponent.ProcessMouse(Console, MouseConsoleState, out bool)"/> of <see cref="IComponent"/>.
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
        public abstract void ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(ComponentHostObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(ComponentHostObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsDraw => false;

        bool IComponent.IsMouse => true;

        bool IComponent.IsKeyboard => false;

        void IComponent.Draw(ComponentHostObject host) { }

        void IComponent.ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled) => handled = false;

        void IComponent.ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled) => ProcessMouse(console, state, out handled);

        void IComponent.Update(ComponentHostObject host) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IComponent.ProcessKeyboard(Console, Keyboard, out bool)"/> of <see cref="IComponent"/>.
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
        /// <param name="info">The state of the keyboard.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(ComponentHostObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(ComponentHostObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsDraw => false;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => true;

        void IComponent.Draw(ComponentHostObject host) { }

        void IComponent.ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled) => ProcessKeyboard(console, info, out handled);

        void IComponent.ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled) => handled = false;

        void IComponent.Update(ComponentHostObject host) { }
    }


    /// <summary>
    /// A base class that implements <see cref="IComponent.ProcessMouse(Console, MouseConsoleState, out bool)"/> and <see cref="IComponent.ProcessKeyboard(Console, Keyboard, out bool)"/> of <see cref="IComponent"/>.
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
        /// <param name="info">The state of the keyboard.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled);

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="state">The state of the mouse in relation to the console.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(ComponentHostObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(ComponentHostObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsDraw => false;

        bool IComponent.IsMouse => true;

        bool IComponent.IsKeyboard => true;

        void IComponent.Draw(ComponentHostObject host) { }

        void IComponent.ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled) => ProcessKeyboard(console, info, out handled);

        void IComponent.ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled) => ProcessMouse(console, state, out handled);

        void IComponent.Update(ComponentHostObject host) { }
    }

    */

    /// <summary>
    /// A base class that implements <see cref="IComponent.Update(SadConsole.Console, TimeSpan)"/> and <see cref="IComponent.Draw(Console, TimeSpan)"/> of <see cref="IComponent"/>.
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
        /// <param name="delta">Time since the last call.</param>
        public abstract void Draw(ScreenObject host);

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="host">The host calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Update(ScreenObject host);

        /// <inheritdoc />
        public virtual void OnAdded(ScreenObject host) { }

        /// <inheritdoc />
        public virtual void OnRemoved(ScreenObject host) { }

        int IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => true;

        bool IComponent.IsDraw => true;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.Draw(ScreenObject host) =>
            Draw(host);

        //void IComponent.ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled) => handled = false;

        //void IComponent.ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled) => handled = false;

        void IComponent.Update(ScreenObject host) =>
            Update(host);
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
        public bool IsDraw => true;

        /// <inheritdoc />
        public bool IsMouse => true;

        /// <inheritdoc />
        public bool IsKeyboard => true;

        /// <inheritdoc />
        public abstract void Draw(ScreenObject host);

        /// <inheritdoc />
        public abstract void Update(ScreenObject host);

        /// <inheritdoc />
        //public abstract void ProcessKeyboard(ComponentHostObject host, Keyboard info, out bool handled);

        /// <inheritdoc />
        //public abstract void ProcessMouse(ComponentHostObject host, MouseConsoleState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnRemoved(ScreenObject host) { }


        /// <inheritdoc />
        public virtual void OnAdded(ScreenObject host) { }
    }
}
