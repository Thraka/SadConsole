using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Hexa.NET.ImGui.SC.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

/// <summary>
/// Represents a key-value pair for editing in the UI.
/// </summary>
public class KeyValuePairItem
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public override string ToString() => string.IsNullOrEmpty(Key) ? "(empty key)" : Key;
}

/// <summary>
/// A window for editing a collection of string key-value pairs.
/// </summary>
public class KeyValuePairEditor : ImGuiWindowBase
{
    private readonly ImGuiList<KeyValuePairItem> _items;
    private string _editingKey = string.Empty;
    private string _editingValue = string.Empty;
    private bool _keyConflict;
    private bool _keyEmpty;

    /// <summary>
    /// Creates a new key-value pair editor.
    /// </summary>
    /// <param name="items">The collection of key-value pairs to edit.</param>
    public KeyValuePairEditor(ImGuiList<KeyValuePairItem> items)
    {
        Title = "Key-Value Pair Editor";
        _items = items;
    }

    /// <summary>
    /// Creates a new key-value pair editor from a dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary to edit.</param>
    public KeyValuePairEditor(IDictionary<string, string> dictionary)
    {
        Title = "Key-Value Pair Editor";
        var items = dictionary.Select(kvp => new KeyValuePairItem { Key = kvp.Key, Value = kvp.Value });
        _items = new ImGuiList<KeyValuePairItem>(items);
    }

    /// <summary>
    /// Gets the edited items as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing all key-value pairs.</returns>
    public Dictionary<string, string> ToDictionary()
    {
        var result = new Dictionary<string, string>();
        foreach (var item in _items.Objects)
        {
            if (!string.IsNullOrEmpty(item.Key) && !result.ContainsKey(item.Key))
                result[item.Key] = item.Value;
        }
        return result;
    }

    /// <summary>
    /// Gets the underlying items collection.
    /// </summary>
    public ImGuiList<KeyValuePairItem> Items => _items;

    /// <summary>
    /// Checks if the specified key already exists in another item.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <param name="excludeItem">The item to exclude from the check (the current item being edited).</param>
    /// <returns>True if the key exists in another item; otherwise, false.</returns>
    private bool IsKeyDuplicate(string key, KeyValuePairItem excludeItem)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        foreach (var item in _items.Objects)
        {
            if (item != excludeItem && string.Equals(item.Key, key, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out Vector2 itemSpacing);

            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new Vector2(35 * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.Columns(2);
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Items");
                ImGui.SetNextItemWidth(-1);
                if (ImGui.BeginListBox("##keyvaluepair_items"))
                {
                    for (int i = 0; i < _items.Objects.Count; i++)
                    {
                        bool isSelected = _items.SelectedItemIndex == i;
                        if (ImGui.Selectable(_items.Objects[i].ToString(), isSelected))
                        {
                            // Apply pending changes before switching selection
                            if (_items.IsItemSelected() && !_keyConflict && !_keyEmpty)
                            {
                                _items.SelectedItem.Key = _editingKey;
                                _items.SelectedItem.Value = _editingValue;
                            }

                            _items.SelectedItemIndex = i;

                            // Load the new item's values for editing
                            _editingKey = _items.SelectedItem.Key;
                            _editingValue = _items.SelectedItem.Value;
                            _keyConflict = false;
                            _keyEmpty = false;
                        }

                        if (isSelected)
                            ImGui.SetItemDefaultFocus();
                    }

                    ImGui.EndListBox();
                }

                if (ImGui.Button("Add New"))
                {
                    // Apply pending changes before adding new item
                    if (_items.IsItemSelected() && !_keyConflict && !_keyEmpty)
                    {
                        _items.SelectedItem.Key = _editingKey;
                        _items.SelectedItem.Value = _editingValue;
                    }

                    // Generate a unique key
                    string baseKey = "NewKey";
                    string newKey = baseKey;
                    int counter = 1;
                    while (_items.Objects.Any(item => string.Equals(item.Key, newKey, StringComparison.Ordinal)))
                    {
                        newKey = $"{baseKey}{counter}";
                        counter++;
                    }

                    var newItem = new KeyValuePairItem { Key = newKey, Value = "" };
                    _items.Objects.Add(newItem);
                    _items.SelectedItem = newItem;

                    // Load the new item's values for editing
                    _editingKey = newItem.Key;
                    _editingValue = newItem.Value;
                    _keyConflict = false;
                    _keyEmpty = false;
                }

                float pos = ImGui.CalcTextSize("Delete").X + framePadding.X / 2;
                ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
                ImGui.BeginDisabled(!_items.IsItemSelected());

                if (ImGui.Button("Delete"))
                    ImGui.OpenPopup("ConfirmDelete");

                ImGui.EndDisabled();

                ImGui.NextColumn();

                bool isItemSelected = _items.IsItemSelected();

                if (isItemSelected)
                {
                    // Initialize editing values if they haven't been set
                    if (_editingKey != _items.SelectedItem.Key && !_keyConflict && !_keyEmpty)
                    {
                        _editingKey = _items.SelectedItem.Key;
                        _editingValue = _items.SelectedItem.Value;
                    }

                    if (SettingsTable.BeginTable("keyvaluepairsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
                    {
                        SettingsTable.DrawString("Key", ref _editingKey, 255);

                        // Check for empty key
                        _keyEmpty = string.IsNullOrEmpty(_editingKey);

                        // Check for duplicate key
                        _keyConflict = IsKeyDuplicate(_editingKey, _items.SelectedItem);

                        if (_keyEmpty)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.TableNextColumn();
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f));
                            ImGui.TextWrapped("Key cannot be empty.");
                            ImGui.PopStyleColor();
                        }
                        else if (_keyConflict)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.TableNextColumn();
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f));
                            ImGui.TextWrapped("Key already exists. Please use a unique key.");
                            ImGui.PopStyleColor();
                        }

                        SettingsTable.DrawString("Value", ref _editingValue, 1024);

                        SettingsTable.EndTable();
                    }

                    // Only apply changes if there's no key conflict and key is not empty
                    if (!_keyConflict && !_keyEmpty)
                    {
                        _items.SelectedItem.Key = _editingKey;
                        _items.SelectedItem.Value = _editingValue;
                    }
                    else
                    {
                        // Still update the value even if key conflicts or is empty
                        _items.SelectedItem.Value = _editingValue;
                    }
                }
                else
                {
                    ImGui.TextDisabled("Select an item to edit");
                }

                ImGui.Columns(1);

                ImGuiSC.CenterNextWindow();
                if (ImGui.BeginPopup("ConfirmDelete"))
                {
                    ImGui.Text("Are you sure you want to delete this item?");

                    if (ImGui.Button("Cancel"))
                        ImGui.CloseCurrentPopup();

                    pos = ImGui.CalcTextSize("Yes").X + framePadding.X / 2;
                    ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
                    if (ImGui.Button("Yes"))
                    {
                        _items.Objects.Remove(_items.SelectedItem);
                        _editingKey = string.Empty;
                        _editingValue = string.Empty;
                        _keyConflict = false;
                        _keyEmpty = false;
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                ImGui.Separator();

                if (DrawButtons(out bool result))
                {
                    DialogResult = result;
                    Close();
                }

                ImGui.EndPopup();
            }
        }
    }
}
