using Microsoft.Xna.Framework;

using System.Runtime.Serialization;

namespace SadConsole.Effects
{

    /// <summary>
    /// Fades both the background and foreground to seperate colors.
    /// </summary>
    [DataContract]
    public class Fade : CellEffectBase
    {
        /// <summary>
        /// Gets or sets the color gradient used to fade for the cell background.
        /// </summary>
        [DataMember]
        public ColorGradient DestinationBackground { get; set; }

        /// <summary>
        /// Gets or sets the color gradient used to fade for the cell background.
        /// </summary>
        [DataMember]
        public ColorGradient DestinationForeground { get; set; }

        /// <summary>
        /// Gets or sets how long the fade takes to complete in milliseconds.
        /// </summary>
        [DataMember]
        public double FadeDuration { get; set; }
        
        /// <summary>
        /// Gets or sets a value to indicate that the fade effect should repeat.
        /// </summary>
        [DataMember]
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate that the color gradient used with the <see cref="DestinationForeground"/> should replace its first color stop with the cell's foreground color.
        /// </summary>
        [DataMember]
        public bool UseCellForeground { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate that the color gradient used with the <see cref="DestinationBackground"/> should replace its first color stop with the cell's background color.
        /// </summary>
        [DataMember]
        public bool UseCellBackground { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate that the fade effect should use the foreground color on the cell's foreground.
        /// </summary>
        [DataMember]
        public bool FadeForeground { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate that the fade effect should use the background color on the cell's background.
        /// </summary>
        [DataMember]
        public bool FadeBackground { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate that the fade effect should automatically reverse itself when it finishes going up the color spectrum. By setting this to true, and setting the <see cref="Repeat"/> property to true, you can create a pulse effect.
        /// </summary>
        [DataMember]
        public bool AutoReverse { get; set; }

        [DataMember]
        private double _timeElapsed;
        [DataMember]
        private double _calculatedValue;
        [DataMember]
        private bool _goingDown;

        public Fade()
        {
            DestinationBackground = Color.Transparent;
            DestinationForeground = Color.Transparent;
            UseCellForeground = true;
            UseCellBackground = true;
            FadeDuration = 1d;
            _timeElapsed = 0d;
            IsFinished = false;
            RemoveOnFinished = false;
            Permanent = false;
            Repeat = false;
            StartDelay = 0d;
        }

        public override bool Apply(Cell cell)
        {
            if (cell.State == null)
                cell.SaveState();

            var oldForeground = cell.Foreground;
            var oldBackground = cell.Background;

            if (FadeForeground)
            {
                if (UseCellForeground)
                    DestinationForeground.Stops[0].Color = cell.State.Value.Foreground;

                cell.Foreground = DestinationForeground.Lerp((float)_calculatedValue);
            }

            if (FadeBackground)
            {
                if (UseCellBackground)
                    DestinationBackground.Stops[0].Color = cell.State.Value.Background;

                cell.Background = DestinationBackground.Lerp((float)_calculatedValue);
            }

            return oldForeground != cell.Foreground || oldBackground != cell.Background;
        }

        public override void Update(double gameTimeSeconds)
        {
           _timeElapsed += gameTimeSeconds;

           if (_delayFinished)
           {
               if (Repeat || !IsFinished)
               {
                   if (_timeElapsed >= FadeDuration)
                   {
                       if (AutoReverse)
                       {
                           if (!_goingDown)
                           {
                               _goingDown = !_goingDown;
                               _timeElapsed = 0.0d;
                           }
                           else
                           {
                               if (!Repeat)
                               {
                                   _calculatedValue = 0f;
                                   IsFinished = true;
                                   _timeElapsed = 0.0d;
                                   return;
                               }
                               else
                               {
                                   _timeElapsed = 0.0d;
                                   _goingDown = !_goingDown;
                               }
                           }
                       }
                       else
                       {
                           // Last color, kill self, quit
                           if (!Repeat)
                           {
                               _calculatedValue = 1f;
                               IsFinished = true;
                               _timeElapsed = 0.0d;
                               return;
                           }
                           else
                               _timeElapsed = 0.0d;
                       }
                   }

                   if (!_goingDown)
                       _calculatedValue = _timeElapsed / FadeDuration;
                   else
                       _calculatedValue = 1f - (_timeElapsed / FadeDuration);
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

            base.Restart();
        }

        public override void Clear(Cell cell)
        {
            base.Clear(cell);

            if (IsFinished && Permanent)
            {
                if (FadeForeground)
                    cell.Foreground = DestinationForeground.Stops[DestinationForeground.Stops.Length - 1].Color;

                if (FadeBackground)
                    cell.Background = DestinationBackground.Stops[DestinationBackground.Stops.Length - 1].Color;
            }
        }

        public override ICellEffect Clone()
        {
            return new Fade()
            {
                DestinationBackground = this.DestinationBackground,
                DestinationForeground = this.DestinationForeground,
                FadeForeground = this.FadeForeground,
                FadeBackground = this.FadeBackground,
                UseCellForeground = this.UseCellForeground,
                UseCellBackground = this.UseCellBackground,
                FadeDuration = this.FadeDuration,
                _timeElapsed = this._timeElapsed,
                IsFinished = this.IsFinished,
                RemoveOnFinished = this.RemoveOnFinished,
                Permanent = this.Permanent,
                Repeat = this.Repeat,
                StartDelay = this.StartDelay,
                CloneOnApply = this.CloneOnApply
            };
        }

        //public override bool Equals(ICellEffect effect)
        //{
        //    if (effect is Fade)
        //    {
        //        if (base.Equals(effect))
        //        {
        //        var effect2 = (Fade)effect;

        //        return DestinationBackground == effect2.DestinationBackground &&
        //               DestinationForeground == effect2.DestinationForeground &&
        //               FadeForeground == effect2.FadeForeground &&
        //               FadeBackground == effect2.FadeBackground &&
        //               UseCellForeground == effect2.UseCellForeground &&
        //               UseCellBackground == effect2.UseCellBackground &&
        //               FadeDuration == effect2.FadeDuration &&
        //               Permanent == effect2.Permanent &&
        //               RemoveOnFinished == effect2.RemoveOnFinished &&
        //               StartDelay == effect2.StartDelay;
        //        }
        //    }

        //    return false;
        //}

        public override string ToString()
        {
            return string.Format("FADE-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", DestinationBackground.ToString(), DestinationForeground.ToString(), FadeBackground, FadeForeground, FadeDuration, Permanent, StartDelay, RemoveOnFinished);
        }
    }
}
