using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole.Surfaces;

namespace SadConsole.Entities
{
    /// <summary>
    /// Keeps a list of entities in sync with the parent <see cref="SurfaceBase"/>.
    /// </summary>
    public class EntityManager: ScreenObject
    {
        private SurfaceBase _surfaceParent;
        private Rectangle _cachedView;

        /// <summary>
        /// The entities this manager manages.
        /// </summary>
        public ObservableCollection<Entity> Entities { get; } = new ObservableCollection<Entity>();
        
        /// <summary>
        /// Creates a new entity manager.
        /// </summary>
        public EntityManager()
        {
            _cachedView = default;
            Children.IsLocked = true;

            Entities.CollectionChanged += EntitiesOnCollectionChanged;
        }

        private void EntitiesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        ((Entity) item).Parent = Parent;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        ((Entity)item).Parent = null;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.NewItems)
                        ((Entity)item).Parent = Parent;
                    foreach (var item in e.OldItems)
                        ((Entity)item).Parent = null;

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Sync();
        }

        /// <inheritdoc />
        protected override void OnParentChanged(ScreenObject oldParent, ScreenObject newParent)
        {
            _surfaceParent = newParent as SurfaceBase;
            IsPaused = _surfaceParent == null;

            if (_surfaceParent != null)
            {
                foreach (var entity in Entities)
                    if (entity.Parent != _surfaceParent)
                        entity.Parent = _surfaceParent;

                Sync();
            }
        }

        /// <inheritdoc />
        public override void Update(TimeSpan timeElapsed)
        {
            if (IsPaused) return;

            if (_cachedView != _surfaceParent.ViewPort)
                Sync();
            else
                foreach (var entity in Entities)
                    entity.IsVisible = _cachedView.Contains(entity.Position);
        }

        /// <summary>
        /// Syncs all entity positions and visibility with the parent surface.
        /// </summary>
        public void Sync()
        {
            if (_surfaceParent == null) return;

            _cachedView = _surfaceParent.ViewPort;
            foreach (var entity in Entities)
            {
                entity.PositionOffset = new Point(-_cachedView.Location.X, -_cachedView.Location.Y);
                entity.IsVisible = _cachedView.Contains(entity.Position);
            }
        }
    }
}
