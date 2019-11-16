using Rectangle = SFML.Graphics.IntRect;
using Matrix = SFML.Graphics.Transform;
using SadConsole;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SFML.Graphics
{
    public class SpriteBatch
    {
        private Matrix _transform;
        private BatchDrawCall _lastDrawCall = new BatchDrawCall();
        private RenderTarget _target;
        private RenderStates _state;

        private int _maxIndex = 800;

        public void Reset(RenderTarget target, RenderStates state, Matrix renderingTransform)
        {
            _transform = renderingTransform;
            _lastDrawCall.VertIndex = 0;
            _target = target;
            _state = state;
            _state.Transform *= renderingTransform;
        }

        private class BatchDrawCall
        {
            public Texture Texture;
            public Vertex[] Verticies = new Vertex[1000];
            public int VertIndex;
        }

        public unsafe void DrawQuad(Rectangle screenRect, Rectangle textCoords, Color color, Texture texture)
        {
            if (_lastDrawCall.Texture != texture && _lastDrawCall.Texture != null)
            {
                End();
                _lastDrawCall.VertIndex = 0;
            }

            _lastDrawCall.Texture = texture;

            if (_lastDrawCall.VertIndex >= _maxIndex)
            {
                global::System.Array.Resize(ref _lastDrawCall.Verticies, _lastDrawCall.Verticies.Length + _lastDrawCall.Verticies.Length / 2);
                _maxIndex = _lastDrawCall.Verticies.Length - 200;
            }

            fixed (Vertex* verts = _lastDrawCall.Verticies)
            {
                verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                verts[_lastDrawCall.VertIndex].TexCoords.X = textCoords.Left;
                verts[_lastDrawCall.VertIndex].TexCoords.Y = textCoords.Top;
                verts[_lastDrawCall.VertIndex].Color = color;
                _lastDrawCall.VertIndex++;

                verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width;
                verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                verts[_lastDrawCall.VertIndex].TexCoords.X = textCoords.Width;
                verts[_lastDrawCall.VertIndex].TexCoords.Y = textCoords.Top;
                verts[_lastDrawCall.VertIndex].Color = color;
                _lastDrawCall.VertIndex++;

                verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width;
                verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                verts[_lastDrawCall.VertIndex].TexCoords.X = textCoords.Width;
                verts[_lastDrawCall.VertIndex].TexCoords.Y = textCoords.Height;
                verts[_lastDrawCall.VertIndex].Color = color;
                _lastDrawCall.VertIndex++;

                verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                verts[_lastDrawCall.VertIndex].TexCoords.X = textCoords.Left;
                verts[_lastDrawCall.VertIndex].TexCoords.Y = textCoords.Height;
                verts[_lastDrawCall.VertIndex].Color = color;
                _lastDrawCall.VertIndex++;
            }
        }

        public unsafe void DrawCell(ColoredGlyph cell, Rectangle screenRect, bool drawBackground, SadConsole.Font font)
        {
            Rectangle solidRect = font.SolidGlyphRectangle.ToIntRect();

            if (_lastDrawCall.Texture != ((SadConsole.Host.GameTexture)font.Image).Texture && _lastDrawCall.Texture != null)
            {
                End();
                _lastDrawCall.VertIndex = 0;
            }

            _lastDrawCall.Texture = ((SadConsole.Host.GameTexture)font.Image).Texture;

            if (_lastDrawCall.VertIndex >= _maxIndex)
            {
                global::System.Array.Resize(ref _lastDrawCall.Verticies, _lastDrawCall.Verticies.Length + _lastDrawCall.Verticies.Length / 2);
                _maxIndex = _lastDrawCall.Verticies.Length - 200;
            }

            var glyphRect = font.GlyphRects[cell.Glyph].ToIntRect();
            var background = cell.Background.ToSFMLColor();
            var foreground = cell.Foreground.ToSFMLColor();

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

            fixed (Vertex* verts = _lastDrawCall.Verticies)
            {
                if (background != Color.Transparent && drawBackground)
                {
                    // Background
                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = solidRect.Left;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = solidRect.Top;
                    verts[_lastDrawCall.VertIndex].Color = background;
                    _lastDrawCall.VertIndex++;

                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = solidRect.Width;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = solidRect.Top;
                    verts[_lastDrawCall.VertIndex].Color = background;
                    _lastDrawCall.VertIndex++;

                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width;
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = solidRect.Width;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = solidRect.Height;
                    verts[_lastDrawCall.VertIndex].Color = background;
                    _lastDrawCall.VertIndex++;

                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = solidRect.Left;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = solidRect.Height;
                    verts[_lastDrawCall.VertIndex].Color = background;
                    _lastDrawCall.VertIndex++;

                    //lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }

                if (foreground != Color.Transparent)
                {
                    // Foreground
                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                    verts[_lastDrawCall.VertIndex].Color = foreground;
                    _lastDrawCall.VertIndex++;

                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                    verts[_lastDrawCall.VertIndex].Color = foreground;
                    _lastDrawCall.VertIndex++;

                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width;
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                    verts[_lastDrawCall.VertIndex].Color = foreground;
                    _lastDrawCall.VertIndex++;

                    verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                    verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                    verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                    verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                    verts[_lastDrawCall.VertIndex].Color = foreground;
                    _lastDrawCall.VertIndex++;

                    //lastDrawCall.Verticies.AddRange(singleDrawVerticies);
                }

                if (cell.Decorators.Length != 0)
                {
                    foreach (var decorator in cell.Decorators)
                    {
                        glyphRect = font.GlyphRects[decorator.Glyph].ToIntRect();
                        foreground = decorator.Color.ToSFMLColor();

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
                            verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                            verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                            verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                            verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                            verts[_lastDrawCall.VertIndex].Color = foreground;
                            _lastDrawCall.VertIndex++;

                            verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width; // SadConsole w/SFML changed Width to be left + width...
                            verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Top;
                            verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                            verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Top;
                            verts[_lastDrawCall.VertIndex].Color = foreground;
                            _lastDrawCall.VertIndex++;

                            verts[_lastDrawCall.VertIndex].Position.X = screenRect.Width;
                            verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                            verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Width;
                            verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                            verts[_lastDrawCall.VertIndex].Color = foreground;
                            _lastDrawCall.VertIndex++;

                            verts[_lastDrawCall.VertIndex].Position.X = screenRect.Left;
                            verts[_lastDrawCall.VertIndex].Position.Y = screenRect.Height;
                            verts[_lastDrawCall.VertIndex].TexCoords.X = glyphRect.Left;
                            verts[_lastDrawCall.VertIndex].TexCoords.Y = glyphRect.Height;
                            verts[_lastDrawCall.VertIndex].Color = foreground;
                            _lastDrawCall.VertIndex++;
                        }
                    }
                }
            }
        }

        public void End()
        {
            if (_lastDrawCall.VertIndex != 0)
            {
                _state.Texture = _lastDrawCall.Texture;
                _target.Draw(_lastDrawCall.Verticies, 0, (uint)(_lastDrawCall.VertIndex), PrimitiveType.Quads, _state);
            }
        }
    }
}
