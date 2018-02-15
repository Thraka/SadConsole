using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole
{
    /// <summary>
    /// Represents an individual glyph on the screen with a foreground, background, and mirror effect.
    /// </summary>
    public interface ICell
    {
        /// <summary>
        /// A temporary state to backup and restore a cell.
        /// </summary>
        CellState? State { get; }

        /// <summary>
        /// The foreground color of this cell.
        /// </summary>
        Color Foreground { get; set; }

        /// <summary>
        /// The background color of this cell.
        /// </summary>
        Color Background { get; set; }

        /// <summary>
        /// The glyph index from a font for this cell.
        /// </summary>
        int Glyph { get; set; }

        /// <summary>
        /// The mirror effect for this cell.
        /// </summary>
        SpriteEffects Mirror { get; set; }

        /// <summary>
        /// When true, indicates this cell should be drawn.
        /// </summary>
        bool IsVisible { get; set; }

        void Clear();
        void ClearState();
        Cell Clone();
        void CopyAppearanceFrom(ICell cell);
        void CopyAppearanceTo(ICell cell);
        void Draw(SpriteBatch batch, Point position, Point size, Font font);
        void Draw(SpriteBatch batch, Rectangle drawingRectangle, Font font);
        void RestoreState();
        void SaveState();
    }
}