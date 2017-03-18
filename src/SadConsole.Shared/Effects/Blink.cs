using Microsoft.Xna.Framework;

using System.Runtime.Serialization;

namespace SadConsole.Effects
{
    /// <summary>
    /// Switches between the normal foreground of a cell and a specified color for an amount of time, and then repeats.
    /// </summary>
    [DataContract]
    public class Blink: CellEffectBase
    {
        [DataMember]
        private int _blinkCounter = 0;
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
            _timeElapsed = 0d;
            StartDelay = 0d;
            _blinkCounter = 0;
        }

        public override bool Apply(Cell cell)
        {
            if (cell.State == null)
                cell.SaveState();

            var oldColor = cell.Foreground;

            if (!_isOn)
            {
                if (UseCellBackgroundColor)
                    cell.Foreground = cell.State.Value.Background;
                else
                    cell.Foreground = BlinkOutColor;
            }
            else
            {
                cell.Foreground = cell.State.Value.Foreground;
            }

            return cell.Foreground != oldColor;
        }


        public override void Update(double timeElapsed)
        {
            _timeElapsed += timeElapsed;

            if (_delayFinished)
            {
                if (_timeElapsed >= BlinkSpeed)
                {
                    _isOn = !_isOn;
                    _timeElapsed = 0.0d;

                    if (BlinkCount != -1 && _blinkCounter > BlinkCount)
                    {
                        _blinkCounter += 1;
                        IsFinished = true;
                    }
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
            _timeElapsed = 0d;
            _isOn = true;
            _blinkCounter = 0;

            base.Restart();
        }
        

        public override ICellEffect Clone()
        {
            return new Blink()
            {
                BlinkOutColor = this.BlinkOutColor,
                BlinkSpeed = this.BlinkSpeed,
                _isOn = this._isOn,
                _timeElapsed = this._timeElapsed,
                UseCellBackgroundColor = this.UseCellBackgroundColor,
                IsFinished = this.IsFinished,
                StartDelay = this.StartDelay,
                BlinkCount = this.BlinkCount,
                RemoveOnFinished = this.RemoveOnFinished,
                CloneOnApply = this.CloneOnApply,
            };
        }

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

        public override string ToString()
        {
            return string.Format("BLINK-{0}-{1}-{2}-{3}-{4}", BlinkOutColor.ToInteger(), BlinkSpeed, UseCellBackgroundColor, StartDelay, BlinkCount);
        }
    }
}
