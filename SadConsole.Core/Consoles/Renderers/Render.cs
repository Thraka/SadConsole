#if SFML
using Point = SFML.System.Vector2i;
using Rectangle = SFML.Graphics.IntRect;
using Texture2D = SFML.Graphics.Texture;
using Matrix = SFML.Graphics.Transform;
using SFML.Graphics;
using System;
using SadConsole;
using SadConsole.Consoles;


namespace SFML.Graphics
{
    public class SpriteBatch
    {
        Vertex[] m_verticies;
        int vertIndexCounter;
        SadConsole.Font font;
        Rectangle solidRect;
        Color backColor;
        Rectangle fillRect;
        Matrix transform;

        public SpriteBatch()
        {
            m_verticies = new Vertex[0];
        }

        public void Start(ITextSurfaceRendered surface, Matrix transform)
        {
            backColor = surface.DefaultBackground;
            fillRect = surface.AbsoluteArea;
            font = surface.Font;
            solidRect = font.GlyphIndexRects[font.SolidGlyphIndex];
            this.transform = transform;

            int count = 4 + (surface.RenderCells.Length * 4 * 2) + 4;
            if (m_verticies.Length != count)
                m_verticies = new Vertex[count];

            vertIndexCounter = 0;
        }

        public unsafe void DrawSurfaceFill(Color color, Color filter)
        {
            if (color != filter)
            {
                fixed (Vertex* verts = m_verticies)
                {
                    verts[vertIndexCounter].Position.X = fillRect.Left;
                    verts[vertIndexCounter].Position.Y = fillRect.Top;
                    verts[vertIndexCounter].TexCoords.X = solidRect.Left;
                    verts[vertIndexCounter].TexCoords.Y = solidRect.Top;
                    verts[vertIndexCounter].Color = color;
                    vertIndexCounter++;

                    verts[vertIndexCounter].Position.X = fillRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[vertIndexCounter].Position.Y = fillRect.Top;
                    verts[vertIndexCounter].TexCoords.X = solidRect.Width;
                    verts[vertIndexCounter].TexCoords.Y = solidRect.Top;
                    verts[vertIndexCounter].Color = color;
                    vertIndexCounter++;

                    verts[vertIndexCounter].Position.X = fillRect.Width;
                    verts[vertIndexCounter].Position.Y = fillRect.Height;
                    verts[vertIndexCounter].TexCoords.X = solidRect.Width;
                    verts[vertIndexCounter].TexCoords.Y = solidRect.Height;
                    verts[vertIndexCounter].Color = color;
                    vertIndexCounter++;

                    verts[vertIndexCounter].Position.X = fillRect.Left;
                    verts[vertIndexCounter].Position.Y = fillRect.Height;
                    verts[vertIndexCounter].TexCoords.X = solidRect.Left;
                    verts[vertIndexCounter].TexCoords.Y = solidRect.Height;
                    verts[vertIndexCounter].Color = color;
                    vertIndexCounter++;
                }
            }
        }

        public unsafe void DrawCell(Cell cell, Rectangle screenRect, Color defaultBackground, SadConsole.Font font)
        {
            if (cell.IsVisible)
            {
                var glyphRect = font.GlyphIndexRects[cell.ActualGlyphIndex];

                if (cell.ActualSpriteEffect == SpriteEffects.FlipHorizontally)
                {

                }
                else if (cell.ActualSpriteEffect == SpriteEffects.FlipVertically)
                {

                }

                fixed (Vertex* verts = m_verticies)
                {
                    if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != defaultBackground)
                    {
                        // Background
                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Top;
                        verts[vertIndexCounter].Color = cell.ActualBackground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Top;
                        verts[vertIndexCounter].Color = cell.ActualBackground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Height;
                        verts[vertIndexCounter].Color = cell.ActualBackground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Height;
                        verts[vertIndexCounter].Color = cell.ActualBackground;
                        vertIndexCounter++;
                    }

                    if (cell.ActualForeground != Color.Transparent)
                    {
                        // Foreground
                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Top;
                        verts[vertIndexCounter].Color = cell.ActualForeground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Top;
                        verts[vertIndexCounter].Color = cell.ActualForeground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Height;
                        verts[vertIndexCounter].Color = cell.ActualForeground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Height;
                        verts[vertIndexCounter].Color = cell.ActualForeground;
                        vertIndexCounter++;

                        
                    }
                }


            }
        }

        public void End(RenderTarget target, RenderStates state)
        {
            Vertex[] vertCopy = new Vertex[vertIndexCounter];
            Array.Copy(m_verticies, vertCopy, vertCopy.Length);
            state.Transform *= transform;
            state.Texture = font.FontImage;
            target.Draw(vertCopy, PrimitiveType.Quads, state);
        }
    }
}
#endif
