using System.Collections.Generic;
using Newtonsoft.Json;
using SadConsole.SerializedTypes;
using SadRogue.Primitives;

namespace SadConsole.Entities
{
    // todo Fix serialization
    /// <summary>
    /// A positionable and animated game object.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Entity")]
    //[JsonConverter(typeof(EntityJsonConverter))]
    public class Entity : ScreenObject
    {
        /// <summary>
        /// Automatically forwards the <see cref="AnimatedScreenSurface.AnimationStateChanged"/> event.
        /// </summary>
        public event System.EventHandler<AnimatedScreenSurface.AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// Animation for the game object.
        /// </summary>
        protected AnimatedScreenSurface _animation;

        /// <summary>
        /// The offset to render this object at.
        /// </summary>
        protected Point _positionOffset;

        /// <summary>
        /// A friendly name of the game object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The width of the <see cref="Animation"/>.
        /// </summary>
        public int Width => _animation.Width;

        /// <summary>
        /// The height of the <see cref="Animation"/>.
        /// </summary>
        public int Height => _animation.Height;

        /// <summary>
        /// The current animation.
        /// </summary>
        public AnimatedScreenSurface Animation
        {
            get => _animation;
            set
            {
                if (_animation != null)
                {
                    _animation.State = AnimatedScreenSurface.AnimationState.Deactivated;
                    _animation.AnimationStateChanged -= OnAnimationStateChanged;
                    Children.Remove(_animation);
                }
                _animation = value;
                _animation.AnimationStateChanged += OnAnimationStateChanged;
                _animation.State = AnimatedScreenSurface.AnimationState.Activated;
                Children.Add(_animation);
            }
        }

        /// <summary>
        /// Collection of animations associated with this game object.
        /// </summary>
        public Dictionary<string, AnimatedScreenSurface> Animations { get; protected set; } = new Dictionary<string, AnimatedScreenSurface>();

        /// <summary>
        /// Offsets the position by this amount.
        /// </summary>
        public Point PositionOffset
        {
            get => _positionOffset;
            set
            {
                if (_positionOffset == value)
                {
                    return;
                }

                _positionOffset = value;
                UpdateAbsolutePosition();
            }
        }

        /// <summary>
        /// Creates a new Entity with the default font.
        /// </summary>
        public Entity(int width, int height) : this(width, height, GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize)) { }

        /// <summary>
        /// Creates a new Entity.
        /// </summary>
        public Entity(int width, int height, Font font, Point fontSize)
        {
            Animation = new AnimatedScreenSurface("default", width, height, font, fontSize);
            _animation.CreateFrame();
            Animations.Add(_animation.Name, _animation);
        }

        /// <summary>
        /// Creates a new 1x1 entity with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground">The foreground color of the entity.</param>
        /// <param name="background">The background color of the entity.</param>
        /// <param name="glyph">The glyph color of the entity.</param>
        public Entity(Color foreground, Color background, int glyph) : this(1, 1, GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize))
        {
            _animation.CurrentFrame.SetGlyph(0, 0, glyph, foreground, background);
            _animation.IsDirty = true;
        }

        /// <summary>
        /// Creates a new Entity with a default animation/
        /// </summary>
        /// <param name="animation">The default animation. The animation will have its <see cref="AnimatedScreenSurface.Name"/> property changesd to "default".</param>
        public Entity(AnimatedScreenSurface animation)
        {
            animation.Name = "default";
            Animation = animation;
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Called when the current <see cref="Animation"/> raises the <see cref="AnimatedScreenSurface.AnimationStateChanged"/> event and then raises the <see cref="AnimationStateChanged"/> event for the entity.
        /// </summary>
        /// <param name="sender">The animation calling this method.</param>
        /// <param name="e">The state of the animation.</param>
        protected void OnAnimationStateChanged(object sender, AnimatedScreenSurface.AnimationStateChangedEventArgs e) => AnimationStateChanged?.Invoke(this, e);

        /// <inheritdoc />
        public override void UpdateAbsolutePosition()
        {
            if (Animation.UsePixelPositioning)
                AbsolutePosition = Position + _positionOffset + (Parent?.AbsolutePosition ?? new Point(0, 0));
            else
                AbsolutePosition = (Animation.FontSize * Position) + (Animation.FontSize * _positionOffset) + (Parent?.AbsolutePosition ?? new Point(0, 0));

            foreach (IScreenObject child in Children)
                child.UpdateAbsolutePosition();
        }

        /// <summary>
        /// Saves the <see cref="Entity"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="Entity"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns>The entity.</returns>
        public static Entity Load(string file) => Serializer.Load<Entity>(file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Arguments for the entity moved event.
        /// </summary>
        public class EntityMovedEventArgs
        {
            /// <summary>
            /// The entity associated with the event.
            /// </summary>
            public readonly Entity Entity;

            /// <summary>
            /// The positiont the <see cref="Entity"/> moved from.
            /// </summary>
            public readonly Point FromPosition;

            /// <summary>
            /// Creates a new event args for the entity movement.
            /// </summary>
            /// <param name="entity">The entity associated with the event.</param>
            /// <param name="oldPosition">The position the entity moved from.</param>
            public EntityMovedEventArgs(Entity entity, Point oldPosition)
            {
                Entity = entity;
                FromPosition = oldPosition;
            }
        }
    }
}
