using System;
using Hexa.NET.ImGui;
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
    /// Adds the ImGui SFML component to SadConsole.
    /// </summary>
    /// <param name="builder">The config builder.</param>
    /// <param name="fontConfig">The font file and size to use with ImGui. Null uses the default ImGui font.</param>
    /// <param name="enableDocking">Enables the docking feature of ImGui. Defaults to false.</param>
    /// <param name="startupAction">A callback to add objects to the ImGui SFML component.</param>
    /// <returns>The config builder.</returns>
    public static Builder EnableImGui(this Builder builder, ImGuiSadConsoleFontConfig? fontConfig = null,
                                                            bool enableDocking = false,
                                                            Action<ImGuiSFMLComponent>? startupAction = null)
    {
        ImGuiConfig config = builder.GetOrCreateConfig<ImGuiConfig>();
        config.FontConfig = fontConfig;
        config.StartupAction = startupAction;
        config.EnableDocking = enableDocking;
        return builder;
    }

    /// <summary>
    /// Adds a <see cref="GameHost.RootComponents"/> component that uses the specified hotkey to invoke <see cref="Debug.Debugger.Start"/>.
    /// </summary>
    /// <param name="builder">The config builder.</param>
    /// <param name="hotkey">The keyboard key to start the debugger.</param>
    /// <param name="fontConfig">The font file and size to use with ImGui. Null uses the default ImGui font.</param>
    /// <param name="startupAction">A callback to add objects to the ImGui SFML component.</param>
    /// <returns>The config builder.</returns>
    public static Builder EnableImGuiDebugger(this Builder builder, Keys hotkey, ImGuiSadConsoleFontConfig? fontConfig = null, Action<ImGuiSFMLComponent>? startupAction = null)
    {
        ImGuiConfig config = builder.GetOrCreateConfig<ImGuiConfig>();
        config.StartDebuggerKey = hotkey;
        config.FontConfig = fontConfig;
        config.StartupAction = startupAction;
        config.EnableDocking = true;
        config.AddDebugger = true;
        return builder;
    }
}

public record struct ImGuiSadConsoleFontConfig(string FontFileTTF, float FontSize)
{
    public static ImGuiSadConsoleFontConfig Default => new ImGuiSadConsoleFontConfig("roboto-regular.ttf", 16f);
}

internal class ImGuiConfig : IConfigurator
{
    public ImGuiSFMLComponent ImGuiInstance { get; private set; }
    public ImGuiSadConsoleFontConfig? FontConfig { get; set; }
    public bool EnableDocking { get; set; }
    public bool AddDebugger { get; set; }
    public Keys StartDebuggerKey { get; set; }

    public Action<ImGuiSFMLComponent>? StartupAction { get; set; }

    public void Run(BuilderBase config, GameHost game)
    {
        Game host = (Game)game;
        ImGuiInstance = new(Global.GraphicsDevice);

        if (FontConfig is not null)
        {
            ImGuiInstance.ImGuiRenderer.SetDefaultFont(
                ImGuiInstance.ImGuiRenderer.AddFontTTF(FontConfig.Value.FontFileTTF, FontConfig.Value.FontSize));
        }

        Themes.SetModernColors();

        if (EnableDocking)
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        if (AddDebugger)
            ImGuiInstance.ImGuiRenderer.UIObjects.Add(ImGuiDebugger.Instance);

        // Update: Use the overlay callback so ImGui input runs before SadConsole input (enabling BlockSadConsoleInput)
        // Note: Not using RootComponent like MonoGame because we need UpdateOverlay to run already, so put both open and update logic in one place
        Global.UpdateOverlay = (delta) =>
        {
            if (AddDebugger && ImGuiDebugger.Instance.IsOpened == false && Game.Instance.FrameNumber != 0 && Game.Instance.Keyboard.IsKeyReleased(StartDebuggerKey))
            {
                Game.Instance.Keyboard.Clear();
                ImGuiDebugger.Start(ImGuiInstance.ImGuiRenderer);
            }

            ImGuiInstance.Update((float)delta.TotalSeconds);
        };

        // Draw: Use the overlay callback so ImGui renders on top of SadConsole after DoFinalDraw
        Global.DrawOverlay = () => ImGuiInstance.Draw();

        StartupAction?.Invoke(ImGuiInstance);
    }
}
