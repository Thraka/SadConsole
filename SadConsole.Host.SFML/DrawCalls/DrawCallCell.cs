using SadRogue.Primitives;
using Rectangle = SFML.Graphics.IntRect;
using Color = SFML.Graphics.Color;
using SFML.Graphics;

namespace SadConsole.DrawCalls
{
    public class DrawCallCell : IDrawCall
    {
        public Font Font;
        public ColoredGlyph Cell;
        public Rectangle TargetRect;
        public bool DrawBackground;

        public DrawCallCell(SadConsole.ColoredGlyph cell, Rectangle targetRect, Font font, bool drawBackground)
        {
            Font = font;
            TargetRect = targetRect;
            Cell = cell;
            DrawBackground = drawBackground;
        }

        public void Draw()
        {
            Host.Global.SharedSpriteBatch.DrawCell(Cell, TargetRect, DrawBackground, Font); 
        }
    }
}
