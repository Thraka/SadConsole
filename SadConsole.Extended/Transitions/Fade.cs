using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Transitions
{
    /// <summary>
    /// Fades out one <see cref="IScreenSurface"/> while fading in another.
    /// </summary>
    public class Fade : Instructions.InstructionBase
    {
        bool _firstRun = false;

        Instructions.AnimatedValue _valueInstructionTo;

        IScreenSurface _fadeFrom;
        IScreenSurface _fadeTo;

        /// <summary>
        /// When <see langword="true"/>, removes the "From" object from the parent when the transition finishes. Supersedes <see cref="HideFromObject"/>.
        /// </summary>
        public bool DeparentFromObject { get; set; }

        /// <summary>
        /// When <see langword="true"/>, sets <see cref="IScreenObject.IsVisible"/> to <see langword="false"/> on the "From" object when the transition finishes.
        /// </summary>
        public bool HideFromObject { get; set; }

        /// <summary>
        /// When <see langword="true"/>, sets <see cref="IScreenObject.Position"/> value on the "To" object to match the transition finishes.
        /// </summary>
        public bool RepositionToObject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"></param>
        public Fade(IScreenSurface from, IScreenSurface to, TimeSpan duration, EasingFunctions.EasingBase easingFunction = null)
        {
            if (easingFunction == null)
                easingFunction = new EasingFunctions.Linear();

            _fadeFrom = from;
            _fadeTo = to;

            _valueInstructionTo = new Instructions.AnimatedValue(duration, 0, 255, easingFunction);
        }

        /// <summary>
        /// Processes the fade between two objects.
        /// </summary>
        /// <param name="componentHost">The host running the component.</param>
        /// <param name="delta">The time difference for the frame.</param>
        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            if (IsFinished) return;

            // First run configures the To target.
            if (!_firstRun)
            {
                if (_fadeFrom.Parent == null) throw new NullReferenceException("The \"From\" target must have a parent assigned to it.");
                if (_fadeFrom.Renderer == null) throw new NullReferenceException("The \"From\" target must have a renderer.");
                if (_fadeTo.Renderer == null) throw new NullReferenceException("The \"To\" target must have a renderer.");

                _fadeTo.Parent = _fadeFrom.Parent;

                if (RepositionToObject)
                    _fadeTo.Position = _fadeFrom.Position;
            }

            _valueInstructionTo.Update(componentHost, delta);

            _fadeTo.Renderer.Opacity = (byte)_valueInstructionTo.Value;
            _fadeFrom.Renderer.Opacity = (byte)(255 - _fadeTo.Renderer.Opacity);

            if (_valueInstructionTo.IsFinished)
            {
                IsFinished = true;

                if (DeparentFromObject)
                    _fadeFrom.Parent = null;
                else if (HideFromObject)
                    _fadeFrom.IsVisible = true;
            }
        }
    }
}
