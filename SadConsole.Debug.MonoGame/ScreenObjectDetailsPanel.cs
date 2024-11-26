using System;
using System.Collections.Generic;
using System.Numerics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

public class ScreenObjectDetailsPanel: ImGuiObjectBase
{
    public static Dictionary<Type, ImGuiObjectBase> RegisteredPanels = new Dictionary<Type, ImGuiObjectBase>();

    public ScreenObjectState CurrentScreenObject;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state)
    {
        CurrentScreenObject = state;
        BuildUI(renderer);
        CurrentScreenObject = null;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (CurrentScreenObject == null) return;

        //ImGui.BeginChild(id, new Vector2(0, 300), false, ImGuiWindowFlags.HorizontalScrollbar);
        ImGui.BeginGroup();
        {
            ImGui.AlignTextToFramePadding();
            ///////
            // Components
            ///////
            //ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Components");
            //ImGui.SameLine();
            //ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            //ImGui.ListBox("##Components", ref CurrentScreenObject.ComponentsSelectedItem, CurrentScreenObject.Components, CurrentScreenObject.Object.SadComponents.Count);

            //ImGui.Separator();

            ///////
            // Position X
            ///////
            ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Position X: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputInt("##x", ref CurrentScreenObject.PositionX))
            {
                CurrentScreenObject.Object.Position = CurrentScreenObject.Object.Position.WithX(CurrentScreenObject.PositionX);
                CurrentScreenObject.Refresh();
            }

            ///////
            // Width
            ///////
            ImGui.SameLine();
            ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Width: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(80);
            var start = ImGui.GetCursorPosX();
            if (CurrentScreenObject.IsScreenSurface)
                ImGui.Text(((IScreenSurface)CurrentScreenObject.Object).Surface.Width.ToString());
            else
                ImGui.Text("??");

            ///////
            // Position Y
            ///////
            ImGui.AlignTextToFramePadding();
            ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Position Y: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputInt("##y", ref CurrentScreenObject.PositionY))
            {
                CurrentScreenObject.Object.Position = CurrentScreenObject.Object.Position.WithY(CurrentScreenObject.PositionY);
                CurrentScreenObject.Refresh();
            }

            ///////
            // Height
            ///////
            ImGui.SameLine();
            ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Height: ");
            ImGui.SameLine();
            ImGui.SetCursorPosX(start);
            ImGui.SetNextItemWidth(80);
            if (CurrentScreenObject.IsScreenSurface)
                ImGui.Text(((IScreenSurface)CurrentScreenObject.Object).Surface.Height.ToString());
            else
                ImGui.Text("??");

            ImGui.Separator();

            ///////
            // Tint
            ///////
            if (CurrentScreenObject.IsScreenSurface)
            {
                ImGui.AlignTextToFramePadding();

                var startX = ImGui.GetCursorPosX();
                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Tint: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.ColorEdit4("##tint", ref CurrentScreenObject.SurfaceState.Tint))
                {
                    ((IScreenSurface)CurrentScreenObject.Object).Tint = CurrentScreenObject.SurfaceState.Tint.ToColor();
                    CurrentScreenObject.Refresh();
                }
            }

            ///////
            // IsVisible/Enabled
            ///////
            ImGui.Separator();

            if (ImGui.Checkbox("Visible", ref CurrentScreenObject.IsVisible))
                GuiState._selectedScreenObject.IsVisible = CurrentScreenObject.IsVisible;
            ImGui.SameLine();

            if (ImGui.Checkbox("Enabled", ref CurrentScreenObject.IsEnabled))
                GuiState._selectedScreenObject.IsEnabled = CurrentScreenObject.IsEnabled;

            ///////
            // Window
            ///////
            if (CurrentScreenObject.IsWindow)
            {
                var window = (UI.Window)CurrentScreenObject.Object;

                ImGui.Separator();

                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Title: ");
                ImGui.SameLine();
                window.Title = ImGuiExt.InputText("##window_title", window.Title);

                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Title Alignment: ");
                ImGui.SameLine();
                if (ImGui.Combo("##window_title_alignment", ref CurrentScreenObject.WindowState.TitleAlignment,
                                                            ConvertHorizontalAlignment.Names,
                                                            ConvertHorizontalAlignment.Names.Length))
                {
                    window.TitleAlignment = (HorizontalAlignment)CurrentScreenObject.WindowState.TitleAlignment;
                }

            }

            ///////
            // Custom editors
            ///////
            if (RegisteredPanels.TryGetValue(CurrentScreenObject.Object.GetType(), out var panel))
            {
                ImGui.Separator();
                panel.BuildUI(renderer);
            }
        }
        ImGui.EndGroup();
    }
}
