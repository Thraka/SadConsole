using SadRogue.Primitives;
using System.Runtime.Serialization;

namespace SadConsole.Effects
{
    /// <summary>
    /// Switches between the normal foreground of a cell and a specified color for an amount of time, and then repeats.
    /// </summary>
    [DataContract]
    public class Blink : CellEffectBase
    {
        [DataMember]
        private int _blinkCounter = 0;
        [DataMember]
        private bool _isOn;

        /// <summary>
        /// In seconds, how fast the fade in and fade out each are
        /// </summary>
        [DataMember]
        public double BlinkSpeed { get; set; }

        /// <summary>
        /// When true, uses the current cells background color for fading instead of the value of <see cref="BlinkOutColor"/>.
        /// </summary>
        [DataMember]
        public bool UseCellBackgroundColor { get; set; }

        /// <summary>
        /// The color to fade out to.
        /// </summary>
        [DataMember]
        public Color BlinkOutColor { get; set; }

        /// <summary>
        /// How many times to blink. The value of -1 represents forever.
        /// </summary>
        [DataMember]
        public int BlinkCount { get; set; }

        /// <summary>
        /// Creates a new instance of the blink effect.
        /// </summary>
        public Blink()
        {
            BlinkCount = -1;
            BlinkSpeed = 1d;
            UseCellBackgroundColor = true;
            BlinkOutColor = Color.Transparent;
            _isOn = true;
            _blinkCounter = 0;
        }

        /// <inheritdoc />
        public override bool ApplyToCell(ColoredGlyph cell, EffectsManager.ColoredGlyphState originalState)
        {
            Color oldColor = cell.Foreground;

            if (!_isOn)
            {
                if (UseCellBackgroundColor)
                    cell.Foreground = originalState.Background;
                else
                    cell.Foreground = BlinkOutColor;
            }
            else
                cell.Foreground = originalState.Foreground;

            return cell.Foreground != oldColor;
        }


        /// <inheritdoc />
        public override void Update(double timeElapsed)
        {
            base.Update(timeElapsed);

            if (_delayFinished && !IsFinished)
            {
                if (_timeElapsed >= BlinkSpeed)
                {
                    _isOn = !_isOn;
                    _timeElapsed = 0.0d;
                    _blinkCounter += 1;

                    if (BlinkCount != -1 && _blinkCounter > (BlinkCount * 2))
                    {
                        IsFinished = true;
                    }
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

            base.Restart();
        }


        /// <inheritdoc />
        public override ICellEffect Clone() => new Blink()
        {
            BlinkOutColor = BlinkOutColor,
            BlinkSpeed = BlinkSpeed,
            _isOn = _isOn,
            UseCellBackgroundColor = UseCellBackgroundColor,
            BlinkCount = BlinkCount,

            IsFinished = IsFinished,
            StartDelay = StartDelay,
            CloneOnAdd = CloneOnAdd,
            RemoveOnFinished = RemoveOnFinished,
            RestoreCellOnRemoved = RestoreCellOnRemoved,
            _timeElapsed = _timeElapsed,
        };

        //public override bool Equals(ICellEffect effect)
        //{

        //    if (effect is Blink)
        //    {
        //        if (base.Equals(effect))
        //        {
        //            var effect2 = (Blink)effect;

        //            return BlinkOutColor == effect2.BlinkOutColor &&
        //                   BlinkSpeed == effect2.BlinkSpeed &&
        //                   UseCellBackgroundColor == effect2.UseCellBackgroundColor &&
        //                   StartDelay == effect2.StartDelay &&
        //                   BlinkCount == effect2.BlinkCount;
        //        }
        //    }

        //    return false;
        //}

        /// <inheritdoc />
        public override string ToString() => string.Format("BLINK-{0}-{1}-{2}-{3}-{4}", BlinkOutColor.PackedValue, BlinkSpeed, UseCellBackgroundColor, StartDelay, BlinkCount);
    }
}
