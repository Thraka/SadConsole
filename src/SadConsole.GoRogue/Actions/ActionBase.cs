using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    /// <summary>
    /// Base class for actions that provide a success and failure callback.
    /// </summary>
    public abstract class ActionBase
    {
        protected Func<ActionBase, bool> OnSuccessMethod;
        protected Func<ActionBase, bool> OnFailureMethod;

        /// <summary>
        /// When <see langword="true"/>, indicates that this action has been completed; otherwise <see langword="false"/>
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// A success or failure result for this action.
        /// </summary>
        public ActionResult Result { get; private set; } = ActionResult.Failure;
        
        /// <summary>
        /// Finishes the action with the specified result.
        /// </summary>
        /// <param name="result"></param>
        public void Finish(ActionResult result)
        {
            Result = result;
            IsFinished = true;

            if (Result.IsSuccess)
            {
                if (OnSuccessMethod?.Invoke(this) ?? true)
                    OnSuccessResult();
            }
            else
            {
                if (OnFailureMethod?.Invoke(this) ?? true)
                    OnFailureResult();
            }
        }

        public abstract void Run(TimeSpan timeElapsed);

        protected virtual void OnSuccessResult() { }

        protected virtual void OnFailureResult() { }

        /// <summary>
        /// Runs <paramref name="action"/> when this action is successful. Main success code will not be run if <paramref name="action"/> returns false.
        /// </summary>
        /// <param name="action">The code to run on success.</param>
        public void OnSuccess(Func<ActionBase, bool> action) => OnSuccessMethod = action;

        /// <summary>
        /// Runs <paramref name="action"/> when this action fails. Main failure code will not be run if <paramref name="action"/> returns false.
        /// </summary>
        /// <param name="action">The code to run on failure.</param>
        public void OnFailure(Func<ActionBase, bool> action) => OnFailureMethod = action;
    }

    public abstract class ActionBase<TSource, TTarget> : ActionBase
    {
        public TSource Source { get; protected set; }
        public TTarget Target { get; protected set; }

        public ActionBase(TSource source, TTarget target)
        {
            Source = source;
            Target = target;
        }
    }
}
