using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Text;
using SadRogue.Primitives;
using System.Linq;

namespace SadConsole.Entities
{
    /// <summary>
    /// Manages a set of entities. Adds a render step and only renders the entities that are in the parent <see cref="IScreenSurface"/> visible area.
    /// </summary>
    [DataContract]
    public class Manager : Renderer, Components.IComponent
    {
        private Dictionary<Entity, EntityState> _entityStates;
        private Dictionary<Point, Entity[]> _entityByPosition;

        /// <summary>
        /// An event to indicate that an entity entered a zone.
        /// </summary>
        public event EventHandler<ZoneEventArgs> EnterZone;

        /// <summary>
        /// An event to indicate that an entity exited a zone.
        /// </summary>
        public event EventHandler<ZoneEventArgs> ExitZone;


        /// <summary>
        /// The zones in this manager.
        /// </summary>
        [DataMember(Name = "Zones")]
        protected List<Zone> _zones = new List<Zone>();

        /// <summary>
        /// The zones associated with this manager.
        /// </summary>
        public IReadOnlyList<Zone> Zones => _zones;

        /// <summary>
        /// Creates a new manager to handle entity movement within zones.
        /// </summary>
        public Manager()
        {
            _entityStates = new Dictionary<Entity, EntityState>();
            _entityByPosition = new Dictionary<Point, Entity[]>();
        }

        /// <summary>
        /// Adds a zone to this manager.
        /// </summary>
        /// <param name="zone">The zone to add.</param>
        public void Add(Zone zone)
        {
            if (_zones.Contains(zone)) return;

            _zones.Add(zone);
        }

        /// <summary>
        /// Removes a zone from this manager.
        /// </summary>
        /// <param name="zone">The hotspot to remove.</param>
        public void Remove(Zone zone)
        {
            if (!_zones.Contains(zone)) return;

            _zones.Remove(zone);

            foreach (var item in Entities)
            {
                var state = _entityStates[item];
                state.Zone = null;
                state.IsInZone = false;
            }
        }

        /// <summary>
        /// Gets the first entity at the specified position or <see langword="null"/> if there is no entity.
        /// </summary>
        /// <param name="position">The position to get an entity at.</param>
        /// <returns>The first entity if it exists; otherwise it returns <see langword="null"/>.</returns>
        public Entity GetEntityAtPosition(Point position)
        {
            if (_entityByPosition.ContainsKey(position))
                return _entityByPosition[position][0];

            return null;
        }

        /// <summary>
        /// Gets the entities at the specified position or <see langword="null"/> if there aren't any at the position.
        /// </summary>
        /// <param name="position">The position to get an entity at.</param>
        /// <returns>An array of entities if they exist; otherwise <see langword="null"/>.</returns>
        public Entity[] GetEntitiesAtPosition(Point position)
        {
            if (_entityByPosition.ContainsKey(position))
                return _entityByPosition[position].ToArray();

            return null;
        }

        /// <summary>
        /// Returns <see langword="true"/> when there is an entity at the specified position; otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>A value indicating if an entity exists.</returns>
        public bool HasEntityAt(Point position) =>
            _entityByPosition.ContainsKey(position);

        /// <inheritdoc/>
        protected override void OnEntityChangedPosition(Entity entity, ValueChangedEventArgs<Point> e) =>
            EvaluateEntityState(entity);

        /// <summary>
        /// Adds entity state information to an entity when it's added.
        /// </summary>
        /// <param name="entity">The entity that was added.</param>
        protected override void OnEntityAdded(Entity entity)
        {
            var state = new EntityState();
            state.Position = entity.Position;
            state.IsInZone = IsPositionZone(state.Position, out Zone zone);
            state.Zone = zone;
            _entityStates.Add(entity, state);
            UpdateEntityPositionCollection(entity);
        }

        /// <summary>
        /// Removes the entity state information of an entity being removed.
        /// </summary>
        /// <param name="entity">The entity that was removed.</param>
        protected override void OnEntityRemoved(Entity entity)
        {
            _entityStates.Remove(entity);
            RemoveEntityPositionCollection(entity, entity.Position);
        }

