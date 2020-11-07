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
    public class EntityLiteManager : Components.UpdateComponent
    {
        [DataMember(Name = "Entities")]
        private List<EntityLite> _entities = new List<EntityLite>();
        private List<EntityLite> _entitiesVisible = new List<EntityLite>();
        private IScreenSurface _screen;
        private Rectangle _offsetAreaPixels;

        private Rectangle _screenCachedView;
        private Font _screenCachedFont;
        private Point _screenCachedFontSize;

        /// <summary>
        /// The entities associated with this manager.
        /// </summary>
        public IReadOnlyList<EntityLite> Entities => _entities;

        /// <summary>
        /// The entities within the visible portion of the parent surface.
        /// </summary>
        public IReadOnlyList<EntityLite> EntitiesVisible => _entities;

        /// <summary>
        /// When <see langword="true"/>, indicates that this object needs to be redrawn; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// When <see langword="true"/>, indicates this manager should call <see cref="IScreenObject.Update(TimeSpan)"/> on each entity; otherwise <see langword="false"/>.
        /// </summary>
        public bool DoEntityUpdate { get; set; } = true;

        /// <summary>
        /// Internal use only
        /// </summary>
        public Renderers.IRenderStep RenderStep;

        /// <summary>
        /// Adds an entity to this manager.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(EntityLite entity)
        {
            if (_entities.Contains(entity)) return;

            _entities.Add(entity);

            if (IsEntityVisible(entity.Position, entity.UsePixelPositioning))
                _entitiesVisible.Add(entity);

            entity.PositionChanged += Entity_PositionChanged;
            entity.IsDirtyChanged += Entity_IsDirtyChanged;
        }

        /// <summary>
        /// Removes an entity to this manager.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(EntityLite entity)
        {
            if (!_entities.Contains(entity)) return;

            entity.PositionChanged -= Entity_PositionChanged;
            entity.IsDirtyChanged -= Entity_IsDirtyChanged;

            _entities.Remove(entity);
            _entitiesVisible.Remove(entity);
        }

        /// <inheritdoc/>
        public override void OnAdded(IScreenObject host)
        {
            if (_screen != null) throw new Exception("Component has already been added to a host.");
            if (!(host is IScreenSurface surface)) throw new ArgumentException($"Must add this component to a type that implements {nameof(IScreenSurface)}");

            RenderStep?.Dispose();
            RenderStep = GameHost.Instance.GetRendererStep("entitylite");
            
            surface.Renderer.AddRenderStep(RenderStep);
            _screen = surface;
            UpdateCachedVisibilityArea();
        }

        /// <inheritdoc/>
        public override void OnRemoved(IScreenObject host)
        {
            ((IScreenSurface)host).Renderer.RemoveRenderStep(RenderStep);
            RenderStep?.Dispose();
            RenderStep = null;
            _screen = null;
            _screenCachedFont = null;
            _screenCachedFontSize = Point.None;
            _screenCachedView = Rectangle.Empty;
        }

        /// <inheritdoc/>
        public override void Update(IScreenObject host, TimeSpan delta)
        {
            // View or font changed on parent surface, re-evaluate everything
            if (_screenCachedFont != _screen.Font || _screenCachedFontSize != _screen.FontSize || _screenCachedView != _screen.Surface.View)
            {
                _screenCachedFont = _screen.Font;
                _screenCachedFontSize = _screen.FontSize;
                _screenCachedView = _screen.Surface.View;

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
                if (DoEntityUpdate)
                    Entities[i].Update(delta);

                if (Entities[i].Appearance.IsDirty)
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
            {
                Point renderPosition = position - _offsetAreaPixels.Position;
                return new Rectangle(renderPosition.X, renderPosition.Y, _screen.FontSize.X, _screen.FontSize.Y);
            }
            else
            {
                Point renderPosition = position - _screen.Surface.View.Position;
                return _screen.Font.GetRenderRect(renderPosition.X, renderPosition.Y, _screen.FontSize);
            }
        }

        private void Entity_PositionChanged(object sender, ValueChangedEventArgs<SadRogue.Primitives.Point> e)
        {
            IsDirty = true;
            SetEntityVisibility((EntityLite)sender);
        }

        private void Entity_IsDirtyChanged(object sender, EventArgs e) =>
            IsDirty |= ((EntityLite)sender).IsDirty;

        private void SetEntityVisibility(EntityLite entity)
        {
            bool isVisible = IsEntityVisible(entity.Position, entity.UsePixelPositioning);
            bool contains = _entitiesVisible.Contains(entity);

            if (isVisible == contains) return;

            IsDirty = true;

            // became visible
            if (isVisible)
                _entitiesVisible.Add(entity);
            else
                _entitiesVisible.Remove(entity);
        }

        private void UpdateCachedVisibilityArea() =>
            _offsetAreaPixels = _screen.AbsoluteArea.WithPosition(_screen.Surface.ViewPosition * _screen.FontSize).Expand(_screen.FontSize.X, _screen.FontSize.Y);

        private bool IsEntityVisible(Point position, bool isPixel) =>
            isPixel ? _offsetAreaPixels.Contains(position) : _screen.Surface.View.Contains(position);
    }
}
