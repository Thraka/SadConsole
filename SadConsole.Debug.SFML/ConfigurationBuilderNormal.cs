using System;
using SadConsole.Components;
using SadConsole.Host;
using SadConsole.ImGuiSystem;

// ReSharper disable once CheckNamespace
namespace SadConsole.Configuration;

/// <summary>
/// Extensions to enable ImGui with SadConsole SFML.
/// </summary>
public static class ConfigurationImGui
{
    /// <summary>
    /// Adds the ImGui SFML component to the game.
    /// </summary>
    /// <param name="builder">The config builder.</param>
    /// <param name="fontFile">The font file to use with ImGui. Defaults to 'Roboto-Regular.ttf'.</param>
    /// <param name="fontSize">The font size to use with ImGui. Defaults to 16.</param>
    /// <param name="enableDocking">Enables the docking feature of ImGui. Defaults to false.</param>
    /// <param name="startupAction">A callback to add objects to the ImGui SFML component.</param>
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
}

internal class ImGuiConfig : IConfigurator
{
    public string FontFileTTF { get; set; } = "Roboto-Regular.ttf";
    public float FontSize { get; set; } = 16f;
    public bool EnableDocking { get; set; }
    public Action<ImGuiSFMLComponent>? StartupAction { get; set; }

    public void Run(BuilderBase config, GameHost game)
    {
        Game host = (Game)game;
        ImGuiSFMLComponent imguiComponent = new(Global.GraphicsDevice, EnableDocking);

        var value = imguiComponent.ImGuiRenderer.AddFontTTF(FontFileTTF, FontSize);
        imguiComponent.ImGuiRenderer.SetDefaultFont(value);
        Themes.SetModernColors();

        ImGuiCore.ImGuiComponent = imguiComponent;

        // Hook into the game loop:
        // Update: Use a RootComponent so ImGui input runs before SadConsole input (enabling BlockSadConsoleInput)
        game.RootComponents.Add(new ImGuiUpdateComponent(imguiComponent));

        // Draw: Use the overlay callback so ImGui renders on top of SadConsole after DoFinalDraw
        Global.DrawOverlay = () => imguiComponent.Draw();

        StartupAction?.Invoke(imguiComponent);
    }
}

/// <summary>
/// A root component that updates ImGui input each frame before SadConsole processes input.
/// </summary>
internal class ImGuiUpdateComponent : RootComponent
{
    private readonly ImGuiSFMLComponent _component;

    public ImGuiUpdateComponent(ImGuiSFMLComponent component) =>
        _component = component;

    public override void Run(TimeSpan delta) =>
        _component.Update((float)delta.TotalSeconds);
}
