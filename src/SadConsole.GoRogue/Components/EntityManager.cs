#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SadConsole.Input;
using SadConsole.Entities;
using SadConsole.GameObjects;

namespace SadConsole.Components
{
    /// <summary>
    /// Manages one or more <see cref="Entity"/> objects in relation to a <see cref="Console"/> and provides events for <see cref="Hotspot"/> and <see cref="Zone"/> interactions.
    /// </summary>
    public class EntityManager: ConsoleComponent
    {
        private bool _hasBeenAdded;
        private Rectangle _cachedView;
        private Dictionary<Entity, EntityState> _entityStates;
        private Dictionary<Point, Entity[]> _entityByPosition;
        private Console _console;
        
        /// <summary>
        /// An event to indicate that an entity entered a hotspot.
        /// </summary>
        public event EventHandler<HotspotEventArgs> EnterHotspot;

        /// <summary>
        /// An event to indicate that an entity exited a hotspot.
        /// </summary>
        public event EventHandler<HotspotEventArgs> ExitHotspot;

        /// <summary>
        /// An event to indicate that an entity entered a zone.
        /// </summary>
        public event EventHandler<ZoneEventArgs> EnterZone;

        /// <summary>
        /// An event to indicate that an entity exited a zone.
        /// </summary>
        public event EventHandler<ZoneEventArgs> ExitZone;

        /// <summary>
        /// An event to indicate that an entity moved within a zone.
        /// </summary>
        public event EventHandler<ZoneMoveEventArgs> MoveZone;

        /// <summary>
        /// An event to indicate that an entity moved.
        /// </summary>
        public event EventHandler<Entity.EntityMovedEventArgs> EntityMoved;

