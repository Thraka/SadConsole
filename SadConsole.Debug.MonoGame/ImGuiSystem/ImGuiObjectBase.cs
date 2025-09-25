namespace SadConsole.ImGuiSystem;

/// <summary>
/// Represents an object drawn in ImGui.
/// </summary>
public abstract class ImGuiObjectBase
{
    /// <summary>
    /// When true, this object should be drawn.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Draws this object.
    /// </summary>
    /// <param name="renderer">The ImGui renderer drawing this object.</param>
    public abstract void BuildUI(ImGuiRenderer renderer);
}
