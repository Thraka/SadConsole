using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadRogueSharp.Entities
{
	class Player : SadConsole.Game.GameObject
    {
		public Player(): base(SadConsole.Engine.DefaultFont)
		{
			AnimatedTextSurface anim = new AnimatedTextSurface("default", 1, 1, SadConsole.Engine.DefaultFont);
			var frame = anim.CreateFrame();
			frame[0].Foreground = Color.Yellow;
			frame[0].Background = Color.Black;
			frame[0].GlyphIndex = 1;

            Animation = anim;
            anim.Start();
		}
	}
}
