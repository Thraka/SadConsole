//using System.Numerics;
//using Hexa.NET.ImGui;
//using Hexa.NET.ImGui.SC;
//using Hexa.NET.ImGui.SC.Windows;
//using SadConsole.ImGuiSystem;
//using SadConsole.ImGuiTypes;

//namespace SadConsole.Editor.Windows;

//public class ZonesEditor : ImGuiWindowBase
//{
//    public ImGuiList<Serialization.ZoneSerialized> _zones;
//    public ColoredGlyphReference Glyph;
//    public string? Name;

//    private bool _isAdding = false;
//    private bool _isEditing = false;

//    private readonly IFont _font;

//    public ZonesEditor(ImGuiList<Serialization.ZoneSerialized> objects, IFont font)
//    {
//        Title = "Simple Object Editor";
//        _font = font;
//        _zones = objects;
//    }

//    public override void BuildUI(ImGuiRenderer renderer)
//    {
//        if (IsOpen)
//        {
//            ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out Vector2 itemSpacing);

//            ImGui.OpenPopup(Title);

//            ImGuiSC.CenterNextWindow();
//            ImGui.SetNextWindowSize(new Vector2(Core.Settings.WindowSimpleObjectEditor * ImGui.GetFontSize(), -1));
//            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
//            {
//                ImGui.Columns(2);
//                ImGui.AlignTextToFramePadding();
//                ImGui.Text("Zones");
//                ImGui.SetNextItemWidth(-1);
//                if (ImGui.BeginListBox("##zoneobjects"))
//                {
//                    Tools.SimpleObjectHelpers.DrawSelectables("zoneobjects", _zones, _font);

//                    ImGui.EndListBox();
//                }

//                if (ImGui.Button("Create New Object"))
//                {
//                    SimpleObjectDefinition newObject = new SimpleObjectDefinition();
//                    newObject.Name = "New Object";
//                    newObject.Visual.Foreground = _defaultForeground.ToColor();
//                    newObject.Visual.Background = _defaultBackground.ToColor();
//                    newObject.Visual.Glyph = 1;
//                    _zones.Objects.Add(newObject);
//                    _zones.SelectedItem = newObject;
//                }

//                float pos = ImGui.CalcTextSize("Delete").X + framePadding.X / 2;
//                ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
//                ImGui.BeginDisabled(!_zones.IsItemSelected());

//                if (ImGui.Button("Delete"))
//                    ImGui.OpenPopup("ConfirmDelete");

//                ImGui.EndDisabled();

//                ImGui.NextColumn();

//                bool isItemSelected = _zones.IsItemSelected();

//                if (isItemSelected)
//                {
//                    ColoredGlyph visual = _zones.SelectedItem.Visual;

//                    Vector4 foreground = visual.Foreground.ToVector4();
//                    Vector4 background = visual.Background.ToVector4();
//                    int glyph = visual.Glyph;
//                    ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(visual.Mirror);
//                    string name = _zones.SelectedItem.Name;

//                    if (SettingsTable.BeginTable("pencilsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
//                    {
//                        SettingsTable.DrawString("Name", ref name, 255);

//                        SettingsTable.DrawCommonSettings(true, true, true, true, true,
//                            ref foreground, _defaultForeground,
//                            ref background, _defaultBackground,
//                            ref mirror,
//                            ref glyph, _font, ImGuiCore.Renderer);

//                        SettingsTable.EndTable();
//                    }

//                    visual.Foreground = foreground.ToColor();
//                    visual.Background = background.ToColor();
//                    visual.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(mirror);
//                    visual.Glyph = glyph;
//                    _zones.SelectedItem.Name = name;
//                }

//                ImGui.Columns(1);

//                ImGuiSC.CenterNextWindow();
//                if (ImGui.BeginPopup("ConfirmDelete"))
//                {
//                    ImGui.Text("Are you sure you want to delete this object?");

//                    if (ImGui.Button("Cancel"))
//                        ImGui.CloseCurrentPopup();

//                    pos = ImGui.CalcTextSize("Yes").X + framePadding.X / 2;
//                    ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
//                    if (ImGui.Button("Yes"))
//                    {
//                        _zones.Objects.Remove(_zones.SelectedItem);
//                        ImGui.CloseCurrentPopup();
//                    }

//                    ImGui.EndPopup();
//                }
//                ImGui.EndPopup();
//            }
//        }
//    }
//}
