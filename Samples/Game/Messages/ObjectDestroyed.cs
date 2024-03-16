namespace ZZTGame.Messages;

class ObjectDestroyed
{
    public readonly Screens.Board Board;
    public readonly GameObject SourceObject;

    public ObjectDestroyed(GameObject sourceObject, Screens.Board board)
    {
        SourceObject = sourceObject;
        Board = board;
    }
}
