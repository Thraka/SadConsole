using Microsoft.Xna.Framework;
using SadConsole.Entities;
using SadConsole.Maps;
using SadConsole.Actions;

namespace SadConsole.GameObjects
{
    /// <summary>
    /// A game object you can use on a map.
    /// </summary>
    public abstract class GameObjectBase: Entity, GoRogue.IHasID, GoRogue.IHasLayer
    {
        /// <summary>
        /// The map this object is associated with.
        /// </summary>
        protected MapConsole map { get; set; }

        /// <summary>
        /// Gets or sets a friendly short title for the object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a longer description for the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// When <see langword="true"/>, blocks this object from moving.
        /// </summary>
        public bool BlockMove { get; set; }

        /// <summary>
        /// A unique ID for the object.
        /// </summary>
        public uint ID { get; set;  } = GoRogue.Random.SingletonRandom.DefaultRNG.NextUInt();

        /// <summary>
        /// Gets the position of the entity.
        /// </summary>
        public new Point Position
        {
            get => base.Position;
            internal set => base.Position = value;
        }

        public int Layer { get; }

        /// <summary>
        /// Creates a new game object with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        protected GameObjectBase(MapConsole map, Color foreground, Color background, int glyph) : base(foreground, background, glyph)
        {
            this.map = map;

            Title = "Unknown";
            Description = "Not much is known about this object.";
        }

        public void MoveTo(Point newPosition)
        {
            if (newPosition == Position) return;

            // Check the map if we can move to this new position
            if (map.IsTileWalkable(newPosition.X, newPosition.Y))
            {
                this.Position = newPosition;
            }

            // Let the map know we moved. So it can sync anything up.
            //map.SignalEntityMoved(this);
        }

        public void MoveBy(Point change) => MoveTo(Position + change);

        public virtual void ProcessAction(ActionBase command) { }

        public virtual void ProcessGameFrame() { }

        public virtual void OnDestroy() { }
    }
}