        private void EvaluateEntityState(Entity entity)
        {
            var state = _entityStates[entity];

            // Check if entity has moved
            if (entity.Position != state.Position)
            {
                var isZone = IsPositionZone(entity.Position, out var zone);
                
                //EntityMoved?.Invoke(this, new Entity.EntityMovedEventArgs(entity, state.Position));

                if (!state.IsDisabled)
                {
                    // Still in zone
                    if (state.IsInZone && isZone)
                    {
                        // Same zone -- trigger move event
                        if (state.Zone == zone)
                        {
                            OnEntityMoveZone(_screen, zone, entity, entity.Position, state.Position);
                            //MoveZone?.Invoke(this, new ZoneMoveEventArgs(_screen, zone, entity, entity.Position, state.Position));
                        }
                        // New zone -- trigger exit and enter event.
                        else
                        {
                            OnEntityExitZone(_screen, state.Zone, entity, entity.Position);
                            OnEntityEnterZone(_screen, zone, entity, entity.Position);
                            ExitZone?.Invoke(this, new ZoneEventArgs(_screen, state.Zone, entity, state.Position));
                            EnterZone?.Invoke(this, new ZoneEventArgs(_screen, zone, entity, entity.Position));
                            state.Zone = zone;
                        }
                    }
                    // Left zone
                    else if (state.IsInZone)
                    {
                        OnEntityExitZone(_screen, state.Zone, entity, entity.Position);
                        ExitZone?.Invoke(this, new ZoneEventArgs(_screen, state.Zone, entity, state.Position));
                        state.IsInZone = false;
                        state.Zone = null;
                    }
                    // Entered zone
                    else if (isZone)
                    {
                        OnEntityEnterZone(_screen, zone, entity, entity.Position);
                        EnterZone?.Invoke(this, new ZoneEventArgs(_screen, zone, entity, entity.Position));
                        state.IsInZone = true;
                        state.Zone = zone;
                    }

                    //// Still in hotspot
                    //if (state.IsInHotspot && isHotspot)
                    //{
                    //    OnEntityExitHotspot(_screen, state.Hotspot, entity, state.Position);
                    //    OnEntityEnterHotspot(_screen, spot, entity, entity.Position);
                    //    ExitHotspot?.Invoke(this, new HotspotEventArgs(_screen, state.Hotspot, entity, state.Position));
                    //    EnterHotspot?.Invoke(this, new HotspotEventArgs(_screen, spot, entity, entity.Position));
                    //    state.Hotspot = spot;
                    //}
                    //// Left hotspot
                    //else if (state.IsInHotspot)
                    //{
                    //    OnEntityExitHotspot(_screen, state.Hotspot, entity, state.Position);
                    //    ExitHotspot?.Invoke(this, new HotspotEventArgs(_screen, state.Hotspot, entity, state.Position));
                    //    state.IsInHotspot = false;
                    //    state.Hotspot = null;
                    //}
                    //else if (isHotspot)
                    //{
                    //    OnEntityEnterHotspot(_screen, spot, entity, entity.Position);
                    //    EnterHotspot?.Invoke(this, new HotspotEventArgs(_screen, spot, entity, entity.Position));
                    //    state.IsInHotspot = true;
                    //    state.Hotspot = spot;
                    //}
                }
                RemoveEntityPositionCollection(entity, state.Position);
                state.Position = entity.Position;
                UpdateEntityPositionCollection(entity);
            }
        }

        /// <summary>
        /// Called when an entity enters a zone.
        /// </summary>
        /// <param name="host">The host that the zone and entity share.</param>
        /// <param name="zone">The zone the entity entered.</param>
        /// <param name="entity">The entity that entered the zone.</param>
        /// <param name="triggeredPosition">The position the entity entered.</param>
        protected virtual void OnEntityEnterZone(IScreenSurface host, Zone zone, Entity entity, Point triggeredPosition) { }

        /// <summary>
        /// Called when an entity enters a zone.
        /// </summary>
        /// <param name="host">The host that the zone and entity share.</param>
        /// <param name="zone">The zone the entity exited.</param>
        /// <param name="entity">The entity that exited the zone.</param>
        /// <param name="triggeredPosition">The new position the entity left.</param>
        protected virtual void OnEntityExitZone(IScreenSurface host, Zone zone, Entity entity, Point triggeredPosition) { }

        /// <summary>
        /// Called when an entity moves within a zone.
        /// </summary>
        /// <param name="host">The host that the zone and entity share.</param>
        /// <param name="zone">The zone the entity moved in.</param>
        /// <param name="entity">The entity that moved in the zone.</param>
        /// <param name="newPosition">The position the entity moved to.</param>
        /// <param name="oldPosition">The position the entity moved from.</param>
        protected virtual void OnEntityMoveZone(IScreenSurface host, Zone zone, Entity entity, Point newPosition, Point oldPosition) { }

        /// <summary>
        /// Creates a new event args for the entity movement.
        /// </summary>
        /// <param name="entity">The entity associated with the event.</param>
        /// <param name="newPosition">The position the entity moved to.</param>
        /// <param name="oldPosition">The position the entity moved from.</param>
        protected virtual void OnEntityMoved(Entity entity, Point newPosition, Point oldPosition) { }

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
                    _entityByPosition[oldPosition] = newList;
                }
            }
        }

        /// <summary>
        /// Prevents an entity from being processed with the <see cref="Zones"/>.
        /// </summary>
        /// <param name="entity">The entity to disable.</param>
        public void DisableEntity(Entity entity)
        {
            if (_entityStates.ContainsKey(entity)) throw new Exception("Entity is not managed by this entity manager.");
            _entityStates[entity].IsDisabled = true;
        }
        /// <summary>
        /// Enables the entity to be processed with with the <see cref="Zones"/>.
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

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {

        }

        private class EntityState
        {
            public Point Position { get; set; }
            public bool IsInZone { get; set; }
            public Zone Zone { get; set; }
            public bool IsDisabled { get; set; }
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
            /// The host that the zone and entity share.
            /// </summary>
            public readonly IScreenSurface Host;

            /// <summary>
            /// The position within the zone associated with the event.
            /// </summary>
            public readonly Point TriggeredPosition;

            /// <summary>
            /// Creates a new event args for a zone interaction.
            /// </summary>
            /// <param name="host">The host that the zone and entity share.</param>
            /// <param name="zone">The zone associated with the event.</param>
            /// <param name="entity">The entity associated with the event.</param>
            /// <param name="triggeredPosition">The new position within the zone associated with the event.</param>
            public ZoneEventArgs(IScreenSurface host, Zone zone, Entity entity, Point triggeredPosition)
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
            /// <param name="host">The host that the zone and entity share.</param>
            /// <param name="zone">The zone associated with the event.</param>
            /// <param name="entity">The entity associated with the event.</param>
            /// <param name="triggeredPosition">The new position within the zone associated with the event.</param>
            /// <param name="movedFromPosition">The position within the zone that the entity moved from.</param>
            public ZoneMoveEventArgs(IScreenSurface host, Zone zone, Entity entity, Point triggeredPosition, Point movedFromPosition) : base(host, zone, entity, triggeredPosition)
            {
                MovedFromPosition = movedFromPosition;
            }
        }

    }
}
