using Hexa.NET.ImGui;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SFMLColor = SFML.Graphics.Color;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// ImGui renderer for use with SFML.
/// </summary>
public partial class ImGuiRenderer
{
    private RenderWindow _window;

    // Textures
    private Dictionary<ImTextureID, Texture> _loadedTexturesByPointer;
    private Dictionary<Texture, ImTextureID> _loadedTexturesByTexture;

    private int _textureId;
    private ImTextureID? _fontTextureId;

    // Input (scroll handled via event in SetupInput)

    internal ImGuiRenderer(RenderWindow window)
    {
        ImGuiContextPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        _window = window ?? throw new ArgumentNullException(nameof(window));

        _loadedTexturesByPointer = [];
        _loadedTexturesByTexture = [];

        SetupInput();
    }

    #region ImGuiRenderer

    /// <summary>
    /// Creates a texture and loads the font data from ImGui. Should be called when the graphics device is initialized but before any rendering is done.
    /// </summary>
    public virtual unsafe void RebuildFontAtlas()
    {
        // Get font texture from ImGui
        ImGuiIOPtr io = ImGui.GetIO();

        byte* pixelData = null;
        int width = 0;
        int height = 0;
        int bytesPerPixel = 0;

        io.Fonts.GetTexDataAsRGBA32(ref pixelData, ref width, ref height, ref bytesPerPixel);

        // Copy the data to a managed array
        byte[] pixels = new byte[width * height * bytesPerPixel];
        Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

        // Create SFML texture from the pixel data
        var texture = new Texture(new SFML.System.Vector2u((uint)width, (uint)height));
        texture.Update(pixels);

        // Should a texture already have been built previously, unbind it first so it can be deallocated
        if (_fontTextureId.HasValue) UnbindTexture(_fontTextureId.Value);

        // Bind the new texture to an ImGui-friendly id
        _fontTextureId = BindTexture(texture);

        // Let ImGui know where to find the texture
        io.Fonts.SetTexID(_fontTextureId.Value);
        io.Fonts.ClearTexData();
    }

    /// <summary>
    /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="ImGui.Image" />. That pointer is then used by ImGui to let us know what texture to draw.
    /// </summary>
    public virtual ImTextureID BindTexture(Texture texture)
    {
        if (_loadedTexturesByTexture.ContainsKey(texture))
            return _loadedTexturesByTexture[texture];

        var id = new ImTextureID(_textureId++);

        _loadedTexturesByPointer.Add(id, texture);
        _loadedTexturesByTexture.Add(texture, id);

        return id;
    }

    /// <summary>
    /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated.
    /// </summary>
    public virtual void UnbindTexture(ImTextureID textureId)
    {
        if (_loadedTexturesByPointer.ContainsKey(textureId))
        {
            var texture = _loadedTexturesByPointer[textureId];
            _loadedTexturesByPointer.Remove(textureId);
            _loadedTexturesByTexture.Remove(texture);
        }
    }

    /// <summary>
    /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated.
    /// </summary>
    public virtual void UnbindTexture(Texture texture)
    {
        if (_loadedTexturesByTexture.ContainsKey(texture))
        {
            var pointer = _loadedTexturesByTexture[texture];
            _loadedTexturesByPointer.Remove(pointer);
            _loadedTexturesByTexture.Remove(texture);
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

        unsafe { RenderDrawData(ImGui.GetDrawData()); }
    }

    #endregion ImGuiRenderer

    #region Setup & Update

    /// <summary>
    /// Setup key input event handler.
    /// </summary>
    protected virtual void SetupInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        _window.TextEntered += (s, a) =>
        {
            if (a.Unicode.Length > 0)
            {
                char c = a.Unicode[0];
                if (c == '\t') return;
                io.AddInputCharacter(c);
            }
        };

        _window.MouseWheelScrolled += (s, a) =>
        {
            if (a.Wheel == Mouse.Wheel.Vertical)
                io.AddMouseWheelEvent(0, a.Delta);
            else
                io.AddMouseWheelEvent(a.Delta, 0);
        };
    }

