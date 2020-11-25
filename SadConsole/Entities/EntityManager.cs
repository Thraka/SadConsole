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
    public class Manager : Renderer
    {
        [DataMember(Name = "Hotspots")]
        protected List<Hotspot> _hotpots = new List<Hotspot>();
        [DataMember(Name = "Zones")]
        protected List<Zone> _zones = new List<Zone>();

        protected bool _drawHotspots = false;
        protected bool _drawZones = false;

        /// <summary>
        /// The hotspots associated with this manager.
        /// </summary>
        public IReadOnlyList<Hotspot> Hotspots => _hotpots;

        /// <summary>
        /// The zones associated with this manager.
        /// </summary>
        public IReadOnlyList<Zone> Zones => _zones;

        /// <summary>
        /// When <see langword="true"/>, indicates to any renderer associated with this manager that hotpsots should be drawn; otherwise <see langword="false"/>.
        /// </summary>
        public bool DrawHotspots
        {
            get => _drawHotspots;
            set { _drawHotspots = value; IsDirty = true; }
        }

        /// <summary>
        /// When <see langword="true"/>, indicates to any renderer associated with this manager that zones should be drawn; otherwise <see langword="false"/>.
        /// </summary>
        public bool DrawZones
        {
            get => _drawZones;
            set { _drawZones = value; IsDirty = true; }
        }

        /// <summary>
        /// Adds a hotspot to this manager.
        /// </summary>
        /// <param name="hotspot">The hotspot to add.</param>
        public void Add(Hotspot hotspot)
        {
            if (_hotpots.Contains(hotspot)) return;

            _hotpots.Add(hotspot);
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
        /// Removes a hotspot from this manager.
        /// </summary>
        /// <param name="hotspot">The hotspot to remove.</param>
        public void Remove(Hotspot hotspot)
        {
            if (!_hotpots.Contains(hotspot)) return;

            _hotpots.Remove(hotspot);
        }

        /// <summary>
        /// Removes a zone from this manager.
        /// </summary>
        /// <param name="zone">The hotspot to remove.</param>
        public void Remove(Zone zone)
        {
            if (!_zones.Contains(zone)) return;

            _zones.Remove(zone);
        }

        /// <inheritdoc/>
        public override void Update(IScreenObject host, TimeSpan delta)
        {
            base.Update(host, delta);
        }

        protected override void OnEntityChangedPosition(Entity entity, ValueChangedEventArgs<Point> e)
        {

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
        /// Contains event data for a <see cref="Zone"/> and <see cref="BasicEntity"/> interaction.
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
            public ZoneMoveEventArgs(Console host, Zone zone, Entity entity, Point triggeredPosition, Point movedFromPosition) : base(host, zone, entity, triggeredPosition)
            {
                MovedFromPosition = movedFromPosition;
            }
        }

    }
}
