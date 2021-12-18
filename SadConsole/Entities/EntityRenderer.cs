using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Entities
{
    /// <summary>
    /// Manages a set of entities. Adds a render step and only renders the entities that are in the parent <see cref="IScreenSurface"/> visible area.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("Entity host")]
    public class Renderer : Components.UpdateComponent, Components.IComponent
    {
        /// <summary>
        /// Indicatest that the entity renderer has been added to a parent object.
        /// </summary>
        protected bool IsAttached;

        private List<Entity> _entityHolding;

        /// <summary>
        /// The entities to process.
        /// </summary>
        [DataMember(Name = "Entities")]
        protected List<Entity> _entities = new List<Entity>();

        /// <summary>
        /// The entities currently visible.
        /// </summary>
        protected List<Entity> _entitiesVisible = new List<Entity>();

        /// <summary>
        /// The parent screen hosting this component.
        /// </summary>
        protected IScreenSurface _screen;

        /// <summary>
        /// Cached rectangle for rendering.
        /// </summary>
        protected Rectangle _offsetAreaPixels;

        /// <summary>
        /// A cached copy of the <see cref="ICellSurface.View"/> of the hosting screen surface.
        /// </summary>
        protected Rectangle _screenCachedView;

        /// <summary>
        /// A cached copy of the <see cref="IScreenSurface.Font"/> of the hosting screen surface.
        /// </summary>
        protected IFont _screenCachedFont;

        /// <summary>
        /// A cached copy of the <see cref="IScreenSurface.FontSize"/> of the hosting screen surface.
        /// </summary>
        protected Point _screenCachedFontSize;

        /// <summary>
        /// The entities associated with this manager.
        /// </summary>
        public IReadOnlyList<Entity> Entities => _entities;

        /// <summary>
        /// The entities within the visible portion of the parent surface.
        /// </summary>
        public IReadOnlyList<Entity> EntitiesVisible => _entitiesVisible;

        /// <summary>
        /// When <see langword="true"/>, indicates that this object needs to be redrawn; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// When <see langword="true"/>, indicates this manager should call <see cref="IScreenObject.Update(TimeSpan)"/> on each entity; otherwise <see langword="false"/>.
        /// </summary>
        public bool DoEntityUpdate { get; set; } = true;

        /// <summary>
        /// When <see langword="true"/>, the <see cref="Add(Entity)"/> and <see cref="Remove(Entity)"/> won't check if the entitiy exists before doing its operation.
        /// </summary>
        public bool SkipExistChecks { get; set; } = false;

        /// <summary>
        /// Internal use only
        /// </summary>
        public Renderers.IRenderStep RenderStep;

        /// <summary>
        /// Adds an entity to this manager.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(Entity entity)
        {
            // Temporary holding of the entities if there is no parent
            if (!IsAttached)
            {
                AddHolding(entity);
                return;
            }

            if (SkipExistChecks == false && _entities.Contains(entity)) return;

            _entities.Add(entity);

            SetEntityVisibility(entity);

            entity.PositionChanged += Entity_PositionChanged;
            entity.VisibleChanged += Entity_VisibleChanged;
            entity.IsDirtyChanged += Entity_IsDirtyChanged;

            OnEntityAdded(entity);
            OnEntityChangedPosition(entity, new ValueChangedEventArgs<Point>(Point.None, entity.Position));
        }

        /// <summary>
        /// Adds a collection of entities to this manager.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        public void AddRange(IEnumerable<Entity> entities)
        {
            // Temporary holding of the entities if there is no parent
            if (!IsAttached)
            {
                AddHolding(entities);
                return;
            }

            foreach (Entity entity in entities)
            {
                if (SkipExistChecks || !_entities.Contains(entity))
                {
                    _entities.Add(entity);

                    SetEntityVisibility(entity);

                    OnEntityAdded(entity);
                    OnEntityChangedPosition(entity, new ValueChangedEventArgs<Point>(Point.None, entity.Position));

                    entity.PositionChanged += Entity_PositionChanged;
                    entity.VisibleChanged += Entity_VisibleChanged;
                    entity.IsDirtyChanged += Entity_IsDirtyChanged;
                }
            }

            _entitiesVisible.Sort(CompareEntity);
            IsDirty = true;
        }

        private bool AddHolding(IEnumerable<Entity> entities)
        {
            if (IsAttached) return false;

            foreach (Entity item in entities)
                AddHolding(item);

            return true;
        }

        private bool AddHolding(Entity entity)
        {
            if (IsAttached) return false;

            if (_entityHolding == null)
                _entityHolding = new List<Entity>();

            _entityHolding.Add(entity);

            return true;
        }

        /// <summary>
        /// Removes an entity from this manager.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(Entity entity)
        {
            if (!IsAttached)
            {
                _entityHolding.Remove(entity);
                return;
            }

            if (SkipExistChecks == false && !_entities.Contains(entity)) return;

            entity.PositionChanged -= Entity_PositionChanged;
            entity.VisibleChanged -= Entity_VisibleChanged;
            entity.IsDirtyChanged -= Entity_IsDirtyChanged;

            _entities.Remove(entity);
            _entitiesVisible.Remove(entity);

            OnEntityRemoved(entity);
        }

        /// <summary>
        /// Removes all entities from this renderer.
        /// </summary>
        public void RemoveAll()
        {
            while (_entities.Count != 0)
                Remove(_entities[0]);
        }

        /// <inheritdoc/>
        public override void OnAdded(IScreenObject host)
        {
            if (_screen != null) throw new Exception("Component has already been added to a host.");
            if (!(host is IScreenSurface surface)) throw new ArgumentException($"Must add this component to a type that implements {nameof(IScreenSurface)}");

            if (RenderStep != null)
            {
                surface.RenderSteps.Remove(RenderStep);
                RenderStep.Dispose();
            }
            
            RenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.EntityRenderer);
            RenderStep.SetData(this);
            surface.RenderSteps.Add(RenderStep);
            _screen = surface;
            IsAttached = true;

            if (_entityHolding != null)
            {
                AddRange(_entityHolding);
                _entityHolding = null;
            }

            UpdateCachedVisibilityArea();
        }

        /// <inheritdoc/>
        public override void OnRemoved(IScreenObject host)
        {
            ((IScreenSurface)host).RenderSteps.Remove(RenderStep);
            RenderStep?.Dispose();
            RenderStep = null;
            _screen = null;
            _screenCachedFont = null;
            _screenCachedFontSize = Point.None;
            _screenCachedView = Rectangle.Empty;

            IsAttached = false;

            // Removed, place existing entities into holding
            _entityHolding = _entities;
            _entities = new List<Entity>();
            _entitiesVisible = new List<Entity>();

            // Detatch events
            foreach (Entity entity in _entityHolding)
            {
                entity.PositionChanged -= Entity_PositionChanged;
                entity.VisibleChanged -= Entity_VisibleChanged;
                entity.IsDirtyChanged -= Entity_IsDirtyChanged;
            }
        }

        void SadConsole.Components.IComponent.OnHostUpdated(IScreenObject host) =>
            UpdateCachedVisibilityArea();

        /// <inheritdoc/>
        public override void Update(IScreenObject host, TimeSpan delta)
        {
            // View or font changed on parent surface, re-evaluate everything
            if (_screenCachedFont != _screen.Font || _screenCachedFontSize != _screen.FontSize || _screenCachedView != _screen.Surface.View)
            {
                _screenCachedFont = _screen.Font;
                _screenCachedFontSize = _screen.FontSize;
                _screenCachedView = _screen.Surface.View;

                UpdateCachedVisibilityArea();

                IsDirty = true;

                for (int i = 0; i < Entities.Count; i++)
                {
                    if (DoEntityUpdate)
                        Entities[i].Update(delta);

                    SetEntityVisibility(Entities[i]);
                }

                return;
            }

            for (int i = 0; i < Entities.Count; i++)
            {
                Entity entity = Entities[i];

                if (DoEntityUpdate)
                    entity.Update(delta);

                // Short out if IsDirty has already been marked
                if (!IsDirty && entity.Appearance.IsDirty && IsEntityVisible(entity.Position, entity.UsePixelPositioning))
                    IsDirty = true;
            }
        }

        /// <summary>
        /// Gets a render rectangle for a position.
        /// </summary>
        /// <param name="position">The position of the entity.</param>
        /// <param name="isPixel">Indicates the entity is pixel positioned.</param>
        /// <returns></returns>
        public Rectangle GetRenderRectangle(Point position, bool isPixel)
        {
            if (isPixel)
                return new Rectangle(position.X - (_screen.Surface.ViewPosition.X * _screen.FontSize.X), position.Y - (_screen.Surface.ViewPosition.Y * _screen.FontSize.Y), _screen.FontSize.X, _screen.FontSize.Y);
            else
            {
                Point renderPosition = position - _screen.Surface.View.Position;
                return _screen.Font.GetRenderRect(renderPosition.X, renderPosition.Y, _screen.FontSize);
            }
        }

        private void Entity_IsDirtyChanged(object sender, EventArgs e)
        {
            var entity = (Entity)sender;

            if (IsEntityVisible(entity.Position, entity.UsePixelPositioning))
                IsDirty |= entity.IsDirty;
        }

        private void Entity_VisibleChanged(object sender, EventArgs e)
        {
            var entity = (Entity)sender;

            if (IsEntityVisible(entity.Position, entity.UsePixelPositioning))
                IsDirty = true;
        }

        private void Entity_PositionChanged(object sender, ValueChangedEventArgs<SadRogue.Primitives.Point> e)
        {
            Entity entity = (Entity)sender;

            // Entity was previously (may no longer be) visible, we always redraw since it moved.
            if (IsEntityVisible(e.OldValue, entity.UsePixelPositioning))
                IsDirty = true;

            SetEntityVisibility(entity);
            OnEntityChangedPosition(entity, e);
        }

        /// <summary>
        /// Called when an entity changes position.
        /// </summary>
        /// <param name="entity">The entity that moved.</param>
        /// <param name="e">The previous and new values of the position.</param>
        protected virtual void OnEntityChangedPosition(Entity entity, ValueChangedEventArgs<SadRogue.Primitives.Point> e) { }

        /// <summary>
        /// Called when an entity is added.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void OnEntityAdded(Entity entity) { }

        /// <summary>
        /// Called when an entity is removed.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void OnEntityRemoved(Entity entity) { }

        /// <summary>
        /// Detects a visibility state change of an entity and changes its internal list position.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns><see langword="true"/> when the entity is visible; otherwise <see langword="false"/>.</returns>
        protected bool SetEntityVisibility(Entity entity)
        {
            bool isVisible = IsEntityVisible(entity.Position, entity.UsePixelPositioning);
            bool contains = _entitiesVisible.Contains(entity);

            if (isVisible == contains) return isVisible;

            IsDirty = true;

            // became visible
            if (isVisible)
            {
                _entitiesVisible.Add(entity);
                _entitiesVisible.Sort(CompareEntity);
            }
            else
                _entitiesVisible.Remove(entity);

            return isVisible;
        }

        /// <summary>
        /// Updates the cached view area based on the parent surface.
        /// </summary>
        protected void UpdateCachedVisibilityArea()
        {
            if (!IsAttached) return;

            _offsetAreaPixels = _screen.AbsoluteArea.WithPosition(_screen.Surface.ViewPosition * _screen.FontSize).Expand(_screen.FontSize.X, _screen.FontSize.Y);
        }
            

        private bool IsEntityVisible(Point position, bool isPixel) =>
            isPixel ? _offsetAreaPixels.Contains(position) : _screen.Surface.View.Contains(position);

        private static int CompareEntity(Entity left, Entity right)
        {
            if (left.ZIndex > right.ZIndex)
                return 1;

            if (left.ZIndex < right.ZIndex)
                return -1;

            return 0;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            List<Entity> entitiesBackup = _entities;
            _entities = new List<Entity>(entitiesBackup.Count);

            AddRange(entitiesBackup);

            IsDirty = true;
        }
    }
}
