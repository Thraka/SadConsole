using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.DrawCalls
{
    public class DrawCallCell : IDrawCall
    {
        public Texture2D Texture;
        public Rectangle FontGlyphRect;
        public Rectangle FontSolidRect;
        public ColoredGlyph Cell;
        public Rectangle TargetRect;

        public DrawCallCell(SadConsole.ColoredGlyph cell, Texture2D texture, Rectangle targetRect, Rectangle fontGlyphRect, Rectangle fontSolidRect)
        {
            Texture = texture;
            FontGlyphRect = fontGlyphRect;
            FontSolidRect = fontSolidRect;
            TargetRect = targetRect;
            Cell = cell;
        }

        public void Draw()
        {
            Host.Global.SharedSpriteBatch.Draw(Texture, TargetRect, FontSolidRect, Cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
            Host.Global.SharedSpriteBatch.Draw(Texture, TargetRect, FontGlyphRect, Cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.65f);
        }
    }
}
