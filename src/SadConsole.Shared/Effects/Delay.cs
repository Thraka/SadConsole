namespace SadConsole.Effects
{
    using System.Runtime.Serialization;

    /// <summary>
    /// An effect that doesn't do anything but delays. Usually used by the ChainEffect effect.
    /// </summary>
    [DataContract]
    public class Delay: CellEffectBase
    {
        [DataMember]
        private double _timeElapsed;

        [DataMember]
        public double DelayTime { get; set; }

        public override void Update(double gameTimeSeconds)
        {
            _timeElapsed += gameTimeSeconds;

            if (_delayFinished)
            {
                if (_timeElapsed >= DelayTime)
                {
                    IsFinished = true;
                }
            }
            else
            {
                if (_timeElapsed >= _startDelay)
                {
                    _delayFinished = true;
                    _timeElapsed = 0.0d;
                }
            }
        }

        /// <summary>
        /// Restarts the cell effect but does not reset it.
        /// </summary>
        public override void Restart()
        {
            _timeElapsed = 0.0d;

            base.Restart();
        }

        public override bool Apply(Cell cell)
        {
            return false;
        }

        public override void Clear(Cell cell)
        {
        }

        public override ICellEffect Clone()
        {
            return new Delay()
            {
                DelayTime = this.DelayTime,
                StartDelay = this.StartDelay,
                CloneOnApply = this.CloneOnApply,
            };
        }

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

        public override string ToString()
        {
            return string.Format("BLINK-{0}-{1}", DelayTime, StartDelay);
        }
    }
}
