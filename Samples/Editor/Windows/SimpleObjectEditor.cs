using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Hexa.NET.ImGui.SC.Windows;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiTypes;

namespace SadConsole.Editor.Windows;

public class SimpleObjectEditor : ImGuiWindowBase
{
    public ImGuiList<SimpleObjectDefinition> _objects;
    public ColoredGlyphReference Glyph;
    public string? Name;

    private bool _isAdding = false;
    private bool _isEditing = false;

    private readonly Vector4 _defaultForeground;
    private readonly Vector4 _defaultBackground;
    private readonly IFont _font;

    public SimpleObjectEditor(ImGuiList<SimpleObjectDefinition> objects, Vector4 defaultForeground, Vector4 defaultBackground, IFont font)
    {
        Title = "Simple Object Editor";
        _defaultForeground = defaultForeground;
        _defaultBackground = defaultBackground;
        _font = font;
        _objects = objects;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out Vector2 itemSpacing);

            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new Vector2(Core.Settings.WindowSimpleObjectEditor * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.Columns(2);
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Objects");
                ImGui.SetNextItemWidth(-1);
                if (ImGui.BeginListBox("##pencil_simplegameobjects"))
                {
                    Tools.SimpleObjectHelpers.DrawSelectables("pencil_simplegameobjects", _objects, _font);

                    ImGui.EndListBox();
                }

                if (ImGui.Button("Create New Object"))
                {
                    SimpleObjectDefinition newObject = new SimpleObjectDefinition();
                    newObject.Name = "New Object";
                    newObject.Visual.Foreground = _defaultForeground.ToColor();
                    newObject.Visual.Background = _defaultBackground.ToColor();
                    newObject.Visual.Glyph = 1;
                    _objects.Objects.Add(newObject);
                    _objects.SelectedItem = newObject;
                }

                float pos = ImGui.CalcTextSize("Delete").X + framePadding.X / 2;
                ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
                ImGui.BeginDisabled(!_objects.IsItemSelected());

                if (ImGui.Button("Delete"))
                    ImGui.OpenPopup("ConfirmDelete");

                ImGui.EndDisabled();

                ImGui.NextColumn();

                bool isItemSelected = _objects.IsItemSelected();

                if (isItemSelected)
                {
                    ColoredGlyph visual = _objects.SelectedItem.Visual;

                    Vector4 foreground = visual.Foreground.ToVector4();
                    Vector4 background = visual.Background.ToVector4();
                    int glyph = visual.Glyph;
                    ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(visual.Mirror);
                    string name = _objects.SelectedItem.Name;

                    if (SettingsTable.BeginTable("pencilsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
                    {
                        SettingsTable.DrawString("Name", ref name, 255);

                        SettingsTable.DrawCommonSettings(true, true, true, true, true,
                            ref foreground, _defaultForeground,
                            ref background, _defaultBackground,
                            ref mirror,
                            ref glyph, _font, ImGuiCore.Renderer);

                        SettingsTable.EndTable();
                    }

                    visual.Foreground = foreground.ToColor();
                    visual.Background = background.ToColor();
                    visual.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(mirror);
                    visual.Glyph = glyph;
                    _objects.SelectedItem.Name = name;
                }

                ImGui.Columns(1);

                ImGuiSC.CenterNextWindow();
                if (ImGui.BeginPopup("ConfirmDelete"))
                {
                    ImGui.Text("Are you sure you want to delete this object?");

                    if (ImGui.Button("Cancel"))
                        ImGui.CloseCurrentPopup();

                    pos = ImGui.CalcTextSize("Yes").X + framePadding.X / 2;
                    ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
                    if (ImGui.Button("Yes"))
                    {
                        _objects.Objects.Remove(_objects.SelectedItem);
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
                ImGui.EndPopup();
            }
        }
    }
}
