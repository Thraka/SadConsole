using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole.Entities;
using SadConsole.Maps;
using SadConsole;
using SadConsole.Actions;

namespace SadConsole.GameObjects
{
    /// <summary>
    /// A game object you can use on a map.
    /// </summary>
    public abstract class GameObjectBase: Entity, GoRogue.IHasID
    {
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
        public uint ID { get; } = GoRogue.Random.SingletonRandom.DefaultRNG.NextUInt();

        /// <summary>
        /// Gets the position of the entity.
        /// </summary>
        public new Point Position
        {
            get => base.Position;
            internal set => base.Position = value;
        }

        /// <summary>
        /// Creates a new game object with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        protected GameObjectBase(Color foreground, Color background, int glyph) : base(1, 1)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            Title = "Unknown";
            Description = "Not much is known about this object.";
        }

        public void MoveTo(Point newPosition, SimpleMap map)
        {
            // Check the map if we can move to this new position
            if (map.IsTileWalkable(newPosition.X, newPosition.Y))
            {
                if (map.GameObjects.Move(this, newPosition.ToCoord()))
                {
                    this.Position = newPosition;

                }
            }

            // Let the map know we moved. So it can sync anything up.
            //map.SignalEntityMoved(this);
        }

        public void MoveBy(Point change, SimpleMap map) => MoveTo(Position + change, map);

        public virtual void ProcessAction(ActionBase command) { }

        public virtual void ProcessGameFrame() { }

        public virtual void OnDestroy() { }

        public virtual void SetMap() { }
    }
}
