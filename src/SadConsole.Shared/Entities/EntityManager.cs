using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Microsoft.Xna.Framework;


namespace SadConsole.Entities
{
    /// <summary>
    /// Keeps a list of entities in sync with the parent <see cref="SurfaceBase"/>.
    /// </summary>
    public class EntityManager
    {
        private Rectangle _cachedView;
        private ScrollingConsole _parent;

        /// <summary>
        /// The entities this manager manages.
        /// </summary>
        public ObservableCollection<Entity> Entities { get; } = new ObservableCollection<Entity>();

        /// <summary>
        /// The entities this manager manages.
        /// </summary>
        public ObservableCollection<Zone> Zones { get; } = new ObservableCollection<Zone>();

        /// <summary>
        /// The entities this manager manages.
        /// </summary>
        public ObservableCollection<Hotspot> Hotspots { get; } = new ObservableCollection<Hotspot>();


        /// <summary>
        /// Creates a new entity manager.
        /// </summary>
        public EntityManager(ScrollingConsole parent)
        {
            _cachedView = default;
            _parent = parent;

            Entities.CollectionChanged += EntitiesOnCollectionChanged;
            Zones.CollectionChanged += EntitiesOnCollectionChanged;
            Hotspots.CollectionChanged += EntitiesOnCollectionChanged;
        }

        private void EntitiesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        ((Console) item).Parent = _parent;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        ((Console)item).Parent = null;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.NewItems)
                        ((Console)item).Parent = _parent;
                    foreach (var item in e.OldItems)
                        ((Console)item).Parent = null;

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException("Calling Clear in this object is not supported. Please use the RemoveAll extension method.");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Sync();
        }

        /// <summary>
        /// Syncs the visibility of all objects with the viewport of the parent surface.
        /// </summary>
        public void Update()
        {
            if (_cachedView != _parent.ViewPort)
                Sync();

            else
            {
                foreach (var entity in Entities)
                    entity.IsVisible = _cachedView.Contains(entity.Position);
            }
        }

        /// <summary>
        /// Syncs all entity positions and visibility with the parent surface.
        /// </summary>
        public void Sync()
        {
            _cachedView = _parent.ViewPort;
            foreach (var entity in Entities)
            {
                entity.PositionOffset = new Point(-_cachedView.Location.X, -_cachedView.Location.Y);
                entity.IsVisible = _cachedView.Contains(entity.Position);
            }
        }
    }
}
