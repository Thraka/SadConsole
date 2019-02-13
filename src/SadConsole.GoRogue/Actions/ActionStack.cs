using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Components;
using SadConsole.Input;

namespace SadConsole.Actions
{
    /// <summary>
    /// A stack of <see cref="ActionBase"/> objects. As each <see cref="ActionBase.IsFinished"/> boolean is set, the action will be automatically removed from the stack.
    /// </summary>
    public class ActionStack : Stack<ActionBase>, SadConsole.Components.IConsoleComponent
    {
        int IConsoleComponent.SortOrder => ComponentSortOrder;

        bool IConsoleComponent.IsUpdate => true;

        bool IConsoleComponent.IsDraw => false;

        bool IConsoleComponent.IsMouse => false;

        bool IConsoleComponent.IsKeyboard => false;

        public int ComponentSortOrder { get; set; }

        /// <summary>
        /// Pushes the action to the stack and immediently calls <see cref="ActionBase.Run(TimeSpan)"/>. Removes it if it finishes.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeElapsed"></param>
        public void PushAndRun(Actions.ActionBase action, TimeSpan timeElapsed = default)
        {
            if (!action.IsFinished)
            {
                if (Peek() != action)
                    Push(action);

                action.Run(timeElapsed);

                if (action.IsFinished && Peek() == action)
                    Pop();
            }
        }

        /// <summary>
        /// Removes all finished actions from the top of the stack and runs the action on the top of the stack.
        /// </summary>
        /// <param name="timeElapsed"></param>
        public void Run(TimeSpan timeElapsed)
        {
            if (Count == 0) return;

            // Pop off all finished commands (happens when they get chained together)
            // to get to one that needs to be run
            while (Count != 0 && Peek().IsFinished)
                Pop();

            // Run the existing command.
            if (Count != 0)
                Peek().Run(timeElapsed);
        }

        void IConsoleComponent.Draw(Console console, TimeSpan delta)
        {
            throw new NotImplementedException();
        }

        void IConsoleComponent.OnAdded(Console console) { }

        void IConsoleComponent.OnRemoved(Console console) { }

        void IConsoleComponent.ProcessKeyboard(Console console, Keyboard info, out bool handled)
        {
            throw new NotImplementedException();
        }

        void IConsoleComponent.ProcessMouse(Console console, MouseConsoleState state, out bool handled)
        {
            throw new NotImplementedException();
        }

        void IConsoleComponent.Update(Console console, TimeSpan delta) => Run(delta);
    }
}