    /// <summary>
    /// Sends SFML input state to ImGui.
    /// </summary>
    protected virtual void UpdateInput()
    {
        if (!_window.HasFocus()) return;

        ImGuiIOPtr io = ImGui.GetIO();

        SFML.System.Vector2i mousePos = Mouse.GetPosition(_window);
        io.MousePos = new System.Numerics.Vector2(mousePos.X, mousePos.Y);
        io.AddMousePosEvent(mousePos.X, mousePos.Y);
        io.AddMouseButtonEvent(0, Mouse.IsButtonPressed(Mouse.Button.Left));
        io.AddMouseButtonEvent(1, Mouse.IsButtonPressed(Mouse.Button.Right));
        io.AddMouseButtonEvent(2, Mouse.IsButtonPressed(Mouse.Button.Middle));
        io.AddMouseButtonEvent(3, Mouse.IsButtonPressed(Mouse.Button.Extra1));
        io.AddMouseButtonEvent(4, Mouse.IsButtonPressed(Mouse.Button.Extra2));

        // Keyboard
        for (int i = 0; i < (int)Keyboard.KeyCount; i++)
        {
            var sfmlKey = (Keyboard.Key)i;
            if (TryMapKeys(sfmlKey, out ImGuiKey imguiKey))
            {
                io.AddKeyEvent(imguiKey, Keyboard.IsKeyPressed(sfmlKey));
            }
        }

        // Modifier keys
        io.AddKeyEvent(ImGuiKey.ModCtrl, Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl));
        io.AddKeyEvent(ImGuiKey.ModShift, Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift));
        io.AddKeyEvent(ImGuiKey.ModAlt, Keyboard.IsKeyPressed(Keyboard.Key.LAlt) || Keyboard.IsKeyPressed(Keyboard.Key.RAlt));
        io.AddKeyEvent(ImGuiKey.ModSuper, Keyboard.IsKeyPressed(Keyboard.Key.LSystem) || Keyboard.IsKeyPressed(Keyboard.Key.RSystem));

        io.DisplaySize = new System.Numerics.Vector2(_window.Size.X, _window.Size.Y);
        io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);

        WantsMouseCapture = io.WantCaptureMouse;
        WantsKeyboardCapture = io.WantCaptureKeyboard;
    }

    private bool TryMapKeys(Keyboard.Key key, out ImGuiKey imguikey)
    {
        imguikey = key switch
        {
            Keyboard.Key.Backspace => ImGuiKey.Backspace,
            Keyboard.Key.Tab => ImGuiKey.Tab,
            Keyboard.Key.Enter => ImGuiKey.Enter,
            Keyboard.Key.Escape => ImGuiKey.Escape,
            Keyboard.Key.Space => ImGuiKey.Space,
            Keyboard.Key.PageUp => ImGuiKey.PageUp,
            Keyboard.Key.PageDown => ImGuiKey.PageDown,
            Keyboard.Key.End => ImGuiKey.End,
            Keyboard.Key.Home => ImGuiKey.Home,
            Keyboard.Key.Left => ImGuiKey.LeftArrow,
            Keyboard.Key.Right => ImGuiKey.RightArrow,
            Keyboard.Key.Up => ImGuiKey.UpArrow,
            Keyboard.Key.Down => ImGuiKey.DownArrow,
            Keyboard.Key.Insert => ImGuiKey.Insert,
            Keyboard.Key.Delete => ImGuiKey.Delete,
            >= Keyboard.Key.Num0 and <= Keyboard.Key.Num9 => ImGuiKey.Key0 + (key - Keyboard.Key.Num0),
            >= Keyboard.Key.A and <= Keyboard.Key.Z => ImGuiKey.A + (key - Keyboard.Key.A),
            >= Keyboard.Key.Numpad0 and <= Keyboard.Key.Numpad9 => ImGuiKey.Keypad0 + (key - Keyboard.Key.Numpad0),
            Keyboard.Key.Multiply => ImGuiKey.KeypadMultiply,
            Keyboard.Key.Add => ImGuiKey.KeypadAdd,
            Keyboard.Key.Subtract => ImGuiKey.KeypadSubtract,
            Keyboard.Key.Period => ImGuiKey.KeypadDecimal,
            Keyboard.Key.Divide => ImGuiKey.KeypadDivide,
            >= Keyboard.Key.F1 and <= Keyboard.Key.F15 => ImGuiKey.F1 + (key - Keyboard.Key.F1),
            Keyboard.Key.LShift => ImGuiKey.LeftShift,
            Keyboard.Key.RShift => ImGuiKey.RightShift,
            Keyboard.Key.LControl => ImGuiKey.LeftCtrl,
            Keyboard.Key.RControl => ImGuiKey.RightCtrl,
            Keyboard.Key.LAlt => ImGuiKey.LeftAlt,
            Keyboard.Key.RAlt => ImGuiKey.RightAlt,
            Keyboard.Key.Semicolon => ImGuiKey.Semicolon,
            Keyboard.Key.Equal => ImGuiKey.Equal,
            Keyboard.Key.Comma => ImGuiKey.Comma,
            Keyboard.Key.Hyphen => ImGuiKey.Minus,
            Keyboard.Key.Slash => ImGuiKey.Slash,
            Keyboard.Key.Grave => ImGuiKey.GraveAccent,
            Keyboard.Key.LBracket => ImGuiKey.LeftBracket,
            Keyboard.Key.RBracket => ImGuiKey.RightBracket,
            Keyboard.Key.Backslash => ImGuiKey.Backslash,
            Keyboard.Key.Apostrophe => ImGuiKey.Apostrophe,
            Keyboard.Key.Menu => ImGuiKey.Menu,
            Keyboard.Key.Pause => ImGuiKey.Pause,
            _ => ImGuiKey.None,
        };

        return imguikey != ImGuiKey.None;
    }

    #endregion Setup & Update

    #region Internals

    /// <summary>
    /// Gets the geometry as set up by ImGui and sends it to the graphics device.
    /// </summary>
    private unsafe void RenderDrawData(ImDrawDataPtr drawData)
    {
        if (drawData.CmdListsCount == 0)
            return;

        // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
        drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);
        View originalView = _window.GetView();
        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[n];

            // Build SFML vertex array from ImGui vertex data
            var vertices = new Vertex[cmdList.VtxBuffer.Size];
            for (int i = 0; i < cmdList.VtxBuffer.Size; i++)
            {
                ImDrawVert vert = cmdList.VtxBuffer[i];
                uint col = vert.Col;
                vertices[i] = new Vertex(
                    new SFML.System.Vector2f(vert.Pos.X, vert.Pos.Y),
                    new SFMLColor((byte)(col & 0xFF), (byte)((col >> 8) & 0xFF), (byte)((col >> 16) & 0xFF), (byte)((col >> 24) & 0xFF)),
                    new SFML.System.Vector2f(vert.Uv.X, vert.Uv.Y)
                );
            }

            // Read index data
            var indices = new ushort[cmdList.IdxBuffer.Size];
            for (int i = 0; i < cmdList.IdxBuffer.Size; i++)
                indices[i] = cmdList.IdxBuffer[i];

            for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
            {
                ImDrawCmd drawCmd = cmdList.CmdBuffer[cmdi];

                if (drawCmd.ElemCount == 0)
                    continue;

                if (!_loadedTexturesByPointer.ContainsKey(drawCmd.TextureId))
                    throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");

                Texture texture = _loadedTexturesByPointer[drawCmd.TextureId];

                // Build triangle vertices for this draw command using the index buffer
                var triVertices = new Vertex[drawCmd.ElemCount];
                for (uint i = 0; i < drawCmd.ElemCount; i++)
                {
                    int idx = indices[drawCmd.IdxOffset + i] + (int)drawCmd.VtxOffset;
                    Vertex v = vertices[idx];
                    // Convert pixel UV to actual pixel coordinates for SFML (SFML uses pixel coords for tex coords)
                    v.TexCoords = new SFML.System.Vector2f(v.TexCoords.X * texture.Size.X, v.TexCoords.Y * texture.Size.Y);
                    triVertices[i] = v;
                }

                // Set up render state with the texture
                var states = RenderStates.Default;
                states.Texture = texture;

                // Apply scissor clipping via a View
                var clipRect = drawCmd.ClipRect;
                var scissorView = new View(
                    new FloatRect(
                        new SFML.System.Vector2f(clipRect.X, clipRect.Y),
                        new SFML.System.Vector2f(clipRect.Z - clipRect.X, clipRect.W - clipRect.Y)
                    )
                );

                // Map the scissor rect to the viewport proportionally
                var windowSize = _window.Size;
                scissorView.Viewport = new FloatRect(
                    new SFML.System.Vector2f(clipRect.X / windowSize.X, clipRect.Y / windowSize.Y),
                    new SFML.System.Vector2f((clipRect.Z - clipRect.X) / windowSize.X, (clipRect.W - clipRect.Y) / windowSize.Y)
                );

                _window.SetView(scissorView);
                _window.Draw(triVertices, PrimitiveType.Triangles, states);
            }
        }

        // Restore the default view
        _window.SetView(_window.DefaultView);
    }

    #endregion Internals
}
