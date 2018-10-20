using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    class ProcessorDirection : ActionBase
    {
        //bool prompted;
        //bool ready;
        //string question;
        //Screens.Messages.MessageTypes messageType;

        //public Point DirectionPoint;

        //public ProcessorDirection(string prompt, Screens.Messages.MessageTypes type)
        //{
        //    messageType = type;
        //    question = prompt;
        //}

        public override void Run(TimeSpan timeElapsed)
        {
        //    if (!prompted)
        //    {
        //        Program.AdventureScreen.MessageScreen.Print(question, messageType);
        //        prompted = true;
        //    }
        //    else if (!ready)
        //    {
        //        ready = SadConsole.Global.KeyboardState.KeysPressed.Count == 0;
        //    }
        //    else
        //    {
        //        if (SadConsole.Global.KeyboardState.KeysPressed.Count != 0)
        //        {
        //            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
        //                DirectionPoint = Directions.West;

        //            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
        //                DirectionPoint = Directions.East;

        //            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
        //                DirectionPoint = Directions.North;

        //            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
        //                DirectionPoint = Directions.South;

        //            else
        //            {
        //                Finish(ActionResult.Failure);
        //                return;
        //            }

        //            Finish(ActionResult.Success);
        //        }
        //    }
        }
    }
}
