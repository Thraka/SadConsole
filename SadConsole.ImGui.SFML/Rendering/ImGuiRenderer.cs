using System;
using System.Runtime.InteropServices;
using Hexa.NET.ImGui;
using SFML.Graphics;
using SFML.Window;
using SFMLColor = SFML.Graphics.Color;

namespace SadConsole.ImGuiSystem.Rendering;

/// <summary>
/// ImGui Renderer for SFML.
/// </summary>
public class ImGuiRenderer : ImGuiRenderer<Texture>, IDisposable
{
    private RenderWindow _window;

    internal ImGuiRenderer(RenderWindow window)
    {
        ImGuiContextPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        _window = window ?? throw new ArgumentNullException(nameof(window));

        SetupInput();
        SetupBackendCapabilities();
    }

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

    private void SetupBackendCapabilities()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasTextures;

        ImGuiPlatformIOPtr platformIO = ImGui.GetPlatformIO();
        platformIO.RendererTextureMaxWidth = 4096;
        platformIO.RendererTextureMaxHeight = 4096;
    }

    protected unsafe override TextureInfo CreateTexture(ImTextureDataPtr textureData)
    {
        var texture = new Texture(new SFML.System.Vector2u((uint)textureData.Width, (uint)textureData.Height));

        if (textureData.Pixels != null)
        {
            int pixelCount = textureData.Width * textureData.Height;
            int bytesPerPixel = textureData.Format == ImTextureFormat.Rgba32 ? 4 : 1;
            int dataSize = pixelCount * bytesPerPixel;

            byte[] managedData = new byte[dataSize];
            Marshal.Copy(new IntPtr(textureData.Pixels), managedData, 0, dataSize);

            if (textureData.Format == ImTextureFormat.Rgba32)
            {
                texture.Update(managedData);
            }
            else
            {
                // SFML textures expect RGBA; expand Alpha8 to RGBA
                byte[] rgbaData = new byte[pixelCount * 4];
                for (int i = 0; i < pixelCount; i++)
                {
                    rgbaData[i * 4] = 255;
                    rgbaData[i * 4 + 1] = 255;
                    rgbaData[i * 4 + 2] = 255;
                    rgbaData[i * 4 + 3] = managedData[i];
                }
                texture.Update(rgbaData);
            }
        }

        TextureInfo textureInfo = new(texture, textureData.GetTexID(), true);

        textureData.SetStatus(ImTextureStatus.Ok);

        return textureInfo;
    }

    protected unsafe override void RefreshTexture(ImTextureDataPtr textureData)
    {
        IntPtr texId = textureData.GetTexID();
        if (!_loadedTexturesByPointer.TryGetValue(texId, out TextureInfo? textureInfo))
            return;

        Texture texture = textureInfo.Texture;

        // Check if the texture's dimensions have changed
        if (texture.Size.X != (uint)textureData.Width || texture.Size.Y != (uint)textureData.Height)
        {
            texture.Dispose();
            texture = new Texture(new SFML.System.Vector2u((uint)textureData.Width, (uint)textureData.Height));
            textureInfo.Texture = texture;
        }

        if (textureData.Pixels != null)
        {
            int pixelCount = textureData.Width * textureData.Height;
            int bytesPerPixel = textureData.Format == ImTextureFormat.Rgba32 ? 4 : 1;
            int dataSize = pixelCount * bytesPerPixel;

            byte[] managedData = new byte[dataSize];
            Marshal.Copy(new IntPtr(textureData.Pixels), managedData, 0, dataSize);

            if (textureData.Format == ImTextureFormat.Rgba32)
            {
                texture.Update(managedData);
            }
            else
            {
                byte[] rgbaData = new byte[pixelCount * 4];
                for (int i = 0; i < pixelCount; i++)
                {
                    rgbaData[i * 4] = 255;
                    rgbaData[i * 4 + 1] = 255;
                    rgbaData[i * 4 + 2] = 255;
                    rgbaData[i * 4 + 3] = managedData[i];
                }
                texture.Update(rgbaData);
            }
        }

        textureData.SetStatus(ImTextureStatus.Ok);
    }

    /// <summary>
    /// Sends SFML input state to ImGui.
    /// </summary>
    protected override void UpdateInput()
    {
        if (!_window.HasFocus()) return;

        ImGuiIOPtr io = ImGui.GetIO();

        SFML.System.Vector2i mousePos = Mouse.GetPosition(_window);
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

    /// <summary>
    /// Gets the geometry as set up by ImGui and sends it to the graphics device.
    /// </summary>
    protected unsafe override void RenderDrawData(ImDrawData* drawData)
    {
        if (drawData->CmdListsCount == 0)
            return;

        // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
        drawData->ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

        // Save and set a clean pixel-perfect view for ImGui rendering
        View previousView = _window.GetView();
        var windowSize = _window.Size;
        _window.SetView(new View(new FloatRect(
            new SFML.System.Vector2f(0, 0),
            new SFML.System.Vector2f(windowSize.X, windowSize.Y)
        )));

        for (int n = 0; n < drawData->CmdListsCount; n++)
        {
            ImDrawList* cmdList = drawData->CmdLists.Data[n];

            // Build SFML vertex array from ImGui vertex data
            var vertices = new Vertex[cmdList->VtxBuffer.Size];
            for (int i = 0; i < cmdList->VtxBuffer.Size; i++)
            {
                ImDrawVert vert = cmdList->VtxBuffer.Data[i];
                uint col = vert.Col;
                vertices[i] = new Vertex(
                    new SFML.System.Vector2f(vert.Pos.X, vert.Pos.Y),
                    new SFMLColor((byte)(col & 0xFF), (byte)((col >> 8) & 0xFF), (byte)((col >> 16) & 0xFF), (byte)((col >> 24) & 0xFF)),
                    new SFML.System.Vector2f(vert.Uv.X, vert.Uv.Y)
                );
            }

            // Read index data
            var indices = new ushort[cmdList->IdxBuffer.Size];
            for (int i = 0; i < cmdList->IdxBuffer.Size; i++)
                indices[i] = cmdList->IdxBuffer.Data[i];

            for (int cmdi = 0; cmdi < cmdList->CmdBuffer.Size; cmdi++)
            {
                ImDrawCmd* drawCmd = &cmdList->CmdBuffer.Data[cmdi];

                if (drawCmd->ElemCount == 0)
                    continue;

                ImTextureRef textureRef = drawCmd->TexRef;
                ImTextureID texId = textureRef.GetTexID();
                if (!_loadedTexturesByPointer.TryGetValue(texId, out TextureInfo? textureInfo))
                    throw new InvalidOperationException($"Could not find a texture with id '{texId}', please check your bindings");

                Texture texture = textureInfo.Texture;

                // Build triangle vertices for this draw command using the index buffer
                var triVertices = new Vertex[drawCmd->ElemCount];
                for (uint i = 0; i < drawCmd->ElemCount; i++)
                {
                    int idx = indices[drawCmd->IdxOffset + i] + (int)drawCmd->VtxOffset;
                    Vertex v = vertices[idx];
                    // Convert normalized UV to pixel coordinates for SFML (SFML uses pixel coords for tex coords)
                    v.TexCoords = new SFML.System.Vector2f(v.TexCoords.X * texture.Size.X, v.TexCoords.Y * texture.Size.Y);
                    triVertices[i] = v;
                }

                // Set up render state with the texture
                var states = RenderStates.Default;
                states.Texture = texture;

                // NOTE: Scissor clipping is not applied. SFML doesn't expose scissor testing
                // without direct OpenGL calls. This means scrollable ImGui regions may show
                // minor overflow artifacts. For a debug overlay this is acceptable.

                _window.Draw(triVertices, PrimitiveType.Triangles, states);
            }
        }

        // Restore the previous view so SadConsole rendering isn't affected
        _window.SetView(previousView);
    }

    public void Dispose()
    {
        // Clean up managed textures
        foreach (TextureInfo textureInfo in _loadedTexturesByPointer.Values)
        {
            if (textureInfo.IsManaged)
                textureInfo.Texture?.Dispose();
        }

        _loadedTexturesByPointer.Clear();
        _loadedTexturesByTexture.Clear();

        ImGui.DestroyContext();
    }
}
