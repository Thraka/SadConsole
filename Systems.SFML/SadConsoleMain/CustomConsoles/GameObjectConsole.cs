using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Consoles.Console;
using SadConsole.Consoles;
using Keys = SFML.Window.Keyboard.Key;

namespace StarterProject.CustomConsoles
{
    class GameObjectConsole: Console
    {
        // The console here acts like a playing field for our entities. You could draw some sort of area for the
        // entity to walk around on. The console also gets focused with the keyboard and accepts keyboard events.
        private SadConsole.Game.GameObject player;
        private SFML.System.Vector2i playerPreviousPosition;

        public GameObjectConsole()
            : base(80, 25)
        {
            var animation = new AnimatedTextSurface("default", 1, 1, SadConsole.Engine.DefaultFont);
            var frame = animation.CreateFrame();
            frame.Cells[0].GlyphIndex = 1;

            player = new SadConsole.Game.GameObject(SadConsole.Engine.DefaultFont);
            player.Animation = animation;
            //player.RepositionRects = true;
            player.Position = new SFML.System.Vector2i(textSurface.Width / 2, textSurface.Height / 2);
            playerPreviousPosition = player.Position;

            // Setup this console to accept keyboard input.
            CanUseKeyboard = true;
            IsVisible = false;
        }

        public override void Update()
        {
            base.Update();

            // Normally you call update on game objects to have them animate, we dont have an animation.
            player.Update();
        }

        public override void Render()
        {
            if (IsVisible)
            {
                base.Render();
                player.Render();
            }
        }

        public override bool ProcessKeyboard(SadConsole.Input.KeyboardInfo info)
        {
            // Forward the keyboard data to the entity to handle the movement code.
            // We could detect if the users hit ESC and popup a menu or something.
            // By not setting the entity as the active object, twe let this
            // "game level" (the console we're hosting the entity on) determine if
            // the keyboard data should be sent to the entity.

            // Process logic for moving the entity.
            bool keyHit = false;

            if (info.IsKeyReleased(Keys.Up))
            {
                player.Position = new SFML.System.Vector2i(player.Position.X, player.Position.Y - 1);
                keyHit = true;
            }
            else if (info.IsKeyReleased(Keys.Down))
            {
                player.Position = new SFML.System.Vector2i(player.Position.X, player.Position.Y + 1);
                keyHit = true;
            }

            if (info.IsKeyReleased(Keys.Left))
            {
                player.Position = new SFML.System.Vector2i(player.Position.X - 1, player.Position.Y);
                keyHit = true;
            }
            else if (info.IsKeyReleased(Keys.Right))
            {
                player.Position = new SFML.System.Vector2i(player.Position.X + 1, player.Position.Y);
                keyHit = true;
            }


            if (keyHit)
            {
                // Entity moved. Let's draw a trail of where they moved from.

                // We are not detecting when the player tries to move off the console area.
                // We could detected that though and then move the player back to where they were.
                SetGlyph(playerPreviousPosition.X, playerPreviousPosition.Y, 250);
                playerPreviousPosition = player.Position;

                return true;
            }

            // You could have multiple entities in the game for example, and change
            // which entity gets keyboard commands.
            return false;
        }
    }
}
