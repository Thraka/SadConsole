using System;
using System.Collections.Generic;
using System.Numerics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

public class ScreenObjectDetailsPanel: ImGuiObjectBase
{
    public static Dictionary<Type, ImGuiObjectBase> RegisteredPanels { get; } = [];

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
            string widthHeight = CurrentScreenObject.IsScreenSurface ?
                                 $"{CurrentScreenObject.SurfaceState.Width}/{CurrentScreenObject.SurfaceState.Height}" :
                                 "??";

            SettingsTable.BeginTable("object_basic_props");
            SettingsTable.DrawText("Width/Height", widthHeight);
            if (SettingsTable.DrawInt("Position X", "##x1", ref CurrentScreenObject.PositionX, -50000))
            {
                CurrentScreenObject.Object.Position = CurrentScreenObject.Object.Position.WithX(CurrentScreenObject.PositionX);
                CurrentScreenObject.Refresh();
            }
            if (SettingsTable.DrawInt("Position Y", "##y1", ref CurrentScreenObject.PositionY, -50000))
            {
                CurrentScreenObject.Object.Position = CurrentScreenObject.Object.Position.WithY(CurrentScreenObject.PositionY);
                CurrentScreenObject.Refresh();
            }
            if (CurrentScreenObject.IsScreenSurface)
            {
                if (SettingsTable.DrawColor("Tint", "##tint1", ref CurrentScreenObject.SurfaceState.Tint, Color.Transparent.ToVector4(), out _))
                {
                    ((IScreenSurface)CurrentScreenObject.Object).Tint = CurrentScreenObject.SurfaceState.Tint.ToColor();
                    CurrentScreenObject.Refresh();
                }
            }
            SettingsTable.EndTable();

            ///////
            // IsVisible/Enabled
            ///////
            ImGui.Separator();

            if (ImGui.Checkbox("Visible", ref CurrentScreenObject.IsVisible))
                GuiState._selectedScreenObject.IsVisible = CurrentScreenObject.IsVisible;
            ImGui.SameLine();

            if (ImGui.Checkbox("Enabled", ref CurrentScreenObject.IsEnabled))
                GuiState._selectedScreenObject.IsEnabled = CurrentScreenObject.IsEnabled;

            bool isFocused = CurrentScreenObject.Object.IsFocused;
            if (ImGui.Checkbox("Is Focused", ref isFocused))
                CurrentScreenObject.Object.IsFocused = isFocused;


            ///////
            // Window
            ///////
            if (CurrentScreenObject.IsWindow)
            {
                var window = (UI.Window)CurrentScreenObject.Object;

                ImGui.SeparatorText("Window Settings");

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Title: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1);
                window.Title = ImGui2.InputText("##window_title", window.Title);

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Title Alignment: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.CalcTextSize("Stretch").X * 2 + ImGui.GetStyle().FramePadding.X * 2.0f);
                if (ImGui.Combo("##window_title_alignment", ref CurrentScreenObject.WindowState.TitleAlignment,
                                                            Enums<HorizontalAlignment>.Names,
                                                            Enums<HorizontalAlignment>.Count))
                {
                    window.TitleAlignment = (HorizontalAlignment)CurrentScreenObject.WindowState.TitleAlignment;
                }

            }

            ///////
            // Custom editors
            ///////
            if (RegisteredPanels.TryGetValue(CurrentScreenObject.Object.GetType(), out var panel))
            {
                ImGui.SeparatorText("Custom Editor");
                panel.BuildUI(renderer);
            }
        }
        ImGui.EndGroup();
    }
}
