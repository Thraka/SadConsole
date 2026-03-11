using System.Collections.Generic;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// Shared objects that drive the ImGui integration for SadConsole.
/// </summary>
public static class ImGuiCore
{
    /// <summary>
    /// SFML component for rendering ImGui.
    /// </summary>
    public static ImGuiSFMLComponent ImGuiComponent = null!;

    /// <summary>
    /// The ImGui objects to draw each game frame.
    /// </summary>
    public static List<ImGuiObjectBase> GuiComponents => ImGuiComponent.UIComponents;

    /// <summary>
    /// The ImGui renderer.
    /// </summary>
    public static ImGuiRenderer Renderer => ImGuiComponent.ImGuiRenderer;
}
