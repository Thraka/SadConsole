using Microsoft.Xna.Framework;

using System.Runtime.Serialization;

namespace SadConsole.Effects
{
   
    /// <summary>
    /// Recors the foreground or the background of a cell.
    /// </summary>
    [DataContract]
    public class Recolor: CellEffectBase
    {
        /// <summary>
        /// The foreground color applied to a cell.
        /// </summary>
        [DataMember]
        public Color Foreground { get; set; }

        /// <summary>
        /// The background color applied to a cell.
        /// </summary>
        [DataMember]
        public Color Background { get; set; }

        [DataMember]
        private double _timeElapsed;

        [DataMember]
        private bool _delayFinished = true;
        [DataMember]
        private double _startDelay;

        /// <summary>
        /// When true, the <see cref="Foreground"/> color will be applied to the cell.
        /// </summary>
        [DataMember]
        public bool DoForeground { get; set; }

        /// <summary>
        /// /// When true, the <see cref="Background"/> color will be applied to the cell.
        /// </summary>
        [DataMember]
        public bool DoBackground { get; set; }

        public Recolor()
        {
            Color Foreground = Color.White;
            Color Background = Color.Transparent;
            DoBackground = true;
            DoForeground = true;
            RemoveOnFinished = false;
            Permanent = false;
            StartDelay = 0d;
            IsFinished = false;
            _timeElapsed = 0d;
        }
        
        public override bool Apply(Cell cell)
        {
            if (cell.State == null)
                cell.SaveState();

            var oldForeground = cell.Foreground;
            var oldBackground = cell.Background;

            if (DoBackground) cell.Background = Background;
            if (DoForeground) cell.Foreground = Foreground;

            return oldForeground != cell.Foreground || oldBackground != cell.Background;
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
            base.Clear(cell);

            if (!Permanent)
            {
                if (DoBackground)
                    cell.Background = Background;

                if (DoForeground)
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
                DoForeground = this.DoForeground,
                DoBackground = this.DoBackground,
                CloneOnApply = this.CloneOnApply
            };
        }

        //public override bool Equals(ICellEffect effect)
        //{
        //    if (effect is Recolor)
        //    {
        //        if (base.Equals(effect))
        //        {
        //            var effect2 = (Recolor)effect;

        //            return Foreground == effect2.Foreground &&
        //                   Background == effect2.Background &&
        //                   Permanent == effect2.Permanent &&
        //                   RemoveOnFinished == effect2.RemoveOnFinished &&
        //                   StartDelay == effect2.StartDelay &&
        //                   DoForeground == effect2.DoForeground &&
        //                   DoBackground == effect2.DoBackground;
        //        }
        //    }
                
        //    return false;
        //}

        public override string ToString()
        {
            return string.Format("RECOLOR-{0}-{1}-{2}-{3}-{4}", Foreground.ToInteger(), Background.ToInteger(), Permanent, StartDelay, RemoveOnFinished);
        }
    }
}
