using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadRogueSharp.Entities
{
	class Hound : SadConsole.Game.GameObject
    {
		public Hound(): base(SadConsole.Engine.DefaultFont)
		{
			AnimatedTextSurface anim = new AnimatedTextSurface("default", 1, 1, SadConsole.Engine.DefaultFont);
			var frame = anim.CreateFrame();
			frame[0].Foreground = Color.Red;
			frame[0].Background = Color.Black;
			frame[0].GlyphIndex = 72;

			Animation = anim;
		}
	}
}
