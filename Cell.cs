using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace SadConsole
{
    /// <summary>
    /// Represents an individual character on the screen with a foreground, background, and effect.
    /// </summary>
    [DataContract]
    public class Cell : ICellAppearance
    {
        private int _characterIndex;
        private Color _foreground;
        private Color _background;

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
        /// The character index of the graphic font to print when this cell is drawn. When set, copies the value to ActualCharacterIndex.
        /// </summary>
        [DataMember]
        public int CharacterIndex
        {
            get { return _characterIndex; }
            set
            {
                _characterIndex = value;
                ActualCharacterIndex = value;
            }
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
        /// The actual character index of this cell when drawing.
        /// <remarks>The actual index may or may not match the desired index. When effects are processed, they may change this value. If the effect is removed, the actual index is taken from desired index.</remarks>
        /// </summary>
        public virtual int ActualCharacterIndex { get; set; }

        /// <summary>
        /// The effect associated with this cell. Processed by the <see cref="T:SadConsole.CellSurface"/> class.
        /// </summary>
        public Effects.ICellEffect Effect { get; set; }

        /// <summary>
        /// The index of the cell in the parent <see cref="T:SadConsole.CellSurface"/>.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The position of the cell in the parent <see cref="T:SadConsole.CellSurface"/>.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// true when this cell will be drawn; otehrwise false.
        /// </summary>
        [DataMember]
        public bool IsVisible { get; set; }

        [DataMember]
        /// <summary>
        /// The SpriteBatch Sprite Effect used when rendering the cell. Defaults to None.
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.SpriteEffects SpriteEffect { get; set; }

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
            CharacterIndex = 0;
            IsVisible = true;
        }

        /// <summary>
        /// Returns a string representing the Actual* property values.
        /// </summary>
        /// <returns>A string representing this cell.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", ActualCharacterIndex, ActualForeground.ToString(), ActualBackground.ToString());
        }

        /// <summary>
        /// This should be called by anything creating and configuring a cell on the console. Configure the cell (foreground, background, index etc) and then call this.
        /// </summary>
        public virtual void OnCreated() { }

        /// <summary>
        /// Copies this cells information to a new cell. Preserves appearance, Actual* properties, and character information.
        /// </summary>
        /// <param name="destination">The cell to copy to.</param>
        public virtual void Copy(Cell destination)
        {
            CopyAppearanceTo(destination);
            destination.ActualCharacterIndex = this.ActualCharacterIndex;
            destination.ActualBackground = this.ActualBackground;
            destination.ActualForeground = this.ActualForeground;
        }

        /// <summary>
        /// Applies this appearance instance values to the destination appearance.
        /// </summary>
        /// <param name="destination">The target of the appearance copy.</param>
        public void CopyAppearanceTo(ICellAppearance destination)
        {
            destination.Foreground = this.Foreground;
            destination.Background = this.Background;
            destination.CharacterIndex = this.CharacterIndex;
        }

        /// <summary>
        /// Updates and applies the <see cref="P:SadConsole.Cell.Effect"/> to this cell. WARNING: Do not use with CellSurface.
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
