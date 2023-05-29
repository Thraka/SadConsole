using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.Effects;
using SadRogue.Primitives;

namespace SadConsole.Entities;

/// <summary>
/// A positioned and animated game object.
/// </summary>
//[JsonConverter(typeof(EntityJsonConverter))]
[DataContract]
public class Entity : ScreenObject, IHasID
{
    private static uint _idGenerator;

    // TODO Change this to where Position/Center/Absolute values all come from this object instead of the AnimatedScreenSurface
    private SingleCell? _appearanceSingleCell;
    private Animated? _appearanceSurface;
    private bool _isSingleCell;
    private bool _usePixelPositioning;

    /// <summary>
    /// Raised when the <see cref="IsDirty"/> property changes value.
    /// </summary>
    public event EventHandler? IsDirtyChanged;

    /// <summary>
    /// A friendly name of the game object.
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The drawing layer this entity is drawn at
    /// </summary>
    [DataMember]
    public int ZIndex { get; set; }

    /// <summary>
    /// Indicates this entity's visual appearance has changed.
    /// </summary>
    public bool IsDirty
    {
        get
        {
            if (IsSingleCell)
                return _appearanceSingleCell.IsDirty;
            else
                return _appearanceSurface.IsDirty;
        }
        
        set
        {
            if (IsSingleCell)
                _appearanceSingleCell.IsDirty = value;
            else
                _appearanceSurface.IsDirty = value;

            OnIsDirtyChanged();
        }
    }

    /// <summary>
    /// Treats the <see cref="IScreenObject.Position"/> of the entity as if it is pixels and not cells.
    /// </summary>
    public bool UsePixelPositioning
    {
        get => _usePixelPositioning;
        set
        {
            _usePixelPositioning = value;
            UpdateAbsolutePosition();
        }
    }

    /// <summary>
    /// The appearance of the entity when <see cref="IsSingleCell"/> is <see langword="true"/>.
    /// </summary>
    public SingleCell? AppearanceSingle
    {
        get => _appearanceSingleCell;
        set
        {
            if (IsSingleCell && value == null) throw new NullReferenceException($"Cannot set the {nameof(AppearanceSingle)} to null when {nameof(IsSingleCell)} is true. First set {nameof(IsSingleCell)} to false.");

            _appearanceSingleCell = value;
        }
    }

