using SadConsole.Input;
using System;

namespace SadConsole
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
        /// <param name="cancel">If set to <see langword="true"/>, tells the host not to continue processing mouse events.</param>
        void ProcessMouse(Console console, MouseConsoleState state, out bool cancel);

        /// <summary>
        /// Called by a host when the keyboard is being processed.
        /// </summary>
        /// <param name="console">The host console.</param>
        /// <param name="info">The keyboard state.</param>
        /// <param name="cancel">If set to <see langword="true"/>, tells the host not to continue processing keyboard events.</param>
        void ProcessKeyboard(Console console, Keyboard info, out bool cancel);
    }

    /// <summary>
    /// An <see cref="IConsoleComponent"/> that only works with <see cref="IConsoleComponent.Update(SadConsole.Console, TimeSpan)"/>.
    /// </summary>
    public abstract class UpdateConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        protected int SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the update frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Update(Console console, TimeSpan delta);


        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => true;

        bool IConsoleComponent.IsDraw => false;

        bool IConsoleComponent.IsMouse => false;

        bool IConsoleComponent.IsKeyboard => false;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) { }

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool cancel) { cancel = false; }

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool cancel) { cancel = false; }

        void IConsoleComponent.Update(Console console, TimeSpan delta) => Update(console, delta);
    }

    /// <summary>
    /// An <see cref="IConsoleComponent"/> that only works with <see cref="IConsoleComponent.Draw(Console, TimeSpan)"/>.
    /// </summary>
    public abstract class DrawConsoleComponent : IConsoleComponent
    {
        /// <summary>
        /// Indicates priority to other components.
        /// </summary>
        protected int SortOrder { get; set; }

        /// <summary>
        /// Called by a host on the draw frame.
        /// </summary>
        /// <param name="console">The console calling the component.</param>
        /// <param name="delta">Time since the last call.</param>
        public abstract void Draw(Console console, TimeSpan delta);


        int IConsoleComponent.SortOrder => SortOrder;

        bool IConsoleComponent.IsUpdate => true;

        bool IConsoleComponent.IsDraw => false;

        bool IConsoleComponent.IsMouse => false;

        bool IConsoleComponent.IsKeyboard => false;

        void IConsoleComponent.Draw(Console console, TimeSpan delta) => Draw(console, delta);

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool cancel) { cancel = false; }

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool cancel) { cancel = false; }

        void IConsoleComponent.Update(Console console, TimeSpan delta) { }
    }
}
