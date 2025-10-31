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

    public PaletteEditorWindow(EditorPalette palette)
    {
        Title = "Palette Editor";
        _palette = palette ?? throw new ArgumentNullException(nameof(palette));
        _editingColors = new List<NamedColor>(palette.Colors);
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
                if (ImGui.BeginMenuBar())
                {
                        if (ImGui.MenuItem("Add Color"))
                            AddNewColor();

                        if (ImGui.MenuItem("Reset to Default"))
                            ResetToDefault();

                        if (ImGui.MenuItem("Delete Selected"))
                            RemoveSelectedColor();

                    ImGui.EndMenuBar();
                }

                ImGui.Separator();

                // Color list
                ImGui.Text("Colors:");
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
                    ImGui.Text("Edit Selected Color:");
                    
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

                ImGui.End();
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

    protected override void OnOpened()
    {
        base.OnOpened();
        // Refresh the editing colors when window opens
        _editingColors = new List<NamedColor>(_palette.Colors);
        _selectedColorIndex = -1;
    }
}
