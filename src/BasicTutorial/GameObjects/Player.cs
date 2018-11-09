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
    public class Player : SadConsole.GameObjects.GameObjectBase
    {
        public Player(): base(Color.Green, Color.Black, 1) { }
    }
}
