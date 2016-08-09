#if SFML
using Color = SFML.Graphics.Color;
#elif MONOGAME
using Color = Microsoft.Xna.Framework.Color;
#endif

namespace SadConsole.Game
{
    /// <summary>
    /// Helpers related to <see cref="Consoles.AnimatedTextSurface"/> animations.
    /// </summary>
    public static class Animations
    {
        /// <summary>
        /// Creates an animated surface that looks like static.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="frames">How many frames the animation should have.</param>
        /// <param name="blankChance">Chance a character will be blank. Characters are between index 48-158. Chance is evaluated versus <see cref="System.Random.NextDouble"/>.</param>
        /// <returns>An animation.</returns>
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
#if SFML
                        byte randomValue = (byte)((float)(Engine.Random.NextDouble() * (1.0d - 0.5d) + 0.5d) * 255);
                        editor.SetForeground(x, y, new Color(randomValue, randomValue, randomValue));
#elif MONOGAME
                        editor.SetForeground(x, y, Color.White * (float)(Engine.Random.NextDouble() * (1.0d - 0.5d) + 0.5d));
#endif
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
