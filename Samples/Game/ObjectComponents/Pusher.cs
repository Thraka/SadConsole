using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace Game.ObjectComponents
{
    class Pusher : ITick, IGameObjectComponent
    {
        public Direction.Types Direction { get; set; } = SadRogue.Primitives.Direction.Types.None;

        public int Tick { get; set; } = 5;

        private int _tickCounter = 0;

        public void Action(Screens.Board board, GameObject obj)
        {
            _tickCounter++;

            if (_tickCounter >= Tick)
            {
                if (obj.HasComponent<ObjectComponents.Movable>())
                {
                    var movable = obj.GetComponent<ObjectComponents.Movable>();
                    movable.RequestMove((SadRogue.Primitives.Direction)Direction, board, obj);
                }

                _tickCounter = 0;
            }
        }

        public void Added(GameObject obj)
        {
        }

        public void Removed(GameObject obj)
        {
        }
    }
}
