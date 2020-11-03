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
    public class EntityLite : ScreenObject
    {
        [DataMember(Name = "Font")]
        private Font _font;
        [DataMember(Name = "FontSize")]
        private Point _fontSize;
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
                _glyph.IsDirty = true;
            }
        }

        /// <summary>
        /// Font used with rendering.
        /// </summary>
        public Font Font
        {
            get => _font;
            set
            {
                if (_font == value) return;

                _font = value;
                FontSize = _font.GetFontSize(Font.Sizes.One);
                _glyph.IsDirty = true;
            }
        }

        /// <summary>
        /// The size of the <see cref="Font"/> cells applied to the object when rendering.
        /// </summary>
        public Point FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize == value) return;

                _fontSize = value;
                _glyph.IsDirty = true;
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
        /// <param name="layer">The rendering order. Lower values are under higher values.</param>
        public EntityLite(Color foreground, Color background, int glyph, int layer)
        {
            Appearance = new ColoredGlyph(foreground, background, glyph);
            Children.IsLocked = true;
            ZIndex = layer;
            Font = SadConsole.GameHost.Instance.DefaultFont;
            FontSize = Font.GetFontSize(SadConsole.GameHost.Instance.DefaultFontSize);
        }

        /// <summary>
        /// Creates a new entity, references the provided glyph as the appearance.
        /// </summary>
        /// <param name="appearance">The appearance of the entity.</param>
        /// <param name="layer">The rendering order. Lower values are under higher values.</param>
        public EntityLite(ref ColoredGlyph appearance, int layer)
        {
            Appearance = appearance;
            Children.IsLocked = true;
            ZIndex = layer;
            Font = SadConsole.GameHost.Instance.DefaultFont;
            FontSize = Font.GetFontSize(SadConsole.GameHost.Instance.DefaultFontSize);
        }

        /// <summary>
        /// Creates a new entity, copying the provided appearance to this entity.
        /// </summary>
        /// <param name="appearance">The appearance of the entity.</param>
        /// <param name="layer">The rendering order. Lower values are under higher values.</param>
        public EntityLite(ColoredGlyph appearance, int layer): this(appearance.Foreground, appearance.Background, appearance.Glyph, layer) { }


        /// <inheritdoc />
        protected override void OnPositionChanged(Point oldPosition, Point newPosition)
        {
            base.OnPositionChanged(oldPosition, newPosition);
            Appearance.IsDirty = true;
        }

        /// <inheritdoc />
        public override void UpdateAbsolutePosition()
        {
            if (UsePixelPositioning)
                AbsolutePosition = Position;
            else
                AbsolutePosition = FontSize * Position;
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
