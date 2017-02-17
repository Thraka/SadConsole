using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SadConsole.GameHelpers;
using SadConsole.Surfaces;

namespace Castle
{
    internal class Player : GameObject
    {
        public Direction CurrentDirection { get; private set; }
        public Direction Facing { get; private set; }
        public int Health { get; private set; }
        public Player() : base()
        {
            Animation = new AnimatedSurface("default", 1, 1);
            var frame = Animation.CreateFrame();
            frame[0].Glyph = 5;
            this.CurrentDirection = Direction.None;
            Health = 70;

        }

        public bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            // Process logic for moving the entity.
            bool keyHit = false;

            if (info.IsKeyDown(Keys.Up))
            {
                this.CurrentDirection = Direction.Up;
                keyHit = true;
            }
            else if (info.IsKeyDown(Keys.Down))
            {
                this.CurrentDirection = Direction.Down;
                keyHit = true;
            }

            if (info.IsKeyDown(Keys.Left))
            {
                this.CurrentDirection = Direction.Left;
                keyHit = true;
            }
            else if (info.IsKeyDown(Keys.Right))
            {
                this.CurrentDirection = Direction.Right;
                keyHit = true;
            }

            switch(CurrentDirection)
            {
                case Direction.Up:
                    if(info.IsKeyReleased(Keys.Up))
                    {
                        this.CurrentDirection = Direction.None;
                        keyHit = true;
                    }
                    break;
                case Direction.Down:
                    if (info.IsKeyReleased(Keys.Down))
                    {
                        this.CurrentDirection = Direction.None;
                        keyHit = true;
                    }
                    break;
                case Direction.Left:
                    if (info.IsKeyReleased(Keys.Left))
                    {
                        this.CurrentDirection = Direction.None;
                        keyHit = true;
                    }
                    break;
                case Direction.Right:
                    if (info.IsKeyReleased(Keys.Right))
                    {
                        this.CurrentDirection = Direction.None;
                        keyHit = true;
                    }
                    break;
            }
            

            return keyHit;

        }
        public void Move(Point location)
        {
            if (this.IsVisible)
            {
                position.X = location.X;
                position.Y = location.Y;
            }
        }
        public Point PreviewMove(Direction direction)
        {
            Point preview = new Point(position.X, position.Y);

            switch (direction)
            {
                case Direction.Up:
                    this.Facing = Direction.Up;
                    preview.Y -= 1;
                    break;
                case Direction.Down:
                    this.Facing = Direction.Down;
                    preview.Y += 1;
                    break;
                case Direction.Left:
                    this.Facing = Direction.Left;
                    preview.X -= 1;
                    break;
                case Direction.Right:
                    this.Facing = Direction.Right;
                    preview.X += 1;
                    break;
            }

            return preview;
        }
        public Point GetFacingPoint(Direction direction)
        {
            Point preview = new Point(position.X, position.Y);

            switch (Facing)
            {
                case Direction.Up:
                    preview.Y -= 1;
                    break;
                case Direction.Down:
                    preview.Y += 1;
                    break;
                case Direction.Left:
                    preview.X -= 1;
                    break;
                case Direction.Right:
                    preview.X += 1;
                    break;
            }

            return preview;
        }

        public void Kill()
        {
            this.IsVisible = false;
        }
        public bool Hit(bool hasHelmet)
        {
            if(Health >= 0)
            {
                Health -= 2;
            }
            if (Health < 0)
            {
                Health = 0;
            }
            if(Health == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
