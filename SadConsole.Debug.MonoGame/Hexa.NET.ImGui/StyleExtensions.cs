using System.Numerics;

namespace Hexa.NET.ImGui;

/// <summary>
/// Extensions that help with getting ImGui style values.
/// </summary>
public static class StyleExtensions
{
    /// <summary>
    /// Gets the frame padding and item spacing values.
    /// </summary>
    /// <param name="style">The style object.</param>
    /// <param name="framePadding">Frame padding.</param>
    /// <param name="itemSpacing">Item spacing.</param>
    public static void GetSpacing(this ImGuiStylePtr style, out Vector2 framePadding, out Vector2 itemSpacing)
    {
        framePadding = style.FramePadding;
        itemSpacing = style.ItemSpacing;
    }

    /// <summary>
    /// Calculates the text size and adds frame padding * 2 for each item provided.
    /// </summary>
    /// <param name="style">The style object.</param>
    /// <param name="items">String values to calculate.</param>
    /// <returns>The total size.</returns>
    public static float GetWidthOfItems(this ImGuiStylePtr style, params string[] items)
    {
        float width = 0;
        GetSpacing(style, out Vector2 framePadding, out Vector2 itemSpacing);

        foreach (string item in items)
            width += itemSpacing.X + ImGui.CalcTextSize(item).X + framePadding.X * 2;

        return width;
    }
}
