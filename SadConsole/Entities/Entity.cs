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
    public class Entity : ScreenSurface
    {
        /// <summary>
        /// Automatically forwards the <see cref="AnimatedScreenSurface.AnimationStateChanged"/> event.
        /// </summary>
        public event System.EventHandler<AnimatedScreenSurface.AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// Animation for the game object.
        /// </summary>
        protected AnimatedScreenSurface animation;

        /// <summary>
        /// The offset to render this object at.
        /// </summary>
        protected Point positionOffset;

        /// <summary>
        /// A friendly name of the game object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current animation.
        /// </summary>
        public AnimatedScreenSurface Animation
        {
            get => animation;
            set
            {
                if (animation != null)
                {
                    animation.State = AnimatedScreenSurface.AnimationState.Deactivated;
                    animation.AnimationStateChanged -= OnAnimationStateChanged;
                    Children.Remove(animation);
                }

                animation = value;
                animation.Font = Font;

                animation.AnimationStateChanged += OnAnimationStateChanged;
                animation.State = AnimatedScreenSurface.AnimationState.Activated;
                Children.Add(animation);
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
            get => positionOffset;
            set
            {
                if (positionOffset == value)
                {
                    return;
                }

                positionOffset = value;
                UpdateAbsolutePosition();
            }
        }

        /// <summary>
        /// Creates a new Entity with the default font.
        /// </summary>
        public Entity(int width, int height) : this(width, height, Global.DefaultFont) { }

        /// <summary>
        /// Creates a new Entity.
        /// </summary>
        public Entity(int width, int height, Font font): base(width, height)
        {
            Font = font;
            Animation = new AnimatedScreenSurface("default", width, height, font, FontSize);
            animation.CreateFrame();
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Creates a new 1x1 entity with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground">The foreground color of the entity.</param>
        /// <param name="background">The background color of the entity.</param>
        /// <param name="glyph">The glyph color of the entity.</param>
        public Entity(Color foreground, Color background, int glyph) : base(1, 1)
        {
            Animation = new AnimatedScreenSurface("default", 1, 1);
            animation.CreateFrame().SetGlyph(0, 0, glyph, foreground, background);
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Creates a new Entity with a default animation/
        /// </summary>
        /// <param name="animation">The default animation. The animation will have its <see cref="AnimatedScreenSurface.Name"/> property changesd to "default".</param>
        public Entity(AnimatedScreenSurface animation): base(animation.Width, animation.Height)
        {
            Font = animation.Font;
            FontSize = animation.FontSize;
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
            if (UsePixelPositioning)
                AbsolutePosition = Position + (Parent?.AbsolutePosition ?? new Point(0, 0));
            else
                AbsolutePosition = (FontSize * Position) + (Parent?.AbsolutePosition ?? new Point(0, 0));

            foreach (IScreenObject child in Children)
                child.UpdateAbsolutePosition();
        }

        /// <inheritdoc />
        protected override void OnFontChanged(Font oldFont, Point oldFontSize)
        {
            foreach (var animation in Animations.Values)
            {
                animation.Font = Font;
                animation.FontSize = FontSize;
            }
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
