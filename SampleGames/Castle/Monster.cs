using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Consoles.Console;
using SadConsole.Entities;
using SadConsole.Consoles;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Castle
{
    internal class Monster : Entity
    {
        public int RoomId { get; private set; }
        public bool IsAlive { get; private set; }
        public string Name { get; private set; }
        public string InventoryName { get; private set; }
        public int Character { get; private set; }
        public int Health { get; private set; }
        public string Description { get; private set; }
        public string DeadDescription { get; private set; }
        public int Value { get; protected set; }
        public bool IsGuard { get; protected set; }

        public Monster(String name, String inventoryName, int roomId, int x, int y, int character, int health, string description, string deadDescription)
        {
            this.IsAlive = true;
            this.Name = inventoryName;
            this.InventoryName = name;
            this.RoomId = roomId;
            _position.X = x;
            _position.Y = y;
            this.Character = character;
            this.Health = health;
            this.Description = description;
            this.DeadDescription = deadDescription;
            this.Value = 100;
            this.IsGuard = false;

            _canUseKeyboard = false;
            _currentAnimation.CurrentFrame[0].CharacterIndex = character;
            
        }

        public virtual Point PreviewMove(Point playerLocation)
        {
            Point preview = new Point(_position.X, _position.Y);

            int xDistance = Math.Abs(playerLocation.X - preview.X);
            int yDistance = Math.Abs(playerLocation.Y - preview.Y);

            if(xDistance >= yDistance)
            {
                if (playerLocation.X > preview.X)
                {
                    preview.X += 1;
                }
                else if (playerLocation.X < preview.X)
                {
                    preview.X -= 1;
                }
            }
            else
            {
                if (playerLocation.Y > preview.Y)
                {
                    preview.Y += 1;
                }
                else if (playerLocation.Y < preview.Y)
                {
                    preview.Y -= 1;
                }
            }

            return preview;
        }

        public void Move(Point location)
        {
            if (this.IsVisible)
            {
                _position.X = location.X;
                _position.Y = location.Y;
            }
        }

        public virtual UserMessage Hit(CastleItem sword)
        {
            UserMessage returnMessage = new UserMessage();

            if(sword == null)
            {
                returnMessage.AddLine("You have no");
                returnMessage.AddLine("Weapon!");
            }
            else if(this.Health > 0)
            {
                Health--;
                if(this.Health == 0)
                {
                    IsAlive = false;
                    returnMessage.AddLine("You Killed");
                    returnMessage.AddLine(String.Format("the {0}", Name));

                }
                else
                {
                    returnMessage.AddLine("You Struck");
                    returnMessage.AddLine(String.Format("the {0}", Name));
                }
            }
            return returnMessage;
        }
        public virtual UserMessage Show(CastleItem item)
        {
            UserMessage returnMessage = new UserMessage();

            returnMessage.AddLine(String.Format("The {0}", Name));
            returnMessage.AddLine("looks at you");
            returnMessage.AddLine("Funny!");

            return returnMessage;
        }

        public virtual UserMessage Play(CastleItem item)
        {
            return null;
        }

    }

    internal class Guard : Monster
    {
        private string playItem;
        private string showItem;
        private string showDescription;

        public Guard(String name, String inventoryName, int roomId, int x, int y, int character, string description, string showItem, string playItem, string showDescription)
            :base(name, inventoryName, roomId, x, y, character, 100, description, description)
        {
            this.showItem = showItem;
            this.playItem = playItem;
            this.showDescription = showDescription;
            this.Value = 0;
            this.IsGuard = true;
        }

        public override Point PreviewMove(Point playerLocation)
        {
            return _position;
        }

        public override UserMessage Hit(CastleItem sword)
        {
            UserMessage returnMessage = new UserMessage();

            returnMessage.AddLine(String.Format("The {0}", Name));
            returnMessage.AddLine("is blocking");
            returnMessage.AddLine("your way. He");
            returnMessage.AddLine("can't be hurt.");

            return returnMessage;
        }

        public override UserMessage Show(CastleItem item)
        {
            UserMessage returnMessage = new UserMessage();

            if (showItem == null)
            {
                returnMessage.AddLine(String.Format("The {0}", Name));
                returnMessage.AddLine("looks at you");
                returnMessage.AddLine("Funny!");
            }
            else
            {
                if (String.Compare(item.InventoryName, showItem, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    this.IsVisible = false;
                    returnMessage.AddLine(showDescription);

                }
                else
                {
                    returnMessage.AddLine(String.Format("The {0}", Name));
                    returnMessage.AddLine("looks at you");
                    returnMessage.AddLine("Funny!");
                }
            }
            return returnMessage;
        }

        public override UserMessage Play(CastleItem item)
        {

            UserMessage returnMessage = null;

            if (playItem != null)
            {
                if (String.Compare(item.InventoryName, playItem, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    returnMessage = new UserMessage();
                    this.IsVisible = false;
                    returnMessage.AddLine(showDescription);

                }
            }
            return returnMessage;
        }



    }
}
