#if SFML
using Point = SFML.System.Vector2i;
using Rectangle = SFML.Graphics.IntRect;
using Texture2D = SFML.Graphics.Texture;
using Matrix = SFML.Graphics.Transform;
using SFML.Graphics;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif


namespace SadConsole.Consoles.Renderers
{
    public class RenderSystem
    {
        SpriteBatch Batch;

        public RenderSystem()
        {
#if SFML
            Batch = new SpriteBatch();
#else
            Batch = new SpriteBatch(Engine.Device);
#endif
        }

        public void Start(ITextSurfaceRendered surface, Matrix transform)
        {
#if SFML
            Batch.Initialize(surface);
#else

#endif
        }

        public void DrawBackground(ITextSurfaceRendered surface)
        {

        }

        public void DrawCell(Cell cell, Rectangle screenRect, Color defaultBackground, Font font)
        {
            if (cell.IsVisible)
            {
                

                if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != defaultBackground)
                    Batch.Draw(font.FontImage, screenRect, font.GlyphIndexRects[font.SolidGlyphIndex], cell.ActualBackground, 0f, , SpriteEffects.None, 0.3f);

                if (cell.ActualForeground != Color.Transparent)
                    Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphIndexRects[cell.ActualGlyphIndex], cell.ActualForeground, 0f, Vector2.Zero, cell.ActualSpriteEffect, 0.4f);
            }
        }
    }
}
