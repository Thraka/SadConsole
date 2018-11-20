using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace SadConsole.Actions
{
    class ProcessorAdventureScreen : ActionBase
    {

        public override void Run(TimeSpan timeElapsed)
        {
            //// Handle keyboard when this screen is being run
            //if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Left))
            //{
            //    Program.AdventureScreen.RunCommand(Move.MoveBy(Program.AdventureScreen.Player, Directions.West));
            //    Program.AdventureScreen.RunLogicFrame = true;
            //}

            //else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Right))
            //{
            //    Program.AdventureScreen.RunCommand(Move.MoveBy(Program.AdventureScreen.Player, Directions.East));
            //    Program.AdventureScreen.RunLogicFrame = true;
            //}

            //if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Up))
            //{
            //    Program.AdventureScreen.RunCommand(Move.MoveBy(Program.AdventureScreen.Player, Directions.North));
            //    Program.AdventureScreen.RunLogicFrame = true;
            //}
            //else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Down))
            //{
            //    Program.AdventureScreen.RunCommand(Move.MoveBy(Program.AdventureScreen.Player, Directions.South));
            //    Program.AdventureScreen.RunLogicFrame = true;
            //}

            //if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.O))
            //{
            //    var directionProcessor = new ProcessorDirection("Press the direction of a door to open...", Screens.Messages.MessageTypes.Status);

            //    directionProcessor.OnSuccess(dp =>
            //    {
            //        Actions.OpenDoor open = new OpenDoor(Program.AdventureScreen.Player,
            //                                             Program.AdventureScreen.Map.GetTile(Program.AdventureScreen.Player.Position +
            //                                                                                ((ProcessorDirection)dp).DirectionPoint));
            //        Program.AdventureScreen.RunCommand(open);
            //        return true;
            //    });

            //    Program.AdventureScreen.RunCommand(directionProcessor);
            //}
            //else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.C))
            //{
            //    var directionProcessor = new ProcessorDirection("Press the direction of a door to close...", Screens.Messages.MessageTypes.Status);

            //    directionProcessor.OnSuccess(dp =>
            //    {
            //        Actions.CloseDoor close = new CloseDoor(Program.AdventureScreen.Player,
            //                                                Program.AdventureScreen.Map.GetTile(Program.AdventureScreen.Player.Position +
            //                                                                                   ((ProcessorDirection)dp).DirectionPoint));
            //        Program.AdventureScreen.RunCommand(close);
            //        return true;
            //    });

            //    Program.AdventureScreen.RunCommand(directionProcessor);
            //}
            //else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.S))
            //{
            //    Program.AdventureScreen.RunCommand(new DungeonSight());
            //    Program.AdventureScreen.RunLogicFrame = true;
            //}
            //else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.M))
            //{
            //    Program.AdventureScreen.RunCommand(new DungeonMapping());
            //    Program.AdventureScreen.RunLogicFrame = true;
            //}

            //else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.I))
            //{
            //    Program.AdventureScreen.RunCommand(new EquipItemPrompt());
            //}

            //else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Escape))
            //{
                
            //}
        }
    }

}
