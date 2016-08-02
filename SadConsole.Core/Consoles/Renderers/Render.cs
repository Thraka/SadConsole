#if SFML
using Rectangle = SFML.Graphics.IntRect;
using Matrix = SFML.Graphics.Transform;
using SadConsole;
using SadConsole.Consoles;
using System.Collections.Generic;

namespace SFML.Graphics
{
    public class SpriteBatch1
    {
        private Vertex[] singleDrawVerticies = new Vertex[4];
        private Matrix transform;

        private DrawCall lastDrawCall;

        private List<DrawCall> drawCalls;
        
        public SpriteBatch1() { Reset(Matrix.Identity); }

        public void Reset(Matrix renderingTransform)
        {
            transform = renderingTransform;
            drawCalls = new List<DrawCall>();
            lastDrawCall = null;
        }

        public void Reset()
        {
            Reset(Matrix.Identity);
        }

        private class DrawCall
        {
            public Texture Texture;
            public List<Vertex> Verticies = new List<Vertex>(8);
        }

        public unsafe void DrawQuad(Rectangle screenRect, Rectangle textCoords, Color color, Texture texture)
        {
            if (lastDrawCall == null)
            {
                lastDrawCall = new DrawCall() { Texture = texture };
            }
            else if (lastDrawCall.Texture != texture)
            {
                drawCalls.Add(lastDrawCall);
                lastDrawCall = new DrawCall() { Texture = texture };
            }

            fixed (Vertex* verts = singleDrawVerticies)
            {
                verts[0].Position.X = screenRect.Left;
                verts[0].Position.Y = screenRect.Top;
                verts[0].TexCoords.X = textCoords.Left;
                verts[0].TexCoords.Y = textCoords.Top;
                verts[0].Color = color;

                verts[1].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                verts[1].Position.Y = screenRect.Top;
                verts[1].TexCoords.X = textCoords.Width;
                verts[1].TexCoords.Y = textCoords.Top;
                verts[1].Color = color;

                verts[2].Position.X = screenRect.Width;
                verts[2].Position.Y = screenRect.Height;
                verts[2].TexCoords.X = textCoords.Width;
                verts[2].TexCoords.Y = textCoords.Height;
                verts[2].Color = color;

                verts[3].Position.X = screenRect.Left;
                verts[3].Position.Y = screenRect.Height;
                verts[3].TexCoords.X = textCoords.Left;
                verts[3].TexCoords.Y = textCoords.Height;
                verts[3].Color = color;
            }

            lastDrawCall.Verticies.AddRange(singleDrawVerticies);
        }

        public unsafe void DrawCell(Cell cell, Rectangle screenRect, Rectangle solidRect, Color defaultBackground, SadConsole.Font font)
        {
            if (lastDrawCall == null)
            {
                lastDrawCall = new DrawCall() { Texture = font.FontImage };
            }
            else if (lastDrawCall.Texture != font.FontImage)
            {
                drawCalls.Add(lastDrawCall);
                lastDrawCall = new DrawCall() { Texture = font.FontImage };
            }

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

            fixed (Vertex* verts = singleDrawVerticies)
            {
                if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != defaultBackground)
                {
                    // Background
                    verts[0].Position.X = screenRect.Left;
                    verts[0].Position.Y = screenRect.Top;
                    verts[0].TexCoords.X = solidRect.Left;
                    verts[0].TexCoords.Y = solidRect.Top;
                    verts[0].Color = cell.ActualBackground;

                    verts[1].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[1].Position.Y = screenRect.Top;
                    verts[1].TexCoords.X = solidRect.Width;
                    verts[1].TexCoords.Y = solidRect.Top;
                    verts[1].Color = cell.ActualBackground;

                    verts[2].Position.X = screenRect.Width;
                    verts[2].Position.Y = screenRect.Height;
                    verts[2].TexCoords.X = solidRect.Width;
                    verts[2].TexCoords.Y = solidRect.Height;
                    verts[2].Color = cell.ActualBackground;

                    verts[3].Position.X = screenRect.Left;
                    verts[3].Position.Y = screenRect.Height;
                    verts[3].TexCoords.X = solidRect.Left;
                    verts[3].TexCoords.Y = solidRect.Height;
                    verts[3].Color = cell.ActualBackground;

                    lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }

                if (cell.ActualForeground != Color.Transparent)
                {
                    // Foreground
                    verts[0].Position.X = screenRect.Left;
                    verts[0].Position.Y = screenRect.Top;
                    verts[0].TexCoords.X = glyphRect.Left;
                    verts[0].TexCoords.Y = glyphRect.Top;
                    verts[0].Color = cell.ActualForeground;

                    verts[1].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[1].Position.Y = screenRect.Top;
                    verts[1].TexCoords.X = glyphRect.Width;
                    verts[1].TexCoords.Y = glyphRect.Top;
                    verts[1].Color = cell.ActualForeground;

                    verts[2].Position.X = screenRect.Width;
                    verts[2].Position.Y = screenRect.Height;
                    verts[2].TexCoords.X = glyphRect.Width;
                    verts[2].TexCoords.Y = glyphRect.Height;
                    verts[2].Color = cell.ActualForeground;

                    verts[3].Position.X = screenRect.Left;
                    verts[3].Position.Y = screenRect.Height;
                    verts[3].TexCoords.X = glyphRect.Left;
                    verts[3].TexCoords.Y = glyphRect.Height;
                    verts[3].Color = cell.ActualForeground;

                    lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }
            }
        }

        public void End(RenderTarget target, RenderStates state)
        {
            drawCalls.Add(lastDrawCall);
            state.Transform *= transform;

            for (int i = 0; i < drawCalls.Count; i++)
            {
                state.Texture = drawCalls[i].Texture;
                target.Draw(drawCalls[i].Verticies.ToArray(), PrimitiveType.Quads, state);
            }
        }
    }

    public class SpriteBatch
    {
        Vertex[] m_verticies;
        int vertIndexCounter;
        Texture texture;
        Matrix transform;

        public SpriteBatch()
        {
            m_verticies = new Vertex[0];
        }

        public void Reset(Matrix renderingTransform)
        {
            transform = renderingTransform;
            m_verticies = new Vertex[10000];
            vertIndexCounter = 0;
        }

        public void Reset()
        {
            Reset(Matrix.Identity);
        }
        

        public unsafe void DrawQuad(Rectangle screenRect, Rectangle textCoords, Color color, Texture texture)
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

            this.texture = texture;
        }

        public unsafe void DrawCell(Cell cell, Rectangle screenRect, Rectangle solidRect, Color defaultBackground, SadConsole.Font font)
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

            this.texture = font.FontImage;
        }

        public void End(RenderTarget target, RenderStates state)
        {
            state.Transform *= transform;
            state.Texture = texture;
            target.Draw(m_verticies, 0, (uint)vertIndexCounter, PrimitiveType.Quads, state);
        }
    }

    public class SpriteBatch_ORG
    {
        Vertex[] m_verticies;
        int vertIndexCounter;
        Texture texture;
        Rectangle solidRect;
        Rectangle fillRect;
        Matrix transform;

        public SpriteBatch_ORG()
        {
            m_verticies = new Vertex[0];
        }

        public void Start(int renderQuads, Texture texture, Matrix transform)
        {
            int count = 4 * renderQuads;

            if (m_verticies.Length != count)
                m_verticies = new Vertex[count];

            vertIndexCounter = 0;
            this.texture = texture;
            this.transform = transform;
        }

        public void Start(ITextSurfaceRendered surface, Matrix transform, int additionalDraws = 250)
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
