using Microsoft.Xna.Framework;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadRogueSharp.Entities
{
	class Hound : Entity
	{
		public Hound()
		{
			Animation anim = new Animation("default", 1, 1);
			var frame = anim.CreateFrame();
			frame[0].Foreground = Color.Red;
			frame[0].Background = Color.Black;
			frame[0].CharacterIndex = 72;
			anim.Commit();

			AddAnimation(anim);
			SetActiveAnimation(anim);
		}
	}
}
