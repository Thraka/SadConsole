using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// Helpers regarding <see cref="SadConsole.Entities.Entity"/> types and animations.
    /// </summary>
    public static class Entities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="frames"></param>
        /// <param name="blankChance"></param>
        /// <returns></returns>
        public static SadConsole.Entities.Entity CreateStaticEntity(int width, int height, int frames, double blankChance)
        {
            SadConsole.Entities.Entity entity = new SadConsole.Entities.Entity(width, height);
            SadConsole.Entities.Animation animation = new SadConsole.Entities.Animation("default", width, height);
            

            for (int f = 0; f < frames; f++)
            {
                var frame = animation.CreateFrame();

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int character = Engine.Random.Next(48, 168);

                        if (Engine.Random.NextDouble() <= blankChance)
                            character = 32;


                        frame.SetCharacter(x, y, character);
                        frame.SetForeground(x, y, Microsoft.Xna.Framework.Color.White * (float)(Engine.Random.NextDouble() * (1.0d - 0.5d) + 0.5d));
                    }
                }

            }

            animation.AnimationDuration = 1;
            animation.Repeat = true;

            entity.AddAnimation(animation);
            entity.SetActiveAnimation(animation);
            entity.Start();

            return entity;
        }
    }
}
