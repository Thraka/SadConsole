using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

#if MONOGAME
using Color = Microsoft.Xna.Framework.Color;
#endif

namespace SadConsole
{
    /// <summary>
    /// Represents an individual glyph on the screen with a foreground, background, and mirror effect.
    /// </summary>
    public class Cell
    {
        public Color Foreground;
        public Color Background;
        public int Glyph;
        public SpriteEffects Mirror;

        public CellState? State { get; private set; }

        public Cell(): this(Color.White, Color.Black, 0, SpriteEffects.None) { }

        public Cell(Color foreground): this(foreground, Color.Black, 0, SpriteEffects.None) { }

        public Cell(Color foreground, Color background): this(foreground, background, 0, SpriteEffects.None) { }

        public Cell(Color foreground, Color background, int glyph): this(foreground, background, glyph, SpriteEffects.None) { }

        public Cell(Color foreground, Color background, int glyph, SpriteEffects mirror)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Mirror = mirror;
        }

        public void CopyAppearanceTo(ref Cell cell)
        {
            cell.Foreground = this.Foreground;
            cell.Background = this.Background;
            cell.Glyph = this.Glyph;
            cell.Mirror = this.Mirror;
        }

        public void CopyAppearanceFrom(Cell cell)
        {
            this.Foreground = cell.Foreground;
            this.Background = cell.Background;
            this.Glyph = cell.Glyph;
            this.Mirror = cell.Mirror;
        }

        public void Clear()
        {
            Foreground = Color.White;
            Background = Color.Black;
            Glyph = 0;
            Mirror = SpriteEffects.None;
        }

        public void SaveState()
        {
            State = new CellState(Foreground, Background, Glyph, Mirror);
        }

        public void RestoreState()
        {
            if (State.HasValue)
            {
                Foreground = State.Value.Foreground;
                Background = State.Value.Background;
                Glyph = State.Value.Glyph;
                Mirror = State.Value.Mirror;

                State = null;
            }
        }

        public Cell Clone()
        {
            return new Cell(Foreground, Background, Glyph, Mirror);
        }
    }

    public struct CellState
    {
        public readonly Color Foreground;
        public readonly Color Background;
        public readonly int Glyph;
        public readonly SpriteEffects Mirror;

        public CellState(Color foreground, Color background, int glyph, SpriteEffects mirror)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Mirror = mirror;
        }
    }
}
