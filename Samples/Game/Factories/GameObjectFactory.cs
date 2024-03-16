using ZZTGame.ObjectComponents;

namespace ZZTGame.Factories;

class GameObjectFactory : SadConsole.Factory.Factory<GameObjectConfig, GameObject>
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
        Add(new GameObjectInvalidBlueprint());
        Add(new GameObjectPlayerBlueprint());
        Add(new GameObjectBoulderBlueprint());
        Add(new GameObjectSliderVerticalBlueprint());
        Add(new GameObjectSliderHorizontalBlueprint());
        Add(new GameObjectPusherNorthBlueprint());
        Add(new GameObjectPusherSouthBlueprint());
        Add(new GameObjectPusherEastBlueprint());
        Add(new GameObjectPusherWestBlueprint());
        Add(new GameObjectPusherBlueprint());
        Add(new GameObjectAmmoBlueprint());
        Add(new GameObjectTorchBlueprint());
        Add(new GameObjectGemBlueprint());
        Add(new GameObjectKeyBlueprint());
        Add(new GameObjectDoorBlueprint());

        //Add(new GameObjectBlueprint("dead", new GameObjectBlueprint.Config(SadRogue.Primitives.ColorAnsi.WhiteBright, SadRogue.Primitives.ColorAnsi.Red, 'x')));
        //Add(new GameObjectBlueprint("player", new GameObjectBlueprint.Config(null, null, 0xDB), baseComponentsPushable));
        //Add(new GameObjectBlueprint("boulder", new GameObjectBlueprint.Config(null, null, 0xFE), baseComponentsPushable));
        //Add(new GameObjectBlueprint("slider-vertical", new GameObjectBlueprint.Config(null, null, 0x12, ConfigureSliderVertical), baseComponentsPushable));
        //Add(new GameObjectBlueprint("slider-horizontal", new GameObjectBlueprint.Config(null, null, 0x1D, ConfigureSliderHorizontal), baseComponentsPushable));
        //Add(new GameObjectBlueprint("pusher-north", new GameObjectBlueprint.Config(null, null, 0x1E, ConfigurePusherNorth), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
        //Add(new GameObjectBlueprint("pusher-south", new GameObjectBlueprint.Config(null, null, 0x1F, ConfigurePusherSouth), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
        //Add(new GameObjectBlueprint("pusher-east", new GameObjectBlueprint.Config(null, null, 0x10, ConfigurePusherEast), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
        //Add(new GameObjectBlueprint("pusher-west", new GameObjectBlueprint.Config(null, null, 0x11, ConfigurePusherWest), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
        //Add(new GameObjectBlueprint("pusher-idle", new GameObjectBlueprint.Config(null, null, 0x10, ConfigurePusherIdle), typeof(BlockingMove), typeof(Movable), typeof(Pusher)));
        //Add(new GameObjectBlueprint("ammo", new GameObjectBlueprint.Config(SadRogue.Primitives.ColorAnsi.Cyan, null, 0x84, ConfigureAmmoTouch)));
        //Add(new GameObjectBlueprint("torch", new GameObjectBlueprint.Config(SadRogue.Primitives.ColorAnsi.Yellow, null, 0x9D, ConfigureAmmoTouch)));
        //Add(new GameObjectBlueprint("gem", new GameObjectBlueprint.Config(null, null, 0x04, ConfigureAmmoTouch)));
        //Add(new GameObjectBlueprint("key", new GameObjectBlueprint.Config(null, null, 0x0C, ConfigureAmmoTouch)));
        //Add(new GameObjectBlueprint("door", new GameObjectBlueprint.Config(null, null, 0x0A, ConfigureAmmoTouch)));
    }
}
