using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using MonoRectangle = Microsoft.Xna.Framework.Rectangle;
using SadRogueRectangle = SadRogue.Primitives.Rectangle;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogue.Primitives.Rectangle"/> that enable operations involving
    /// <see cref="Microsoft.Xna.Framework.Rectangle"/>.
    /// </summary>
    public static class SadRogueRectangleExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Rectangle"/> to <see cref="Microsoft.Xna.Framework.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoRectangle ToMonoRectangle(this SadRogueRectangle self)
            => new MonoRectangle(self.X, self.Y, self.Width, self.Height);

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Rectangle"/> to a <see cref="Microsoft.Xna.Framework.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRogueRectangle self, MonoRectangle other)
            => self.X == other.X && self.Y == other.Y && self.Width == other.Width && self.Height == other.Height;
    }
}

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.Xna.Framework.Rectangle"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Rectangle"/>.
    /// </summary>
    public static class MonoRectangleExtensions
    {
        /// <summary>
        /// Converts from <see cref="Microsoft.Xna.Framework.Rectangle"/> to <see cref="SadRogue.Primitives.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRogueRectangle ToRectangle(this MonoRectangle self)
            => new SadRogueRectangle(self.X, self.Y, self.Width, self.Height);

        /// <summary>
        /// Compares a <see cref="Microsoft.Xna.Framework.Rectangle"/> to a <see cref="SadRogue.Primitives.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this MonoRectangle self, SadRogueRectangle other)
            => self.X == other.X && self.Y == other.Y && self.Width == other.Width && self.Height == other.Height;
    }
}
