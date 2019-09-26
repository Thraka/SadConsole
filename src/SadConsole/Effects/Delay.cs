namespace SadConsole.Effects
{
    using System.Runtime.Serialization;

    /// <summary>
    /// An effect that doesn't do anything but delays. Usually used with the <see cref="EffectsChain"/> effect.
    /// </summary>
    [DataContract]
    public class Delay : CellEffectBase
    {
        /// <inheritdoc />
        public override void Update(double gameTimeSeconds)
        {
            base.Update(gameTimeSeconds);

            if (_delayFinished && !IsFinished)
            {
                IsFinished = true;
            }
        }

        /// <inheritdoc />
        public override void AddCell(Cell cell)
        {
        }

        /// <inheritdoc />
        public override bool UpdateCell(Cell cell) => false;

        /// <inheritdoc />
        public override void ClearCell(Cell cell)
        {
        }

        /// <inheritdoc />
        public override ICellEffect Clone() => new Delay()
        {
            IsFinished = IsFinished,
            StartDelay = StartDelay,
            CloneOnApply = CloneOnApply,
            RemoveOnFinished = RemoveOnFinished,
            DiscardCellState = DiscardCellState,
            Permanent = Permanent,
            _timeElapsed = _timeElapsed,
        };

        //public override bool Equals(ICellEffect effect)
        //{
        //    if (effect is Delay)
        //    {
        //        if (base.Equals(effect))
        //        {
        //            var effect2 = (Delay)effect;

        //            return StartDelay == effect2.StartDelay &&
        //                   DelayTime == effect2.DelayTime;
        //        }
        //    }

        //    return false;
        //}

        /// <inheritdoc />
        public override string ToString() => string.Format("DELAY-{0}", StartDelay);
    }
}
