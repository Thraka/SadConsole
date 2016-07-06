using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// Helpers related to <see cref="Consoles.AnimatedTextSurface"/> animations.
    /// </summary>
    public static class Animation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="frames"></param>
        /// <param name="blankChance"></param>
        /// <returns></returns>
        public static Consoles.AnimatedTextSurface CreateStatic(int width, int height, int frames, double blankChance)
        {
            var animation = new Consoles.AnimatedTextSurface("default", width, height, Engine.DefaultFont);
            var editor = new Consoles.SurfaceEditor(new Consoles.TextSurface(1, 1, Engine.DefaultFont));

            for (int f = 0; f < frames; f++)
            {
                var frame = animation.CreateFrame();
                editor.TextSurface = frame;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int character = Engine.Random.Next(48, 168);

                        if (Engine.Random.NextDouble() <= blankChance)
                            character = 32;

                        editor.SetGlyph(x, y, character);
                        editor.SetForeground(x, y, Microsoft.Xna.Framework.Color.White * (float)(Engine.Random.NextDouble() * (1.0d - 0.5d) + 0.5d));
                    }
                }

            }

            animation.AnimationDuration = 1;
            animation.Repeat = true;
            
            animation.Start();

            return animation;
        }
    }
}
