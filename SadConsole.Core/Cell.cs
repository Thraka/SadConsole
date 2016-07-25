using System.Runtime.Serialization;

#if SFML
using SFML.Graphics;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif


namespace SadConsole
{
    /// <summary>
    /// Represents an individual glyph on the screen with a foreground, background, and effect.
    /// </summary>
    [DataContract]
    public class Cell : ICellAppearance
    {
        private int _glyphIndex;
        private Color _foreground;
        private Color _background;
        private SpriteEffects _spriteEffect;

        /// <summary>
        /// The desired foreground color of this cell. When set, copies the value to ActualForeground.
        /// </summary>
        [DataMember]
        public Color Foreground
        {
            get { return _foreground; }
            set
            {
                _foreground = value;
                ActualForeground = value;
            }
        }
        /// <summary>
        /// The desired background color of this cell. When set, copies the value to ActualBackground.
        /// </summary>
        [DataMember]
        public Color Background
        {
            get { return _background; }
            set
            {
                _background = value;
                ActualBackground = value;
            }
        }

        /// <summary>
        /// The glyph index of the graphic font to print when this cell is drawn. When set, copies the value to <see cref="ActualGlyphIndex"/>.
        /// </summary>
        [DataMember]
        public int GlyphIndex
        {
            get { return _glyphIndex; }
            set
            {
                _glyphIndex = value;
                ActualGlyphIndex = value;
            }
        }

        /// <summary>
        /// The SpriteBatch sprite mirror effect used when rendering the cell. Defaults to None.
        /// </summary>
        [DataMember]
        public SpriteEffects SpriteEffect
        {
            get { return _spriteEffect; }
            set { _spriteEffect = value; ActualSpriteEffect = value; }
        }

        /// <summary>
        /// The actual foreground color of this cell when drawing.
        /// <remarks>The actual foreground may or may not match the desired foreground. When effects are processed, they will normally set this value. If the effect is removed, the actual foreground color is taken from desired foreground color.</remarks>
        /// </summary>
        public virtual Color ActualForeground { get; set; }

        /// <summary>
        /// The actual background color of this cell when drawing.
        /// <remarks>The actual background may or may not match the desired background. When effects are processed, they will normally set this value. If the effect is removed, the actual background color is taken from desired background color.</remarks>
        /// </summary>
        public virtual Color ActualBackground { get; set; }


        /// <summary>
        /// The actual glyph index of this cell when drawing.
        /// <remarks>The actual index may or may not match the desired index. When effects are processed, they may change this value. If the effect is removed, the actual index is taken from desired index.</remarks>
        /// </summary>
        public virtual int ActualGlyphIndex { get; set; }

        /// <summary>
        /// The effect associated with this cell. Processed by the <see cref="T:SadConsole.TextSurface"/> class.
        /// </summary>
        public Effects.ICellEffect Effect { get; set; }

        /// <summary>
        /// true when this cell will be drawn; otehrwise false.
        /// </summary>
        [DataMember]
        public bool IsVisible { get; set; }

        [DataMember]
        /// <summary>
        /// The SpriteBatch sprite mirror effect used when rendering the cell.
        /// <remarks>The actual sprite effect may or may not match the desired sprite effect. When cell effects are processed, they may change this value. If the cell effect is removed, the actual sprite effect is taken from desired sprite effect.</remarks>
        /// </summary>
        public SpriteEffects ActualSpriteEffect { get; set; }

#region Constructors
        public Cell()
        {
            Reset();
        }
#endregion

        /// <summary>
        /// Resets this cell with default values.
        /// </summary>
        public virtual void Reset()
        {
            Foreground = Color.White;
            Background = Color.Transparent;
            GlyphIndex = 0;
            IsVisible = true;
            SpriteEffect = SpriteEffects.None;
            Effect = null;
        }

        /// <summary>
        /// Returns a string representing the Actual* property values.
        /// </summary>
        /// <returns>A string representing this cell.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", ActualGlyphIndex, ActualForeground.ToString(), ActualBackground.ToString());
        }

        /// <summary>
        /// This should be called by anything creating and configuring a cell on the console. Configure the cell (foreground, background, index etc) and then call this.
        /// </summary>
        public virtual void OnCreated() { }

        /// <summary>
        /// Copies this cells information to a new cell. Preserves appearance, Actual* properties, and glyph information.
        /// </summary>
        /// <param name="destination">The cell to copy to.</param>
        public virtual void Copy(Cell destination)
        {
            CopyAppearanceTo(destination);
            destination.ActualGlyphIndex = this.ActualGlyphIndex;
            destination.ActualBackground = this.ActualBackground;
            destination.ActualForeground = this.ActualForeground;
            destination.ActualSpriteEffect = this.ActualSpriteEffect;
        }

        /// <summary>
        /// Applies this appearance instance values to the destination appearance.
        /// </summary>
        /// <param name="destination">The target of the appearance copy.</param>
        public void CopyAppearanceTo(ICellAppearance destination)
        {
            destination.Foreground = this.Foreground;
            destination.Background = this.Background;
            destination.GlyphIndex = this.GlyphIndex;
            destination.SpriteEffect = this.SpriteEffect;
        }

        /// <summary>
        /// Updates and applies the <see cref="P:SadConsole.Cell.Effect"/> to this cell. WARNING: Do not use with TextSurface. This should only be called when the cell has a standalone effect that isn't managed by the TextSurface.
        /// </summary>
        public void UpdateAndApplyEffect(double elapsedTime)
        {
            if (Effect != null)
            {
                Effect.Update(elapsedTime);
                Effect.Apply(this);
            }
        }
    }
}
