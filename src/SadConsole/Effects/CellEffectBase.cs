namespace SadConsole.Effects
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A base class for cell effects.
    /// </summary>
    [DataContract]
    public abstract class CellEffectBase : ICellEffect
    {
        private double _startDelay;

        /// <summary>
        /// A flag to indidcate that the delay timer has finished.
        /// </summary>
        [DataMember]
        protected bool _delayFinished = true;

        /// <summary>
        /// The total time elapsed while processing the effect.
        /// </summary>
        [DataMember]
        protected double _timeElapsed;

        /// <inheritdoc />
        [DataMember]
        public bool IsFinished { get; protected set; }

        /// <inheritdoc />
        [DataMember]
        public bool CloneOnApply { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double StartDelay
        {
            get => _startDelay;
            set { _startDelay = value; _delayFinished = _startDelay <= 0.0d; }
        }

        /// <inheritdoc />
        [DataMember]
        public bool RemoveOnFinished { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool DiscardCellState { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool Permanent { get; set; }

        protected CellEffectBase()
        {
            RemoveOnFinished = false;
            Permanent = false;
            StartDelay = 0d;
            IsFinished = false;
            _timeElapsed = 0d;
        }

        /// <inheritdoc />
        public virtual void AddCell(Cell cell)
        {
            if (cell.State == null)
            {
                cell.SaveState();
            }
        }

        /// <inheritdoc />
        public abstract bool UpdateCell(Cell cell);

        /// <inheritdoc />
        public virtual void Update(double gameTimeSeconds)
        {
            if (!IsFinished)
            {
                _timeElapsed += gameTimeSeconds;

                if (!_delayFinished)
                {
                    if (_timeElapsed >= _startDelay)
                    {
                        _delayFinished = true;
                        _timeElapsed = 0.0d;
                    }
                }
            }
        }

        /// <inheritdoc />
        public virtual void ClearCell(Cell cell)
        {
            if (!DiscardCellState)
            {
                cell.RestoreState();
            }
        }

        /// <inheritdoc />
        public virtual void Restart()
        {
            _timeElapsed = 0d;
            IsFinished = false;
            StartDelay = _startDelay;
        }

        /// <inheritdoc />
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
