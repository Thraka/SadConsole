namespace SadConsole.Instructions
{
    using System;
    using System.Runtime.Serialization;


    /// <summary>
    /// Base class for instructions that target and interact with an object.
    /// </summary>
    /// <typeparam name="TInstructedType">The type of object used with the instruction</typeparam>
    [DataContract]
    public abstract class InstructionBase<TInstructedType>: InstructionBase
    {
        /// <summary>
        /// The target object to be used when the <see cref="Run"/> method is called.
        /// </summary>
        [DataMember]
        public TInstructedType Target { get; set; }

        /// <summary>
        /// Creates a new instruction with the specified target object.
        /// </summary>
        /// <param name="targetObject">The object to target</param>
        public InstructionBase(TInstructedType targetObject)
            : base()
        {
            Target = targetObject;
        }
    }


    /// <summary>
    /// Base class for all instructions.
    /// </summary>
    [DataContract]
    public abstract class InstructionBase
    {
        #region Events
        /// <summary>
        /// Raised when the instruction completes.
        /// </summary>
        public event EventHandler ExecutionFinished;

        /// <summary>
        /// Raised when the instruction completes but is going to repeat.
        /// </summary>
        public event EventHandler ExecutionRepeating;
        #endregion

        #region Properties
        /// <summary>
        /// Flags the instruction as completed or not. If completed, the <see cref="ExecutionFinished"/> event will be raised.
        /// </summary>
        [DataMember]
        public bool IsFinished { get; set; }

        /// <summary>
        /// Indicates how many times this set will repeat. Counts down every run. If set to -1 it will repeat forever. As this represents how many times to repeat, setting this value to 1 would allow the instruction to execute twice, once for the original time, and again for the repeat counter of 1.
        /// </summary>
        [DataMember]
        public int RepeatCount { get; set; }
        #endregion

        #region Constructors
        public InstructionBase() { }
        #endregion

        #region Methods
        /// <summary>
        /// Resets the Done flag.
        /// </summary>
        /// <remarks>On the base class, resets the <paramref name="Done"/> to false. Override this method to reset the derived class' counters and status flags for the instruction.</remarks>
        public virtual void Reset()
        {
            IsFinished = false;
        }

        /// <summary>
        /// Repeats the current instruction. Decrements the <see cref="RepeatCount"/> value (if applicable), and raises the <see cref="ExecutionRepeating"/> event. This method should be overridden in derived classes to customize how the object is reset for a repeat.
        /// </summary>
        public virtual void Repeat()
        {
            if (RepeatCount > 0)
                RepeatCount--;

            Reset();

            OnExecutionRepeating();
        }
        
        /// <summary>
        /// Executes the instruction. This base class method should be called from derived classes. If the Done property is set to true, will try to repeat if needed and will raise all appropriate events.
        /// </summary>
        public virtual void Run()
        {
            if (IsFinished)
            {
                if (RepeatCount > 0 || RepeatCount == -1)
                    Repeat();
                else
                    OnExecutionFinished();
            }
        }

        /// <summary>
        /// Raises the FinishedExecuting event.
        /// </summary>
        protected virtual void OnExecutionFinished()
        {
            if (ExecutionFinished != null)
                ExecutionFinished(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fires the FinishedExecuting event.
        /// </summary>
        protected virtual void OnExecutionRepeating()
        {
            if (ExecutionRepeating != null)
                ExecutionRepeating(this, EventArgs.Empty);
        }
        #endregion
    }
}
