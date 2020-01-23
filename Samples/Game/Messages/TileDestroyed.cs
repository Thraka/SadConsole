using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Messages
{
    class TileDestroyed
    {
        public readonly Screens.Board Board;
        public readonly Tiles.BasicTile SourceTile;

        public TileDestroyed(Tiles.BasicTile sourceTile, Screens.Board board)
        {
            SourceTile = sourceTile;
            Board = board;
        }
    }
}
