#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Decorates a cell with a colored glyph.
    /// </summary>
    [DataContract]
    public readonly struct CellDecorator : IEquatable<CellDecorator>
    {
        /// <summary>
        /// An empty cell decorator.
        /// </summary>
        public static CellDecorator Empty => default;

        /// <summary>
        /// Foreground color of the decorator.
        /// </summary>
        [DataMember]
        public readonly Color Color;

        /// <summary>
        /// Glyph of the decorator.
        /// </summary>
        [DataMember]
        public readonly int Glyph;

        /// <summary>
        /// Mirror setting of the decorator.
        /// </summary>
        [DataMember]
        public readonly SpriteEffects Mirror;

        /// <summary>
        /// Creates a new decorator with the specified colors, glyph, visiblity, and mirror settings.
        /// </summary>
        /// <param name="color">Foreground color.</param>
        /// <param name="glyph">Glyph value.</param>
        /// <param name="mirror">Mirror setting.</param>
        public CellDecorator(Color color, int glyph, SpriteEffects mirror)
        {
            Color = color;
            Glyph = glyph;
            Mirror = mirror;
        }

        /// <summary>
        /// Determines if this object has the same value as the other.
        /// </summary>
        /// <param name="other">The object to test against.</param>
        /// <returns>True if the objects have the same values.</returns>
        public bool Equals(CellDecorator other) => other == this;

        /// <summary>
        /// Checks that the left and right objects match.
        /// </summary>
        /// <param name="left">The first object to test.</param>
        /// <param name="right">The second object to test.</param>
        /// <returns>True when the <see cref="Color"/>, <see cref="Glyph"/>, and <see cref="Mirror"/> match.</returns>
        public static bool operator ==(CellDecorator left, CellDecorator right) => left.Color == right.Color &&
                   left.Glyph == right.Glyph &&
                   left.Mirror == right.Mirror;

        /// <summary>
        /// Checks that the left and right objects do not match.
        /// </summary>
        /// <param name="left">The first object to test.</param>
        /// <param name="right">The second object to test.</param>
        /// <returns>True when the <see cref="Color"/>, <see cref="Glyph"/>, and <see cref="Mirror"/> do not match.</returns>
        public static bool operator !=(CellDecorator left, CellDecorator right) => left.Color != right.Color ||
                   left.Glyph != right.Glyph ||
                   left.Mirror != right.Mirror;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is CellDecorator decorator && Equals(decorator);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Glyph;
                hashCode = (hashCode * 397) ^ (int)Mirror;
                return hashCode;
            }
        }
    }
}
