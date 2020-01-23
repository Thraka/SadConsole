using System;
using System.Collections.Generic;
using System.Text;
using Game.ObjectComponents;
using Game.Tiles;
using SadConsole;
using SadRogue.Primitives;

namespace Game.Factories
{
    class GameObjectConfig: SadConsole.Factory.BlueprintConfig
    {
        public readonly Color? Foreground;
        public readonly Color? Background;
        public readonly int? Glyph;

        public GameObjectConfig(Color? foreground, Color? background, int? glyph) =>
            (Foreground, Background, Glyph) = (foreground, background, glyph);

        public AppearanceComponent GetAppearanceComponent(Color defaultForeground, Color defaultBackground, int defaultGlyph) =>
            new AppearanceComponent(Foreground ?? defaultForeground, Background ?? defaultBackground, Glyph ?? defaultGlyph);

        public static GameObjectConfig Empty => new GameObjectConfig(null, null, null);
    }

    class GameObjectInvalidBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "dead";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.Black, Color.Red, 'X'));

            return obj;
        }
    }

    class GameObjectPusherBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "pusher-idle";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0x10));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(new Pusher() { Direction = Direction.Types.None });

            return obj;
        }
    }

    class GameObjectPusherNorthBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "pusher-north";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0x10));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(new Pusher() { Direction = Direction.Types.Up });

            return obj;
        }
    }

    class GameObjectPusherSouthBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "pusher-south";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0x10));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(new Pusher() { Direction = Direction.Types.Down });

            return obj;
        }
    }

    class GameObjectPusherEastBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "pusher-east";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0x10));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(new Pusher() { Direction = Direction.Types.Right });

            return obj;
        }
    }

    class GameObjectPusherWestBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "pusher-west";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0x10));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(new Pusher() { Direction = Direction.Types.Left });

            return obj;
        }
    }

    class GameObjectBoulderBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "boulder";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0xFE));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.All });

            return obj;
        }
    }

    class GameObjectSliderVerticalBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "slider-vertical";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0x12));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.Vertical });

            return obj;
        }
    }

    class GameObjectSliderHorizontalBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "slider-horizontal";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0x1D));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.Horizontal });

            return obj;
        }
    }

    class GameObjectPlayerBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "player";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(Color.White, Color.Black, 0xDB));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.All });

            return obj;
        }
    }

    class GameObjectAmmoBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "ammo";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(SadRogue.Primitives.ColorAnsi.Cyan, Color.Black, 0x84));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.All, Mode = Pushable.Modes.CreatureOnly });
            obj.AddComponent(DestroyOnPlayerTouch.Singleton);

            return obj;
        }
    }

    class GameObjectTorchBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "torch";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(SadRogue.Primitives.ColorAnsi.Yellow, Color.Black, 0x9D));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.All, Mode = Pushable.Modes.CreatureOnly });
            obj.AddComponent(DestroyOnPlayerTouch.Singleton);

            return obj;
        }
    }

    class GameObjectGemBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "gem";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(SadRogue.Primitives.ColorAnsi.White, Color.Black, 0x04));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.All, Mode = Pushable.Modes.CreatureOnly });
            obj.AddComponent(DestroyOnPlayerTouch.Singleton);

            return obj;
        }
    }

    class GameObjectKeyBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "key";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(SadRogue.Primitives.ColorAnsi.White, Color.Black, 0x0C));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(new Pushable() { Direction = Pushable.Directions.All, Mode = Pushable.Modes.CreatureOnly });
            obj.AddComponent(DestroyOnPlayerTouch.Singleton);

            return obj;
        }
    }

    class GameObjectDoorBlueprint : SadConsole.Factory.IBlueprint<GameObjectConfig, GameObject>
    {
        public string Id => "door";

        public GameObject Create(GameObjectConfig config)
        {
            GameObject obj = new GameObject();

            obj.AddComponent(config.GetAppearanceComponent(SadRogue.Primitives.ColorAnsi.White, Color.Black, 0x0A));
            obj.AddComponent(BlockingMove.Singleton);
            obj.AddComponent(Movable.Singleton);
            obj.AddComponent(Touchable.Singleton);
            obj.AddComponent(DestroyOnPlayerTouch.Singleton);

            return obj;
        }
    }
}









