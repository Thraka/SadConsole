using System.Runtime.CompilerServices;
using SFMLColor = SFML.Graphics.Color;
using SadRogueColor = SadRogue.Primitives.Color;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogue.Primitives.Color"/> that enable operations involving
    /// <see cref="SFML.Graphics.Color"/>.
    /// </summary>
    public static class SadRogueColorExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Color"/> to <see cref="SFML.Graphics.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFMLColor ToSFMLColor(this SadRogueColor self) => new SFMLColor(self.R, self.G, self.B, self.A);

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Color"/> to a <see cref="SFML.Graphics.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRogueColor self, SFMLColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}

namespace SFML.Graphics
{
    /// <summary>
    /// Extension methods for <see cref="SFML.Graphics.Color"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Color"/>.
    /// </summary>
    public static class SFMLColorExtensions
    {
        /// <summary>
        /// Converts from <see cref="SFML.Graphics.Color"/> to <see cref="SadRogue.Primitives.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRogueColor ToSadRogueColor(this SFMLColor self)
            => new SadRogueColor(self.R, self.G, self.B, self.A);

        /// <summary>
        /// Compares a <see cref="SFML.Graphics.Color"/> to a  <see cref="SadRogue.Primitives.Color"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SFMLColor self, SadRogueColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}
