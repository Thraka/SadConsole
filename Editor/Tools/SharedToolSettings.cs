using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Serialization;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal static class SharedToolSettings
{
    public static ColoredGlyph Tip { get; set; }

    static SharedToolSettings()
    {
        Tip = new ColoredGlyph() { Glyph = 1 };
    }

    public static bool ImGuiDrawZones(Document document, ref bool showZones, [NotNullWhen(true)] out ZoneSerialized? zone)
    {
        ImGui.Checkbox("Show Zones", ref showZones);

        ImGui.SeparatorText("Defined Zones"u8);

        ImGuiList<ZoneSerialized> zones = ((IDocumentZones)document).Zones;

        ImGui.SetNextItemWidth(-1);
        ImGui.ListBox("##zoneslist", ref zones.SelectedItemIndex, zones.Names, zones.Count, 6);

        zone = zones.SelectedItem;

        if (zone is not null)
        {
            ImGui.SeparatorText("Properties"u8);
            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##zone_name", ref zones.SelectedItem.Name, 256);

            ImGui.Text("Debug Apperance"u8);
            SettingsTable.BeginTable("##debugglyph");
            ImGuiTypes.ColoredGlyphReference glyphRef = zones.SelectedItem.Appearance;
            SettingsTable.DrawCommonSettings(true, true, false, true, true,
                ref glyphRef,
                null,
                null,
                document.EditingSurfaceFont,
                ImGuiCore.Renderer);
            zone.Appearance = glyphRef.ToColoredGlyph();
            SettingsTable.EndTable();
            ImGui.SeparatorText("Settings"u8);
            if (ImGui.Button("Edit Settings"u8))
            {
                var tempZone = zone;
                var window = new Windows.KeyValuePairEditor(zone.Settings);
                window.Closed += (_, _) =>
                {
                    tempZone.Settings = window.ToDictionary();
                };
                window.Open();
            }

            if (zone.Settings.Count > 0)
            {
                if (ImGui.BeginChild("##zone_settings_child", new Vector2(-1, ImGui.GetFrameHeight() * zone.Settings.Count + ImGui.GetStyle().FramePadding.Y * 2)))
                {
                    if (ImGui.BeginTable("##zone_settings_table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollX))
                    {
                        ImGui.TableSetupColumn("Key"u8);
                        ImGui.TableSetupColumn("Value"u8);
                        ImGui.TableHeadersRow();

                        foreach (var kvp in zone.Settings)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.Text(kvp.Key);
                            ImGui.TableNextColumn();
                            ImGui.Text(kvp.Value);
                        }

                        ImGui.EndTable();
                    }
                    ImGui.EndChild();
                }
            }
            else
                ImGui.Text("No settings defined."u8);

            return true;
        }

        return false;
    }


    public static bool ImGuiDrawObjects(Document document, [NotNullWhen(true)] out SimpleObjectDefinition? obj)
    {
        IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)document;

        if (ImGui.Button("Manage Objects"u8))
            new Windows.SimpleObjectEditor(docSimpleObjects.SimpleObjects, document.EditingSurface.Surface.DefaultForeground.ToVector4(), document.EditingSurface.Surface.DefaultBackground.ToVector4(), document.EditingSurfaceFont).Open();

        ImGui.SetNextItemWidth(-1);

        if (ImGui.BeginListBox("##pencil_simplegameobjects"u8))
        {
            SimpleObjectHelpers.DrawSelectables("pencil_simplegameobjects", docSimpleObjects.SimpleObjects, document.EditingSurfaceFont);

            ImGui.EndListBox();
        }

        bool isItemSelected = docSimpleObjects.SimpleObjects.IsItemSelected();

        if (isItemSelected)
        {
            ImGui.SeparatorText("Selected Object"u8);

            ImGuiSC.FontGlyph.Draw(ImGuiCore.Renderer, "gameobject_definition",
                document.EditingSurfaceFont,
                docSimpleObjects.SimpleObjects.SelectedItem!.Visual);
            ImGui.SameLine();
            ImGui.Text(docSimpleObjects.SimpleObjects.SelectedItem!.ToString());

            obj = docSimpleObjects.SimpleObjects.SelectedItem;

            return true;
        }

        obj = null;
        return false;
    }
}
