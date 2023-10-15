using System;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// Animates the movement of an object. 
    /// </summary>
    public class SmoothMove : UpdateComponent
    {
        private bool _usedPixelPositioning;
        private Instructions.AnimatedValue _animatedValueX;
        private Instructions.AnimatedValue _animatedValueY;
        private Point _oldValue;
        private bool _isAnimating;
        private readonly TimeSpan _defaultTime = new TimeSpan(0, 0, 0, 0, 200);
        private bool _isEntity;
        private bool _isEnabled = true;

        /// <summary>
        /// <see langword="true"/> when this component currently is animating the movement of the host object; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsMoving => _isAnimating;

        /// <summary>
        /// Raised when the smoothing component starts moving an object.
        /// </summary>
        public event EventHandler MoveStarted;

        /// <summary>
        /// Raised when the smoothing component ends moving an object.
        /// </summary>
        public event EventHandler MoveEnded;

        /// <summary>
        /// The size of the parent object's font.
        /// </summary>
        public Point FontSize { get; set; } = Point.None;

        /// <summary>
        /// <see langword="false"/> to pause this component and prevent it from animating movement; otherwise <see langword="true"/> to enable animating movement.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                if (_isAnimating)
                    throw new InvalidOperationException("Can't change enabled while the component is animating movement.");

                _isEnabled = value;
            }
        }

        /// <summary>
        /// The easing function applied to smoothing the objects movement.
        /// </summary>
        public EasingFunctions.EasingBase TransitionEasingFunction { get; set; }

        /// <summary>
        /// The amount of time it takes to animated the movement.
        /// </summary>
        public TimeSpan TransitionTime { get; set; }

        /// <summary>
        /// Creates a new instance of this object with a default transition time of 200 milliseconds.
        /// </summary>
        public SmoothMove() =>
            TransitionTime = _defaultTime;

        /// <summary>
        /// Creates a new instance of this object with a fixed size for calculating smooth movement.
        /// </summary>
        /// <param name="fontSize">The size of the font used to display the object hosting the component.</param>
        public SmoothMove(Point fontSize)
        {
            FontSize = fontSize;
            TransitionTime = _defaultTime;
        }

        /// <summary>
        /// Creates a new instance of this object with a specific transition time.
        /// </summary>
        /// <param name="transitionTime">The amount of time it takes to animate the movement.</param>
        public SmoothMove(TimeSpan transitionTime) =>
            TransitionTime = transitionTime;

        /// <summary>
        /// Creates a new instance of this object with a fixed size for calculating smooth movement and the specified transition time.
        /// </summary>
        /// <param name="fontSize">The size of the font used to display the object hosting the component.</param>
        /// <param name="transitionTime">The amount of time it takes to animate the movement.</param>
        public SmoothMove(Point fontSize, TimeSpan transitionTime)
        {
            FontSize = fontSize;
            TransitionTime = transitionTime;
        }

        /// <summary>
        /// Called by the <paramref name="host"/> when the component is added to an object. Add to a <see cref="IScreenSurface"/> or <see cref="Entities.Entity"/>.
        /// </summary>
        /// <param name="host">The <see cref="IScreenSurface"/> or <see cref="Entities.Entity"/>.</param>
        public override void OnAdded(IScreenObject host)
        {
            if (host is Entities.Entity entity)
            {
                _isEntity = true;
                _usedPixelPositioning = entity.UsePixelPositioning;

                // Component wasn't created with a specific size.
                if (!_usedPixelPositioning && FontSize == Point.None)
                    throw new InvalidOperationException($"The {nameof(SmoothMove)} component must have {nameof(FontSize)} set if using with an entity.");
            }
            else if (host is IScreenSurface surface)
            {
                _isEntity = false;
                _usedPixelPositioning = surface.UsePixelPositioning;

                // Component wasn't created with a specific size, use the surface
                if (FontSize == Point.None)
                    FontSize = surface.FontSize;
            }
            else
                throw new NotSupportedException($"This object must implement {nameof(IScreenSurface)}");

            host.PositionChanged += Host_PositionChanged;
        }

        /// <inheritdoc/>
        public override void OnRemoved(IScreenObject host) =>
            host.PositionChanged -= Host_PositionChanged;

        private void Host_PositionChanged(object sender, ValueChangedEventArgs<Point> e)
        {
            if (!_isEnabled || _isAnimating || e.OldValue == e.NewValue)
                return;

            _isAnimating = true;

            _oldValue = e.OldValue;

            // Object was already using pixel positioning.
            if (_usedPixelPositioning)
            {
                _animatedValueX = new Instructions.AnimatedValue(TransitionTime, (double)_oldValue.X, (double)e.NewValue.X, TransitionEasingFunction);
                _animatedValueY = new Instructions.AnimatedValue(TransitionTime, (double)_oldValue.Y, (double)e.NewValue.Y, TransitionEasingFunction);

                var obj = (IScreenObject)sender;
                obj.Position = _oldValue;
            }

            // Object wasn't using pixel positioning, move by tiles.
            else
            {
                _animatedValueX = new Instructions.AnimatedValue(TransitionTime, (double)_oldValue.X * FontSize.X, (double)e.NewValue.X * FontSize.X, TransitionEasingFunction);
                _animatedValueY = new Instructions.AnimatedValue(TransitionTime, (double)_oldValue.Y * FontSize.Y, (double)e.NewValue.Y * FontSize.Y, TransitionEasingFunction);

                if (_isEntity)
                {
                    var obj = (Entities.Entity)sender;
                    obj.UsePixelPositioning = true;
                    obj.Position = _oldValue * FontSize;
                }
                else
                {
                    var obj = (IScreenSurface)sender;
                    obj.UsePixelPositioning = true;
                    obj.Position = _oldValue * FontSize;
                }
            }

            MoveStarted?.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the component. If the object is moving, changes to pixel positioning (if required) and animates the movement.
        /// </summary>
        /// <param name="host">The host object being moved.</param>
        /// <param name="delta">The time difference from the previous frame.</param>
        public override void Update(IScreenObject host, TimeSpan delta)
        {
            if (_isAnimating)
            {
                _animatedValueX.Update(null, delta);
                _animatedValueY.Update(null, delta);
                Point newPosition = ((int)_animatedValueX.Value, (int)_animatedValueY.Value);
                host.Position = newPosition;

                if (_animatedValueX.IsFinished && _animatedValueY.IsFinished)
                {
                    if (!_usedPixelPositioning)
                    {
                        Point translatedPosition = host.Position / FontSize;

                        if (_isEntity)
                        {
                            var obj = (Entities.Entity)host;
                            obj.UsePixelPositioning = false;
                            obj.Position = translatedPosition;
                        }
                        else
                        {
                            var obj = (IScreenSurface)host;
                            obj.UsePixelPositioning = false;
                            obj.Position = translatedPosition;
                        }
                    }

                    _isAnimating = false;
                    MoveEnded?.Invoke(host, EventArgs.Empty);
                }
            }
        }
    }
}
