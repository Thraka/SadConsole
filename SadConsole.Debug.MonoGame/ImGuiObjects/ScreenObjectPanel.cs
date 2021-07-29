using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ImGuiNET;
using SadConsole.Numerics;
using SadRogue.Primitives;

namespace SadConsole.Debug.MonoGame.ImGuiObjects
{
    public static class ScreenObjectPanel
    {
        public static void Begin(string id, ScreenObjectState screenObjectState)
        {
            ImGui.BeginChild(id, new Vector2(0, 300), false, ImGuiWindowFlags.HorizontalScrollbar);
            {
                ///////
                // Components
                ///////
                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Components");
                ImGui.SameLine();
                ImGui.ListBox("##Components", ref screenObjectState.ComponentsSelectedItem, screenObjectState.Components, screenObjectState.Object.SadComponents.Count);

                ///////
                // Position X
                ///////
                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Position X: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                if (ImGui.InputInt("##x", ref screenObjectState.PositionX))
                {
                    screenObjectState.Object.Position = screenObjectState.Object.Position.WithX(screenObjectState.PositionX);
                    screenObjectState.Refresh();
                }

                ///////
                // Width
                ///////
                ImGui.SameLine();
                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Width: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(80);
                if (screenObjectState.IsScreenSurface)
                    ImGui.Text(((IScreenSurface)screenObjectState.Object).Surface.Width.ToString());
                else
                    ImGui.Text("??");

                ///////
                // Position Y
                ///////
                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Position Y: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                if (ImGui.InputInt("##y", ref screenObjectState.PositionY))
                {
                    screenObjectState.Object.Position = screenObjectState.Object.Position.WithY(screenObjectState.PositionY);
                    screenObjectState.Refresh();
                }

                ///////
                // Height
                ///////
                ImGui.SameLine();
                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Height: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(80);
                if (screenObjectState.IsScreenSurface)
                    ImGui.Text(((IScreenSurface)screenObjectState.Object).Surface.Height.ToString());
                else
                    ImGui.Text("??");

                ImGui.Separator();

                ///////
                // Tint
                ///////
                if (screenObjectState.IsScreenSurface)
                {
                    ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Tint: ");
                    ImGui.SameLine();
                    if (ImGui.ColorEdit4("##tint", ref screenObjectState.Tint))
                    {
                        ((IScreenSurface)screenObjectState.Object).Tint = screenObjectState.Tint.ToColor();
                        screenObjectState.Refresh();
                    }
                }
            }
        }

        public static void End() =>
            ImGui.EndChild();
    }
}
