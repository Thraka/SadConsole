using Rectangle = SFML.Graphics.IntRect;
using Matrix = SFML.Graphics.Transform;
using SadConsole;
using SadRogue.Primitives;

namespace SFML.Graphics;

/// <summary>
/// A sprite batching class.
/// </summary>
public class SpriteBatch
{
    private Matrix _transform;
    private BatchDrawCall _lastDrawCall = new BatchDrawCall();
    private RenderTarget _target;
    private RenderStates _state;

    private int _maxIndex = 800;

    /// <summary>
    /// Resets the batcher.
    /// </summary>
    /// <param name="target">The new rendering target.</param>
    /// <param name="blend">The blending mode.</param>
    /// <param name="renderingTransform">The transform.</param>
    /// <param name="shader">An optional shader.</param>
    public void Reset(RenderTarget target, BlendMode blend, Matrix renderingTransform, Shader shader = null)
    {
        _transform = renderingTransform;
        _lastDrawCall.VertIndex = 0;
        _target = target;
        _state = RenderStates.Default;
        _state.BlendMode = blend;
        _state.Transform *= renderingTransform;

        if (shader != null)
            _state.Shader = shader;
    }

    private class BatchDrawCall
    {
        public Texture Texture;
        public Vertex[] Verticies = new Vertex[1000];
        public int VertIndex;
    }

    /// <summary>
    /// Draws a textured quad to the render target.
    /// </summary>
    /// <param name="screenRect">The rectangle on the render target to draw the quad.</param>
    /// <param name="textCoords">The texture coordinates used with <paramref name="texture"/>.</param>
    /// <param name="color">A color to shade the quad. Use <see cref="Color.White"/> to disable shading.</param>
    /// <param name="texture">The texture to draw with.</param>
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

        // Change rects to correct rendering size
        screenRect.Width += screenRect.Left;
        screenRect.Height += screenRect.Top;
        textCoords.Width += textCoords.Left;
        textCoords.Height += textCoords.Top;

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

    /// <summary>
    /// Draws a single glyph to the screen.
    /// </summary>
    /// <param name="cell">The glyph information.</param>
    /// <param name="screenRect">The rectangle on the render target to draw the glyph.</param>
    /// <param name="drawBackground">A boolean value to indicate that the background of the glyph should be drawn.</param>
    /// <param name="font">The SadConsole font containing the glyph texture.</param>
    public unsafe void DrawCell(ColoredGlyphBase cell, Rectangle screenRect, bool drawBackground, IFont font)
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

        Rectangle glyphRect = font.GetGlyphSourceRectangle(cell.Glyph).ToIntRect();
        Color background = cell.Background.ToSFMLColor();
        Color foreground = cell.Foreground.ToSFMLColor();

        // Change rects to correct rendering size
        screenRect.Width += screenRect.Left;
        screenRect.Height += screenRect.Top;
        glyphRect.Width += glyphRect.Left;
        glyphRect.Height += glyphRect.Top;
        solidRect.Width += solidRect.Left;
        solidRect.Height += solidRect.Top;

        if ((cell.Mirror & Mirror.Horizontal) == Mirror.Horizontal)
        {
            int temp = glyphRect.Left;
            glyphRect.Left = glyphRect.Width;
            glyphRect.Width = temp;
        }

        if ((cell.Mirror & Mirror.Vertical) == Mirror.Vertical)
        {
            int temp = glyphRect.Top;
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

            if (cell.Glyph != 0 && foreground != Color.Transparent && foreground != background)
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
                for (int d = 0; d < cell.Decorators.Length; d++)
                {
                    glyphRect = font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToIntRect();

                    // Change rects to correct rendering size;
                    glyphRect.Width += glyphRect.Left;
                    glyphRect.Height += glyphRect.Top;

                    foreground = cell.Decorators[d].Color.ToSFMLColor();

                    if ((cell.Mirror & Mirror.Horizontal) == Mirror.Horizontal)
                    {
                        int temp = glyphRect.Left;
                        glyphRect.Left = glyphRect.Width;
                        glyphRect.Width = temp;
                    }

                    if ((cell.Mirror & Mirror.Vertical) == Mirror.Vertical)
                    {
                        int temp = glyphRect.Top;
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

    /// <summary>
    /// Draws the background color of a cell to the render target.
    /// </summary>
    /// <param name="cell">The cell information containing the background color.</param>
    /// <param name="screenRect">The rectangle on the render target to draw the background color.</param>
    /// <param name="font">The SadConsole font containing the solid color glyph used in drawing backgrounds.</param>
    public unsafe void DrawCellBackground(ColoredGlyph cell, Rectangle screenRect, SadConsole.IFont font)
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

        Rectangle glyphRect = font.GetGlyphSourceRectangle(cell.Glyph).ToIntRect();
        Color background = cell.Background.ToSFMLColor();

        // Change rects to correct rendering size
        screenRect.Width += screenRect.Left;
        screenRect.Height += screenRect.Top;
        glyphRect.Width += glyphRect.Left;
        glyphRect.Height += glyphRect.Top;
        solidRect.Width += solidRect.Left;
        solidRect.Height += solidRect.Top;

        fixed (Vertex* verts = _lastDrawCall.Verticies)
        {
            if (background != Color.Transparent)
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
        }
    }

    /// <summary>
    /// Ends the sprite batch and draws the data to the screen.
    /// </summary>
    public void End()
    {
        if (_lastDrawCall.VertIndex != 0)
        {
            _state.Texture = _lastDrawCall.Texture;
            _target.Draw(_lastDrawCall.Verticies, 0, (uint)(_lastDrawCall.VertIndex), PrimitiveType.Quads, _state);
        }
    }
}
