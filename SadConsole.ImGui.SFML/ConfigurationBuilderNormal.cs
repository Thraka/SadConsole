using System;
using Hexa.NET.ImGui;
using SadConsole.Components;
using SadConsole.Debug;
using SadConsole.Host;
using SadConsole.ImGuiSystem.Rendering;
using SadConsole.Input;

// ReSharper disable once CheckNamespace
namespace SadConsole.Configuration;

/// <summary>
/// Extensions to enable ImGui with SadConsole.
/// </summary>
public static class ConfigurationImGui
{
    /// <summary>
    /// Adds the ImGui MonoGame component to MonoGame.
    /// </summary>
    /// <param name="builder">The config builder.</param>
    /// <param name="fontFile">The font file to use with ImGui. Defaults to 'Roboto-Regular.ttf'.</param>
    /// <param name="fontSize">The font size to use with ImGui. Defaults to 16.</param>
    /// <param name="enableDocking">Enables the docking feature of ImGui. Defaults to false.</param>
    /// <param name="startupAction">A callback to add objects to the ImGui MonoGame component.</param>
    /// <returns>The config builder.</returns>
    public static Builder EnableImGui(this Builder builder, string fontFile = "Roboto-Regular.ttf",
                                                            float fontSize = 16f,
                                                            bool enableDocking = false,
                                                            Action<ImGuiSFMLComponent>? startupAction = null)
    {
        ImGuiConfig config = builder.GetOrCreateConfig<ImGuiConfig>();
        config.FontFileTTF = fontFile;
        config.FontSize = fontSize;
        config.StartupAction = startupAction;
        config.EnableDocking = enableDocking;
        return builder;
    }

    /// <summary>
    /// Adds a <see cref="GameHost.RootComponents"/> component that uses the specified hotkey to invoke <see cref="Debug.Debugger.Start"/>.
    /// </summary>
    /// <param name="builder">The config builder.</param>
    /// <param name="hotkey">The keyboard key to start the debugger.</param>
    /// <returns>The config builder.</returns>
    public static Builder EnableImGuiDebugger(this Builder builder, Keys hotkey)
    {
        ImGuiConfig config = builder.GetOrCreateConfig<ImGuiConfig>();
        config.StartDebuggerKey = hotkey;
        config.EnableDocking = true;
        config.AddDebugger = true;
        return builder;
    }
}

internal class ImGuiConfig : RootComponent, IConfigurator
{
    public ImGuiSFMLComponent ImGuiInstance { get; private set; }
    public string FontFileTTF { get; set; } = "Roboto-Regular.ttf";
    public float FontSize { get; set; } = 16f;
    public bool EnableDocking { get; set; }
    public bool AddDebugger { get; set; }
    public Keys StartDebuggerKey { get; set; }

    public Action<ImGuiSFMLComponent>? StartupAction { get; set; }
    public override void Run(TimeSpan delta)
    {
        if (AddDebugger && ImGuiDebugger.Instance.IsOpened == false && Game.Instance.FrameNumber != 0 && Game.Instance.Keyboard.IsKeyReleased(StartDebuggerKey))
            ImGuiDebugger.Start(ImGuiInstance.ImGuiRenderer);

        ImGuiInstance.Update((float)delta.TotalSeconds);
    }

    public void Run(BuilderBase config, GameHost game)
    {
        Game host = (Game)game;
        ImGuiInstance = new(Global.GraphicsDevice, EnableDocking);

        var value = ImGuiInstance.ImGuiRenderer.AddFontTTF(FontFileTTF, FontSize);
        ImGuiInstance.ImGuiRenderer.SetDefaultFont(value);
        Themes.SetModernColors();

        // Hook into the game loop:
        // Update: Use a RootComponent so ImGui input runs before SadConsole input (enabling BlockSadConsoleInput)
        game.RootComponents.Add(this);

        if (EnableDocking)
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        if (AddDebugger)
            ImGuiInstance.UIComponents.Add(ImGuiDebugger.Instance);

        // Draw: Use the overlay callback so ImGui renders on top of SadConsole after DoFinalDraw
        Global.DrawOverlay = () => ImGuiInstance.Draw();

        StartupAction?.Invoke(ImGuiInstance);
    }
}
