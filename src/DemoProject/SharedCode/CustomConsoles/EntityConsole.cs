using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ColorHelper = Microsoft.Xna.Framework.Color;

using Console = SadConsole.Console;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Surfaces;

namespace StarterProject.CustomConsoles
{
    class EntityConsole: Console
    {
        // The console here acts like a playing field for our entities. You could draw some sort of area for the
        // entity to walk around on. The console also gets focused with the keyboard and accepts keyboard events.
        private SadConsole.Entities.Entity player;
        private Point playerPreviousPosition;
        private SadConsole.Entities.Zone zone1;
        
        public EntityConsole()
            : base(90, 24
                , new Rectangle(1,1,80,23))
        {
            var animation = new Animated("default", 1, 1);
            var frame = animation.CreateFrame();
            frame.Cells[0].Glyph = 1;

            player = new SadConsole.Entities.Entity(animation);
            //player.RepositionRects = true;
            player.Position = new Point(Width / 2, Height / 2);
            playerPreviousPosition = player.Position;

            // Setup this console to accept keyboard input.
            UseKeyboard = true;
            IsVisible = false;

            zone1 = new Zone(new Rectangle(1, 1, 10, 10));
            zone1.DebugTitle = "Zone1";
            zone1.DebugAppearance = new Cell(Color.White, Color.DarkGreen);
            zone1.IsVisible = true;

            var spot = new Hotspot();
            spot.DebugAppearance = new Cell(Color.Green, Color.Yellow, 99);
            spot.IsVisible = true;
            spot.Positions.Add(new Point(1, 1));
            spot.Positions.Add(new Point(25, 1));
            spot.Positions.Add(new Point(25, 9));
            spot.Positions.Add(new Point(14, 14));

            EntityManager manager = new EntityManager();

            manager.Entities.Add(player);
            manager.Zones.Add(zone1);
            manager.Hotspots.Add(spot);

            Children.Add(manager);
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
                if (ViewPort.Contains(player.Position))
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