    /// <summary>
    /// The appearance of the entity when <see cref="IsSingleCell"/> is <see langword="false"/>.
    /// </summary>
    public Animated? AppearanceSurface
    {
        get => _appearanceSurface;
        set
        {
            if (!IsSingleCell && value == null) throw new NullReferenceException($"Cannot set the {nameof(AppearanceSurface)} to null when {nameof(IsSingleCell)} is false. First set {nameof(IsSingleCell)} to true.");

            _appearanceSurface = value;
            UpdateAbsolutePosition();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSingleCell
    {
        [MemberNotNullWhen(true, nameof(_appearanceSingleCell))]
        [MemberNotNullWhen(true, nameof(AppearanceSingle))]
        [MemberNotNullWhen(false, nameof(_appearanceSurface))]
        [MemberNotNullWhen(false, nameof(AppearanceSurface))]
        get => _isSingleCell;
        set
        {
            if (value && _appearanceSingleCell == null) throw new Exception($"{nameof(AppearanceSurface)} must be set to an instance before settings this property to true.");
            if (!value && _appearanceSurface == null) throw new Exception($"{nameof(AppearanceSurface)} must be set to an instance before settings this property to true.");

            _isSingleCell = value;
        }
    }

    uint IHasID.ID { get; } = _idGenerator++;

    /// <summary>
    /// Creates a new entity as an animated surface.
    /// </summary>
    /// <param name="appearance">The surface appearance to use for the entity.</param>
    /// <param name="zIndex">The rendering order. Higher values are drawn on top of lower values.</param>
    public Entity(Animated appearance, int zIndex)
    {
        _appearanceSurface = appearance;
        Children.IsLocked = true;
        ZIndex = zIndex;
    }

    /// <summary>
    /// Creates a new entity as an animated surface.
    /// </summary>
    /// <param name="appearance">The surface appearance to use for the entity.</param>
    /// <param name="zIndex">The rendering order. Higher values are drawn on top of lower values.</param>
    public Entity(AnimatedScreenSurface appearance, int zIndex)
    {
        _appearanceSurface = new Animated(appearance);
        Children.IsLocked = true;
        ZIndex = zIndex;
    }


    /// <summary>
    /// Creates a new entity as a single cell.
    /// </summary>
    /// <param name="appearance">The single cell appearance to use for the entity.</param>
    /// <param name="zIndex">The rendering order. Higher values are drawn on top of lower values.</param>
    public Entity(SingleCell appearance, int zIndex)
    {
        _appearanceSingleCell = appearance;
        _isSingleCell = true;
        Children.IsLocked = true;
        ZIndex = zIndex;
    }

    /// <summary>
    /// Creates a new entity, copying the provided appearance to this entity.
    /// </summary>
    /// <param name="appearance">The appearance of the entity.</param>
    /// <param name="zIndex">The rendering order. Lower values are under higher values.</param>
    public Entity(ColoredGlyph appearance, int zIndex) : this(new SingleCell(appearance), zIndex) { }

    /// <summary>
    /// Creates a new entity, copying the provided appearance to this entity.
    /// </summary>
    /// <param name="foreground">The foreground color of the entity.</param>
    /// <param name="background">The background color of the entity.</param>
    /// <param name="glyph">The glyph color of the entity.</param>
    /// <param name="zIndex">The rendering order. Lower values are under higher values.</param>
    public Entity(Color foreground, Color background, int glyph, int zIndex) : this(new SingleCell(foreground, background, glyph), zIndex) { }

    [JsonConstructor]
    private Entity(SingleCell? appearanceSingleCell, Animated? appearanceSurface, bool isSingleCell)
    {
        _appearanceSingleCell = appearanceSingleCell;
        _appearanceSurface = appearanceSurface;
        _isSingleCell = isSingleCell;
    }

    /// <inheritdoc />
    protected override void OnPositionChanged(Point oldPosition, Point newPosition)
    {
        base.OnPositionChanged(oldPosition, newPosition);

        IsDirty = true;

        if (IsSingleCell)
            _appearanceSingleCell.IsDirty = true;
        else
            _appearanceSurface.IsDirty = true;
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
    /// If an effect is applied to the cell, updates the effect.
    /// </summary>
    /// <param name="delta"></param>
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (IsSingleCell)
            _appearanceSingleCell.Update(delta);
        else
            _appearanceSurface.Update(delta);
    }

    /// <summary>
    /// Returns the name of the entity prefixed with "Entity - ".
    /// </summary>
    /// <returns>The name.</returns>
    public override string ToString() =>
        Name;

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
    /// An entity that is a single cell.
    /// </summary>
    public class SingleCell
    {
        private ColoredGlyph _glyph;

        [DataMember(Name = "Effect")]
        private ICellEffect? _effect;

        [DataMember(Name = "Appearance")]
        private ColoredGlyphState _effectState;

        /// <summary>
        /// When <see langword="true"/>, indicates that this cell is dirty and needs to be redrawn.
        /// </summary>
        public bool IsDirty { get => _glyph.IsDirty; set => _glyph.IsDirty = value; }

        /// <summary>
        /// Represents what the entity looks like.
        /// </summary>
        public ColoredGlyph Appearance
        {
            get => _glyph;

            [MemberNotNull(nameof(_glyph))]
            protected set
            {
                _glyph = value ?? throw new System.NullReferenceException("Appearance cannot be null.");
                IsDirty = true;
            }
        }


        /// <summary>
        /// An effect that can be applied to the <see cref="Appearance"/>.
        /// </summary>
        public ICellEffect? Effect
        {
            get => _effect;
            set
            {
                if (_effect != null)
                {
                    if (_effect.RestoreCellOnRemoved)
                        _effectState.RestoreState(ref _glyph);
                }

                if (value == null)
                {
                    _effectState = default;
                    _effect = null;
                }
                else
                {
                    _effectState = new ColoredGlyphState(_glyph);
                    _effect = value.CloneOnAdd ? value.Clone() : value;
                }
            }
        }

        /// <summary>
        /// Creates a new entity with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground">The foreground color of the entity.</param>
        /// <param name="background">The background color of the entity.</param>
        /// <param name="glyph">The glyph color of the entity.</param>
        public SingleCell(Color foreground, Color background, int glyph)
        {
            Appearance = new ColoredGlyph(foreground, background, glyph);
        }

        [JsonConstructor]
        private SingleCell(ColoredGlyphState appearance, ICellEffect effect)
        {
            Appearance = new ColoredGlyph(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror, appearance.IsVisible, appearance.Decorators);
            Effect = effect;
        }

        /// <summary>
        /// Creates a new entity, references the provided glyph as the appearance.
        /// </summary>
        /// <param name="appearance">The appearance of the entity.</param>
        public SingleCell(ColoredGlyph appearance)
        {
            Appearance = appearance;
        }

        /// <summary>
        /// If an effect is applied to the cell, updates the effect.
        /// </summary>
        /// <param name="delta"></param>
        public void Update(TimeSpan delta)
        {
            if (_effect != null && !_effect.IsFinished)
            {
                _effect.Update(delta);
                _effect.ApplyToCell(Appearance, _effectState);

                if (_effect.IsFinished)
                {
                    if (_effect.RemoveOnFinished)
                    {
                        if (_effect.RestoreCellOnRemoved)
                            _effectState.RestoreState(ref _glyph);

                        _effect = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// An entity that is a an animated surface.
    /// </summary>
    public class Animated
    {
        private AnimatedScreenSurface _surface;

        /// <summary>
        /// The animation associated with this animated entity.
        /// </summary>
        public AnimatedScreenSurface Animation
        {
            get => _surface;
        }

        /// <summary>
        /// Represents the collision rectangle for this animated surface which is the size of the animation frame.
        /// </summary>
        public Rectangle DefaultCollisionRectangle
        {
            get => new Rectangle(0, 0, _surface.CurrentFrame.ViewWidth, _surface.CurrentFrame.ViewHeight);
        }

        /// <summary>
        /// A collision rectangle that you can specify.
        /// </summary>
        /// <remarks>
        /// This rectangle should be declared without using the animation center. Only apply the center when you're testing for collision and reading this rectangle.
        /// </remarks>
        public Rectangle CustomCollisionRectangle { get; set; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this animation is dirty and needs to be redrawn.
        /// </summary>
        public bool IsDirty { get => _surface.IsDirty; set => _surface.IsDirty = value; }

        /// <summary>
        /// Creates a new instance of this type from an animated screen surface.
        /// </summary>
        /// <param name="surface">The animation to use.</param>
        public Animated(AnimatedScreenSurface surface)
        {
            _surface = surface;
        }

        /// <summary>
        /// Updates the <see cref="Animation"/>.
        /// </summary>
        /// <param name="delta">The time that has elapsed since this method was last called.</param>
        public void Update(TimeSpan delta) =>
            _surface.Update(delta);
    }

    /// <summary>
    /// Arguments for the entity moved event.
    /// </summary>
    public class EntityMovedEventArgs : EventArgs
    {
        /// <summary>
        /// The entity associated with the event.
        /// </summary>
        public readonly Entity Entity;

        /// <summary>
        /// The position the <see cref="Entity"/> moved from.
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
