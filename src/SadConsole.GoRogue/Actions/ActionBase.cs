using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    public abstract class ActionBase
    {
        protected Func<ActionBase, bool> OnSuccessMethod;
        protected Func<ActionBase, bool> OnFailureMethod;

        public bool IsFinished { get; private set; }

        public ActionResult Result { get; private set; } = ActionResult.Empty;

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

    abstract class ActionBase<TSource, TTarget> : ActionBase
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
