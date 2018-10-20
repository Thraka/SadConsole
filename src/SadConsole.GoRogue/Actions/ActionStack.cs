using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    /// <summary>
    /// A stack of <see cref="ActionBase"/> objects. As each <see cref="ActionBase.IsFinished"/> boolean is set, the action will be automatically removed from the stack.
    /// </summary>
    class ActionStack : Stack<ActionBase>
    {
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
    }
}
