#if SFML
using Rectangle = SFML.Graphics.IntRect;
using Texture2D = SFML.Graphics.Texture;
using Matrix = SFML.Graphics.Transform;
using SadConsole;
using SadConsole.Consoles;


namespace SFML.Graphics
{
    public class SpriteBatch
    {
        Vertex[] m_verticies;
        int vertIndexCounter;
        Texture2D texture;
        Rectangle solidRect;
        Rectangle fillRect;
        Matrix transform;

        public SpriteBatch()
        {
            m_verticies = new Vertex[0];
        }

        public void Start(int renderQuads, Texture2D texture, Matrix transform)
        {
            int count = 4 * renderQuads;

            if (m_verticies.Length != count)
                m_verticies = new Vertex[count];

            vertIndexCounter = 0;
            this.texture = texture;
            this.transform = transform;
        }

        public void Start(ITextSurfaceRendered surface, Matrix transform, int additionalDraws = 0)
        {
            fillRect = surface.AbsoluteArea;
            texture = surface.Font.FontImage;
            solidRect = surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex];
            this.transform = transform;

            int count = ((surface.RenderCells.Length + 8 + additionalDraws) * 4 * 2);
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

        public unsafe void DrawQuad(Rectangle screenRect, Rectangle textCoords, Color color)
        {
            fixed (Vertex* verts = m_verticies)
            {
                verts[vertIndexCounter].Position.X = screenRect.Left;
                verts[vertIndexCounter].Position.Y = screenRect.Top;
                verts[vertIndexCounter].TexCoords.X = textCoords.Left;
                verts[vertIndexCounter].TexCoords.Y = textCoords.Top;
                verts[vertIndexCounter].Color = color;
                vertIndexCounter++;

                verts[vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                verts[vertIndexCounter].Position.Y = screenRect.Top;
                verts[vertIndexCounter].TexCoords.X = textCoords.Width;
                verts[vertIndexCounter].TexCoords.Y = textCoords.Top;
                verts[vertIndexCounter].Color = color;
                vertIndexCounter++;

                verts[vertIndexCounter].Position.X = screenRect.Width;
                verts[vertIndexCounter].Position.Y = screenRect.Height;
                verts[vertIndexCounter].TexCoords.X = textCoords.Width;
                verts[vertIndexCounter].TexCoords.Y = textCoords.Height;
                verts[vertIndexCounter].Color = color;
                vertIndexCounter++;

                verts[vertIndexCounter].Position.X = screenRect.Left;
                verts[vertIndexCounter].Position.Y = screenRect.Height;
                verts[vertIndexCounter].TexCoords.X = textCoords.Left;
                verts[vertIndexCounter].TexCoords.Y = textCoords.Height;
                verts[vertIndexCounter].Color = color;
                vertIndexCounter++;
            }
        }

        public unsafe void DrawCell(Cell cell, Rectangle screenRect, Color defaultBackground, SadConsole.Font font)
        {
            if (cell.IsVisible)
            {
                var glyphRect = font.GlyphIndexRects[cell.ActualGlyphIndex];

                if ((cell.ActualSpriteEffect & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
                {
                    var temp = glyphRect.Left;
                    glyphRect.Left = glyphRect.Width;
                    glyphRect.Width = temp;
                }

                if ((cell.ActualSpriteEffect & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically)
                {
                    var temp = glyphRect.Top;
                    glyphRect.Top = glyphRect.Height;
                    glyphRect.Height = temp;
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
            state.Transform *= transform;
            state.Texture = texture;
            target.Draw(m_verticies, 0, (uint)vertIndexCounter, PrimitiveType.Quads, state);
        }
    }
}
#endif
