namespace SadConsole.Effects
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Switches between the glyph of a cell and a specified glyph for an amount of time, and then repeats.
    /// </summary>
    [DataContract]
    public class BlinkGlyph: CellEffectBase
    {
        [DataMember]
        private bool _isOn;
        [DataMember]
        private double _timeElapsed;

        /// <summary>
        /// In seconds, how fast the fade in and fade out each are
        /// </summary>
        [DataMember]
        public double BlinkSpeed { get; set; }

        /// <summary>
        /// The glyph index to blink into.
        /// </summary>
        [DataMember]
        public int GlyphIndex { get; set; }

        public BlinkGlyph()
        {
            BlinkSpeed = 1d;
            GlyphIndex = 0;
            _isOn = true;
            _timeElapsed = 0d;
            StartDelay = 0d;
        }

        #region ICellEffect Members

        public override bool Apply(Cell cell)
        {
            if (cell.State == null)
                cell.SaveState();

            var oldGlyph = cell.Glyph;

            if (!_isOn)
                cell.Glyph = GlyphIndex;
            else
                cell.Glyph = cell.State?.Glyph ?? 0;

            return cell.Glyph != oldGlyph;
        }

        public override void Update(double gameTimeSeconds)
        {
            _timeElapsed += gameTimeSeconds;

            if (_delayFinished)
            {
                if (_timeElapsed >= BlinkSpeed)
                {
                    _isOn = !_isOn;
                    _timeElapsed = 0.0d;
                }
            }
            else
            {
                if (_timeElapsed >= _startDelay)
                {
                    _delayFinished = true;
                    _timeElapsed = 0d;
                }
            }
        }

        /// <summary>
        /// Restarts the cell effect but does not reset it.
        /// </summary>
        public override void Restart()
        {
            _timeElapsed = 0d;
            _isOn = true;

            base.Restart();
        }
        
        public override ICellEffect Clone()
        {
            return new BlinkGlyph()
            {
                _isOn = this._isOn,
                _timeElapsed = this._timeElapsed,
                BlinkSpeed = this.BlinkSpeed,
                GlyphIndex = this.GlyphIndex,
                IsFinished = this.IsFinished,
                StartDelay = this.StartDelay,
                RemoveOnFinished = this.RemoveOnFinished,
                CloneOnApply = this.CloneOnApply
            };
        }

        //public override bool Equals(ICellEffect effect)
        //{
        //    if (effect is BlinkGlyph)
        //    {
        //        if (base.Equals(effect))
        //        {
        //            var effect2 = (BlinkGlyph)effect;

        //            return GlyphIndex == effect2.GlyphIndex &&
        //                   BlinkSpeed == effect2.BlinkSpeed &&
        //                   RemoveOnFinished == effect2.RemoveOnFinished &&
        //                   StartDelay == effect2.StartDelay;
        //        }
        //    }
            
        //    return false;
        //}

        public override string ToString()
        {
            return string.Format("BLINKCHAR-{0}-{1}-{2}-{3}", GlyphIndex, BlinkSpeed, StartDelay, RemoveOnFinished);
        }
        #endregion
    }
}
