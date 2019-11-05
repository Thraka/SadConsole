namespace SadConsole.Effects
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Switches between the glyph of a cell and a specified glyph for an amount of time, and then repeats.
    /// </summary>
    [DataContract]
    public class BlinkGlyph : CellEffectBase
    {
        [DataMember]
        private bool _isOn;

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
        }

        public override bool UpdateCell(Cell cell)
        {
            int oldGlyph = cell.Glyph;

            if (!_isOn)
            {
                cell.Glyph = GlyphIndex;
            }
            else
            {
                cell.Glyph = cell.State?.Glyph ?? 0;
            }

            return cell.Glyph != oldGlyph;
        }

        public override void Update(double gameTimeSeconds)
        {
            base.Update(gameTimeSeconds);

            if (_delayFinished && !IsFinished)
            {
                if (_timeElapsed >= BlinkSpeed)
                {
                    _isOn = !_isOn;
                    _timeElapsed = 0.0d;
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

        public override ICellEffect Clone() => new BlinkGlyph()
        {
            _isOn = _isOn,
            BlinkSpeed = BlinkSpeed,
            GlyphIndex = GlyphIndex,

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

        /// <inheritdoc />
        public override string ToString() => string.Format("BLINKCHAR-{0}-{1}-{2}-{3}", GlyphIndex, BlinkSpeed, StartDelay, RemoveOnFinished);
    }
}
