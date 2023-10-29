using SadConsole.Entities;
using SadConsole.Input;

namespace SadConsole.Examples;

internal class DemoEntitySurface : IDemo
{
    public string Title => "Entity lite demonstration";

    public string Description => "SadConsole contains an entity system which lets you create individual objects " +
                                 "that are drawn on top of an existing surface. The system is highly performant, " +
                                 "supports collision detection, and has optimizations for single-cell 1x1 " +
                                 "entities (although it supports larger entities)." +
                                 "\r\n\r\n" +
                                 "Press [c:r f:Red:2]F1 to toggle movement\r\n" +
                                 "Use the [c:r f:Red:5]Arrow keys to move the player entity\r\n";

    public string CodeFile => "DemoEntity.cs";

    public IScreenSurface CreateDemoScreen() =>
        new EntitySurface();

    public override string ToString() =>
        Title;
}

internal class EntitySurface : ScreenSurface
{
    // The console here acts like a playing field for our entities. You could draw some sort of area for the
    // entity to walk around on. The console also gets focused with the keyboard and accepts keyboard events.
    private Entity player;
    private List<Entity> others;
    private Point playerPreviousPosition;
    private EntityManager entityManager;
    private bool moveEntities;
    private bool usePixelPositioning;

    public EntitySurface()
        : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height, 160, 46)
    {
        // This is the fade effect we use on some entities, just to make it look neat.
        var fadeEffect = new SadConsole.Effects.Fade
        {
            AutoReverse = true,
            DestinationForeground = new Gradient(Color.Blue, Color.Yellow),
            FadeForeground = true,
            UseCellForeground = false,
            Repeat = true,
            FadeDuration = System.TimeSpan.FromSeconds(0.7d),
            RemoveOnFinished = true,
            CloneOnAdd = true
        };

        // The player entity could be a large animated entity. This code is commented out and a single cell entity is used
        //player = new Entity(AnimatedScreenSurface.CreateStatic(3, 3, 3, 0.5d, Color.Black), 0);
        //player.AppearanceSurface!.Animation.Center = (1, 1);

        // Create and track the player's entity in the "player" variable
        player = new Entity(new Entity.SingleCell(Color.Yellow, Color.Black, 1), 100)
        {
            //Position = new Point(Surface.BufferWidth / 2, Surface.BufferHeight / 2)
            Position = new Point(0, 0),
            UsePixelPositioning = usePixelPositioning,
        };
        
        // Setup this console to accept keyboard input.
        UseKeyboard = true;


        Surface.DefaultBackground = Color.DarkGray;
        Surface.Clear();

        playerPreviousPosition = player.Position;

        // This component makes the parent surface's view follow a target
        SadComponents.Add(new SadConsole.Components.SurfaceComponentFollowTarget() { Target = player });

        // Create the entity renderer. This component should contain all the entities you want drawn on the surface
        entityManager = new SadConsole.Entities.EntityManager();
        SadComponents.Add(entityManager);

        // Add player to the entity manager
        entityManager.Add(player);

        // Create a large amount of other entites
        others = new List<Entity>();
        for (int i = 0; i < 1000; i++)
        {
            Entity item;

            // Entites will be either randomly small 1x1 entities, or 3x3 animated entites
            if (Game.Instance.Random.Next(0, 500) < 480)
            {
                item = new Entity(new Entity.SingleCell(Color.Red.GetRandomColor(SadConsole.Game.Instance.Random), Color.Black, Game.Instance.Random.Next(0, 60)), 0)
                {
                    Position = GetRandomPosition(),
                    UsePixelPositioning = usePixelPositioning,
                };
            }
            else
            {
                item = new Entity(AnimatedScreenObject.CreateStatic(3, 3, 3, 0.5d, Color.Black), 0)
                {
                    Position = GetRandomPosition(),
                    UsePixelPositioning = usePixelPositioning,
                };
                item.AppearanceSurface!.Animation.Center = (1, 1);
            }

            // If it's a single cell, randomly make it a fading entity (just for fun)
            if (item.IsSingleCell)
                if (Game.Instance.Random.Next(0, 500) < 50)
                    item.AppearanceSingle.Effect = fadeEffect;

            entityManager.Add(item);
            others.Add(item);
        }

        Surface.Print(3, 10, "Entity count: " + others.Count);
    }

    private Point GetRandomPosition()
    {
        Point position = (0, 0);

        bool restart = false;
        for (int i = 0; i < 10; i++)
        {
            position = new Point(SadConsole.Game.Instance.Random.Next(0, usePixelPositioning ? 160 * FontSize.X : 3), SadConsole.Game.Instance.Random.Next(0, usePixelPositioning ? 46 * FontSize.Y : 5));

            foreach (var entity in entityManager.Entities)
            {
                if (entity.Position == position)
                {
                    restart = true;
                    break;
                }
            }

            if (!restart)
                return position;
        }

        return position;
    }

    public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
    {
        // Forward the keyboard data to the entity to handle the movement code.
        // We could detect if the users hit ESC and popup a menu or something.
        // By not setting the entity as the active object, twe let this
        // "game level" (the console we're hosting the entity on) determine if
        // the keyboard data should be sent to the entity.

        // Process logic for moving the entity.
        bool keyHit = false;
        Point newPosition = (0, 0);

        // Toggles entity random movements
        if (info.IsKeyPressed(Keys.F1))
        {
            moveEntities = !moveEntities;
            return true;
        }

        // Process UP/DOWN movements
        if (info.IsKeyPressed(Keys.Up))
        {
            newPosition = player.Position + (0, -1);
            keyHit = true;
        }
        else if (info.IsKeyPressed(Keys.Down))
        {
            newPosition = player.Position + (0, 1);
            keyHit = true;
        }

        // Process LEFT/RIGHT movements
        if (info.IsKeyPressed(Keys.Left))
        {
            newPosition = player.Position + (-1, 0);
            keyHit = true;
        }
        else if (info.IsKeyPressed(Keys.Right))
        {
            newPosition = player.Position + (1, 0);
            keyHit = true;
        }

        if (info.IsKeyPressed(Keys.L))
        {
            SadConsole.Serializer.Save(this, "entity.surface", false);
            return true;
        }

        // If a movement key was pressed
        if (keyHit)
        {
            // Check if the new position is valid
            if (Surface.Area.Contains(newPosition))
            {
                // Entity moved. Let's draw a trail of where they moved from.
                Surface.SetGlyph(player.Position.X, player.Position.Y, 250);
                player.Position = newPosition;

                return true;
            }
        }

        // You could have multiple entities in the game for example, and change
        // which entity gets keyboard commands.
        return false;
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (moveEntities)
            foreach (var item in others)
            {
                var newPosition = item.Position + new Point(Game.Instance.Random.Next(-1, 2), Game.Instance.Random.Next(-1, 2));

                if (Surface.Area.Contains(newPosition))
                    item.Position = newPosition;
            }
    }
}
