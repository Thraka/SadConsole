using System;
using System.Collections.Generic;
using System.Numerics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;

namespace SadConsole.Debug;

public class ScreenObjectDetailsPanel
{
    private string _serializeFileName;
    public static Dictionary<Type, Editors.IScreenObjectPanel> RegisteredPanels { get; } = [];

    public ScreenObjectState CurrentScreenObject;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state)
    {
        CurrentScreenObject = state;
        if (CurrentScreenObject == null) return;

        ImGuiSC.SeparatorText(state.Object.ToString(), Debugger.Settings.Color_PanelHeader);
        ImGui.Separator();

        //ImGui.BeginChild(id, new Vector2(0, 300), false, ImGuiWindowFlags.HorizontalScrollbar);
        ImGui.BeginGroup();
        {
            string widthHeight = CurrentScreenObject.IsScreenSurface ?
                                 $"{CurrentScreenObject.SurfaceState.Width}/{CurrentScreenObject.SurfaceState.Height}" :
                                 "??";

            if (SettingsTable.BeginTable("object_basic_props"))
            {
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
                    if (SettingsTable.DrawColor("Tint", "##tint1", ref CurrentScreenObject.SurfaceState.Tint, Color.Transparent.ToVector4(), true, out _))
                    {
                        ((IScreenSurface)CurrentScreenObject.Object).Tint = CurrentScreenObject.SurfaceState.Tint.ToColor();
                        CurrentScreenObject.Refresh();
                    }
                }

                SettingsTable.EndTable();
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

            bool isFocused = CurrentScreenObject.Object.IsFocused;
            if (ImGui.Checkbox("Is Focused", ref isFocused))
                CurrentScreenObject.Object.IsFocused = isFocused;

            ImGui.Separator();

            if (ImGui.Button("Save Object"))
            {
                ImGui.OpenPopup("serialize_object");
                _serializeFileName = "";
            }

            if (ImGui.BeginPopupModal("serialize_object"))
            {
                ImGui.SetNextItemWidth(400);
                ImGui.InputText("##filename", ref _serializeFileName, 50 );

                if (ImGuiWindowBase.DrawButtons(out bool savedClicked, string.IsNullOrEmpty(_serializeFileName.Trim())))
                {
                    if (savedClicked)
                        Serializer.Save(GuiState._selectedScreenObject, _serializeFileName.Trim(), false);
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            ///////
            // Custom editors
            ///////
            if (RegisteredPanels.TryGetValue(CurrentScreenObject.Object.GetType(), out var panel))
            {
                panel.BuildUI(renderer, CurrentScreenObject);
            }
        }
        ImGui.EndGroup();
        CurrentScreenObject = null;
    }
}
