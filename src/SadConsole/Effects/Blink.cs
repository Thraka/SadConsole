#if XNA
using Microsoft.Xna.Framework;
#endif

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

        public Blink()
        {
            BlinkCount = -1;
            BlinkSpeed = 1d;
            UseCellBackgroundColor = true;
            BlinkOutColor = Color.Transparent;
            _isOn = true;
            _blinkCounter = 0;
        }

        public override bool UpdateCell(Cell cell)
        {
            Color oldColor = cell.Foreground;

            if (!_isOn)
            {
                if (UseCellBackgroundColor)
                {
                    cell.Foreground = cell.State.Value.Background;
                }
                else
                {
                    cell.Foreground = BlinkOutColor;
                }
            }
            else
            {
                cell.Foreground = cell.State.Value.Foreground;
            }

            return cell.Foreground != oldColor;
        }


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


        public override ICellEffect Clone() => new Blink()
        {
            BlinkOutColor = BlinkOutColor,
            BlinkSpeed = BlinkSpeed,
            _isOn = _isOn,
            UseCellBackgroundColor = UseCellBackgroundColor,
            BlinkCount = BlinkCount,

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

        public override string ToString() => string.Format("BLINK-{0}-{1}-{2}-{3}-{4}", BlinkOutColor.ToInteger(), BlinkSpeed, UseCellBackgroundColor, StartDelay, BlinkCount);
    }
}
