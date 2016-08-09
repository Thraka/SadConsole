#if SFML
using Rectangle = SFML.Graphics.IntRect;
using Matrix = SFML.Graphics.Transform;
using SadConsole;
using SadConsole.Consoles;
using System.Collections.Generic;

namespace SFML.Graphics
{
    public class SpriteBatch
    {
        //private Vertex[] singleDrawVerticies = new Vertex[4];
        private Matrix transform;

        private DrawCall lastDrawCall = new DrawCall();

        private List<DrawCall> drawCalls;

        private RenderTarget target;

        private RenderStates state;

        private int maxIndex = 800;

        public void Reset(RenderTarget target, RenderStates state, Matrix renderingTransform)
        {
            transform = renderingTransform;
            //drawCalls = new List<DrawCall>();
            //lastDrawCall = new DrawCall();
            lastDrawCall.VertIndex = 0;
            this.target = target;
            this.state = state;
            this.state.Transform *= renderingTransform;
        }

        private class DrawCall
        {
            public Texture Texture;
            public Vertex[] Verticies = new Vertex[1000];
            public int VertIndex;
        }

        public unsafe void DrawQuad(Rectangle screenRect, Rectangle textCoords, Color color, Texture texture)
        {
            if (lastDrawCall.Texture != texture && lastDrawCall.Texture != null)
            {
                //drawCalls.Add(lastDrawCall);
                End();
                lastDrawCall.VertIndex = 0;
            }

            lastDrawCall.Texture = texture;

            if (lastDrawCall.VertIndex >= maxIndex)
            {
                global::System.Array.Resize(ref lastDrawCall.Verticies, lastDrawCall.Verticies.Length + lastDrawCall.Verticies.Length / 2);
                maxIndex = lastDrawCall.Verticies.Length - 200;
            }

            fixed (Vertex* verts = lastDrawCall.Verticies)
            {
                verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                verts[lastDrawCall.VertIndex].TexCoords.X = textCoords.Left;
                verts[lastDrawCall.VertIndex].TexCoords.Y = textCoords.Top;
                verts[lastDrawCall.VertIndex].Color = color;
                lastDrawCall.VertIndex++;

                verts[lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                verts[lastDrawCall.VertIndex].TexCoords.X = textCoords.Width;
                verts[lastDrawCall.VertIndex].TexCoords.Y = textCoords.Top;
                verts[lastDrawCall.VertIndex].Color = color;
                lastDrawCall.VertIndex++;

                verts[lastDrawCall.VertIndex].Position.X = screenRect.Width;
                verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                verts[lastDrawCall.VertIndex].TexCoords.X = textCoords.Width;
                verts[lastDrawCall.VertIndex].TexCoords.Y = textCoords.Height;
                verts[lastDrawCall.VertIndex].Color = color;
                lastDrawCall.VertIndex++;

                verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                verts[lastDrawCall.VertIndex].TexCoords.X = textCoords.Left;
                verts[lastDrawCall.VertIndex].TexCoords.Y = textCoords.Height;
                verts[lastDrawCall.VertIndex].Color = color;
                lastDrawCall.VertIndex++;
            }

            //lastDrawCall.Verticies.AddRange(singleDrawVerticies);

        }

        public unsafe void DrawCell(Cell cell, Rectangle screenRect, Rectangle solidRect, Color defaultBackground, SadConsole.Font font)
        {
            if (lastDrawCall.Texture != font.FontImage && lastDrawCall.Texture != null)
            {
                End();
                lastDrawCall.VertIndex = 0;
            }

            lastDrawCall.Texture = font.FontImage;

            if (lastDrawCall.VertIndex >= maxIndex)
            {
                global::System.Array.Resize(ref lastDrawCall.Verticies, lastDrawCall.Verticies.Length + lastDrawCall.Verticies.Length / 2);
                maxIndex = lastDrawCall.Verticies.Length - 200;
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

            fixed (Vertex* verts = lastDrawCall.Verticies)
            {
                if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != defaultBackground)
                {
                    // Background
                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Top;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualBackground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Top;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualBackground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Height;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualBackground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Height;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualBackground;
                    lastDrawCall.VertIndex++;

                    //lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }

                if (cell.ActualForeground != Color.Transparent)
                {
                    // Foreground
                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualForeground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualForeground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualForeground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                    verts[lastDrawCall.VertIndex].Color = cell.ActualForeground;
                    lastDrawCall.VertIndex++;

                    //lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }
            }
        }

        public void End()
        {
            if (lastDrawCall.VertIndex != 0)
            {
                state.Texture = lastDrawCall.Texture;
                target.Draw(lastDrawCall.Verticies, 0, (uint)(lastDrawCall.VertIndex), PrimitiveType.Quads, state);
            }
        }
    }

    public class SpriteBatch2
    {
        Vertex[] m_verticies;
        int vertIndexCounter;
        Texture texture;
        Matrix transform;

        public SpriteBatch2()
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
