using Microsoft.Xna.Framework;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadRogueSharp.Entities
{
	class Player : Entity
	{
		public Player()
		{
			Animation anim = new Animation("default", 1, 1);
			var frame = anim.CreateFrame();
			frame[0].Foreground = Color.Yellow;
			frame[0].Background = Color.Black;
			frame[0].CharacterIndex = 1;
			anim.Commit();

			AddAnimation(anim);
			SetActiveAnimation(anim);
		}
	}
}
