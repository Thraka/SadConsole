using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws an image ot the active <see cref="Host.Global.SharedSpriteBatch"/>.
/// </summary>
public class DrawCallTexture : IDrawCall
{
    /// <summary>
    /// The image to draw.
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// Where on the <see cref="Host.Global.SharedSpriteBatch"/> to draw the texture.
    /// </summary>
    public Vector2 Position;

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
    /// <exception cref="System.NullReferenceException">Thrown when <paramref name="texture"/> is null.</exception>
    public DrawCallTexture(Texture2D texture, Vector2 position, Color? tint = null)
    {
        if (texture == null) throw new System.NullReferenceException($"{nameof(texture)} cannot be null.");

        Texture = texture;
        Position = position;

        if (tint.HasValue)
            Tint = tint.Value;
        else
            Tint = Color.White;

    }

    /// <inheritdoc/>
    public void Draw() =>
        Host.Global.SharedSpriteBatch.Draw(Texture, Position, Tint);
}
