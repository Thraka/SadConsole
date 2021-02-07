using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Messages
{
    class TileCreated
    {
        public readonly Screens.Board Board;
        public readonly Tiles.BasicTile SourceTile;

        public TileCreated(Tiles.BasicTile sourceTile, Screens.Board board)
        {
            SourceTile = sourceTile;
            Board = board;
        }
    }
}
