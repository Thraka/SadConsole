using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadRogueSharp.Entities
{
	class Player : SadConsole.GameHelpers.GameObject
    {
		public Player(): base(SadConsole.Global.FontDefault)
		{
			var anim = new SadConsole.Surfaces.AnimatedSurface("default", 1, 1);
			var frame = anim.CreateFrame();
			frame[0].Foreground = Color.Yellow;
			frame[0].Background = Color.Black;
			frame[0].Glyph = 1;

            Animation = anim;
            anim.Start();
		}
	}
}
