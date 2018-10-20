using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Entities;

namespace SadConsole.Actions
{
    class Move : ActionBase
    {

        public static Move MoveBy(Entity source, Point change)
        {
            return new Move() { Source = source, PositionChange = change };
        }

        public Entity Source;
        public Point PositionChange;
        public Point TargetPosition;

        public override void Run(TimeSpan timeElapsed)
        {
            TargetPosition = Source.Position + PositionChange;

            if (TargetPosition == Source.Position)
                return;

            //if (Program.AdventureScreen.Map.IsTileWalkable(TargetPosition.X, TargetPosition.Y))
            //{
            //    var ent = Program.AdventureScreen.Map.GetEntity(TargetPosition);

            //    if (ent == null)
            //    {
            //        Source.MoveBy(PositionChange, Program.AdventureScreen.Map);

            //        if (Source == Program.AdventureScreen.Player)
            //        {
            //            if (PositionChange == Directions.West)
            //                Program.AdventureScreen.MessageScreen.Print("You move west.", Screens.Messages.MessageTypes.Status);
            //            else if (PositionChange == Directions.East)
            //                Program.AdventureScreen.MessageScreen.Print("You move east.", Screens.Messages.MessageTypes.Status);
            //            else if (PositionChange == Directions.North)
            //                Program.AdventureScreen.MessageScreen.Print("You move north.", Screens.Messages.MessageTypes.Status);
            //            else if (PositionChange == Directions.South)
            //                Program.AdventureScreen.MessageScreen.Print("You move south.", Screens.Messages.MessageTypes.Status);

            //        }
            //    }
            //    else
            //    {
            //        BumpEntity bump = new BumpEntity(Source, ent);
            //        Program.AdventureScreen.RunCommand(bump);
            //    }
            //}
            //else
            //{

            //    BumpTile bump = new BumpTile(Source, Program.AdventureScreen.Map[TargetPosition]);
            //    Program.AdventureScreen.RunCommand(bump);
            //}

            Finish(ActionResult.Success);
        }
    }

}
