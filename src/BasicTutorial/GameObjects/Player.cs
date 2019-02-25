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
    class Player : GameObjects.LivingCharacter
    {
        public Player(MapConsole map): base(map, Color.Green, Color.Black, 1) { }
    }
}
