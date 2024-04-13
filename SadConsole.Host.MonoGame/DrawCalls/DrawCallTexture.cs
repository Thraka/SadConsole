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
    /// The shader to use when drawing the texture.
    /// </summary>
    public Effect ShaderEffect;

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
    /// <param name="effect">A shader to apply to the texture being drawn.</param>
    /// <exception cref="System.NullReferenceException">Thrown when <paramref name="texture"/> is null.</exception>
    public DrawCallTexture(Texture2D texture, Vector2 position, Color? tint = null, Effect effect = null)
    {
        Texture = texture ?? throw new System.NullReferenceException($"{nameof(texture)} cannot be null.");
        Position = position;

        if (tint.HasValue)
            Tint = tint.Value;
        else
            Tint = Color.White;

        ShaderEffect = effect;

    }

    /// <inheritdoc/>
    public void Draw()
    {
        if (ShaderEffect == null)
        {
            Host.Global.SharedSpriteBatch.Draw(Texture, Position, Tint);
        }
        else
        {
            DrawCallManager.InterruptBatch();

            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, Host.Settings.MonoGameScreenBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, ShaderEffect);
            Host.Global.SharedSpriteBatch.Draw(Texture, Position, Tint);
            Host.Global.SharedSpriteBatch.End();

            DrawCallManager.ResumeBatch(true);
        }
    }
}
