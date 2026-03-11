using Hexa.NET.ImGui;
using SFML.Graphics;

namespace SadConsole.ImGuiSystem;

public partial class ImGuiRenderer
{
    /// <summary>
    /// When <see langword="true"/>, indicates that the ImGui input system is going to use the mouse, for example when the mouse is over a UI element; otherwise <see langword="false"/>.
    /// </summary>
    public bool WantsMouseCapture { get; private set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that the ImGui input system is going to use the keyboard, for example when focus is on a textbox; otherwise <see langword="false"/>.
    /// </summary>
    public bool WantsKeyboardCapture { get; private set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that the UI should be hidden and stopped. Once it's hidden and stopped, this property is set to <see langword="false"/>.
    /// </summary>
    public bool HideRequested { get; set; }

    /// <summary>
    /// Adds a TTF font file to ImGui.
    /// </summary>
    /// <param name="file">Path to the TTF font file.</param>
    /// <param name="size">The font size in pixels.</param>
    /// <returns>The loaded font pointer.</returns>
    public ImFontPtr AddFontTTF(string file, float size) =>
        ImGui.GetIO().Fonts.AddFontFromFileTTF(file, size);

    /// <summary>
    /// Sets the default font for ImGui and rebuilds the font atlas.
    /// </summary>
    /// <param name="value">The font to set as default.</param>
    public unsafe void SetDefaultFont(ImFontPtr value)
    {
        ImGui.GetIO().Handle->FontDefault = value;
        RebuildFontAtlas();
    }

    /// <summary>
    /// Returns whether a texture is already bound.
    /// </summary>
    /// <param name="texture">The SFML texture to check.</param>
    /// <returns><see langword="true"/> if the texture is bound; otherwise <see langword="false"/>.</returns>
    public bool HasBoundTexture(Texture texture) =>
        _loadedTexturesByTexture.ContainsKey(texture);

    /// <summary>
    /// Gets the ImGui texture ID for a bound SFML texture.
    /// </summary>
    /// <param name="texture">The SFML texture.</param>
    /// <returns>The ImGui texture ID.</returns>
    public ImTextureID GetBoundTexturePointer(Texture texture)
    {
        if (!_loadedTexturesByTexture.ContainsKey(texture)) throw new System.Exception("Texture isn't bound");

        return _loadedTexturesByTexture[texture];
    }

    /// <summary>
    /// Replaces an already-bound texture with a new one, keeping the same ImGui texture ID.
    /// </summary>
    /// <param name="textureID">The ImGui texture ID to update.</param>
    /// <param name="texture">The new SFML texture.</param>
    public void ReplaceBoundTexture(ImTextureID textureID, Texture texture)
    {
        if (_loadedTexturesByPointer.TryGetValue(textureID, out var oldTexture))
        {
            _loadedTexturesByTexture.Remove(oldTexture);
            _loadedTexturesByTexture[texture] = textureID;
            _loadedTexturesByPointer[textureID] = texture;
        }
    }
}
