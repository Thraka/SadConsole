using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Actions
{
    class DungeonSight : ActionBase
    {
        public override void Run(TimeSpan timeElapsed)
        {
            //Program.AdventureScreen.MessageScreen.Print("You gain Dungeon Sight!", Screens.Messages.MessageTypes.Warning);

            //foreach (var tile in Program.AdventureScreen.Map.Tiles)
            //{
            //    tile.SetFlag(Tiles.Flags.Seen | Tiles.Flags.PermaLight | Tiles.Flags.PermaInLOS);
            //}

            Finish(ActionResult.Success);
        }
    }
}
