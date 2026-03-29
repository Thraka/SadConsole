using System;
using System.Collections.Generic;
using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem.Rendering;

/// <summary>
/// ImGui renderer
/// </summary>
public abstract class ImGuiRenderer<TTexture>
    where TTexture : notnull, IDisposable
{
    protected Dictionary<ImTextureID, TextureInfo> _loadedTexturesByPointer = [];
    protected Dictionary<TTexture, TextureInfo> _loadedTexturesByTexture = [];

    private int _textureIdCounter = 1;
    protected ImTextureID? _fontTextureId;

    /// <summary>
    /// When <see langword="true"/>, indicates that the ImGui input system is going to use the mouse, for example when the mouse is over a UI element; otherwise <see langword="false"/>.
    /// </summary>
    public bool WantsMouseCapture { get; protected set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that the ImGui input system is going to use the keyboard, for example when focus is on a textbox; otherwise <see langword="false"/>.
    /// </summary>
    public bool WantsKeyboardCapture { get; protected set; }

    protected abstract TextureInfo CreateTexture(ImTextureDataPtr data);

    protected abstract void RefreshTexture(ImTextureDataPtr data);

    /// <summary>
    /// A list of objects to draw in ImGui.
    /// </summary>
    public List<ImGuiObjectBase> UIObjects = [];

    /// <summary>
    /// Updates the texture resource based on the current status of the specified texture data.
    /// </summary>
    /// <remarks>Call this method to synchronize the texture resource with the state indicated by the texture
    /// data. Ensure that the texture data is valid and properly initialized before invoking this method.</remarks>
    /// <param name="textureData">A pointer to the texture data that determines the operation to perform. The status of this data specifies
    /// whether to create, update, or destroy the associated texture.</param>
    public virtual void UpdateTexture(ImTextureDataPtr textureData)
    {
        switch (textureData.Status)
        {
            case ImTextureStatus.WantCreate:
                TextureInfo info = CreateTexture(textureData);
                _loadedTexturesByPointer[info.TextureID] = info;
                _loadedTexturesByTexture[info.Texture] = info;
                break;

            case ImTextureStatus.WantUpdates:
                RefreshTexture(textureData);
                break;

            case ImTextureStatus.WantDestroy:
                DestroyTexture(textureData);
                break;

            case ImTextureStatus.Ok:
                // Nothing to do
                break;
        }
    }


    private void DestroyTexture(ImTextureDataPtr textureData)
    {
        IntPtr texId = textureData.GetTexID();
        if (_loadedTexturesByPointer.TryGetValue(texId, out TextureInfo? textureInfo))
        {
            if (textureInfo.IsManaged)
                textureInfo.Texture?.Dispose();

            _loadedTexturesByPointer.Remove(texId);
            _loadedTexturesByTexture.Remove(textureInfo.Texture!);
        }
    }


    /// <summary>
    /// Creates a pointer to a texture, which can be passed through ImGui calls. That pointer is then used by ImGui to let us know what texture to draw.
    /// </summary>
    public virtual unsafe ImTextureRef BindTexture(TTexture texture)
    {
        // Return if already bound
        if (_loadedTexturesByTexture.TryGetValue(texture, out TextureInfo? textureInfo))
            return new ImTextureRef(null, textureInfo.TextureID);

        // Create if not bound
        IntPtr texId = new(_textureIdCounter++);

        textureInfo = new(texture, texId, false);

        _loadedTexturesByPointer[texId] = textureInfo;
        _loadedTexturesByTexture[texture] = textureInfo;

        return new ImTextureRef(null, texId);
    }

    /// <summary>
    /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated.
    /// </summary>
    public void UnbindTexture(ImTextureID textureId)
    {
        if (_loadedTexturesByPointer.TryGetValue(textureId, out var textureInfo))
        {
            if (textureInfo.IsManaged)
                textureInfo.Texture.Dispose();

            _loadedTexturesByTexture.Remove(textureInfo.Texture);
            _loadedTexturesByPointer.Remove(textureId);
        }
    }

    /// <summary>
    /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated.
    /// </summary>
    public void UnbindTexture(TTexture texture)
    {
        if (_loadedTexturesByTexture.ContainsKey(texture))
        {
            var pointer = _loadedTexturesByTexture[texture];
            _loadedTexturesByPointer.Remove(pointer.TextureID);
            _loadedTexturesByTexture.Remove(texture);
        }
    }

    /// <summary>
    /// Returns whether a texture is already bound.
    /// </summary>
    /// <param name="texture">The SFML texture to check.</param>
    /// <returns><see langword="true"/> if the texture is bound; otherwise <see langword="false"/>.</returns>
    public bool HasBoundTexture(TTexture texture) =>
        _loadedTexturesByTexture.ContainsKey(texture);

    /// <summary>
    /// Gets the ImGui texture ID for a bound SFML texture.
    /// </summary>
    /// <param name="texture">The SFML texture.</param>
    /// <returns>The ImGui texture ID.</returns>
    public ImTextureID GetBoundTexturePointer(TTexture texture)
    {
        if (!_loadedTexturesByTexture.ContainsKey(texture)) throw new System.Exception("Texture isn't bound");

        return _loadedTexturesByTexture[texture].TextureID;
    }

    /// <summary>
    /// Replaces an already-bound texture with a new one, keeping the same ImGui texture ID.
    /// </summary>
    /// <param name="textureID">The ImGui texture ID to update.</param>
    /// <param name="texture">The new SFML texture.</param>
    public void ReplaceBoundTexture(ImTextureID textureID, TTexture texture)
    {
        if (_loadedTexturesByPointer.TryGetValue(textureID, out TextureInfo? oldTextureData))
        {
            _loadedTexturesByTexture.Remove(oldTextureData.Texture);

            if (oldTextureData.IsManaged)
                oldTextureData.Texture.Dispose();

            oldTextureData = new(texture, textureID, oldTextureData.IsManaged);

            _loadedTexturesByTexture[texture] = oldTextureData;
            _loadedTexturesByPointer[textureID] = oldTextureData;
        }
    }

    /// <summary>
    /// Runs the ImGui input. Call before <see cref="BeforeLayout"/>.
    /// </summary>
    /// <param name="deltaSeconds">Time elapsed since last frame in seconds.</param>
    public void BeforeLayoutInput(float deltaSeconds)
    {
        ImGui.GetIO().DeltaTime = deltaSeconds;

        UpdateInput();
    }

    /// <summary>
    /// Sets up ImGui for a new frame, should be called at frame start.
    /// </summary>
    public virtual void BeforeLayout()
    {
        ImGui.NewFrame();
    }

    /// <summary>
    /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls.
    /// </summary>
    public virtual void AfterLayout()
    {
        ImGui.Render();

        unsafe
        {
            ImDrawDataPtr drawData = ImGui.GetDrawData();
            ProcessTextureUpdates(drawData);
            RenderDrawData(drawData);
        }
    }

    private unsafe void ProcessTextureUpdates(ImDrawDataPtr drawData)
    {
        if (drawData.Textures.Data == null) return;

        for (int i = 0; i < drawData.Textures.Size; i++)
        {
            ImTextureDataPtr textureData = drawData.Textures.Data[i];
            UpdateTexture(textureData);
        }
    }

    /// <summary>
    /// Sends SFML input state to ImGui.
    /// </summary>
    protected abstract void UpdateInput();

    /// <summary>
    /// Gets the geometry as set up by ImGui and sends it to the graphics device.
    /// </summary>
    protected unsafe abstract void RenderDrawData(ImDrawData* drawData);

    /// <summary>
    /// Adds a TTF font file to ImGui.
    /// </summary>
    /// <param name="file">Path to the TTF font file.</param>
    /// <param name="size">The font size in pixels.</param>
    /// <returns>The loaded font pointer.</returns>
    public unsafe ImFontPtr AddFontTTF(string file, float size) =>
        ImGui.GetIO().Fonts.AddFontFromFileTTF(file, size);

    /// <summary>
    /// Sets the default font for ImGui and rebuilds the font atlas.
    /// </summary>
    /// <param name="value">The font to set as default.</param>
    public unsafe void SetDefaultFont(ImFontPtr value) =>
        ImGui.GetIO().Handle->FontDefault = value;

    /// <summary>
    /// Represents information about a texture, including its associated texture object and management status.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the TextureInfo class with the specified texture, texture identifier, and management status.
    /// </remarks>
    /// <param name="texture">The texture object to associate with this instance.</param>
    /// <param name="textureID">The unique identifier used to reference the texture for rendering operations.</param>
    /// <param name="isManaged">A value indicating whether the texture is managed by the application. Set to <see langword="true"/> if the
    /// texture should be automatically disposed when destroyed by ImGui; otherwise, <see langword="false"/>.</param>
    public sealed class TextureInfo(TTexture texture, ImTextureID textureID, bool isManaged)
    {
        /// <summary>
        /// Gets or sets the texture associated with the object.
        /// </summary>
        public TTexture Texture { get; set; } = texture;

        /// <summary>
        /// Gets or sets the ImGui texture identifier associated with this texture.
        /// </summary>
        public ImTextureID TextureID { get; set; } = textureID;

        /// <summary>
        /// Indicates this texture instance is managed by ImGui.
        /// </summary>
        public bool IsManaged { get; set; } = isManaged;
    }
}
