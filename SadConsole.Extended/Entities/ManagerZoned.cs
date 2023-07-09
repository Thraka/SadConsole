using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadRogue.Primitives;
using System.Linq;
using SadRogue.Primitives.SpatialMaps;

namespace SadConsole.Entities;

/// <summary>
/// Manages a set of entities. Adds a render step and only renders the entities that are in the parent <see cref="IScreenSurface"/> visible area.
/// </summary>
[DataContract]
public class ManagerZoned : EntityManager, Components.IComponent
{
    private Dictionary<Entity, EntityState> _entityStates;
    private AutoSyncMultiSpatialMap<Entity> _spatialMap;

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
    protected List<Zone> _zones = new();

    /// <summary>
    /// The zones associated with this manager.
    /// </summary>
    public IReadOnlyList<Zone> Zones => _zones;

    /// <summary>
    /// Creates a new manager to handle entity movement within zones.
    /// </summary>
    public ManagerZoned()
    {
        _entityStates = new(new IDComparer<Entity>());
        _spatialMap = new();
    }

    /// <summary>
    /// Adds a zone to this manager.
    /// </summary>
    /// <param name="zone">The zone to add.</param>
    public void Add(Zone zone)
    {
        if (_zones.Contains(zone)) return;

        _zones.Add(zone);

        for (int i = 0; i < _entities.Count; i++)
        {
            Entity ent = _entities[i];
            if (zone.Area.Contains(ent.Position))
            {
                _entityStates[ent].Zones.Add(zone);
                OnEntityEnterZone(_screen, zone, ent, ent.Position);
            }
        }
    }

    /// <summary>
    /// Removes a zone from this manager.
    /// </summary>
    /// <param name="zone">The hotspot to remove.</param>
    public void Remove(Zone zone)
    {
        if (!_zones.Contains(zone)) return;

        _zones.Remove(zone);

        for (int i = 0; i < _entities.Count; i++)
        {
            Entity ent = _entities[i];
            EntityState state = _entityStates[ent];

            if (state.Zones.Remove(zone))
                OnEntityExitZone(_screen, zone, ent, ent.Position);
        }
    }

    /// <summary>
    /// Checks if the manager contains the specified zone.
    /// </summary>
    /// <param name="zone">The zone to check for.</param>
    public bool Contains(Zone zone) =>
        _zones.Contains(zone);

    /// <summary>
    /// Returns an entity at the specified position or <see langword="null"/> if there is no entity. If there are multiple entities at the specified position, only one of them is returned.
    /// </summary>
    /// <param name="position">The position to get an entity at.</param>
    /// <returns>The entity if it exists; otherwise it returns <see langword="null"/>.</returns>
    public Entity GetEntityAtPosition(Point position)
    {
        ListEnumerator<Entity> items = _spatialMap.GetItemsAt(position);
        return items.MoveNext() ? items.Current : null;
    }

    /// <summary>
    /// Returns an enumerator containing the entities, if any, at the specified position.
    /// </summary>
    /// <param name="position">The position to get an entity at.</param>
    /// <returns>An array of entities if they exist; otherwise <see langword="null"/>.</returns>
    public ListEnumerator<Entity> GetEntitiesAtPosition(Point position) =>
        _spatialMap.GetItemsAt(position);

    /// <summary>
    /// Returns <see langword="true"/> when there is an entity at the specified position; otherwise <see langword="false"/>.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>A value indicating if an entity exists.</returns>
    public bool HasEntityAt(Point position) =>
        _spatialMap.Contains(position);

    /// <summary>
    /// Returns a collection of zones at the specified position.
    /// </summary>
    /// <param name="position">The position to check for zones.</param>
    /// <returns>Every zone that contains the position.</returns>
    public Zone[] GetZonesAtPosition(Point position)
    {
        List<Zone> foundZones = new();
        
        foreach (Zone zone in _zones)
        {
            if (zone.Area.Contains(position))
                foundZones.Add(zone);
        }
        return foundZones.ToArray();
    }

    /// <summary>
    /// Returns a list of entities that are located in the specified zone.
    /// </summary>
    /// <param name="zone">The zone to filter entities by.</param>
    /// <returns>A list of entities.</returns>
    public IReadOnlyList<Entity> GetEntitiesInZone(Zone zone)
    {
        ListEnumerator<Entity> enumerator = new(_entities);
        List<Entity> results = new();

        foreach (Entity entity in enumerator)
        {
            if (zone.Area.Contains(entity.Position))
                results.Add(entity);
        }

        return results.Count != 0 ? results.ToArray() : Array.Empty<Entity>();
    }

    /// <inheritdoc/>
    protected override void OnEntityChangedPosition(Entity entity, ValueChangedEventArgs<Point> e) =>
        EvaluateEntityState(entity, e.OldValue);

