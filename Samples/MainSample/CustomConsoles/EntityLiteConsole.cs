using System;
using System.Collections.Generic;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Input;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class EntityLiteConsole : ScreenSurface
    {
        // The console here acts like a playing field for our entities. You could draw some sort of area for the
        // entity to walk around on. The console also gets focused with the keyboard and accepts keyboard events.
        private Entity player;
        private List<Entity> others;
        private Point playerPreviousPosition;
        private Renderer entityManager;
        private bool moveEntities;
        private bool usePixelPositioning = false;
        private bool useSmoothMovements = false;

        public EntityLiteConsole()
            : base(80, 23, 160, 46)
        {

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

            player = new Entity(Color.Yellow, Color.Black, 1, 100)
            {
                //Position = new Point(Surface.BufferWidth / 2, Surface.BufferHeight / 2)
                Position = new Point(0, 0),
                UsePixelPositioning = usePixelPositioning,
            };

            // If we're allowing smooth movements, add the component
            if (useSmoothMovements)
                player.SadComponents.Add(new SadConsole.Components.SmoothMove(FontSize, new TimeSpan(0, 0, 0, 0, 300)));

            Surface.DefaultBackground = Color.DarkGray;
            Surface.Clear();
            
            playerPreviousPosition = player.Position;
            SadComponents.Add(new SadConsole.Components.SurfaceComponentFollowTarget() { Target = player });

            entityManager = new Renderer();
            //SadComponents.Add(new SadConsole.Components.SurfaceComponentEntityOffsets());
            SadComponents.Add(entityManager);
            //player.Components.Add(new SadConsole.Components.EntityViewSync());
            entityManager.Add(player);

            //Children.Add(player);
            others = new List<Entity>();
            for (int i = 0; i < 1000; i++)
            {
                var item = new Entity(Color.Red.GetRandomColor(SadConsole.Game.Instance.Random), Color.Black, Game.Instance.Random.Next(0, 60), 0)
                {
                    Position = GetRandomPosition(),
                    UsePixelPositioning = usePixelPositioning,
                };

                if (useSmoothMovements)
                    item.SadComponents.Add(new SadConsole.Components.SmoothMove(FontSize, new TimeSpan(0, 0, 0, 0, 300)));

                if (Game.Instance.Random.Next(0, 500) < 50)
                    item.Effect = fadeEffect;

                entityManager.Add(item);
                others.Add(item);
            }

            // Setup this console to accept keyboard input.
            UseKeyboard = true;
            IsVisible = false;
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
            Point oldPosition = player.Position;
            Point newPosition = (0, 0);

            // Toggles entity random movements
            if (info.IsKeyPressed(Keys.Q))
            {
                moveEntities = !moveEntities;
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

            if (moveEntities) foreach (var item in others)
            {
                var newPosition = item.Position + new Point(Game.Instance.Random.Next(-1, 2), Game.Instance.Random.Next(-1, 2));

                if (Surface.Area.Contains(newPosition))
                    item.Position = newPosition;
            }
        }
    }
}
