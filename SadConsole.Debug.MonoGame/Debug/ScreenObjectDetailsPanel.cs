using System;
using System.Collections.Generic;
using System.Numerics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using System.Runtime.Serialization;

namespace SadConsole.Debug;

public class ScreenObjectDetailsPanel
{
    private string _serializeFileName;
    private ComponentsPanel _guiComponents = new();

    public static Dictionary<Type, Editors.IScreenObjectPanel> RegisteredPanels { get; } = [];

    public ScreenObjectState CurrentScreenObject;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state)
    {
        CurrentScreenObject = state;
        if (CurrentScreenObject == null) return;

        ImGuiSC.SeparatorText(state.Object.ToString(), Debugger.Settings.Color_PanelHeader);

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

            if (ImGui.BeginTabBar("item_properties_tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
            {
                if (ImGui.BeginTabItem("Settings", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
                {

                    ///////
                    // IsVisible/Enabled/Mouse/Keyboard
                    ///////
                    ImGui.SeparatorText("Flags"u8);

                    if (ImGui.Checkbox("Visible"u8, ref CurrentScreenObject.IsVisible))
                        GuiState._selectedScreenObject.IsVisible = CurrentScreenObject.IsVisible;
                    ImGui.SameLine();

                    if (ImGui.Checkbox("Enabled"u8, ref CurrentScreenObject.IsEnabled))
                        GuiState._selectedScreenObject.IsEnabled = CurrentScreenObject.IsEnabled;
                    ImGui.SameLine();

                    bool isFocused = CurrentScreenObject.Object.IsFocused;
                    if (ImGui.Checkbox("Is Focused"u8, ref isFocused))
                        CurrentScreenObject.Object.IsFocused = isFocused;

                    ImGui.SeparatorText("Input"u8);

                    bool useKeyboard = CurrentScreenObject.Object.UseKeyboard;
                    if (ImGui.Checkbox("Use Keyboard"u8, ref useKeyboard))
                        CurrentScreenObject.Object.UseKeyboard = useKeyboard;

                    bool useMouse = CurrentScreenObject.Object.UseMouse;
                    if (ImGui.Checkbox("Use Mouse"u8, ref useMouse))
                        CurrentScreenObject.Object.UseMouse = useMouse;
                    ImGui.SameLine();

                    bool mouseExclusive = CurrentScreenObject.Object.IsExclusiveMouse;
                    ImGui.BeginDisabled();
                    ImGui.Checkbox("Is Mouse Exclusive"u8, ref mouseExclusive);
                    ImGui.EndDisabled();

                    ImGui.SeparatorText("Serialization"u8);

                    if (ImGui.Button("Save Object"u8))
                    {
                        ImGui.OpenPopup("serialize_object"u8);
                        _serializeFileName = "";
                    }

                    if (ImGui.BeginPopupModal("serialize_object"u8))
                    {
                        ImGui.SetNextItemWidth(400);
                        ImGui.InputText("##filename"u8, ref _serializeFileName, 50);

                        if (ImGuiWindowBase.DrawButtons(out bool savedClicked, string.IsNullOrEmpty(_serializeFileName.Trim())))
                        {
                            if (savedClicked)
                                Serializer.Save(GuiState._selectedScreenObject, _serializeFileName.Trim(), false);
                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.EndPopup();
                    }

                    ImGui.EndTabItem();
                }

                ///////
                // Custom editors
                ///////
                if (RegisteredPanels.TryGetValue(CurrentScreenObject.Object.GetType(), out var panel))
                {
                    panel.BuildTabItem(renderer, CurrentScreenObject);
                }

                ///////
                // Components
                ///////

                if (ImGui.BeginTabItem("Components", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
                {
                    _guiComponents.BuildUI(renderer, GuiState._selectedScreenObjectState);

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
        ImGui.EndGroup();
        CurrentScreenObject = null;
    }
}
