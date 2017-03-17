using Microsoft.Xna.Framework;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadRogueSharp.Entities
{
	class Hound : SadConsole.GameHelpers.GameObject
    {
		public Hound(): base()
		{
			var anim = new AnimatedSurface("default", 1, 1);
			var frame = anim.CreateFrame();
			frame[0].Foreground = Color.Red;
			frame[0].Background = Color.Black;
			frame[0].Glyph = 72;

			Animation = anim;
		}
	}
}
