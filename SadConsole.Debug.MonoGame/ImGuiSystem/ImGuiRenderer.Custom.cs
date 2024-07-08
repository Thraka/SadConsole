using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.ImGuiSystem;

public partial class ImGuiRenderer
{
    /// <summary>
    /// When <see langword="true"/>, indicates thats the ImGui input system is going to use the mouse, for example when the mouse is over a UI element; otherwise <see langword="false"/>.
    /// </summary>
    public bool WantsMouseCapture { get; private set; }

    /// <summary>
    /// When <see langword="true"/>, indicates thats the ImGui input system is going to use the mouse, for example when focus is on a textbox; otherwise <see langword="false"/>.
    /// </summary>
    public bool WantsKeyboardCapture { get; private set; }

    /// <summary>
    /// When <see langword="true"/>, indicates thats the UI should be hidden and stopped. Once it's hidden and stopped, this property is set to <see langword="false"/>.
    /// </summary>
    public bool HideRequested { get; set; }

    public ImFontPtr AddFontTTF(string file, float size) =>
        ImGui.GetIO().Fonts.AddFontFromFileTTF(file, size);

    public unsafe void SetDefaultFont(ImFontPtr value)
    {
        ImGui.GetIO().NativePtr->FontDefault = value;
        RebuildFontAtlas();
    }

    public bool HasBoundTexture(Texture2D texture) =>
        _loadedTexturesByTexture.ContainsKey(texture);

    public IntPtr GetBoundTexturePointer(Texture2D texture)
    {
        if (!_loadedTexturesByTexture.ContainsKey(texture)) throw new Exception("Texture isn't bound");

        return _loadedTexturesByTexture[texture];
    }
}
