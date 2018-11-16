using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SadConsole.Entities;
using SadConsole.Surfaces;

namespace Castle
{
    internal class Player : Entity
    {
        public CastleDirection CurrentDirection { get; private set; }
        public CastleDirection Facing { get; private set; }
        public int Health { get; private set; }
        public Player() : base(1, 1)
        {
            Animation.CurrentFrame[0].Glyph = 5;
            Animation.IsDirty = true;
            Animation.Start();
            this.CurrentDirection = CastleDirection.None;
            Health = 70;
        }

        public bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            // Process logic for moving the entity.
            bool keyHit = false;

            if (info.IsKeyDown(Keys.Up))
            {
                this.CurrentDirection = CastleDirection.Up;
                keyHit = true;
            }
            else if (info.IsKeyDown(Keys.Down))
            {
                this.CurrentDirection = CastleDirection.Down;
                keyHit = true;
            }

            if (info.IsKeyDown(Keys.Left))
            {
                this.CurrentDirection = CastleDirection.Left;
                keyHit = true;
            }
            else if (info.IsKeyDown(Keys.Right))
            {
                this.CurrentDirection = CastleDirection.Right;
                keyHit = true;
            }

            switch(CurrentDirection)
            {
                case CastleDirection.Up:
                    if(info.IsKeyReleased(Keys.Up))
                    {
                        this.CurrentDirection = CastleDirection.None;
                        keyHit = true;
                    }
                    break;
                case CastleDirection.Down:
                    if (info.IsKeyReleased(Keys.Down))
                    {
                        this.CurrentDirection = CastleDirection.None;
                        keyHit = true;
                    }
                    break;
                case CastleDirection.Left:
                    if (info.IsKeyReleased(Keys.Left))
                    {
                        this.CurrentDirection = CastleDirection.None;
                        keyHit = true;
                    }
                    break;
                case CastleDirection.Right:
                    if (info.IsKeyReleased(Keys.Right))
                    {
                        this.CurrentDirection = CastleDirection.None;
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
                Position = location;
            }
        }

        public override void OnCalculateRenderPosition()
        {
            base.OnCalculateRenderPosition();
        }

        public Point PreviewMove(CastleDirection direction)
        {
            Point preview = new Point(Position.X, Position.Y);

            switch (direction)
            {
                case CastleDirection.Up:
                    this.Facing = CastleDirection.Up;
                    preview.Y -= 1;
                    break;
                case CastleDirection.Down:
                    this.Facing = CastleDirection.Down;
                    preview.Y += 1;
                    break;
                case CastleDirection.Left:
                    this.Facing = CastleDirection.Left;
                    preview.X -= 1;
                    break;
                case CastleDirection.Right:
                    this.Facing = CastleDirection.Right;
                    preview.X += 1;
                    break;
            }

            return preview;
        }
        public Point GetFacingPoint(CastleDirection direction)
        {
            Point preview = new Point(Position.X, Position.Y);

            switch (Facing)
            {
                case CastleDirection.Up:
                    preview.Y -= 1;
                    break;
                case CastleDirection.Down:
                    preview.Y += 1;
                    break;
                case CastleDirection.Left:
                    preview.X -= 1;
                    break;
                case CastleDirection.Right:
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
