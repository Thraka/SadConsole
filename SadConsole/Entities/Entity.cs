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
public partial class Entity : ScreenObject, IHasID
{
    private static uint s_idGenerator;

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
    /// When <see langword="true"/>, indicates that this entity is a single cell entity; otherwise <see langword="false"/> and it's an animated surface entity.
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

    uint IHasID.ID { get; } = s_idGenerator++;

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
