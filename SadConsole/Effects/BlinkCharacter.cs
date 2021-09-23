using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Effects
{
    /// <summary>
    /// Switches between the glyph of a cell and a specified glyph for an amount of time, and then repeats.
    /// </summary>
    [DataContract]
    public class BlinkGlyph : CellEffectBase
    {
        private bool _isOn;
        private int _blinkCounter = 0;
        private double _duration = 0d;

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

        /// <summary>
        /// How many times to blink. The value of -1 represents forever.
        /// </summary>
        [DataMember]
        public int BlinkCount { get; set; }

        /// <summary>
        /// The total duraction this effect will run for, before being flagged as finished. -1 represents forever.
        /// </summary>
        [DataMember]
        public double Duration { get; set; }

        /// <summary>
        /// Creates an instance of the blink glyph effect.
        /// </summary>
        public BlinkGlyph()
        {
            Duration = -1;
            BlinkCount = -1;
            BlinkSpeed = 1d;
            GlyphIndex = 0;
            _isOn = true;
        }

        /// <inheritdoc />
        public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState)
        {
            int oldGlyph = cell.Glyph;

            if (!_isOn)
                cell.Glyph = GlyphIndex;
            else
                cell.Glyph = originalState.Glyph;

            return cell.Glyph != oldGlyph;
        }

        /// <inheritdoc />
        public override void Update(double gameTimeSeconds)
        {
            base.Update(gameTimeSeconds);

            if (_delayFinished && !IsFinished)
            {
                if (Duration != -1)
                {
                    _duration += gameTimeSeconds;
                    if (_duration >= Duration)
                    {
                        IsFinished = true;
                        return;
                    }
                }

                if (_timeElapsed >= BlinkSpeed)
                {
                    _isOn = !_isOn;
                    _timeElapsed = 0.0d;

                    if (BlinkCount != -1)
                    {
                        _blinkCounter += 1;

                        if (BlinkCount != -1 && _blinkCounter > (BlinkCount * 2))
                            IsFinished = true;
                    }
                }

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
            _isOn = true;
            _blinkCounter = 0;
            _duration = 0d;

            base.Restart();
        }

        /// <inheritdoc />
        public override ICellEffect Clone() => new BlinkGlyph()
        {
            _isOn = _isOn,
            BlinkSpeed = BlinkSpeed,
            GlyphIndex = GlyphIndex,

            IsFinished = IsFinished,
            StartDelay = StartDelay,
            CloneOnAdd = CloneOnAdd,
            RemoveOnFinished = RemoveOnFinished,
            RestoreCellOnRemoved = RestoreCellOnRemoved,
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
        public override string ToString() =>
            string.Format("BLINKCHAR-{0}-{1}-{2}-{3}", GlyphIndex, BlinkSpeed, StartDelay, RemoveOnFinished);
    }
}
