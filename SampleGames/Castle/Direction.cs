using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castle
{
    internal enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpStairs,
        DownStairs

    }

    internal enum ObjectType
    {
        None,
        Wall,
        Door,
        LockedDoor,
        UpStairs,
        DownStairs,
        Item,
        Trap,
        Monster
    }
}
