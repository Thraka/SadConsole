using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Hexa.NET.ImGui.SC.Windows;
using SadConsole.ImGuiSystem;
using SadConsole.UI;
using System.Numerics;

namespace SadConsole.Editor.Windows;

public class PaletteEditorWindow : ImGuiWindowBase
{
    private List<NamedColor> _editingColors;
    private int _selectedColorIndex = -1;
    private string _editingColorName = "";
    private Vector4 _editingColorValue = Vector4.One;

    private EditorPalette _palette;
    private Documents.Document? _sourceDocument;

    public PaletteEditorWindow(EditorPalette palette)
    {
        Title = "Palette Editor";
        _palette = palette ?? throw new ArgumentNullException(nameof(palette));
        _editingColors = [.. palette.Colors];
        _sourceDocument = Core.State.SelectedDocument;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Core.Settings.WindowNewDocWidthFactor * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.MenuBar))
            {
                bool menuClearAll = false;
                bool menuImportColors = false;

                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Options"u8))
                    {
                        if (ImGui.MenuItem("Clear All"u8))
                            menuClearAll = true;

                        ImGui.BeginDisabled(_sourceDocument == null);
                        if (ImGui.MenuItem("Import from Document"u8))
                            menuImportColors = true;

                        ImGui.EndDisabled();

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }

                // Sub popups
                if (menuClearAll)
                    ImGui.OpenPopup("clear_all_popup"u8);

                if (ImGui.BeginPopup("clear_all_popup"u8))
                {
                    ImGui.Text("Are you sure?"u8);
                    if (DrawButtons(out bool result, false, "No", "Yes"))
                    {
                        if (result)
                            ResetToDefault();

                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.EndPopup();
                }

                if (menuImportColors)
                    ImGui.OpenPopup("import_colors"u8);

                if (ImGui.BeginPopup("import_colors"u8))
                {
                    ImGui.Text("Erase existing before importing?"u8);
                    if (DrawButtons(out bool result, false, "No", "Yes"))
                    {
                        if (result)
                            ImportColorsFromDocument(true);
                        else
                            ImportColorsFromDocument(false);

                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.EndPopup();
                }

                // Color list
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Colors:");

                ImGui.SameLine();
                if (ImGui.Button("Add"))
                    AddNewColor();

                ImGui.SameLine();
                ImGui.BeginDisabled(_selectedColorIndex == -1);
                if (ImGui.Button("Remove Selected"))
                    RemoveSelectedColor();
                ImGui.EndDisabled();


                if (ImGui.BeginChild("ColorList", new Vector2(0, 200), ImGuiChildFlags.Borders))
                {
                    Vector2 padding = ImGui.GetStyle().FramePadding;
                    Vector2 spacing = ImGui.GetStyle().ItemSpacing;
                    ImDrawListPtr drawData = ImGui.GetWindowDrawList();

                    for (int i = 0; i < _editingColors.Count; i++)
                    {
                        var color = _editingColors[i];
                        
                        Vector2 pos = ImGui.GetCursorPos();
                        if (ImGui.Selectable($"##{color.Name}", i == _selectedColorIndex))
                        {
                            _selectedColorIndex = i;
                            _editingColorName = color.Name;
                            _editingColorValue =color.Color.ToVector4();
                        }

                        Vector2 topLeft = ImGui.GetItemRectMin();
                        Vector2 bottomRight = ImGui.GetItemRectMax();
                        Vector2 bottomRightMax = ImGui.GetItemRectMax();

                        bottomRight = topLeft + new Vector2(bottomRight.Y - topLeft.Y, bottomRight.Y - topLeft.Y);

                        topLeft += padding;
                        bottomRight.Y -= padding.Y;
                        bottomRight.X = topLeft.X + bottomRight.Y - topLeft.Y;

                        pos += new Vector2(bottomRight.X - topLeft.X + spacing.X, 0f);

                        ImGui.SetCursorPos(pos);
                        ImGui.Text(color.Name);

                        drawData.AddRectFilled(topLeft, bottomRight, color.Color.PackedValue);

                        pos += new Vector2(bottomRight.X - topLeft.X + spacing.X, 0f);

                        topLeft.X = ImGui.CalcTextSize(color.Name).X + bottomRight.X + spacing.X + spacing.X;
                        bottomRight = bottomRightMax;
                        bottomRight -= padding;

                        drawData.AddRectFilled(topLeft, bottomRight, color.Color.PackedValue);

                    }
                }
                ImGui.EndChild();

                ImGui.Separator();

                // Color editor
                if (_selectedColorIndex >= 0 && _selectedColorIndex < _editingColors.Count)
                {
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Edit Selected Color:");
                    
                    ImGui.SameLine();
                    ImGui.BeginDisabled(_selectedColorIndex == 0);
                    if (ImGui.Button("\uf102##totop"u8))
                        MoveColorToTop();
                    ImGui.EndDisabled();
                    
                    ImGui.SameLine();
                    ImGui.BeginDisabled(_selectedColorIndex == 0);
                    if (ImGui.Button("\uf062##up"u8))
                        MoveColorUp();
                    ImGui.EndDisabled();
                    
                    ImGui.SameLine();
                    ImGui.BeginDisabled(_selectedColorIndex >= _editingColors.Count - 1);
                    if (ImGui.Button("\uf063##down"u8))
                        MoveColorDown();
                    ImGui.EndDisabled();
                    
                    ImGui.SameLine();
                    ImGui.BeginDisabled(_selectedColorIndex >= _editingColors.Count - 1);
                    if (ImGui.Button("\uf103##tobottom"u8))
                        MoveColorToBottom();
                    ImGui.EndDisabled();
                    
                    // Name input
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Name:");
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1);
                    if (ImGui.InputText("##editname", ref _editingColorName, 256))
                    {
                        UpdateSelectedColor();
                    }
                    
                    // Color picker
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Color:");
                    ImGui.SameLine();
                    if (ImGui.ColorEdit4("##editcolor", ref _editingColorValue))
                    {
                        UpdateSelectedColor();
                    }

                    if (ImGui.Button($"Choose From Palette##editor"))
                        ImGui.OpenPopup($"palettepopup##editor");

                    Color col = _editingColorValue.ToColor();
                    if (PalettePopup.Show($"palettepopup##editor", ref col))
                    {
                        _editingColorValue = col.ToVector4();
                        UpdateSelectedColor();
                    }
                }
                else
                    ImGui.Text("Select a color to edit");

                ImGui.Separator();

                // Action buttons
                if (DrawButtons(out DialogResult, acceptButtonText: "Apply Changes"))
                {
                    if (DialogResult)
                        ApplyChanges();

                    Close();
                }

                ImGui.EndPopup();
            }
            else
            {
                Close();
            }
        }
    }

    private void AddNewColor() =>
        _editingColors.Add(new NamedColor(Documents.Document.GenerateName("Color"), Color.White));

    private void RemoveSelectedColor()
    {
        if (_selectedColorIndex >= 0 && _selectedColorIndex < _editingColors.Count)
        {
            _editingColors.RemoveAt(_selectedColorIndex);
            _selectedColorIndex = -1;
        }
    }

    private void MoveColorToTop()
    {
        if (_selectedColorIndex > 0 && _selectedColorIndex < _editingColors.Count)
        {
            var color = _editingColors[_selectedColorIndex];
            _editingColors.RemoveAt(_selectedColorIndex);
            _editingColors.Insert(0, color);
            _selectedColorIndex = 0;
        }
    }

    private void MoveColorUp()
    {
        if (_selectedColorIndex > 0 && _selectedColorIndex < _editingColors.Count)
        {
            var color = _editingColors[_selectedColorIndex];
            _editingColors.RemoveAt(_selectedColorIndex);
            _editingColors.Insert(_selectedColorIndex - 1, color);
            _selectedColorIndex--;
        }
    }

    private void MoveColorDown()
    {
        if (_selectedColorIndex >= 0 && _selectedColorIndex < _editingColors.Count - 1)
        {
            var color = _editingColors[_selectedColorIndex];
            _editingColors.RemoveAt(_selectedColorIndex);
            _editingColors.Insert(_selectedColorIndex + 1, color);
            _selectedColorIndex++;
        }
    }

    private void MoveColorToBottom()
    {
        if (_selectedColorIndex >= 0 && _selectedColorIndex < _editingColors.Count - 1)
        {
            var color = _editingColors[_selectedColorIndex];
            _editingColors.RemoveAt(_selectedColorIndex);
            _editingColors.Add(color);
            _selectedColorIndex = _editingColors.Count - 1;
        }
    }

    private void ResetToDefault()
    {
        _editingColors.Clear();
        _editingColors.Add(new NamedColor("White", SadRogue.Primitives.Color.White));
        _editingColors.Add(new NamedColor("Black", SadRogue.Primitives.Color.Black));
        _selectedColorIndex = -1;
    }

    private void UpdateSelectedColor()
    {
        if (_selectedColorIndex >= 0 && _selectedColorIndex < _editingColors.Count)
        {
            var newColor = new NamedColor(_editingColorName,
                new SadRogue.Primitives.Color((byte)(_editingColorValue.X * 255),
                                            (byte)(_editingColorValue.Y * 255),
                                            (byte)(_editingColorValue.Z * 255)));
            _editingColors[_selectedColorIndex] = newColor;
        }
    }

    private void ApplyChanges()
    {
        _palette.Colors = _editingColors.ToArray();
        Core.State.SyncEditorPalette();
    }

    private void ImportColorsFromDocument(bool clear)
    {
        if (_sourceDocument == null) return;

        // Use a HashSet to track unique colors from the document
        HashSet<SadRogue.Primitives.Color> uniqueColors = new();

        // Iterate through all cells in the surface
        var surface = _sourceDocument.EditingSurface.Surface;
        for (int i = 0; i < surface.Count; i++)
        {
            var cell = surface[i];
            uniqueColors.Add(cell.Foreground);
            uniqueColors.Add(cell.Background);
        }

        // If not clearing, build a set of existing colors to skip
        HashSet<SadRogue.Primitives.Color> existingColors = new();
        if (!clear)
        {
            foreach (var namedColor in _editingColors)
            {
                existingColors.Add(namedColor.Color);
            }
        }
        else
        {
            // Clear existing colors
            _editingColors.Clear();
        }

        int colorIndex = 1;
        foreach (var color in uniqueColors.OrderBy(c => c.GetHSLLightness()))
        {
            // Skip colors that already exist in the palette
            if (existingColors.Contains(color))
                continue;

            string colorName = Documents.Document.GenerateName($"Color{colorIndex}");
            _editingColors.Add(new NamedColor(colorName, color));
            colorIndex++;
        }

        // Reset selection
        _selectedColorIndex = -1;
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        // Refresh the editing colors when window opens
        _editingColors = [.. _palette.Colors];
        _selectedColorIndex = -1;
        _sourceDocument = Core.State.SelectedDocument;
    }
}
