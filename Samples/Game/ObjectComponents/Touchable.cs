using System;
using System.Collections.Generic;
using System.Text;
using Game.Tiles;
using SadRogue.Primitives;

namespace Game.ObjectComponents
{
    class Touchable: IGameObjectComponent, ITileComponent
    {
        private readonly Action<GameObject, BasicTile, GameObject, Screens.Board> _reaction;

        public Touchable(Action<GameObject, BasicTile, GameObject, Screens.Board> reaction) =>
            _reaction = reaction;

        public Touchable() { }

        public void Touch(GameObject targetObject, BasicTile targetTile, GameObject source, Screens.Board board)
        {
            if (_reaction != null)
                _reaction(targetObject, targetTile, source, board);
            else
            {
                if (targetObject != null)
                {
                    if (targetObject.HasComponents(typeof(Pushable), typeof(Movable)))
                    {
                        Pushable pushComponent = targetObject.GetComponent<Pushable>();
                        var pushDirection = Direction.GetDirection(source.Position, targetObject.Position);

                        if (pushComponent.Mode == Pushable.Modes.Horizontal && (pushDirection == Direction.Down || pushDirection == Direction.Up))
                            return;
                        else if (pushComponent.Mode == Pushable.Modes.Vertical && (pushDirection == Direction.Left || pushDirection == Direction.Right))
                            return;

                        var moveResult = targetObject.GetComponent<Movable>().RequestMove(pushDirection, board, targetObject);

                        if (moveResult == Movable.MoveResults.Moved || moveResult == Movable.MoveResults.TouchedObjectThenMoved || moveResult == Movable.MoveResults.TouchedWallThenMoved)
                        {

                        }
                    }
                }
            }
        }

        public void Added(GameObject obj)
        {
        }

        public void Removed(GameObject obj)
        {
        }

        public void Added(BasicTile obj)
        {
        }

        public void Removed(BasicTile obj)
        {
        }
    }
}
