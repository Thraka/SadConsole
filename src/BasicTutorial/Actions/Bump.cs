using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Entities;

namespace SadConsole.Actions
{
    class BumpTile : ActionBase<Entity, Maps.Tile>
    {
        public BumpTile(Entity source, Maps.Tile target): base (source, target) { }

        public override void Run(TimeSpan timeElapsed)
        {
            // Assume that this bump is a failure
            Finish(ActionResult.Failure);

            // Tell the tile to process this bump. The tile may set Finish to success or failure.
            Target.ProcessCommand(this);
        }
    }

    class BumpEntity : ActionBase<Entity, Entity>
    {
        public BumpEntity(Entity source, Entity target): base (source, target) { }

        public override void Run(TimeSpan timeElapsed)
        {
            // Assume that this bump is a failure
            Finish(ActionResult.Failure);

            // Tell the entity to process this bump. The entity may set Finish to success or failure.
            Target.ProcessAction(this);
        }
    }
}
