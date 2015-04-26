using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Effects;
using System.Runtime.Serialization;

namespace SadConsole
{
    /// <summary>
    /// Describes the appearance of a cell. This includes the foreground and background colors, and the effect.
    /// </summary>
    [DataContract]
    public class CellAppearance : ICellAppearance
    {
        /// <summary>
        /// The foreground color of the cell.
        /// </summary>
        [DataMember]
        public Color Foreground { get; set; }

        /// <summary>
        /// The background color of the cell.
        /// </summary>
        [DataMember]
        public Color Background { get; set; }

        /// <summary>
        /// The character index of the cell, used by a font sheet.
        /// </summary>
        [DataMember]
        public int CharacterIndex { get; set; }

        /// <summary>
        /// The SpriteBatch sprite mirror effect used when rendering the cell.
        /// </summary>
        [DataMember]
        public SpriteEffects SpriteEffect { get; set; }

        /// <summary>
        /// Creates a new instance of the cell appearance with a white foreground and transparent background.
        /// </summary>
        public CellAppearance() : this(Color.White, Color.Transparent, -1) { }

        /// <summary>
        /// Creates a new instance of the cell appearance with the specified colors and effect.
        /// </summary>
        /// <param name="foreground">The foreground color of the cell appearance.</param>
        /// <param name="background">The background color of the cell appearance.</param>
        public CellAppearance(Color foreground, Color background): this(foreground, background, 0) { }

        /// <summary>
        /// Creates a new instance of the cell appearance with the specified colors and effect.
        /// </summary>
        /// <param name="foreground">The foreground color of the cell appearance.</param>
        /// <param name="background">The background color of the cell appearance.</param>
        /// <param name="characterIndex">The character of the cell appearance.</param>
        public CellAppearance(Color foreground, Color background, int characterIndex): this(foreground, background, characterIndex, SpriteEffects.None) { }

        /// <summary>
        /// Creates a new instance of the cell appearance with the specified colors and effect.
        /// </summary>
        /// <param name="foreground">The foreground color of the cell appearance.</param>
        /// <param name="background">The background color of the cell appearance.</param>
        /// <param name="characterIndex">The character of the cell appearance.</param>
        /// <param name="spriteEffect">The sprite mirror effect of the cell appearance.</param>
        public CellAppearance(Color foreground, Color background, int characterIndex, SpriteEffects spriteEffect)
        {
            this.Foreground = foreground;
            this.Background = background;
            this.CharacterIndex = characterIndex;
            this.SpriteEffect = spriteEffect;
        }

        /// <summary>
        /// Applies this appearance instance values to the destination appearance.
        /// </summary>
        /// <param name="destination">The target of the appearance copy.</param>
        public void CopyAppearanceTo(ICellAppearance destination)
        {
            destination.Foreground = this.Foreground;
            destination.Background = this.Background;
            destination.SpriteEffect = this.SpriteEffect;

            if (this.CharacterIndex != -1)
                destination.CharacterIndex = this.CharacterIndex;
        }

        /// <summary>
        /// Swaps the foreground and background colors.
        /// </summary>
        public void SwapColors()
        {
            var tempColor = Foreground;
            Foreground = Background;
            Background = tempColor;
        }

        /// <summary>
        /// Creates a new copy of this cell appearance.
        /// </summary>
        /// <returns>The cloned cell appearance.</returns>
        public CellAppearance Clone()
        {
            return new CellAppearance(this.Foreground, this.Background, this.CharacterIndex);
        }
    }
}
