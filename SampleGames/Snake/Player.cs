using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Consoles.Console;
using SadConsole.Entities;
using SadConsole.Consoles;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Snake
{
    internal enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    /// <summary>
    /// Player entity class
    /// </summary>
    internal class Player : Entity
    {
        public Direction CurrentDirection { get; private set; }

        public int MaxTailLength { get; set; }
        private int currentTailLength;
        public LinkedList<Point> TailNodes;

        public LinkedList<Point> RemoveNodes;

        public Player()
        {
            CurrentDirection = Direction.Right;

            MaxTailLength = 3;
            currentTailLength = 1;

            TailNodes = new LinkedList<Point>();
            RemoveNodes = new LinkedList<Point>();

            // Enable keyboard. This isn't really used right now. Currently, the engine sends keyboard
            // data the IConsole object set as the active console. We could add the entity as the active
            // console, and if we did that, would need this boolean set.
            _canUseKeyboard = true;

            // Update the default animation frame (cellsurface that is 1x1 of nothing) to have a smiley character
            _currentAnimation.CurrentFrame[0].CharacterIndex = 1;
        }

        public override bool ProcessKeyboard(SadConsole.Input.KeyboardInfo info)
        {
            // Process logic for moving the entity.
            bool keyHit = false;

            if (info.IsKeyDown(Keys.Up))
            {
                if (CurrentDirection != Direction.Down)
                {
                    this.CurrentDirection = Direction.Up;
                }
                keyHit = true;
            }
            else if (info.IsKeyDown(Keys.Down))
            {
                if (CurrentDirection != Direction.Up)
                {
                    this.CurrentDirection = Direction.Down;
                }
                keyHit = true;
            }

            if (info.IsKeyDown(Keys.Left))
            {
                if (CurrentDirection != Direction.Right)
                {
                    this.CurrentDirection = Direction.Left;
                }
                keyHit = true;
            }
            else if (info.IsKeyDown(Keys.Right))
            {
                if (CurrentDirection != Direction.Left)
                {
                    this.CurrentDirection = Direction.Right;
                }
                keyHit = true;
            }

            return keyHit;

        }

        public void Move()
        {
            switch(CurrentDirection)
            {
                case Direction.Up:
                    _position.Y -= 1;
                    break;

                case Direction.Down:
                    _position.Y += 1;
                    break;

                case Direction.Left:
                    _position.X -= 1;
                    break;

                case Direction.Right:
                    _position.X += 1;
                    break;
            }
        }

        public void SetStartingPosition(Point start)
        {
            TailNodes.AddFirst(start);
        }

        public void ProcessTail(Point point)
        {
            TailNodes.AddFirst(point);
            if (currentTailLength >= MaxTailLength)
            {
                RemoveNodes.Clear();
                RemoveNodes.AddFirst(TailNodes.Last.Value);
                TailNodes.RemoveLast();
                currentTailLength--;
            }
            currentTailLength++;
        }

    }
}
