using System.Runtime.CompilerServices;
using RaylibColor = Raylib_cs.Color;
using SadRogueColor = SadRogue.Primitives.Color;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogue.Primitives.Color"/> that enable operations involving
    /// <see cref="RaylibColor"/>.
    /// </summary>
    public static class SadRogueColorExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Color"/> to <see cref="RaylibColor"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RaylibColor ToHostColor(this SadRogueColor self) => new RaylibColor(self.R, self.G, self.B, self.A);

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Color"/> to a <see cref="RaylibColor"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRogueColor self, RaylibColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}

namespace Raylib_cs
{
    /// <summary>
    /// Extension methods for <see cref="RaylibColor"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Color"/>.
    /// </summary>
    public static class RaylibColorExtensions
    {
        /// <summary>
        /// Converts from <see cref="RaylibColor"/> to <see cref="SadRogue.Primitives.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRogueColor ToSadRogueColor(this RaylibColor self)
            => new SadRogueColor(self.R, self.G, self.B, self.A);

        /// <summary>
        /// Compares a <see cref="RaylibColor"/> to a <see cref="SadRogue.Primitives.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this RaylibColor self, SadRogueColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}
