#if XNA
using Microsoft.Xna.Framework;
#endif

using System.Collections.Generic;
using Newtonsoft.Json;
using SadConsole.SerializedTypes;

namespace SadConsole.Entities
{
    // todo Fix serialization
    /// <summary>
    /// A positionable and animated game object.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Entity")]
    [JsonConverter(typeof(EntityJsonConverter))]
    public class Entity : Console
    {
        /// <summary>
        /// Automatically forwards the <see cref="AnimatedConsole.AnimationStateChanged"/> event.
        /// </summary>
        public event System.EventHandler<AnimatedConsole.AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// Triggered when the entity changes position.
        /// </summary>
        public event System.EventHandler<EntityMovedEventArgs> Moved;

        /// <summary>
        /// Animation for the game object.
        /// </summary>
        protected AnimatedConsole animation;

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
        public AnimatedConsole Animation
        {
            get => animation;
            set
            {
                if (animation != null)
                {
                    animation.State = AnimatedConsole.AnimationState.Deactivated;
                    animation.AnimationStateChanged -= OnAnimationStateChanged;
                    Children.Remove(animation);
                }

                animation = value;
                animation.Font = Font;

                animation.AnimationStateChanged += OnAnimationStateChanged;
                animation.State = AnimatedConsole.AnimationState.Activated;
                Children.Add(animation);
            }
        }

        /// <summary>
        /// Collection of animations associated with this game object.
        /// </summary>
        public Dictionary<string, AnimatedConsole> Animations { get; protected set; } = new Dictionary<string, AnimatedConsole>();

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
                OnCalculateRenderPosition();
            }
        }

        /// <summary>
        /// Creates a new Entity with the default font.
        /// </summary>
        public Entity(int width, int height) : this(width, height, Global.FontDefault) { }

        /// <summary>
        /// Creates a new Entity.
        /// </summary>
        public Entity(int width, int height, Font font)
        {
            Font = font;
            IsCursorDisabled = true;
            Animation = new AnimatedConsole("default", width, height, font);
            animation.CreateFrame();
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Creates a new 1x1 entity with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground">The foreground color of the entity.</param>
        /// <param name="background">The background color of the entity.</param>
        /// <param name="glyph">The glyph color of the entity.</param>
        public Entity(Color foreground, Color background, int glyph)
        {
            IsCursorDisabled = true;
            Animation = new AnimatedConsole("default", 1, 1, Global.FontDefault);
            animation.CreateFrame().SetGlyph(0, 0, glyph, foreground, background);
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Creates a new Entity with a default animation/
        /// </summary>
        /// <param name="animation">The default animation. The animation will have its <see cref="AnimatedConsole.Name"/> property changesd to "default".</param>
        public Entity(AnimatedConsole animation)
        {
            Font = animation.Font;
            IsCursorDisabled = true;
            animation.Name = "default";
            Animation = animation;
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Called when the current <see cref="Animation"/> raises the <see cref="AnimatedConsole.AnimationStateChanged"/> event and then raises the <see cref="AnimationStateChanged"/> event for the entity.
        /// </summary>
        /// <param name="sender">The animation calling this method.</param>
        /// <param name="e">The state of the animation.</param>
        protected void OnAnimationStateChanged(object sender, AnimatedConsole.AnimationStateChangedEventArgs e) => AnimationStateChanged?.Invoke(this, e);

        /// <inheritdoc />
        public override void OnCalculateRenderPosition()
        {
            if (UsePixelPositioning)
            {
                CalculatedPosition = Position + PositionOffset + (Parent?.CalculatedPosition ?? Point.Zero);
            }
            else
            {
                CalculatedPosition = Position.ConsoleLocationToPixel(Font) + PositionOffset.ConsoleLocationToPixel(Font) + (Parent?.CalculatedPosition ?? Point.Zero);
            }

            foreach (Console child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }

        /// <inheritdoc />
        protected override void OnFontChanged()
        {
            if (animation != null)
            {
                animation.Font = Font;
            }
        }

        /// <summary>
        /// Called when the <see cref="Console.Position"/> value changes. Triggers <see cref="Moved"/>.
        /// </summary>
        /// <param name="oldPosition"></param>
        protected override void OnPositionChanged(Point oldPosition) => Moved?.Invoke(this, new EntityMovedEventArgs(this, oldPosition));

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
