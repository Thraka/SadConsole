using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    [DataContract]
    public class Entity : ScreenObject
    {
        /// <summary>
        /// Raised when the <see cref="IsDirty"/> property changes value.
        /// </summary>
        public event EventHandler IsDirtyChanged;

        [DataMember(Name = "Appearance")]
        private ColoredGlyph _glyph;

        /// <summary>
        /// A friendly name of the game object.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The drawing layer this entity is drawn at
        /// </summary>
        public int ZIndex { get; }

        /// <summary>
        /// Represents what the entity looks like.
        /// </summary>
        [DataMember]
        public ColoredGlyph Appearance
        {
            get => _glyph;
            set
            {
                if (value == null) throw new System.NullReferenceException();
                _glyph = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Indidcates this entity's visual appearance has changed.
        /// </summary>
        public bool IsDirty
        {
            get => _glyph.IsDirty;
            set
            {
                _glyph.IsDirty = value;
                OnIsDirtyChanged();
            }
        }

        /// <summary>
        /// Treats the <see cref="IScreenObject.Position"/> of the entity as if it is pixels and not cells.
        /// </summary>
        public bool UsePixelPositioning { get; set; }

        /// <summary>
        /// Creates a new entity with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground">The foreground color of the entity.</param>
        /// <param name="background">The background color of the entity.</param>
        /// <param name="glyph">The glyph color of the entity.</param>
        /// <param name="zIndex">The rendering order. Higher values are drawn on top of lower values.</param>
        public Entity(Color foreground, Color background, int glyph, int zIndex)
        {
            Appearance = new ColoredGlyph(foreground, background, glyph);
            Children.IsLocked = true;
            ZIndex = zIndex;
        }

        /// <summary>
        /// Creates a new entity, references the provided glyph as the appearance.
        /// </summary>
        /// <param name="appearance">The appearance of the entity.</param>
        /// <param name="layer">The rendering order. Lower values are under higher values.</param>
        public Entity(ref ColoredGlyph appearance, int layer)
        {
            Appearance = appearance;
            Children.IsLocked = true;
            ZIndex = layer;
        }

        /// <summary>
        /// Creates a new entity, copying the provided appearance to this entity.
        /// </summary>
        /// <param name="appearance">The appearance of the entity.</param>
        /// <param name="layer">The rendering order. Lower values are under higher values.</param>
        public Entity(ColoredGlyph appearance, int layer): this(appearance.Foreground, appearance.Background, appearance.Glyph, layer) { }


        /// <inheritdoc />
        protected override void OnPositionChanged(Point oldPosition, Point newPosition)
        {
            base.OnPositionChanged(oldPosition, newPosition);
            Appearance.IsDirty = true;
        }

        /// <summary>
        /// Raises the <see cref="IsDirtyChanged"/> event.
        /// </summary>
        protected virtual void OnIsDirtyChanged() =>
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);

        /// <inheritdoc />
        public override void UpdateAbsolutePosition()
        {
            AbsolutePosition = Position;
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
        public class EntityMovedEventArgs: EventArgs
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
