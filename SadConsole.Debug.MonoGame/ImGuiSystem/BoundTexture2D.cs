using System;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// Represents a texture coupled with its pointer as known by ImGUI.
/// </summary>
/// <remarks>
/// This object is meant to be managed by a single reference. You must unbind the texture manually once you're done with it.
///
/// The <see cref="Refresh(ImGuiRenderer, Texture2D)"/> method is used to automatically unbind the old texture and replace it with the one provided, when they don't match.
/// </remarks>
public sealed class BoundTexture2D
{
    /// <summary>
    /// The <see cref="Texture2D"/> bound to ImGUI.
    /// </summary>
    public Texture2D Texture { get; private set; }

    /// <summary>
    /// The current pointer to the texture as known by ImGUI.
    /// </summary>
    public IntPtr Pointer { get; private set; }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="renderer">The ImGUI render object.</param>
    /// <param name="texture">The texture you want to bind with ImGUI.</param>
    public BoundTexture2D(ImGuiRenderer renderer, Texture2D texture)
    {
        Bind(renderer, texture);
    }

    /// <summary>
    /// Checks to see that the passed in texture matches the one on this object. If it doesn't the old one is unbound and this one is bound.
    /// </summary>
    /// <param name="renderer">The ImGUI renderer.</param>
    /// <param name="texture">The texture.</param>
    public void Refresh(ImGuiRenderer renderer, Texture2D texture)
    {
        if (!renderer.HasBoundTexture(texture))
        {
            if (Texture != texture)
                Unbind(renderer);

            Bind(renderer, texture);
        }
    }

    private void Unbind(ImGuiRenderer renderer)
    {
        renderer.UnbindTexture(Texture);
        Texture = null;
        Pointer = IntPtr.Zero;
    }

    private void Bind(ImGuiRenderer renderer, Texture2D texture)
    {
        Pointer = renderer.BindTexture(texture);
        Texture = texture;
    }
}
