using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Entities;

namespace SadConsole.Actions
{
    class OpenDoor : ActionBase<Entity, Maps.Tile>
    {
        public OpenDoor(Entity source, Maps.Tile target) : base(source, target) { }

        public override void Run(TimeSpan timeElapsed)
        {
            if (Target.Type == Maps.Tile.TileTypeDoor)
            {
                if (Target.TileState == (int)Maps.StateDoor.Closed)
                {
                    Target.TileState = (int)Maps.StateDoor.Opened;
                    
                    Finish(ActionResult.Success);
                }
                else if (Target.TileState == (int)Maps.StateDoor.Opened)
                {
                    Finish(ActionResult.Failure);
                }
                else

                    Finish(ActionResult.Failure);
            }
            else
            {
                Finish(ActionResult.Failure);
            }
        }

        protected override void OnSuccessResult()
        {
            // If source entity is set, that means someone did this action
            // If source entity is not set, it means something opened the door and we shouldn't run game logic.
            //if (Source != null)
            //    Program.AdventureScreen.RunLogicFrame = true;
        }
    }

    class CloseDoor : ActionBase<Entity, Maps.Tile>
    {
        public CloseDoor(Entity source, Maps.Tile target) : base(source, target) { }

        public override void Run(TimeSpan timeElapsed)
        {
            if (Target.Type == Maps.Tile.TileTypeDoor)
            {
                if (Target.TileState == (int)Maps.StateDoor.Opened)
                {
                    Target.TileState = (int)Maps.StateDoor.Closed;


                    Finish(ActionResult.Success);
                }
                else if (Target.TileState == (int)Maps.StateDoor.Closed)
                {
                    Finish(ActionResult.Failure);
                }
                else
                    Finish(ActionResult.Failure);
            }
            else
            {
                Finish(ActionResult.Failure);
            }
        }

        protected override void OnSuccessResult()
        {
            // If source entity is set, that means someone did this action
            // If source entity is not set, it means something opened the door and we shouldn't run game logic.
            //if (Source != null)
            //    Program.AdventureScreen.RunLogicFrame = true;
        }
    }

}
