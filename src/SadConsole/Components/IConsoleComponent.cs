using System;
using SadConsole.Input;

namespace SadConsole.Components
{
    /// <summary>
    /// A component that can be added to a <see cref="Console"/>.
    /// </summary>
    public interface IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="Update(Console, TimeSpan)"/> method.
        /// </summary>
        bool IsUpdate { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="Draw(Console, TimeSpan)"/> method.
        /// </summary>
        bool IsDraw { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessMouse(Console, MouseConsoleState, out bool)"/> method.
        /// </summary>
        bool IsMouse { get; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this component calls the <see cref="ProcessKeyboard(Console, Keyboard, out bool)"/> method.
        /// </summary>
        bool IsKeyboard { get; }

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        void Update(Console console, TimeSpan delta);

        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        void Draw(Console console, TimeSpan delta);

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="console">The host console.</param>
        /// <param name="state">The mouse state.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        void ProcessMouse(Console console, MouseConsoleState state, out bool handled);

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="console">The host console.</param>
        /// <param name="info">The keyboard state.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        void ProcessKeyboard(Console console, Keyboard info, out bool handled);

        /// <summary>
        /// Called when the component is added to a host.
        /// </summary>
        /// <param name="console">The host that added the component.</param>
        void OnAdded(Console console);

        /// <summary>
        /// Called when the component is removed from the host.
        /// </summary>
        /// <param name="console">The host that removed the component.</param>
        void OnRemoved(Console console);
    }

    /// <summary>
    /// A base class that implements <see cref="IConsoleComponent.Update(SadConsole.Console, TimeSpan)"/> of <see cref="IConsoleComponent"/>.
    /// </summary>
    public abstract class UpdateConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Update(Console console, TimeSpan delta);

        /// <inheritdoc />
        public virtual void OnAdded(Console console) { }

        /// <inheritdoc />
        public virtual void OnRemoved(Console console) { }

        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => true;

        bool IConsoleComponent.IsDraw => false;

        bool IConsoleComponent.IsMouse => false;

        bool IConsoleComponent.IsKeyboard => false;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) { }

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool handled) => handled = false;

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool handled) => handled = false;

        void IConsoleComponent.Update(Console console, TimeSpan delta) => Update(console, delta);
    }

    /// <summary>
    /// A base class that implements <see cref="IConsoleComponent.Draw(Console, TimeSpan)"/> of <see cref="IConsoleComponent"/>.
    /// </summary>
    public abstract class DrawConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Draw(Console console, TimeSpan delta);

        /// <inheritdoc />
        public virtual void OnAdded(Console console) { }

        /// <inheritdoc />
        public virtual void OnRemoved(Console console) { }

        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => false;

        bool IConsoleComponent.IsDraw => true;

        bool IConsoleComponent.IsMouse => false;

        bool IConsoleComponent.IsKeyboard => false;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) => Draw(console, delta);

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool handled) => handled = false;

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool handled) => handled = false;

        void IConsoleComponent.Update(Console console, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IConsoleComponent.ProcessMouse(Console, MouseConsoleState, out bool)"/> of <see cref="IConsoleComponent"/>.
    /// </summary>
    public abstract class MouseConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="state">The state of the mouse in relation to the console.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessMouse(Console console, MouseConsoleState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(Console console) { }

        /// <inheritdoc />
        public virtual void OnRemoved(Console console) { }

        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => false;

        bool IConsoleComponent.IsDraw => false;

        bool IConsoleComponent.IsMouse => true;

        bool IConsoleComponent.IsKeyboard => false;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) { }

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool handled) => handled = false;

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool handled) => ProcessMouse(console, state, out handled);

        void IConsoleComponent.Update(Console console, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IConsoleComponent.ProcessKeyboard(Console, Keyboard, out bool)"/> of <see cref="IConsoleComponent"/>.
    /// </summary>
    public abstract class KeyboardConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="info">The state of the keyboard.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessKeyboard(Console console, Keyboard info, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(Console console) { }

        /// <inheritdoc />
        public virtual void OnRemoved(Console console) { }

        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => false;

        bool IConsoleComponent.IsDraw => false;

        bool IConsoleComponent.IsMouse => false;

        bool IConsoleComponent.IsKeyboard => true;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) { }

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool handled) => ProcessKeyboard(console, info, out handled);

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool handled) => handled = false;

        void IConsoleComponent.Update(Console console, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IConsoleComponent.ProcessMouse(Console, MouseConsoleState, out bool)"/> and <see cref="IConsoleComponent.ProcessKeyboard(Console, Keyboard, out bool)"/> of <see cref="IConsoleComponent"/>.
    /// </summary>
    public abstract class InputConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="info">The state of the keyboard.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessKeyboard(Console console, Keyboard info, out bool handled);

        /// <summary>
        /// Called by a host when the mouse is being processed.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="state">The state of the mouse in relation to the console.</param>
        /// <param name="handled">When set to <see langword="true"/> informs the host caller that we handled the mouse and to stop others from handling.</param>
        public abstract void ProcessMouse(Console console, MouseConsoleState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnAdded(Console console) { }

        /// <inheritdoc />
        public virtual void OnRemoved(Console console) { }

        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => false;

        bool IConsoleComponent.IsDraw => false;

        bool IConsoleComponent.IsMouse => true;

        bool IConsoleComponent.IsKeyboard => true;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) { }

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool handled) => ProcessKeyboard(console, info, out handled);

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool handled) => ProcessMouse(console, state, out handled);

        void IConsoleComponent.Update(Console console, TimeSpan delta) { }
    }

    /// <summary>
    /// A base class that implements <see cref="IConsoleComponent.Update(SadConsole.Console, TimeSpan)"/> and <see cref="IConsoleComponent.Draw(Console, TimeSpan)"/> of <see cref="IConsoleComponent"/>.
    /// </summary>
    public abstract class LogicConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        public int SortOrder { get; set; }


        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Draw(Console console, TimeSpan delta);

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Update(Console console, TimeSpan delta);

        /// <inheritdoc />
        public virtual void OnAdded(Console console) { }

        /// <inheritdoc />
        public virtual void OnRemoved(Console console) { }

        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => true;

        bool IConsoleComponent.IsDraw => true;

        bool IConsoleComponent.IsMouse => false;

        bool IConsoleComponent.IsKeyboard => false;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) => Draw(console, delta);

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool handled) => handled = false;

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool handled) => handled = false;

        void IConsoleComponent.Update(Console console, TimeSpan delta) => Update(console, delta);
    }

    /// <summary>
    /// A base class that implements all of <see cref="IConsoleComponent"/>.
    /// </summary>
    public abstract class ConsoleComponent : IConsoleComponent
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
        public abstract void Draw(Console console, TimeSpan delta);

        /// <inheritdoc />
        public abstract void Update(Console console, TimeSpan delta);

        /// <inheritdoc />
        public abstract void ProcessKeyboard(Console console, Keyboard info, out bool handled);

        /// <inheritdoc />
        public abstract void ProcessMouse(Console console, MouseConsoleState state, out bool handled);

        /// <inheritdoc />
        public virtual void OnRemoved(Console console) { }


        /// <inheritdoc />
        public virtual void OnAdded(Console console) { }
    }
}
