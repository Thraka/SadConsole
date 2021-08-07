using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Instructions
{
    /// <summary>
    /// Animates drawing a box. It can either 
    /// </summary>
    public class AnimatedBoxGrow: InstructionBase
    {
        private ShapeParameters _shapeParameters;

        private Rectangle _area;
        private TimeSpan _duration;

        private AnimatedValue _animationWidth;
        private AnimatedValue _animationHeight;

        private Rectangle _previousArea;

        public AnimatedBoxGrow(Rectangle area, TimeSpan duration, ShapeParameters shapeParameters)
        {
            if (area.Width < 2 || area.Height < 2) throw new ArgumentOutOfRangeException(nameof(area), "Area must be at least 2x2");

            _area = area;
            _duration = duration;
            _shapeParameters = shapeParameters;

            RepeatCount = 0;
            RemoveOnFinished = true;

            _animationWidth = new AnimatedValue(_duration, 1, _area.Width / 2);
            _animationHeight = new AnimatedValue(_duration, 1, _area.Height / 2);
        }

        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            if (!IsFinished)
            {
                _animationWidth.Update(componentHost, delta);
                _animationHeight.Update(componentHost, delta);

                // Get the new area of the animated width/height
                var newArea = new Rectangle(_area.Center, Math.Max((int)_animationWidth.Value, 1), Math.Max((int)_animationHeight.Value, 1));

                // If the area changed, draw
                if (_previousArea != newArea)
                {
                    ((IScreenSurface)componentHost).Surface.DrawBox(newArea, _shapeParameters);

                    _previousArea = newArea;
                }

                if (_animationWidth.IsFinished && _animationHeight.IsFinished)
                    IsFinished = true;

                base.Update(componentHost, delta);
            }
        }

        public override void OnAdded(IScreenObject host)
        {
            if (!(host is IScreenSurface surface)) throw new ArgumentException($"Animated box can only be added to an {nameof(IScreenSurface)}");

            if (!_shapeParameters.HasBorder)
                _shapeParameters.BorderGlyph = new ColoredGlyph(surface.Surface.DefaultForeground, surface.Surface.DefaultBackground, surface.Surface.DefaultGlyph);

            if (!_shapeParameters.HasFill)
                _shapeParameters.FillGlyph = new ColoredGlyph(surface.Surface.DefaultForeground, surface.Surface.DefaultBackground, surface.Surface.DefaultGlyph);
        }
    }
}
