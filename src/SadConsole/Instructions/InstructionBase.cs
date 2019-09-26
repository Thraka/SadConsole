namespace SadConsole.Instructions
{
    using System;
    using SadConsole.Components;

    /// <summary>
    /// Base class for all instructions.
    /// </summary>
    public abstract class InstructionBase : UpdateConsoleComponent
    {
        /// <summary>
        /// Raised when the instruction starts.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Raised when the instruction completes.
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// Raised when the instruction completes but is going to repeat.
        /// </summary>
        public event EventHandler Repeating;

        /// <summary>
        /// When true, this instruction will automatically remove itself from the parent's <see cref="SadConsole.Console.Components"/> collection.
        /// </summary>
        public bool RemoveOnFinished { get; set; }

        /// <summary>
        /// Flags the instruction as completed or not. If completed, the <see cref="Finished"/> event will be raised.
        /// </summary>
        public bool IsFinished { get; set; }

        /// <summary>
        /// Indicates how many times this set will repeat. Use 0 to not repeat and -1 to repeat forever.
        /// </summary>
        /// <remarks>This property counts down each time the instruction finishes. If set to -1 it will repeat forever. As this represents how many times to repeat, setting this value to 1 would allow the instruction to execute twice, once for the original time, and again for the repeat counter of 1.</remarks>
        public int RepeatCount { get; set; }

        /// <summary>
        /// Resets the Done flag.
        /// </summary>
        /// <remarks>On the base class, resets the <see cref="IsFinished"/> to false. Override this method to reset the derived class' counters and status flags for the instruction.</remarks>
        public virtual void Reset() => IsFinished = false;

        /// <summary>
        /// Repeats the current instruction. Decrements the <see cref="RepeatCount"/> value (if applicable), and raises the <see cref="Repeating"/> event. This method should be overridden in derived classes to customize how the object is reset for a repeat.
        /// </summary>
        public virtual void Repeat()
        {
            if (RepeatCount > 0)
            {
                RepeatCount--;
            }

            Reset();

            OnRepeating();
        }

        /// <summary>
        /// Executes the instruction. This base class method should be called from derived classes. If the IsFinished property is set to true, will try to repeat if needed and will raise all appropriate events.
        /// </summary>
        public override void Update(SadConsole.Console console, TimeSpan delta)
        {
            if (IsFinished)
            {
                if (RepeatCount > 0 || RepeatCount == -1)
                {
                    Repeat();
                }
                else
                {
                    OnFinished(console);
                }
            }
        }

        /// <summary>
        /// Called when the instruction finishes.
        /// </summary>
        protected virtual void OnFinished(SadConsole.Console componentHost)
        {
            Finished?.Invoke(this, EventArgs.Empty);

            if (RemoveOnFinished)
            {
                componentHost.Components.Remove(this);
            }
        }

        /// <summary>
        /// Called when the instruction repeats.
        /// </summary>
        protected virtual void OnRepeating() =>
            Repeating?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Called when the instruction first runs.
        /// </summary>
        protected virtual void OnStarted() =>
            Started?.Invoke(this, EventArgs.Empty);
    }
}
