using SadConsole.Surfaces;
using Color = Microsoft.Xna.Framework.Color;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// Helpers related to <see cref="Surfaces.AnimatedSurface"/> animations.
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
        public static AnimatedSurface CreateStatic(int width, int height, int frames, double blankChance)
        {
            var animation = new AnimatedSurface("default", width, height, Global.FontDefault);
            var editor = new SurfaceEditor(new NoDrawSurface(1, 1));

            for (int f = 0; f < frames; f++)
            {
                var frame = animation.CreateFrame();
                editor.TextSurface = frame;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int character = Global.Random.Next(48, 168);

                        if (Global.Random.NextDouble() <= blankChance)
                            character = 32;

                        editor.SetGlyph(x, y, character);
                        editor.SetForeground(x, y, Color.White * (float)(Global.Random.NextDouble() * (1.0d - 0.5d) + 0.5d));
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
