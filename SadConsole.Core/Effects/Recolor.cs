namespace SadConsole.Effects
{
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;
    
    /// <summary>
    /// Recors the foreground or the background of a cell.
    /// </summary>
    [DataContract]
    public class Recolor: CellEffectBase
    {
        [DataMember]
        public Color Foreground { get; set; }

        [DataMember]
        public Color Background { get; set; }

        [DataMember]
        private double _timeElapsed;

        [DataMember]
        private bool _delayFinished = true;
        [DataMember]
        private double _startDelay;

        public Recolor()
        {
            Color Foreground = Color.White;
            Color Background = Color.Transparent;
            RemoveOnFinished = false;
            Permanent = false;
            StartDelay = 0d;
            IsFinished = false;
            _timeElapsed = 0d;
        }
        
        public override void Apply(Cell cell)
        {
            cell.ActualBackground = Background;
            cell.ActualForeground = Foreground;
        }

        public override void Update(double gameTimeSeconds)
        {
            if (_delayFinished)
            {
                IsFinished = true;
            }
            else
            {
                _timeElapsed += gameTimeSeconds;

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
            base.Restart();

            _timeElapsed = 0d;
        }

        public override void Clear(Cell cell)
        {
            if (!Permanent)
            {
                cell.Background = cell.Background;
                cell.Foreground = cell.Foreground;
            }
            else
            {
                cell.Background = Background;
                cell.Foreground = Foreground;
            }
        }

        public override ICellEffect Clone()
        {
            return new Recolor()
            {
                Foreground = this.Foreground,
                Background = this.Background,
                Permanent = this.Permanent,
                RemoveOnFinished = this.RemoveOnFinished,
                StartDelay = this.StartDelay,
                CloneOnApply = this.CloneOnApply
            };
        }

        public override bool Equals(ICellEffect effect)
        {
            if (effect is Recolor)
            {
                if (base.Equals(effect))
                {
                    var effect2 = (Recolor)effect;

                    return Foreground == effect2.Foreground &&
                           Background == effect2.Background &&
                           Permanent == effect2.Permanent &&
                           RemoveOnFinished == effect2.RemoveOnFinished &&
                           StartDelay == effect2.StartDelay;
                }
            }
                
            return false;
        }

        public override string ToString()
        {
            return string.Format("PULSE-{0}-{1}-{2}-{3}-{4}", Foreground.PackedValue, Background.PackedValue, Permanent, StartDelay, RemoveOnFinished);
        }
    }
}
