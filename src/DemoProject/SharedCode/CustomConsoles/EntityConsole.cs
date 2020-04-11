using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Entities;
using ScrollingConsole = SadConsole.ScrollingConsole;


namespace StarterProject.CustomConsoles
{
    internal class EntityConsole : ScrollingConsole
    {
        // The console here acts like a playing field for our entities. You could draw some sort of area for the
        // entity to walk around on. The console also gets focused with the keyboard and accepts keyboard events.
        private readonly SadConsole.Entities.Entity player;
        private Point playerPreviousPosition;

        public EntityConsole()
            : base(80, 23)
        {
            var animation = new AnimatedConsole("default", 1, 1);
            CellSurface frame = animation.CreateFrame();
            frame.Cells[0].Glyph = 1;
            frame.SetDecorator(0, 2, new CellDecorator(Color.Yellow, 69, SpriteEffects.None));

            player = new Entity(animation)
            {
                Position = new Point(Width / 2, Height / 2)
            };
            player.Components.Add(new SadConsole.Components.EntityViewSyncComponent());
            playerPreviousPosition = player.Position;
            //SadConsole.Global.KeyboardState.InitialRepeatDelay = SadConsole.Global.KeyboardState.RepeatDelay;
            Children.Add(player);

            // Setup this console to accept keyboard input.
            UseKeyboard = true;
            IsVisible = false;
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
                player.Animation.AddDecorator(0, 2, new[] { new CellDecorator(Color.Green, 67, SpriteEffects.None) });
                keyHit = true;
            }
            if (info.IsKeyPressed(Keys.Q))
            {
                player.Animation.ClearDecorators(0, 1);
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
                if (ViewPort.Contains(player.Position))
                {
                    // Entity moved. Let's draw a trail of where they moved from.
                    SetGlyph(playerPreviousPosition.X, playerPreviousPosition.Y, 250);
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
    }
}
