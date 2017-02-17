using System.Collections.Generic;
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
    internal class Player
    {
        private int currentTailLength;
        private Point position;

        public LinkedList<Point> TailNodes;
        public LinkedList<Point> RemoveNodes;

        public Direction CurrentDirection { get; private set; }
        public Point Position { get { return position; } set { position = value; } }

        public int MaxTailLength { get; set; }
        
        public Player()
        {
            CurrentDirection = Direction.Right;

            MaxTailLength = 3;
            currentTailLength = 1;

            TailNodes = new LinkedList<Point>();
            RemoveNodes = new LinkedList<Point>();
        }

        public bool ProcessKeyboard(SadConsole.Input.Keyboard info)
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
                    position.Y -= 1;
                    break;

                case Direction.Down:
                    position.Y += 1;
                    break;

                case Direction.Left:
                    position.X -= 1;
                    break;

                case Direction.Right:
                    position.X += 1;
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
