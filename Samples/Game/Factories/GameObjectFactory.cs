using System;
using System.Collections.Generic;
using System.Text;
using Game.ObjectComponents;

namespace Game.Factories
{
    class GameObjectFactory : SadConsole.Factory.Factory<GameObjectBlueprint.Config, GameObject>
    {
        public static GameObjectFactory Instance { get; } = new GameObjectFactory();

        private GameObjectFactory()
        {
            Type[] baseComponents = new Type[] { typeof(BlockingMove),
                                                 typeof(Movable),
                                                 typeof(Touchable)};

            Type[] baseComponentsPushable = new Type[] { typeof(BlockingMove),
                                                         typeof(Pushable),
                                                         typeof(Movable),
                                                         typeof(Touchable)};

            // Seed the factory with all the default tiles
            Add(new GameObjectBlueprint("dead", new GameObjectBlueprint.Config(SadRogue.Primitives.ColorAnsi.WhiteBright, SadRogue.Primitives.ColorAnsi.Red, 'x')));
            Add(new GameObjectBlueprint("player", new GameObjectBlueprint.Config(null, null, 0xDB), baseComponentsPushable));
            Add(new GameObjectBlueprint("boulder", new GameObjectBlueprint.Config(null, null, 0xFE), baseComponentsPushable));
            Add(new GameObjectBlueprint("slider-vertical", new GameObjectBlueprint.Config(null, null, 0x12, ConfigureSliderVertical), baseComponentsPushable));
            Add(new GameObjectBlueprint("slider-horizontal", new GameObjectBlueprint.Config(null, null, 0x1D, ConfigureSliderHorizontal), baseComponentsPushable));
            Add(new GameObjectBlueprint("pusher-north", new GameObjectBlueprint.Config(null, null, 0x1E, ConfigurePusherNorth), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
            Add(new GameObjectBlueprint("pusher-south", new GameObjectBlueprint.Config(null, null, 0x1F, ConfigurePusherSouth), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
            Add(new GameObjectBlueprint("pusher-east", new GameObjectBlueprint.Config(null, null, 0x10, ConfigurePusherEast), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
            Add(new GameObjectBlueprint("pusher-west", new GameObjectBlueprint.Config(null, null, 0x11, ConfigurePusherWest), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
            Add(new GameObjectBlueprint("pusher-idle", new GameObjectBlueprint.Config(null, null, 0x10, ConfigurePusherIdle), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
            Add(new GameObjectBlueprint("ammo", new GameObjectBlueprint.Config(SadRogue.Primitives.ColorAnsi.Cyan, null, 0x84, ConfigureAmmoTouch)));
            Add(new GameObjectBlueprint("torch", new GameObjectBlueprint.Config(SadRogue.Primitives.ColorAnsi.Yellow, null, 0x9D, ConfigureAmmoTouch)));
            Add(new GameObjectBlueprint("gem", new GameObjectBlueprint.Config(null, null, 0x04, ConfigureAmmoTouch)));
            Add(new GameObjectBlueprint("key", new GameObjectBlueprint.Config(null, null, 0x0C, ConfigureAmmoTouch)));
            Add(new GameObjectBlueprint("door", new GameObjectBlueprint.Config(null, null, 0x0A, ConfigureAmmoTouch)));
        }

        private void ConfigureSliderVertical(GameObject obj) =>
            obj.GetComponent<Pushable>().Mode = Pushable.Modes.Vertical;

        private void ConfigureSliderHorizontal(GameObject obj) =>
            obj.GetComponent<Pushable>().Mode = Pushable.Modes.Horizontal;

        private void ConfigurePusherNorth(GameObject obj) =>
            obj.GetComponent<Pusher>().Direction = SadRogue.Primitives.Direction.Types.Up;

        private void ConfigurePusherSouth(GameObject obj) =>
            obj.GetComponent<Pusher>().Direction = SadRogue.Primitives.Direction.Types.Down;

        private void ConfigurePusherEast(GameObject obj) =>
            obj.GetComponent<Pusher>().Direction = SadRogue.Primitives.Direction.Types.Right;

        private void ConfigurePusherWest(GameObject obj) =>
            obj.GetComponent<Pusher>().Direction = SadRogue.Primitives.Direction.Types.Left;

        private void ConfigurePusherIdle(GameObject obj) =>
            obj.GetComponent<Pusher>().Direction = SadRogue.Primitives.Direction.Types.None;

        private void ConfigureAmmoTouch(GameObject obj)
        {
            var touchable = new Touchable((targetObj, targetTile, sourceObj, board) =>
            {

                if (sourceObj == board.PlayerControlledObject)
                {
                    board.DestroyGameObject(targetObj);
                }
                else
                {
                    var originalTouchLogic = new Touchable();
                    originalTouchLogic.Touch(targetObj, targetTile, sourceObj, board);
                }

            });

            obj.AddComponent(touchable);
            obj.AddComponent(new Movable());
            obj.AddComponent(new Pushable());
            obj.AddComponent(new BlockingMove());
        }
    }
}
