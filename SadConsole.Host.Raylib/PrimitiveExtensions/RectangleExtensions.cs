using System.Runtime.CompilerServices;
using SadConsole;
using HostRectangle = Raylib_cs.Rectangle;
using SadRogueRectangle = SadRogue.Primitives.Rectangle;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogueRectangle"/> that enable operations involving
    /// <see cref="HostRectangle"/>.
    /// </summary>
    public static class SadRogueRectangleExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogueRectangle"/> to <see cref="HostRectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HostRectangle ToHostRectangle(this SadRogueRectangle self)
            => new HostRectangle(self.X, self.Y, self.Width, self.Height);

        /// <summary>
        /// Converts from <see cref="SadRogueRectangle"/> to <see cref="HostRectangle"/> and applies the specified <see cref="Mirror"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="mirror">The mirror setting to apply to the rectangle.</param>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HostRectangle ToHostRectangle(this SadRogueRectangle self, Mirror mirror)
            => new HostRectangle(self.X,
                                 self.Y,
                                 Helpers.HasFlag((int)mirror, (int)Mirror.Horizontal) ? -self.Width : self.Width,
                                 Helpers.HasFlag((int)mirror, (int)Mirror.Vertical) ? -self.Height : self.Height);

        /// <summary>
        /// Compares a <see cref="SadRogueRectangle"/> to a <see cref="HostRectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRogueRectangle self, HostRectangle other)
            => self.X == other.X && self.Y == other.Y && self.Width == other.Width && self.Height == other.Height;
    }
}

namespace Raylib_cs
{
    /// <summary>
    /// Extension methods for <see cref="HostRectangle"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Rectangle"/>.
    /// </summary>
    public static class HostRectangleExtensions
    {
        /// <summary>
        /// Converts from <see cref="HostRectangle"/> to <see cref="SadRogue.Primitives.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRogueRectangle ToRectangle(this HostRectangle self)
            => new SadRogueRectangle((int)self.X, (int)self.Y, (int)self.Width, (int)self.Height);

        /// <summary>
        /// Compares a <see cref="HostRectangle"/> to a <see cref="SadRogue.Primitives.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this HostRectangle self, SadRogueRectangle other)
            => self.X == other.X && self.Y == other.Y && self.Width == other.Width && self.Height == other.Height;
    }
}
