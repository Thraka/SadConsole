using System;
using System.Collections.Generic;
using System.Text;
using Game.Tiles;

namespace Game.Messages
{
    class Touched
    {
        public readonly GameObject SourceObject;
        public readonly GameObject TargetObject;
        public readonly Tiles.BasicTile TargetTile;
        public readonly Screens.Board Board;

        public Touched(GameObject sourceObject, GameObject targetObject, BasicTile targetTile, Screens.Board board)
        {
            SourceObject = sourceObject;
            TargetObject = targetObject;
            TargetTile = targetTile;
            Board = board;
        }
    }
}
