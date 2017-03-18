using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ColorHelper = Microsoft.Xna.Framework.Color;

using Console = SadConsole.Console;
using SadConsole;
using SadConsole.Surfaces;

namespace StarterProject.CustomConsoles
{
    class GameObjectConsole: Console, IConsoleMetadata
    {
        // The console here acts like a playing field for our entities. You could draw some sort of area for the
        // entity to walk around on. The console also gets focused with the keyboard and accepts keyboard events.
        private SadConsole.GameHelpers.GameObject player;
        private Point playerPreviousPosition;

        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Game object", Summary = "Use the cursor keys to move the little character" };
            }
        }

        public GameObjectConsole()
            : base(80, 23)
        {
            var animation = new AnimatedSurface("default", 1, 1);
            var frame = animation.CreateFrame();
            frame.Cells[0].Glyph = 1;

            player = new SadConsole.GameHelpers.GameObject(animation);
            //player.RepositionRects = true;
            player.Position = new Point(textSurface.Width / 2, textSurface.Height / 2);
            playerPreviousPosition = player.Position;

            // Setup this console to accept keyboard input.
            UseKeyboard = true;
            IsVisible = false;

            Children.Add(player);
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
            var oldPosition = player.Position;

            if (info.IsKeyReleased(Keys.Up))
            {
                player.Position = new Point(player.Position.X, player.Position.Y - 1);
                keyHit = true;
            }
            else if (info.IsKeyReleased(Keys.Down))
            {
                player.Position = new Point(player.Position.X, player.Position.Y + 1);
                keyHit = true;
            }

            if (info.IsKeyReleased(Keys.Left))
            {
                player.Position = new Point(player.Position.X - 1, player.Position.Y);
                keyHit = true;
            }
            else if (info.IsKeyReleased(Keys.Right))
            {
                player.Position = new Point(player.Position.X + 1, player.Position.Y);
                keyHit = true;
            }


            if (keyHit)
            {
                // Check if the new position is valid
                if (textSurface.RenderArea.Contains(player.Position))
                {
                    // Entity moved. Let's draw a trail of where they moved from.
                    SetGlyph(playerPreviousPosition.X, playerPreviousPosition.Y, 250);
                    playerPreviousPosition = player.Position;

                    return true;
                }
                else  // New position was not in the area of the console, move back
                    player.Position = oldPosition;
            }

            // You could have multiple entities in the game for example, and change
            // which entity gets keyboard commands.
            return false;
        }
    }
}
