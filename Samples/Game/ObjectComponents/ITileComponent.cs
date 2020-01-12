using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ObjectComponents
{
    interface ITileComponent
    {
        void Added(Tiles.BasicTile obj);
        void Removed(Tiles.BasicTile obj);
    }
}
