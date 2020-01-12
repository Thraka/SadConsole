using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;
using SadConsole;
using Game.Tiles;

namespace Game.ObjectComponents
{
    class BlockingMove : IFlag, IGameObjectComponent, ITileComponent
    {
        public void Added(GameObject obj)
        {
        }

        public void Added(BasicTile obj)
        {
        }

        public void Removed(GameObject obj)
        {
        }

        public void Removed(BasicTile obj)
        {
        }
    }
}
