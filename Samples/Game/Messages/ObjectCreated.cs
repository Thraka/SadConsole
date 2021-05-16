using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Messages
{
    class ObjectCreated
    {
        public readonly Screens.Board Board;
        public readonly GameObject SourceObject;

        public ObjectCreated(GameObject sourceObject, Screens.Board board)
        {
            SourceObject = sourceObject;
            Board = board;
        }
    }
}
