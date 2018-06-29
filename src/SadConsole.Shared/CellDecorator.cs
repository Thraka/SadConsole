using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// Decorates a cell with a colored glyph.
    /// </summary>
    [DataContract]
    public struct CellDecorator : IEquatable<CellDecorator>
    {
        /// <summary>
        /// An empty cell decorator.
        /// </summary>
        public static CellDecorator Empty { get; }

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

        public static bool operator ==(CellDecorator left, CellDecorator right)
        {
            return left.Color == right.Color &&
                   left.Glyph == right.Glyph &&
                   left.Mirror == right.Mirror;
        }

        public static bool operator !=(CellDecorator left, CellDecorator right)
        {
            return left.Color != right.Color ||
                   left.Glyph != right.Glyph ||
                   left.Mirror != right.Mirror;
        }
    }
}