    /// <summary>
    /// Adds entity state information to an entity when it's added.
    /// </summary>
    /// <param name="entity">The entity that was added.</param>
    protected override void OnEntityAdded(Entity entity)
    {
        var state = new EntityState();

        _entityStates.Add(entity, state);
        _spatialMap.Add(entity);

        if (GetZonesAtPosition(entity.Position, out HashSet<Zone> zones))
        {
            state.Zones = zones;
            foreach (Zone zone in zones)
            {
                zone._members.Add(entity);
                OnEntityEnterZone(_screen, zone, entity, entity.Position);
            }
        }
    }

    /// <summary>
    /// Removes the entity state information of an entity being removed.
    /// </summary>
    /// <param name="entity">The entity that was removed.</param>
    protected override void OnEntityRemoved(Entity entity)
    {
        foreach (Zone zone in _entityStates[entity].Zones)
        {
            zone._members.Remove(entity);
            OnEntityExitZone(_screen, zone, entity, entity.Position);
        }

        _entityStates.Remove(entity);
        _spatialMap.Remove(entity);
    }

    private void EvaluateEntityState(Entity entity, Point oldPosition)
    {
        EntityState state = _entityStates[entity];

        GetZonesAtPosition(entity.Position, out HashSet<Zone> newZones);

        if (!state.Zones.SetEquals(newZones))
        {
            IEnumerable<Zone> sameZones = newZones.Intersect(state.Zones);

            foreach (Zone zone in state.Zones.Except(newZones))
            {
                state.Zones.Remove(zone);
                zone._members.Remove(entity);
                OnEntityExitZone(_screen, zone, entity, entity.Position);
            }

            foreach (Zone zone in newZones.Except(state.Zones))
            {
                state.Zones.Add(zone);
                zone._members.Add(entity);
                OnEntityEnterZone(_screen, zone, entity, entity.Position);
            }

            foreach (Zone zone in sameZones)
            {
                OnEntityMoveZone(_screen, zone, entity, entity.Position, oldPosition);
            }
        }
    }

    /// <summary>
    /// Called when an entity enters a zone and raises the <see cref="EnterZone"/> event.
    /// </summary>
    /// <param name="host">The host that the zone and entity share.</param>
    /// <param name="zone">The zone the entity entered.</param>
    /// <param name="entity">The entity that entered the zone.</param>
    /// <param name="triggeredPosition">The position the entity entered.</param>
    protected virtual void OnEntityEnterZone(IScreenSurface host, Zone zone, Entity entity, Point triggeredPosition) =>
        EnterZone?.Invoke(this, new ZoneEventArgs(_screen, zone, entity, entity.Position));

    /// <summary>
    /// Called when an entity enters a zone and raises the <see cref="ExitZone"/> event.
    /// </summary>
    /// <param name="host">The host that the zone and entity share.</param>
    /// <param name="zone">The zone the entity exited.</param>
    /// <param name="entity">The entity that exited the zone.</param>
    /// <param name="triggeredPosition">The new position the entity left.</param>
    protected virtual void OnEntityExitZone(IScreenSurface host, Zone zone, Entity entity, Point triggeredPosition) =>
        ExitZone?.Invoke(this, new ZoneEventArgs(_screen, zone, entity, triggeredPosition));


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
    /// Gets the zones that contain the specified position.
    /// </summary>
    /// <param name="point">The position to check.</param>
    /// <param name="zones">The zones that contain this position.</param>
    /// <returns><see langword="true"/> when at least one zone was found; otherwise <see langword="false"/>.</returns>
    public bool GetZonesAtPosition(in Point point, out HashSet<Zone> zones)
    {
        zones = new HashSet<Zone>();

        for (int i = 0; i < Zones.Count; i++)
        {
            Zone z = Zones[i];
            if (z.Area.Contains(point))
            {
                zones.Add(z);
            }
        }

        return zones.Count != 0;
    }

    /// <summary>
    /// Prevents an entity from being processed with the <see cref="Zones"/>.
    /// </summary>
    /// <param name="entity">The entity to disable.</param>
    public void DisableEntity(Entity entity)
    {
        if (_entityStates.ContainsKey(entity)) throw new Exception("Entity is not managed by this entity manager.");

        EntityState state = _entityStates[entity];
        state.IsDisabled = true;
        foreach  (Zone zone in state.Zones.ToArray())
        {
            zone._members.Remove(entity);
            state.Zones.Remove(zone);
            OnEntityExitZone(_screen, zone, entity, entity.Position);
        }

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

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
    }

    [DataContract]
    private class EntityState
    {
        [DataMember]
        public HashSet<Zone> Zones { get; set; } = new();

        [DataMember]
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
