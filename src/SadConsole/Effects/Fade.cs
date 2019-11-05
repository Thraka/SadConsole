#if XNA
using Microsoft.Xna.Framework;
#endif

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
        /// Gets or sets how long the fade takes to complete in seconds.
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

        /// <summary>
        /// When <see cref="UseCellForeground"/> or <see cref="UseCellBackground"/> is set, and this is true, the last color in the fade will be set to the cell instead of the first.
        /// </summary>
        [DataMember]
        public bool UseCellDestinationReverse { get; set; }

        [DataMember]
        protected double _calculatedValue;

        [DataMember]
        protected bool _goingDown;

        public Fade()
        {
            DestinationBackground = Color.Transparent;
            DestinationForeground = Color.Transparent;
            UseCellForeground = true;
            UseCellBackground = true;
            FadeDuration = 1d;
            Repeat = false;
        }

        /// <inheritdoc />
        public override bool UpdateCell(Cell cell)
        {
            Color oldForeground = cell.Foreground;
            Color oldBackground = cell.Background;

            if (FadeForeground)
            {
                if (UseCellForeground)
                {
                    DestinationForeground.Stops[UseCellDestinationReverse ? DestinationForeground.Stops.Length - 1 : 0].Color = cell.State.Value.Foreground;
                }

                cell.Foreground = DestinationForeground.Lerp((float)_calculatedValue);
            }

            if (FadeBackground)
            {
                if (UseCellBackground)
                {
                    DestinationBackground.Stops[UseCellDestinationReverse ? DestinationBackground.Stops.Length - 1 : 0].Color = cell.State.Value.Background;
                }

                cell.Background = DestinationBackground.Lerp((float)_calculatedValue);
            }

            return oldForeground != cell.Foreground || oldBackground != cell.Background;
        }

        /// <inheritdoc />
        public override void Update(double gameTimeSeconds)
        {
            base.Update(gameTimeSeconds);

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
                            {
                                _timeElapsed = 0.0d;
                            }
                        }
                    }

                    if (!_goingDown)
                    {
                        _calculatedValue = _timeElapsed / FadeDuration;
                    }
                    else
                    {
                        _calculatedValue = 1f - (_timeElapsed / FadeDuration);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void ClearCell(Cell cell)
        {
            base.ClearCell(cell);

            if (Permanent)
            {
                if (FadeForeground)
                {
                    cell.Foreground = DestinationForeground.Stops[DestinationForeground.Stops.Length - 1].Color;
                }

                if (FadeBackground)
                {
                    cell.Background = DestinationBackground.Stops[DestinationBackground.Stops.Length - 1].Color;
                }
            }
        }

        /// <inheritdoc />
        public override ICellEffect Clone() => new Fade()
        {
            DestinationBackground = DestinationBackground,
            DestinationForeground = DestinationForeground,
            FadeForeground = FadeForeground,
            FadeBackground = FadeBackground,
            UseCellForeground = UseCellForeground,
            UseCellBackground = UseCellBackground,
            FadeDuration = FadeDuration,
            Repeat = Repeat,
            UseCellDestinationReverse = UseCellDestinationReverse,

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

        /// <inheritdoc />
        public override string ToString() => string.Format("FADE-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", DestinationBackground.ToString(), DestinationForeground.ToString(), FadeBackground, FadeForeground, FadeDuration, Permanent, StartDelay, RemoveOnFinished);
    }
}
