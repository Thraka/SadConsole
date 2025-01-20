using System;
using SadConsole.Components;
using SadConsole.Host;
using SadConsole.ImGuiSystem;
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
                                                            Action<ImGuiMonoGameComponent> startupAction = null)
    {
        ImGuiConfig config = builder.GetOrCreateConfig<ImGuiConfig>();
        config.FontFileTTF = fontFile;
        config.FontSize = fontSize;
        config.StartupAction = startupAction;
        config.EnableDocking = enableDocking;
        return builder;
    }
}

internal class ImGuiConfig : IConfigurator
{
    public string FontFileTTF { get; set; } = "Roboto-Regular.ttf";
    public float FontSize { get; set; } = 16f;
    public bool EnableDocking { get; set; }
    public Action<ImGuiMonoGameComponent> StartupAction { get; set; }

    public void Run(BuilderBase config, GameHost game)
    {
        Game host = (Game)game;
        ImGuiMonoGameComponent imguiComponent = new(Global.GraphicsDeviceManager, host.MonoGameInstance, EnableDocking);

        var value = imguiComponent.ImGuiRenderer.AddFontTTF(FontFileTTF, FontSize);
        imguiComponent.ImGuiRenderer.SetDefaultFont(value);
        ImGuiSystem.Themes.SetModernColors();

        host.MonoGameInstance.Components.Add(imguiComponent);

        StartupAction?.Invoke(imguiComponent);
    }
}

