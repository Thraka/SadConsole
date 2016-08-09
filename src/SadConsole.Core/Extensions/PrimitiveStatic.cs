#if SFML
using Point = SFML.System.Vector2i;
using Vector2 = SFML.System.Vector2f;
using SFML.System;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    /// <summary>
    /// Static primitives for compatibility with other rendering engines (monogame, sfml)
    /// </summary>
    public static class PrimitiveStatic
    {
        private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

        /// <summary>
        /// Returns a <see cref="Vector2"/> with components 0, 0.
        /// </summary>
        public static Vector2 Vector2Zero
        {
            get { return zeroVector; }
        }

    }
}
