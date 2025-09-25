using System.Numerics;

namespace Hexa.NET.ImGui;

public static class ViewportExtensions
{
    // ImVec2              GetCenter() const       { return ImVec2(Pos.x + Size.x * 0.5f, Pos.y + Size.y * 0.5f); }
    public static Vector2 GetCenter(this ImGuiViewportPtr viewport) =>
        new(viewport.Pos.X + viewport.Size.X * 0.5f, viewport.Pos.Y + viewport.Size.Y * 0.5f);

    // ImVec2              GetWorkCenter() const   { return ImVec2(WorkPos.x + WorkSize.x * 0.5f, WorkPos.y + WorkSize.y * 0.5f); }
    public static Vector2 GetWorkCenter(this ImGuiViewportPtr viewport) =>
        new(viewport.WorkPos.X + viewport.WorkSize.X * 0.5f, viewport.WorkPos.Y + viewport.WorkSize.Y * 0.5f);
}
