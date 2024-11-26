using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// A basic ImGui interface that adds the <see cref="ImGuiMonoGameComponent"/> to MonoGame, rendering ImGui over SadConsole.
/// </summary>
public static class ImGuiImplementation
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private static ImGuiMonoGameComponent _imGui;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Gets the renderer for ImGui.
    /// </summary>
    public static ImGuiRenderer Renderer => _imGui.ImGuiRenderer;

    /// <summary>
    /// Gets the MonoGame component that is rendering ImGui.
    /// </summary>
    public static ImGuiMonoGameComponent MonoGameComponent => _imGui;

    /// <summary>
    /// Gets the list of UI objects that is drawn by ImGui.
    /// </summary>
    public static System.Collections.Generic.List<ImGuiObjectBase> UIObjects => _imGui.UIComponents;

    /// <summary>
    /// Starts the ImGui system.
    /// </summary>
    /// <param name="disableFinalDraw">Turns off <see cref="Settings.DoFinalDraw"/>, which causes SadConsole to only render ImGui to the screen.</param>
    /// <param name="disableUpdate">Turns off <see cref="Settings.DoUpdate"/>, which pauses SadConsole's update loop.</param>
    /// <param name="disableInput">Turns off <see cref="Settings.Input.DoKeyboard"/> and <see cref="Settings.Input.DoMouse"/>, allowing only ImGui to handle input.</param>
    /// <param name="fontSize">The font size to use with ImGui.</param>
    /// <param name="fontFileTTF">The TTF font file to use with ImGui.</param>
    public static void Start(bool disableFinalDraw = true, bool disableUpdate = false, bool disableInput = true, float fontSize = 18f, string fontFileTTF = "JetBrains Mono SemiBold Nerd Font Complete.ttf")
    {
        Settings.DoFinalDraw = disableFinalDraw!;
        Settings.DoUpdate = disableUpdate!;
        Settings.Input.DoKeyboard = disableInput!;
        Settings.Input.DoMouse = disableInput!;

        _imGui = new ImGuiMonoGameComponent(Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
        var value = _imGui.ImGuiRenderer.AddFontTTF(fontFileTTF, fontSize);
        _imGui.ImGuiRenderer.SetDefaultFont(value);
        
        Game.Instance.MonoGameInstance.Components.Add(_imGui);
    }
}
