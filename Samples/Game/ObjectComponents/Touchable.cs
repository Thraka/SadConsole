using ZZTGame.Tiles;

namespace ZZTGame.ObjectComponents;

class Touchable: IGameObjectComponent, ITileComponent
{
    public static Touchable Singleton { get; } = new Touchable();

    public void Touch(GameObject targetObject, BasicTile targetTile, GameObject source, Screens.Board board)
    {
        if (targetObject != null)
        {
            if (targetObject.HasComponents(typeof(Pushable), typeof(Movable)))
            {
                Pushable pushComponent = targetObject.GetComponent<Pushable>();
                var pushDirection = Direction.GetDirection(source.Position, targetObject.Position);

                if (!(pushComponent.Direction == Pushable.Directions.Horizontal && (pushDirection == Direction.Down || pushDirection == Direction.Up)) &&
                    !(pushComponent.Direction == Pushable.Directions.Vertical && (pushDirection == Direction.Left || pushDirection == Direction.Right)) &&
                    pushComponent.Mode == Pushable.Modes.All || (pushComponent.Mode == Pushable.Modes.PlayerOnly && source.HasComponent<PlayerControlled>()) || (pushComponent.Mode == Pushable.Modes.CreatureOnly && !source.HasComponent<PlayerControlled>())
                    )

                    targetObject.GetComponent<Movable>().RequestMove(pushDirection, board, targetObject);
            }

            targetObject.SendMessage(new Messages.Touched(source, targetObject, targetTile, board));
        }

        if (targetTile != null)
        {
            targetTile.SendMessage(new Messages.Touched(source, targetObject, targetTile, board));
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
