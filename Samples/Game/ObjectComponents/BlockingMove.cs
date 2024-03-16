using ZZTGame.Tiles;

namespace ZZTGame.ObjectComponents;

class BlockingMove : IFlag, IGameObjectComponent, ITileComponent
{
    public static BlockingMove Singleton { get; } = new BlockingMove();

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
