using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

/// <summary>
/// Helper class for drawing and managing zones in ImGui.
/// </summary>
internal static class ZoneHelpers
{
    // State tracking for zone name editing
    private static ZoneSimplified? _lastSelectedZone;
    private static string _editingZoneName = string.Empty;
    private static string _lastValidZoneName = string.Empty;
    private static bool _zoneNameConflict;

    /// <summary>
    /// Checks if the specified zone name already exists in another zone.
    /// </summary>
    private static bool IsZoneNameDuplicate(string name, ZoneSimplified excludeZone, ImGuiList<ZoneSimplified> zones)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        foreach (var zone in zones.Objects)
        {
            if (zone != excludeZone && string.Equals(zone.Name, name, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Draws the zone editing UI for a document.
    /// </summary>
    /// <param name="document">The document containing zones.</param>
    /// <param name="zone">The currently selected zone, if any.</param>
    /// <returns>True if a zone is selected; otherwise, false.</returns>
    public static bool ImGuiDrawZones(Document document, [NotNullWhen(true)] out ZoneSimplified? zone, out bool zoneVisualChanged)
    {
        zoneVisualChanged = false;

        ImGui.SeparatorText("Defined Zones"u8);

        ImGuiList<ZoneSimplified> zones = ((IDocumentZones)document).Zones;

        ImGui.SetNextItemWidth(-1);
        int previousSelection = zones.SelectedItemIndex;
        if (ImGui.ListBox("##zoneslist", ref zones.SelectedItemIndex, zones.Names, zones.Count, 6))
        {
            // Selection changed - apply pending changes to previous item before switching
            if (previousSelection != -1 && !_zoneNameConflict && !string.IsNullOrEmpty(_editingZoneName))
            {
                zones.Objects[previousSelection].Name = _editingZoneName;
            }

            // Load the new item's values for editing
            if (zones.IsItemSelected())
            {
                _editingZoneName = zones.SelectedItem.Name;
                _lastValidZoneName = zones.SelectedItem.Name;
                _lastSelectedZone = zones.SelectedItem;
                _zoneNameConflict = false;
            }
        }

        // Add and Delete buttons
        if (ImGui.Button("Add"u8))
        {
            // Apply pending changes before adding new zone
            if (zones.IsItemSelected() && !_zoneNameConflict && !string.IsNullOrEmpty(_editingZoneName))
            {
                zones.SelectedItem.Name = _editingZoneName;
            }

            // Generate a unique name
            string baseName = "New Zone";
            string newName = baseName;
            int counter = 1;
            while (zones.Objects.Any(z => string.Equals(z.Name, newName, StringComparison.Ordinal)))
            {
                newName = $"{baseName} {counter}";
                counter++;
            }

            var newZone = new ZoneSimplified
            {
                Name = newName,
                ZoneArea = new SadRogue.Primitives.Area(),
                Appearance = new ColoredGlyph
                {
                    Foreground = document.EditingSurface.Surface.DefaultForeground,
                    Background = document.EditingSurface.Surface.DefaultBackground,
                    Glyph = '?'
                },
                Settings = []
            };

            zones.Objects.Add(newZone);
            zones.SelectedItem = newZone;

            // Load the new zone's values for editing
            _editingZoneName = newZone.Name;
            _lastValidZoneName = newZone.Name;
            _lastSelectedZone = newZone;
            _zoneNameConflict = false;
        }

        ImGui.SameLine();

        ImGui.BeginDisabled(!zones.IsItemSelected());
        if (ImGui.Button("Delete"u8))
            ImGui.OpenPopup("ConfirmDeleteZone"u8);
        ImGui.EndDisabled();

        ImGui.SameLine();

        ImGui.BeginDisabled(!zones.IsItemSelected());
        if (ImGui.Button("Erase"u8))
            ImGui.OpenPopup("ConfirmEraseZone"u8);
        ImGui.EndDisabled();

        // Delete confirmation popup
        ImGuiSC.CenterNextWindow();
        if (ImGui.BeginPopup("ConfirmDeleteZone"u8))
        {
            ImGui.Text("Are you sure you want to delete this zone?");

            if (ImGui.Button("Cancel"u8))
                ImGui.CloseCurrentPopup();

            ImGui.SameLine();
            if (ImGui.Button("Yes"u8))
            {
                zones.Objects.Remove(zones.SelectedItem!);
                _editingZoneName = string.Empty;
                _lastValidZoneName = string.Empty;
                _lastSelectedZone = null;
                _zoneNameConflict = false;
                zoneVisualChanged = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        // Erase zone popup
        ImGuiSC.CenterNextWindow();
        if (ImGui.BeginPopup("ConfirmEraseZone"u8))
        {
            ImGui.Text("Remove all points of a zone?");

            if (ImGui.Button("Cancel"u8))
                ImGui.CloseCurrentPopup();

            ImGui.SameLine();
            if (ImGui.Button("Yes"u8))
            {
                zones.SelectedItem!.ZoneArea = new();
                zoneVisualChanged = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        zone = zones.SelectedItem;

        if (zone is not null)
        {
            // Initialize or update editing state when selection changes
            if (_lastSelectedZone != zone)
            {
                _editingZoneName = zone.Name;
                _lastValidZoneName = zone.Name;
                _lastSelectedZone = zone;
                _zoneNameConflict = false;
            }

            ImGui.SeparatorText("Properties"u8);
            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##zone_name", ref _editingZoneName, 256);
            bool isEmpty = string.IsNullOrEmpty(_editingZoneName);

            // Check for duplicate name
            _zoneNameConflict = IsZoneNameDuplicate(_editingZoneName, zone, zones);

            if (isEmpty)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f));
                ImGui.TextWrapped("Name cannot be empty.");
                ImGui.PopStyleColor();
            }
            else if (_zoneNameConflict)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f));
                ImGui.TextWrapped("Name already exists. Please use a unique name.");
                ImGui.PopStyleColor();
            }

            // Only apply name change if valid
            if (!isEmpty && !_zoneNameConflict)
            {
                zone.Name = _editingZoneName;
                _lastValidZoneName = _editingZoneName;
            }

            ImGui.Text("Debug Apperance"u8);
            if (SettingsTable.BeginTable("debugglyph"))
            {
                ImGuiTypes.ColoredGlyphReference glyphRef = zone.Appearance;
                SettingsTable.DrawCommonSettings(true, true, false, true, true,
                    ref glyphRef,
                    null,
                    null,
                    document.EditingSurfaceFont,
                    ImGuiCore.Renderer);

                if (glyphRef != zone.Appearance)
                {
                    zoneVisualChanged = true;
                    zone.Appearance = glyphRef.ToColoredGlyph();
                }

                SettingsTable.EndTable();
            }
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
                // Help wrap the table so that it can stretch and still show all rows plus the scroll bars
                if (ImGui.BeginChild("zone_settings_child", new Vector2(-1, ImGui.GetFrameHeight() * zone.Settings.Count + ImGui.GetStyle().FramePadding.Y * 2)))
                {
                    if (ImGui.BeginTable("zone_settings_table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollX))
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
                }
                ImGui.EndChild();
            }
            else
                ImGui.Text("No settings defined."u8);

            return true;
        }

        return false;
    }
}
