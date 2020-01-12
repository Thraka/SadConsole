using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;
using SadConsole;

namespace Game.ObjectComponents
{
    class Movable: IGameObjectComponent
    {
        public enum MoveResults
        {
            Moved,
            TouchedObject,
            TouchedObjectThenMoved,
            TouchedWall,
            TouchedWallThenMoved,
            BlockedByWall,
            BlockedByObject
        }

        public void Added(GameObject obj)
        {
        }

        public void Removed(GameObject obj)
        {
        }

        public MoveResults RequestMove(Direction direction, Screens.Board board, GameObject source)
        {
            var newPosition = source.Position + direction;

            if (board.Surface.IsValidCell(newPosition.X, newPosition.Y))
            {
                GoRogue.IHasComponents targetComponents;
                GameObject targetObj;
                Tiles.BasicTile targetTile = null;
                bool touchedSomething = false;

                // Check for another object
                if (board.IsObjectAtPosition(newPosition, out targetObj))
                {
                    targetComponents = targetObj;
                }
                else
                {
                    targetTile = (Tiles.BasicTile)board.Surface[newPosition.X, newPosition.Y];
                    targetComponents = targetTile;
                }

                if (targetComponents.HasComponent<Touchable>())
                {
                    var touch = targetComponents.GetComponent<Touchable>();
                    
                    touchedSomething = true;

                    touch.Touch(targetObj, targetTile, source, board);
                }

                // After being touched, the object could have moved or removed BlockingMove. Refresh and check.
                if (board.IsObjectAtPosition(newPosition, out targetObj))
                {
                    targetTile = null;
                    targetComponents = targetObj;
                }
                else
                {
                    targetObj = null;
                    targetTile = (Tiles.BasicTile)board.Surface[newPosition.X, newPosition.Y];
                    targetComponents = targetTile;
                }

                // Check if something will block the move
                if (targetComponents.HasComponent<BlockingMove>())
                {
                    if (targetObj != null)
                        return MoveResults.BlockedByObject;
                    else
                        return MoveResults.BlockedByWall;
                }

                if (touchedSomething)
                {
                    source.Position = newPosition;

                    if (targetObj != null)
                        return MoveResults.TouchedObjectThenMoved;
                    else
                        return MoveResults.TouchedWallThenMoved;
                }

                source.Position = newPosition;
                return MoveResults.Moved;
            }

            return MoveResults.BlockedByWall;
        }
    }
}
