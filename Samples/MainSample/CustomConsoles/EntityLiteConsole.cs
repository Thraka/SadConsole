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

        public EntityLiteConsole()
            : base(80, 23, 160, 46)
        {

            var fadeEffect = new SadConsole.Effects.Fade
            {
                AutoReverse = true,
                DestinationForeground = new ColorGradient(Color.Blue, Color.Yellow),
                FadeForeground = true,
                UseCellForeground = false,
                Repeat = true,
                FadeDuration = 0.7f,
                RemoveOnFinished = true,
                CloneOnAdd = true
            };

            player = new Entity(Color.Yellow, Color.Black, 1, 100)
            {
                //Position = new Point(Surface.BufferWidth / 2, Surface.BufferHeight / 2)
                Position = new Point(0, 0)
            };

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
            for (int i = 0; i < 2500; i++)
            {
                var item = new Entity(Color.Red.GetRandomColor(SadConsole.Game.Instance.Random), Color.Black, Game.Instance.Random.Next(0, 60), 0)
                {
                    //Position = new Point(Surface.BufferWidth / 2, Surface.BufferHeight / 2)
                    Position = GetPosition()
                };

                if (Game.Instance.Random.Next(0, 500) < 50)
                    item.Effect = fadeEffect;

                entityManager.Add(item);
                others.Add(item);
            }

            // Setup this console to accept keyboard input.
            UseKeyboard = true;
            IsVisible = false;
        }

        private Point GetPosition()
        {
            var position = new Point(SadConsole.Game.Instance.Random.Next(0, 160), SadConsole.Game.Instance.Random.Next(0, 46));

            bool restart = false;
            for (int i = 0; i < 10; i++)
            {
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
                else
                {
                    restart = false;
                    position = new Point(SadConsole.Game.Instance.Random.Next(0, 160), SadConsole.Game.Instance.Random.Next(0, 46));
                }
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

            if (info.IsKeyPressed(Keys.W))
            {
                //player.Animation.Surface.AddDecorator(0, 2, new[] { new CellDecorator(Color.Green, 67, Mirror.None) });
                keyHit = true;
            }
            if (info.IsKeyPressed(Keys.Q))
            {
                moveEntities = !moveEntities;
                keyHit = true;
            }

            if (info.IsKeyPressed(Keys.Up))
            {
                player.Position = new Point(player.Position.X, player.Position.Y - 1);
                keyHit = true;
            }
            else if (info.IsKeyPressed(Keys.Down))
            {
                player.Position = new Point(player.Position.X, player.Position.Y + 1);
                keyHit = true;
            }

            if (info.IsKeyPressed(Keys.Left))
            {
                player.Position = new Point(player.Position.X - 1, player.Position.Y);
                keyHit = true;
            }
            else if (info.IsKeyPressed(Keys.Right))
            {
                player.Position = new Point(player.Position.X + 1, player.Position.Y);
                keyHit = true;
            }


            if (keyHit)
            {
                // Check if the new position is valid
                if (Surface.Area.Contains(player.Position))
                {
                    // Entity moved. Let's draw a trail of where they moved from.
                    Surface.SetGlyph(playerPreviousPosition.X, playerPreviousPosition.Y, 250);
                    playerPreviousPosition = player.Position;

                    return true;
                }
                else  // New position was not in the area of the console, move back
                {
                    player.Position = oldPosition;
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
}
