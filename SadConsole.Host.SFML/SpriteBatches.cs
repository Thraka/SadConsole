using Rectangle = SFML.Graphics.IntRect;
using Matrix = SFML.Graphics.Transform;
using SadConsole;
using System.Collections.Generic;
using SadConsole.Host;

namespace SFML.Graphics
{
    public class SpriteBatch
    {
        //private Vertex[] singleDrawVerticies = new Vertex[4];
        private Matrix transform;

        private BatchDrawCall lastDrawCall = new BatchDrawCall();

        private List<BatchDrawCall> drawCalls;

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

        private class BatchDrawCall
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

        public unsafe void DrawCell(ColoredGlyph cell, Rectangle screenRect, bool drawBackground, SadConsole.Font font)
        {
            Rectangle solidRect = font.SolidGlyphRectangle.ToSFML();

            if (lastDrawCall.Texture != ((SadConsole.Host.GameTexture)font.Image).Texture && lastDrawCall.Texture != null)
            {
                End();
                lastDrawCall.VertIndex = 0;
            }

            lastDrawCall.Texture = ((SadConsole.Host.GameTexture)font.Image).Texture;

            if (lastDrawCall.VertIndex >= maxIndex)
            {
                global::System.Array.Resize(ref lastDrawCall.Verticies, lastDrawCall.Verticies.Length + lastDrawCall.Verticies.Length / 2);
                maxIndex = lastDrawCall.Verticies.Length - 200;
            }

            var glyphRect = font.GlyphRects[cell.Glyph].ToSFML();
            var background = cell.Background.ToSFML();
            var foreground = cell.Foreground.ToSFML();

            if ((cell.Mirror & Mirror.Horizontal) == Mirror.Horizontal)
            {
                var temp = glyphRect.Left;
                glyphRect.Left = glyphRect.Width;
                glyphRect.Width = temp;
            }

            if ((cell.Mirror & Mirror.Vertical) == Mirror.Vertical)
            {
                var temp = glyphRect.Top;
                glyphRect.Top = glyphRect.Height;
                glyphRect.Height = temp;
            }

            fixed (Vertex* verts = lastDrawCall.Verticies)
            {
                if (background != Color.Transparent && drawBackground)
                {
                    // Background
                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Top;
                    verts[lastDrawCall.VertIndex].Color = background;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Top;
                    verts[lastDrawCall.VertIndex].Color = background;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Height;
                    verts[lastDrawCall.VertIndex].Color = background;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = solidRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = solidRect.Height;
                    verts[lastDrawCall.VertIndex].Color = background;
                    lastDrawCall.VertIndex++;

                    //lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }

                if (foreground != Color.Transparent)
                {
                    // Foreground
                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                    verts[lastDrawCall.VertIndex].Color = foreground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                    verts[lastDrawCall.VertIndex].Color = foreground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Width;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                    verts[lastDrawCall.VertIndex].Color = foreground;
                    lastDrawCall.VertIndex++;

                    verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                    verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                    verts[lastDrawCall.VertIndex].Color = foreground;
                    lastDrawCall.VertIndex++;

                    //lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }

                if (cell.Decorators.Length != 0)
                {
                    foreach (var decorator in cell.Decorators)
                    {
                        glyphRect = font.GlyphRects[decorator.Glyph].ToSFML();
                        foreground = decorator.Color.ToSFML();

                        if ((cell.Mirror & Mirror.Horizontal) == Mirror.Horizontal)
                        {
                            var temp = glyphRect.Left;
                            glyphRect.Left = glyphRect.Width;
                            glyphRect.Width = temp;
                        }

                        if ((cell.Mirror & Mirror.Vertical) == Mirror.Vertical)
                        {
                            var temp = glyphRect.Top;
                            glyphRect.Top = glyphRect.Height;
                            glyphRect.Height = temp;
                        }

                        if (foreground != Color.Transparent)
                        {
                            // Foreground
                            verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                            verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                            verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                            verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                            verts[lastDrawCall.VertIndex].Color = foreground;
                            lastDrawCall.VertIndex++;

                            verts[lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                            verts[lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                            verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                            verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                            verts[lastDrawCall.VertIndex].Color = foreground;
                            lastDrawCall.VertIndex++;

                            verts[lastDrawCall.VertIndex].Position.X = screenRect.Width;
                            verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                            verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                            verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                            verts[lastDrawCall.VertIndex].Color = foreground;
                            lastDrawCall.VertIndex++;

                            verts[lastDrawCall.VertIndex].Position.X = screenRect.Left;
                            verts[lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                            verts[lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                            verts[lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                            verts[lastDrawCall.VertIndex].Color = foreground;
                            lastDrawCall.VertIndex++;
                        }
                    }
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

        public unsafe void DrawCell(ColoredGlyph cell, Rectangle screenRect, Color defaultBackground, SadConsole.Font font)
        {
            if (cell.IsVisible)
            {
                Rectangle solidRect = font.SolidGlyphRectangle.ToSFML();

                var glyphRect = font.GlyphRects[cell.Glyph].ToSFML();
                var background = cell.Background.ToSFML();
                var foreground = cell.Foreground.ToSFML();

                if ((cell.Mirror & Mirror.Horizontal) == Mirror.Horizontal)
                {
                    var temp = glyphRect.Left;
                    glyphRect.Left = glyphRect.Width;
                    glyphRect.Width = temp;
                }

                if ((cell.Mirror & Mirror.Vertical) == Mirror.Vertical)
                {
                    var temp = glyphRect.Top;
                    glyphRect.Top = glyphRect.Height;
                    glyphRect.Height = temp;
                }

                fixed (Vertex* verts = m_verticies)
                {
                    if (background != Color.Transparent && background != defaultBackground)
                    {
                        // Background
                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Top;
                        verts[vertIndexCounter].Color = background;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Top;
                        verts[vertIndexCounter].Color = background;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Height;
                        verts[vertIndexCounter].Color = background;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = solidRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = solidRect.Height;
                        verts[vertIndexCounter].Color = background;
                        vertIndexCounter++;
                    }

                    if (foreground != Color.Transparent)
                    {
                        // Foreground
                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Top;
                        verts[vertIndexCounter].Color = foreground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                        verts[vertIndexCounter].Position.Y = screenRect.Top;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Top;
                        verts[vertIndexCounter].Color = foreground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Width;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Width;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Height;
                        verts[vertIndexCounter].Color = foreground;
                        vertIndexCounter++;

                        verts[vertIndexCounter].Position.X = screenRect.Left;
                        verts[vertIndexCounter].Position.Y = screenRect.Height;
                        verts[vertIndexCounter].TexCoords.X = glyphRect.Left;
                        verts[vertIndexCounter].TexCoords.Y = glyphRect.Height;
                        verts[vertIndexCounter].Color = foreground;
                        vertIndexCounter++;


                    }
                }


            }

            this.texture = ((SadConsole.Host.GameTexture)font.Image).Texture;
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
        Vertex[] _verticies;
        int _vertIndexCounter;
        Texture _texture;
        Rectangle _solidRect;
        Rectangle _fillRect;
        Matrix _transform;

        public SpriteBatch_ORG()
        {
            _verticies = new Vertex[0];
        }

        public void Start(int renderQuads, Texture texture, Matrix transform)
        {
            int count = 4 * renderQuads;

            if (_verticies.Length != count)
                _verticies = new Vertex[count];

            _vertIndexCounter = 0;
            this._texture = texture;
            this._transform = transform;
        }

        public void Start(ScreenObjectSurface surface, SadConsole.Font font, Matrix transform, int additionalDraws = 250)
        {
            _fillRect = surface.AbsoluteArea.ToSFML();
            _texture = ((SadConsole.Host.GameTexture)font.Image).Texture;
            _solidRect = font.GlyphRects[font.SolidGlyphIndex].ToSFML();
            this._transform = transform;

            int count = (((surface.Surface.ViewWidth * surface.Surface.ViewHeight) + 8 + additionalDraws) * 4 * 2);
            if (_verticies.Length != count)
                _verticies = new Vertex[count];

            _vertIndexCounter = 0;
        }

        public unsafe void DrawSurfaceFill(Color color, Color filter)
        {
            if (color != filter)
            {
                fixed (Vertex* verts = _verticies)
                {
                    verts[_vertIndexCounter].Position.X = _fillRect.Left;
                    verts[_vertIndexCounter].Position.Y = _fillRect.Top;
                    verts[_vertIndexCounter].TexCoords.X = _solidRect.Left;
                    verts[_vertIndexCounter].TexCoords.Y = _solidRect.Top;
                    verts[_vertIndexCounter].Color = color;
                    _vertIndexCounter++;

                    verts[_vertIndexCounter].Position.X = _fillRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[_vertIndexCounter].Position.Y = _fillRect.Top;
                    verts[_vertIndexCounter].TexCoords.X = _solidRect.Width;
                    verts[_vertIndexCounter].TexCoords.Y = _solidRect.Top;
                    verts[_vertIndexCounter].Color = color;
                    _vertIndexCounter++;

                    verts[_vertIndexCounter].Position.X = _fillRect.Width;
                    verts[_vertIndexCounter].Position.Y = _fillRect.Height;
                    verts[_vertIndexCounter].TexCoords.X = _solidRect.Width;
                    verts[_vertIndexCounter].TexCoords.Y = _solidRect.Height;
                    verts[_vertIndexCounter].Color = color;
                    _vertIndexCounter++;

                    verts[_vertIndexCounter].Position.X = _fillRect.Left;
                    verts[_vertIndexCounter].Position.Y = _fillRect.Height;
                    verts[_vertIndexCounter].TexCoords.X = _solidRect.Left;
                    verts[_vertIndexCounter].TexCoords.Y = _solidRect.Height;
                    verts[_vertIndexCounter].Color = color;
                    _vertIndexCounter++;
                }
            }
        }

        public unsafe void DrawQuad(Rectangle screenRect, Rectangle textCoords, Color color)
        {
            fixed (Vertex* verts = _verticies)
            {
                verts[_vertIndexCounter].Position.X = screenRect.Left;
                verts[_vertIndexCounter].Position.Y = screenRect.Top;
                verts[_vertIndexCounter].TexCoords.X = textCoords.Left;
                verts[_vertIndexCounter].TexCoords.Y = textCoords.Top;
                verts[_vertIndexCounter].Color = color;
                _vertIndexCounter++;

                verts[_vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                verts[_vertIndexCounter].Position.Y = screenRect.Top;
                verts[_vertIndexCounter].TexCoords.X = textCoords.Width;
                verts[_vertIndexCounter].TexCoords.Y = textCoords.Top;
                verts[_vertIndexCounter].Color = color;
                _vertIndexCounter++;

                verts[_vertIndexCounter].Position.X = screenRect.Width;
                verts[_vertIndexCounter].Position.Y = screenRect.Height;
                verts[_vertIndexCounter].TexCoords.X = textCoords.Width;
                verts[_vertIndexCounter].TexCoords.Y = textCoords.Height;
                verts[_vertIndexCounter].Color = color;
                _vertIndexCounter++;

                verts[_vertIndexCounter].Position.X = screenRect.Left;
                verts[_vertIndexCounter].Position.Y = screenRect.Height;
                verts[_vertIndexCounter].TexCoords.X = textCoords.Left;
                verts[_vertIndexCounter].TexCoords.Y = textCoords.Height;
                verts[_vertIndexCounter].Color = color;
                _vertIndexCounter++;
            }
        }

        public unsafe void DrawCell(ColoredGlyph cell, Rectangle screenRect, Color defaultBackground, SadConsole.Font font)
        {
            if (cell.IsVisible)
            {
                var glyphRect = font.GlyphRects[cell.Glyph].ToSFML();
                var background = cell.Background.ToSFML();
                var foreground = cell.Foreground.ToSFML();

                if ((cell.Mirror & Mirror.Horizontal) == Mirror.Horizontal)
                {
                    var temp = glyphRect.Left;
                    glyphRect.Left = glyphRect.Width;
                    glyphRect.Width = temp;
                }

                if ((cell.Mirror & Mirror.Vertical) == Mirror.Vertical)
                {
                    var temp = glyphRect.Top;
                    glyphRect.Top = glyphRect.Height;
                    glyphRect.Height = temp;
                }

                fixed (Vertex* verts = _verticies)
                {
                    if (background != Color.Transparent && background != defaultBackground)
                    {
                        // Background
                        verts[_vertIndexCounter].Position.X = screenRect.Left;
                        verts[_vertIndexCounter].Position.Y = screenRect.Top;
                        verts[_vertIndexCounter].TexCoords.X = _solidRect.Left;
                        verts[_vertIndexCounter].TexCoords.Y = _solidRect.Top;
                        verts[_vertIndexCounter].Color = background;
                        _vertIndexCounter++;

                        verts[_vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                        verts[_vertIndexCounter].Position.Y = screenRect.Top;
                        verts[_vertIndexCounter].TexCoords.X = _solidRect.Width;
                        verts[_vertIndexCounter].TexCoords.Y = _solidRect.Top;
                        verts[_vertIndexCounter].Color = background;
                        _vertIndexCounter++;

                        verts[_vertIndexCounter].Position.X = screenRect.Width;
                        verts[_vertIndexCounter].Position.Y = screenRect.Height;
                        verts[_vertIndexCounter].TexCoords.X = _solidRect.Width;
                        verts[_vertIndexCounter].TexCoords.Y = _solidRect.Height;
                        verts[_vertIndexCounter].Color = background;
                        _vertIndexCounter++;

                        verts[_vertIndexCounter].Position.X = screenRect.Left;
                        verts[_vertIndexCounter].Position.Y = screenRect.Height;
                        verts[_vertIndexCounter].TexCoords.X = _solidRect.Left;
                        verts[_vertIndexCounter].TexCoords.Y = _solidRect.Height;
                        verts[_vertIndexCounter].Color = background;
                        _vertIndexCounter++;
                    }

                    if (foreground != Color.Transparent)
                    {
                        // Foreground
                        verts[_vertIndexCounter].Position.X = screenRect.Left;
                        verts[_vertIndexCounter].Position.Y = screenRect.Top;
                        verts[_vertIndexCounter].TexCoords.X = glyphRect.Left;
                        verts[_vertIndexCounter].TexCoords.Y = glyphRect.Top;
                        verts[_vertIndexCounter].Color = foreground;
                        _vertIndexCounter++;

                        verts[_vertIndexCounter].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                        verts[_vertIndexCounter].Position.Y = screenRect.Top;
                        verts[_vertIndexCounter].TexCoords.X = glyphRect.Width;
                        verts[_vertIndexCounter].TexCoords.Y = glyphRect.Top;
                        verts[_vertIndexCounter].Color = foreground;
                        _vertIndexCounter++;

                        verts[_vertIndexCounter].Position.X = screenRect.Width;
                        verts[_vertIndexCounter].Position.Y = screenRect.Height;
                        verts[_vertIndexCounter].TexCoords.X = glyphRect.Width;
                        verts[_vertIndexCounter].TexCoords.Y = glyphRect.Height;
                        verts[_vertIndexCounter].Color = foreground;
                        _vertIndexCounter++;

                        verts[_vertIndexCounter].Position.X = screenRect.Left;
                        verts[_vertIndexCounter].Position.Y = screenRect.Height;
                        verts[_vertIndexCounter].TexCoords.X = glyphRect.Left;
                        verts[_vertIndexCounter].TexCoords.Y = glyphRect.Height;
                        verts[_vertIndexCounter].Color = foreground;
                        _vertIndexCounter++;

                        
                    }
                }


            }
        }

        public void End(RenderTarget target, RenderStates state)
        {
            state.Transform *= _transform;
            state.Texture = _texture;
            target.Draw(_verticies, 0, (uint)_vertIndexCounter, PrimitiveType.Quads, state);
        }
    }
}
