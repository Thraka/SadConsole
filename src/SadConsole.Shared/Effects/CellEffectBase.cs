namespace SadConsole.Effects
{
    using System.Runtime.Serialization;


    /// <summary>
    /// A base class for cell effects.
    /// </summary>
    [DataContract]
    public abstract class CellEffectBase : ICellEffect
    {
        protected bool _delayFinished = true;
        protected double _startDelay;

        [DataMember]
        public bool IsFinished { get; protected set; }

        [DataMember]
        public bool CloneOnApply { get; set; }

        [DataMember]
        public double StartDelay
        {
            get { return _startDelay; }
            set { _startDelay = value; _delayFinished = _startDelay <= 0.0d; }
        }

        [DataMember]
        public bool RemoveOnFinished { get; set; }

        /// <summary>
        /// When true, the effect should not call <see cref="Cell.RestoreState"/> when it has finished processing.
        /// </summary>
        [DataMember]
        public bool KeepStateOnFinished { get; set; }

        [DataMember]
        public bool Permanent { get; set; }

        public abstract bool Apply(Cell cell);

        public abstract void Update(double timeElapsed);

        public virtual void Clear(Cell cell)
        {
            if (!KeepStateOnFinished)
                cell.RestoreState();
        }

        public virtual void Restart()
        {
            IsFinished = false;
            StartDelay = _startDelay;
        }

        public abstract ICellEffect Clone();

        ///// <summary>
        ///// Determines if the passed in ICellEffect equals this one or not.
        ///// </summary>
        ///// <param name="other">The other ICellEffect to test.</param>
        ///// <returns>True or false indicating equality.</returns>
        //public virtual bool Equals(ICellEffect other)
        //{
        //    if (IsFinished == other.IsFinished &&
        //        CloneOnApply == other.CloneOnApply &&
        //        StartDelay == other.StartDelay &&
        //        RemoveOnFinished == other.RemoveOnFinished &&
        //        Permanent == other.Permanent &&
        //        IsFinished == other.IsFinished)

        //        return true;

        //    else
        //        return false;
        //}
    }
}