        /// <summary>
        /// When <see langword="true"/>, indicates that the attached console is <see cref="ScrollingConsole"/>.
        /// </summary>
        protected bool HasViewport { get; private set; }


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
        /// Creates a new game object manager.
        /// </summary>
        public EntityManager()
        {
            _cachedView = default;
            _entityStates = new Dictionary<Entity, EntityState>();
            _entityByPosition = new Dictionary<Point, Entity[]>();

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
                    {
                        if (((Console)item).Parent != null) throw new Exception("Object can't be parented to another and added to the entity manager.");

                        if (item is Entity ent)
                        {
                            var state = new EntityState();

                            state.Position = ent.Position;
                            state.IsInHotspot = IsPositionHotspot(state.Position, out Hotspot spot);
                            state.Hotspot = spot;
                            state.IsInZone = IsPositionZone(state.Position, out Zone zone);
                            state.Zone = zone;

                            ent.Moved += Entity_Moved;

                            _entityStates.Add(ent, state);
                            UpdateEntityPositionCollection(ent);
                            OnEntityAdded(_console, ent);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:

                    foreach (var item in e.OldItems)
                    {
                        if (item is Entity ent)
                        {
                            _entityStates.Remove(ent);
                            RemoveEntityPositionCollection(ent, ent.Position);
                            ent.Moved -= Entity_Moved;
                            OnEntityRemoved(_console, ent);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:

                    foreach (var item in e.OldItems)
                    {
                        if (item is Entity ent)
                        {
                            _entityStates.Remove(ent);
                            RemoveEntityPositionCollection(ent, ent.Position);
                            ent.Moved -= Entity_Moved;
                            OnEntityRemoved(_console, ent);
                        }
                    }

                    foreach (var item in e.NewItems)
                    { 
                        if (((Console)item).Parent != null) throw new Exception("Object can't be parented to another and added to the entity manager.");

                        if (item is Entity ent)
                        {
                            var state = new EntityState();

                            state.Position = ent.Position;
                            state.IsInHotspot = IsPositionHotspot(state.Position, out Hotspot spot);
                            state.Hotspot = spot;
                            state.IsInZone = IsPositionZone(state.Position, out Zone zone);
                            state.Zone = zone;

                            ent.Moved += Entity_Moved;

                            _entityStates.Add(ent, state);
                            UpdateEntityPositionCollection(ent);
                            OnEntityAdded(_console, ent);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException("Calling Clear in this object is not supported. Please use the RemoveAll extension method.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Entity_Moved(object sender, Entity.EntityMovedEventArgs e) =>
            EvaluateEntityState((Entity)sender);

        private bool IsPositionZone(in Point point, out Zone zone)
        {
            foreach (var z in Zones)
            {
                if (z.Area.Contains(point))
                {
                    zone = z;
                    return true;
                }
            }

            zone = null;
            return false;
        }

        private bool IsPositionHotspot(in Point point, out Hotspot hotspot)
        {
            foreach (var h in Hotspots)
            {
                if (h.Positions.Contains(point))
                {
                    hotspot = h;
                    return true;
                }
            }

            hotspot = null;
            return false;
        }

        /// <inheritdoc />
        public override void Draw(Console console, TimeSpan delta)
        {
            foreach (var entity in Entities)
            {
                if (entity.IsVisible)
                    entity.Draw(delta);
            }
        }

        /// <summary>
        /// Syncs all entity positions and visibility with the component host.
        /// </summary>
        public override void Update(Console console, TimeSpan delta)
        {
            Point offsetPosition;

            // If we have a viewport cache viewport and set offsetposition
            if (HasViewport)
            {
                var parentViewPort = ((IConsoleViewPort)console).ViewPort;

                _cachedView = parentViewPort;

                offsetPosition = new Point(-_cachedView.Location.X, -_cachedView.Location.Y);
            }
            else
                offsetPosition = console.CalculatedPosition;

            // Loop through entities and call update and raise events
            foreach (var entity in Entities)
            {
                entity.PositionOffset = offsetPosition;

                if (HasViewport)
                {
                    entity.IsVisible = _cachedView.Contains(entity.Position);
                }
                else
                {
                    entity.IsVisible = entity.Position.X >= 0 && entity.Position.Y >= 0 &&
                                       entity.Position.X < console.Width && entity.Position.Y < console.Height;
                }

                entity.Update(delta);
            }
        }

        /// <summary>
        /// Returns each entity at the specified position.
        /// </summary>
        /// <param name="position">The position to get entities from.</param>
        /// <returns>A collecction of entities at the specified position.</returns>
        public IEnumerable<Entity> GetEntities(Point position)
        {
            if (_entityByPosition.ContainsKey(position))
            {
                var entities = _entityByPosition[position];
                foreach (var item in entities)
                    yield return item;
            }
        }

        private void EvaluateEntityState(Entity entity)
        {
            var state = _entityStates[entity];

            // Check if entity has moved
            if (entity.Position != state.Position)
            {
                var isHotspot = IsPositionHotspot(entity.Position, out var spot);
                var isZone = IsPositionZone(entity.Position, out var zone);

                OnEntityMoved(entity, entity.Position, state.Position);
                EntityMoved?.Invoke(this, new Entity.EntityMovedEventArgs(entity, state.Position));

                if (!state.IsDisabled)
                {
                    // Still in zone
                    if (state.IsInZone && isZone)
                    {
                        // Same zone -- trigger move event
                        if (state.Zone == zone)
                        {
                            OnEntityMoveZone(_console, zone, entity, entity.Position, state.Position);
                            MoveZone?.Invoke(this, new ZoneMoveEventArgs(_console, zone, entity, entity.Position, state.Position));
                        }

                        // New zone -- trigger exit and enter event.
                        else
                        {
                            OnEntityExitZone(_console, state.Zone, entity, entity.Position);
                            OnEntityEnterZone(_console, zone, entity, entity.Position);
                            ExitZone?.Invoke(this, new ZoneEventArgs(_console, state.Zone, entity, state.Position));
                            EnterZone?.Invoke(this, new ZoneEventArgs(_console, zone, entity, entity.Position));

                            state.Zone = zone;
                        }
                    }

                    // Left zone
                    else if (state.IsInZone)
                    {
                        OnEntityExitZone(_console, state.Zone, entity, entity.Position);
                        ExitZone?.Invoke(this, new ZoneEventArgs(_console, state.Zone, entity, state.Position));
                        state.IsInZone = false;
                        state.Zone = null;
                    }

                    // Entered zone
                    else if (isZone)
                    {
                        OnEntityEnterZone(_console, zone, entity, entity.Position);
                        EnterZone?.Invoke(this, new ZoneEventArgs(_console, zone, entity, entity.Position));
                        state.IsInZone = true;
                        state.Zone = zone;
                    }

                    // Still in hotspot
                    if (state.IsInHotspot && isHotspot)
                    {
                        OnEntityExitHotspot(_console, state.Hotspot, entity, state.Position);
                        OnEntityEnterHotspot(_console, spot, entity, entity.Position);
                        ExitHotspot?.Invoke(this, new HotspotEventArgs(_console, state.Hotspot, entity, state.Position));
                        EnterHotspot?.Invoke(this, new HotspotEventArgs(_console, spot, entity, entity.Position));

                        state.Hotspot = spot;
                    }

                    // Left hotspot
                    else if (state.IsInHotspot)
                    {
                        OnEntityExitHotspot(_console, state.Hotspot, entity, state.Position);
                        ExitHotspot?.Invoke(this, new HotspotEventArgs(_console, state.Hotspot, entity, state.Position));

                        state.IsInHotspot = false;
                        state.Hotspot = null;
                    }
                    else if (isHotspot)
                    {
                        OnEntityEnterHotspot(_console, spot, entity, entity.Position);
                        EnterHotspot?.Invoke(this, new HotspotEventArgs(_console, spot, entity, entity.Position));

                        state.IsInHotspot = true;
                        state.Hotspot = spot;
                    }
                }

                RemoveEntityPositionCollection(entity, state.Position);
                state.Position = entity.Position;
                UpdateEntityPositionCollection(entity);
            }
        }

        private void UpdateEntityPositionCollection(Entity entity)
        {
            if (_entityByPosition.ContainsKey(entity.Position))
            {
                var entities = _entityByPosition[entity.Position];
                var newList = new Entity[entities.Length];
                entities.CopyTo(newList, 0);
                newList[newList.Length] = entity;
                _entityByPosition[entity.Position] = newList;
            }
            else
                _entityByPosition.Add(entity.Position, new[] { entity });
        }

        private void RemoveEntityPositionCollection(Entity entity, Point oldPosition)
        {
            if (_entityByPosition.ContainsKey(oldPosition))
            {
                var entities = _entityByPosition[oldPosition];
                if (entities.Length == 1)
                    _entityByPosition.Remove(oldPosition);
                else
                {
                    var newList = new Entity[entities.Length - 1];
                    int index = 0;
                    foreach (var item in entities)
                    {
                        if (item != entity)
                        {
                            newList[index] = item;
                            index += 1;
                        }
                    }

                }
            }
        }

        /// <inheritdoc />
        public override void ProcessKeyboard(Console console, Keyboard info, out bool handled)
        {
            foreach (var entity in Entities)
            {
                if (entity.ProcessKeyboard(info))
                {
                    handled = true;
                    return;
                }
            }

            handled = false;
        }

        /// <inheritdoc />
        public override void ProcessMouse(Console console, MouseConsoleState state, out bool handled)
        {
            foreach (var entity in Entities)
            {
                if (entity.ProcessMouse(new MouseConsoleState(entity, state.Mouse)))
                {
                    handled = true;
                    return;
                }
            }

            handled = false;
        }


        /// <inheritdoc />
        public override void OnAdded(Console console)
        {
            if (_hasBeenAdded) throw new Exception("The entity manager does not support being added to two different consoles. Remove this component from the previous console before adding it to another.");

            if (console is IConsoleViewPort view)
            {
                HasViewport = true;
                _cachedView = view.ViewPort;
            }

            _console = console;
        }

        /// <inheritdoc />
        public override void OnRemoved(Console console)
        {
            _hasBeenAdded = false;
            _cachedView = default;
            _console = null;
        }

        /// <summary>
        /// Prevents an entity from being processed with the <see cref="Hotspots"/> and <see cref="Zones"/>.
        /// </summary>
        /// <param name="entity">The entity to disable.</param>
        public void DisableEntity(Entity entity)
        {
            if (_entityStates.ContainsKey(entity)) throw new Exception("Entity is not managed by this entity manager.");

            _entityStates[entity].IsDisabled = true;
        }

        /// <summary>
        /// Enables the entity to be processed with with the <see cref="Hotspots"/> and <see cref="Zones"/>.
        /// </summary>
        /// <param name="entity">The entity to disable.</param>
        public void EnableEntity(Entity entity)
        {
            if (_entityStates.ContainsKey(entity)) throw new Exception("Entity is not managed by this entity manager.");

            _entityStates[entity].IsDisabled = false;
        }

        /// <summary>
        /// Returns <see langword="true"/> when the entity has been disabled by <see cref="DisableEntity(Entity)"/>; otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns><see langword="true"/> when the entity is disabled.</returns>
        public bool IsEntityDisabled(Entity entity)
        {
            if (_entityStates.ContainsKey(entity)) throw new Exception("Entity is not managed by this entity manager.");

            return _entityStates[entity].IsDisabled;
        }

        /// <summary>
        /// Called when an entity is added to the manager.
        /// </summary>
        /// <param name="host">The console this manager is attached to.</param>
        /// <param name="entity">The entity added.</param>
        protected virtual void OnEntityAdded(Console host, Entity entity) { }

        /// <summary>
        /// Called when an entity is removed from the manager.
        /// </summary>
        /// <param name="host">The console this manager is attached to.</param>
        /// <param name="entity">The entity removed.</param>
        protected virtual void OnEntityRemoved(Console host, Entity entity) { }

        /// <summary>
        /// Called when an entity enters a hotspot.
        /// </summary>
        /// <param name="host">The host console that the hotspot and entity share.</param>
        /// <param name="hotspot">The hotspot the entity entered.</param>
        /// <param name="entity">The entity that entered a hotspot.</param>
        /// <param name="triggeredPosition">The position within the <see cref="Hotspot.Positions"/>.</param>
        protected virtual void OnEntityEnterHotspot(Console host, Hotspot hotspot, Entity entity, Point triggeredPosition) { }

        /// <summary>
        /// Called when an entity exits a hotspot.
        /// </summary>
        /// <param name="host">The host console that the hotspot and entity share.</param>
        /// <param name="hotspot">The hotspot the entity exited.</param>
        /// <param name="entity">The entity that exited a hotspot.</param>
        /// <param name="triggeredPosition">The position within the <see cref="Hotspot.Positions"/>.</param>
        protected virtual void OnEntityExitHotspot(Console host, Hotspot hotspot, Entity entity, Point triggeredPosition) { }

        /// <summary>
        /// Called when an entity enters a zone.
        /// </summary>
        /// <param name="host">The host console that the zone and entity share.</param>
        /// <param name="zone">The zone the entity entered.</param>
        /// <param name="entity">The entity that entered the zone.</param>
        /// <param name="triggeredPosition">The position the entity entered.</param>
        protected virtual void OnEntityEnterZone(Console host, Zone zone, Entity entity, Point triggeredPosition) { }

        /// <summary>
        /// Called when an entity enters a zone.
        /// </summary>
        /// <param name="host">The host console that the zone and entity share.</param>
        /// <param name="zone">The zone the entity exited.</param>
        /// <param name="entity">The entity that exited the zone.</param>
        /// <param name="triggeredPosition">The new position the entity left.</param>
        protected virtual void OnEntityExitZone(Console host, Zone zone, Entity entity, Point triggeredPosition) { }

        /// <summary>
        /// Called when an entity moves within a zone.
        /// </summary>
        /// <param name="host">The host console that the zone and entity share.</param>
        /// <param name="zone">The zone the entity moved in.</param>
        /// <param name="entity">The entity that moved in the zone.</param>
        /// <param name="newPosition">The position the entity moved to.</param>
        /// <param name="oldPosition">The position the entity moved from.</param>
        protected virtual void OnEntityMoveZone(Console host, Zone zone, Entity entity, Point newPosition, Point oldPosition) { }

        /// <summary>
        /// Creates a new event args for the entity movement.
        /// </summary>
        /// <param name="entity">The entity associated with the event.</param>
        /// <param name="newPosition">The position the entity moved to.</param>
        /// <param name="oldPosition">The position the entity moved from.</param>
        protected virtual void OnEntityMoved(Entity entity, Point newPosition, Point oldPosition) { }

        private class EntityState
        {
            public Point Position { get; set; }
            public bool IsInZone { get; set; }
            public Zone Zone { get; set; }
            public bool IsInHotspot { get; set; }
            public Hotspot Hotspot { get; set; }
            public bool IsDisabled { get; set; }
        }

        /// <summary>
        /// Contains event data for a <see cref="Hotspot"/> and <see cref="Entity"/> interaction.
        /// </summary>
        public class HotspotEventArgs
        {
            /// <summary>
            /// The hotspot associated with the event.
            /// </summary>
            public readonly Hotspot Hotspot;
            
            /// <summary>
            /// The entity associated with the event.
            /// </summary>
            public readonly Entity Entity;

            /// <summary>
            /// The host console that the hotspot and entity share.
            /// </summary>
            public readonly Console Host;

            /// <summary>
            /// The position within the <see cref="Hotspot.Positions"/> associated with the event.
            /// </summary>
            public readonly Point TriggeredPosition;

            /// <summary>
            /// Creates a new event args for a hotspot interaction.
            /// </summary>
            /// <param name="host">The host console that the hotspot and entity share.</param>
            /// <param name="hotspot">The hotspot associated with the event.</param>
            /// <param name="entity">The entity associated with the event.</param>
            /// <param name="triggeredPosition">The position within the <see cref="Hotspot.Positions"/> associated with the event.</param>
            public HotspotEventArgs(Console host, Hotspot hotspot, Entity entity, Point triggeredPosition)
            {
                Host = host;
                Hotspot = hotspot;
                Entity = entity;
                TriggeredPosition = triggeredPosition;
            }
        }

        /// <summary>
        /// Contains event data for a <see cref="Zone"/> and <see cref="Entity"/> interaction.
        /// </summary>
        public class ZoneEventArgs
        {
            /// <summary>
            /// The zone associated with the event.
            /// </summary>
            public readonly Zone Zone;

            /// <summary>
            /// The entity associated with the event.
            /// </summary>
            public readonly Entity Entity;

            /// <summary>
            /// The host console that the zone and entity share.
            /// </summary>
            public readonly Console Host;

            /// <summary>
            /// The position within the zone associated with the event.
            /// </summary>
            public readonly Point TriggeredPosition;

            /// <summary>
            /// Creates a new event args for a zone interaction.
            /// </summary>
            /// <param name="host">The host console that the zone and entity share.</param>
            /// <param name="zone">The zone associated with the event.</param>
            /// <param name="entity">The entity associated with the event.</param>
            /// <param name="triggeredPosition">The new position within the zone associated with the event.</param>
            public ZoneEventArgs(Console host, Zone zone, Entity entity, Point triggeredPosition)
            {
                Host = host;
                Zone = zone;
                Entity = entity;
                TriggeredPosition = triggeredPosition;
            }
        }

        /// <summary>
        /// Contains event data for a <see cref="Zone"/> and <see cref="Entity"/> interaction.
        /// </summary>
        public class ZoneMoveEventArgs : ZoneEventArgs
        {

            /// <summary>
            /// The position within the zone that the entity moved from.
            /// </summary>
            public readonly Point MovedFromPosition;

            /// <summary>
            /// Creates a new event args for a zone movement event.
            /// </summary>
            /// <param name="host">The host console that the zone and entity share.</param>
            /// <param name="zone">The zone associated with the event.</param>
            /// <param name="entity">The entity associated with the event.</param>
            /// <param name="triggeredPosition">The new position within the zone associated with the event.</param>
            /// <param name="movedFromPosition">The position within the zone that the entity moved from.</param>
            public ZoneMoveEventArgs(Console host, Zone zone, Entity entity, Point triggeredPosition, Point movedFromPosition): base(host, zone, entity, triggeredPosition)
            {
                MovedFromPosition = movedFromPosition;
            }
        }
    }
}
