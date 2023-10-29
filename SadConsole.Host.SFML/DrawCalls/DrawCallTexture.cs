using SFML.Graphics;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws an image ot the active <see cref="Host.Global.SharedSpriteBatch"/>.
/// </summary>
public class DrawCallTexture : IDrawCall
{
    /// <summary>
    /// The image to draw.
    /// </summary>
    public Texture Texture;

    /// <summary>
    /// Where on the <see cref="Host.Global.SharedSpriteBatch"/> to draw the texture.
    /// </summary>
    public SFML.System.Vector2i Position;

    /// <summary>
    /// A color tint to apply when drawn.
    /// </summary>
    public Color Tint;

    /// <summary>
    /// Creates a new instance of this draw call.
    /// </summary>
    /// <param name="texture">The image to draw.</param>
    /// <param name="position">The position on the <see cref="Host.Global.SharedSpriteBatch"/> to draw the image.</param>
    /// <param name="tint">A color tint to apply to the drawn image.</param>
    public DrawCallTexture(Texture texture, SFML.System.Vector2i position, Color? tint = null)
    {
        Texture = texture;
        Position = position;

        if (tint.HasValue)
            Tint = tint.Value;
        else
            Tint = Color.White;
    }

    /// <inheritdoc/>
    public void Draw() =>
        Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(Position.X, Position.Y, (int)Texture.Size.X, (int)Texture.Size.Y),
                                               new IntRect(0, 0, (int)Texture.Size.X, (int)Texture.Size.Y),
                                               Tint,
                                               Texture);
}
