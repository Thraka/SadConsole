using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    class DungeonMapping : ActionBase
    {
        public override void Run(TimeSpan timeElapsed)
        {
            //Program.AdventureScreen.MessageScreen.Print("You gain Dungeon Mapping!", Screens.Messages.MessageTypes.Warning);

            //foreach (var tile in Program.AdventureScreen.Map.Tiles)
            //{
            //    tile.SetFlag(Tiles.Flags.Seen);
            //}
            
            Finish(ActionResult.Success);
        }
    }
}
