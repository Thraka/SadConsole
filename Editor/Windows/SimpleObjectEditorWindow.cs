using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Types;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class SimpleObjectEditorWindow
{
    protected class Instance : ImGuiObjectBase
    {
        private ImGuiList<SimpleObjectDefinition> _objects;

        private readonly Vector4 _defaultForeground;
        private readonly Vector4 _defaultBackground;
        private readonly IFont _font;
        private bool _firstShow = true;

        public Instance(ImGuiList<SimpleObjectDefinition> objects, Vector4 defaultForeground, Vector4 defaultBackground, IFont font)
        {
            _defaultForeground = defaultForeground;
            _defaultBackground = defaultBackground;
            _font = font;
            _objects = objects;
        }

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("Simple Object Editor"u8);
                _firstShow = false;
            }

            ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out Vector2 itemSpacing);

            ImGuiSC.CenterNextWindowOnAppearing(new Vector2(Core.Settings.WindowSimpleObjectEditor * ImGui.GetFontSize(), -1));

            if (ImGui.BeginPopupModal("Simple Object Editor"u8, ImGuiWindowFlags.NoResize))
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

                float pos = ImGui.CalcTextSize("Delete"u8).X + framePadding.X / 2;
                ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
                ImGui.BeginDisabled(!_objects.IsItemSelected());

                if (ImGui.Button("Delete"u8))
                    ImGui.OpenPopup("ConfirmDelete"u8);

                ImGui.EndDisabled();

                ImGui.NextColumn();

                bool isItemSelected = _objects.IsItemSelected();

                if (isItemSelected)
                {
                    ColoredGlyph visual = _objects.SelectedItem.Visual;

                    Vector4 foreground = visual.Foreground.ToVector4();
                    Vector4 background = visual.Background.ToVector4();
                    int glyph = visual.Glyph;
                    ImGuiSystem.Types.Mirror mirror = ImGuiSystem.Types.MirrorConverter.FromSadConsoleMirror(visual.Mirror);
                    string name = _objects.SelectedItem.Name;

                    if (SettingsTable.BeginTable("pencilsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
                    {
                        SettingsTable.DrawString("Name", ref name, 255);

                        SettingsTable.DrawCommonSettings(true, true, true, true, true,
                            ref foreground, _defaultForeground,
                            ref background, _defaultBackground,
                            ref mirror,
                            ref glyph, _font, Core.ImGuiComponent.ImGuiRenderer);

                        SettingsTable.EndTable();
                    }

                    visual.Foreground = foreground.ToColor();
                    visual.Background = background.ToColor();
                    visual.Mirror = ImGuiSystem.Types.MirrorConverter.ToSadConsoleMirror(mirror);
                    visual.Glyph = glyph;
                    _objects.SelectedItem.Name = name;
                }

                ImGui.Columns(1);

                if (ImGuiSC.ConfirmPopup("ConfirmDelete"u8, "Are you sure you want to delete this object?"u8, null, null))
                    _objects.Objects.Remove(_objects.SelectedItem);

                ImGui.Separator();

                if (ImGuiSC.WindowDrawButtons(out bool dialogResult, cancelButtonText: "", acceptButtonText: "Close"))
                {
                    renderer.UIObjects.Remove(this);
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }
    }

    public static void Show(ImGuiRenderer renderer, ImGuiList<SimpleObjectDefinition> objects, Vector4 defaultForeground, Vector4 defaultBackground, IFont font)
    {
        Instance instance = new(objects, defaultForeground, defaultBackground, font);
        renderer.UIObjects.Add(instance);
    }
}
