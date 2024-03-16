namespace ZZTGame.ObjectComponents;

class Movable: IGameObjectComponent
{
    public static Movable Singleton { get; } = new Movable();

    public void Added(GameObject obj)
    {
    }

    public void Removed(GameObject obj)
    {
    }

    public bool RequestMove(Direction direction, Screens.Board board, GameObject source)
    {
        var newPosition = source.Position + direction;

        if (board.Surface.IsValidCell(newPosition.X, newPosition.Y))
        {
            bool result = true;
            GameObject[] targetObjs;
            bool objectsRemain = false;

            // Check for another object
            if (board.GetObjectsAtPosition(newPosition, out targetObjs))
            {
                // Handle objects
                foreach (GameObject obj in targetObjs)
                {
                    if (obj.HasComponent<Touchable>())
                        obj.GetComponent<Touchable>().Touch(obj, null, source, board);

                    // Check if (1) the object still alive (2) still positioned in same spot (3) blocks move
                    if (obj.IsAlive && obj.Position == newPosition && obj.HasComponent<BlockingMove>())
                        result = false;
                }

                if (board.GetObjectsAtPosition(newPosition, out _))
                    objectsRemain = true;
            }

            if (!objectsRemain)
            {
                var tile = (Tiles.BasicTile)board.Surface[newPosition.X, newPosition.Y]; ;

                if (tile.HasComponent<Touchable>())
                    tile.GetComponent<Touchable>().Touch(null, tile, source, board);

                // Check if something will block the move
                if (tile.HasComponent<BlockingMove>())
                    result = false;
            }

            if (result)
                source.Position = newPosition;

            return result;
        }

        return false;
    }
}
