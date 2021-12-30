using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using MonoColor = Microsoft.Xna.Framework.Color;
using SadRogueColor = SadRogue.Primitives.Color;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogue.Primitives.Color"/> that enable operations involving
    /// <see cref="Microsoft.Xna.Framework.Color"/>.
    /// </summary>
    public static class SadRogueColorExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Color"/> to <see cref="Microsoft.Xna.Framework.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoColor ToMonoColor(this SadRogueColor self) => new MonoColor(self.R, self.G, self.B, self.A);

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Color"/> to a <see cref="Microsoft.Xna.Framework.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRogueColor self, MonoColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.Xna.Framework.Color"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Color"/>.
    /// </summary>
    public static class MonoColorExtensions
    {
        /// <summary>
        /// Converts from <see cref="Microsoft.Xna.Framework.Color"/> to <see cref="SadRogue.Primitives.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRogueColor ToSadRogueColor(this MonoColor self)
            => new SadRogueColor(self.R, self.G, self.B, self.A);

        /// <summary>
        /// Compares a <see cref="Microsoft.Xna.Framework.Color"/> to a <see cref="SadRogue.Primitives.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this MonoColor self, SadRogueColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}
