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

namespace BasicTutorial.GameObjects
{
    class GameObjectBase: Entity
    {
        public string Title;
        public string Description;

        public bool BlockMove = false;

        public new Point Position => base.Position;

        public GameObjectBase(Color foreground, Color background, int glyph) : base(1, 1)
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
                base.Position = newPosition;

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
